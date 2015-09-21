using System;
using SDRSharp.Radio;
using System.Runtime.InteropServices;
using System.Threading;

namespace SDRSharp.BladeRF
{
    public sealed class BladeRFDevice : IDisposable
    {
        private const uint DefaultFrequency = 405500000U;
        private const int DefaultSamplerate = 4000000;
        private const uint MinFrequency = 232500000;
        private const uint MaxFrequency = 3800000000;
        private const int MinBandwidth = 1500000;
        private const int MaxBandwidth = 28000000;
        private const uint SampleTimeoutMs = 1000;
        private const uint NumBuffers = 32;
        
        private static object syncLock = new object();
        private string DeviceName = "BladeRF";
        private string _serial;
        private IntPtr _dev;
        private long _centerFrequency = DefaultFrequency;
        private double _sampleRate = DefaultSamplerate;
        private int _bandwidth;
        private bool _isFpgaLoaded = false;
        private bool _RXConfigured = false;
        private string _fpga_path = Utils.GetStringSetting("BladeRFFPGA", "");
        private bladerf_lna_gain _lnaGain = (bladerf_lna_gain)Utils.GetIntSetting("BladeRFLNAGain", (int) bladerf_lna_gain.BLADERF_LNA_GAIN_MID);
        private int _vga1Gain = Utils.GetIntSetting("BladeRFRXVGA1Gain", 20);
        private int _vga2Gain = Utils.GetIntSetting("BladeRFRXVGA2Gain", 20);
        private bladerf_sampling _sampling = (bladerf_sampling)Utils.GetIntSetting("BladeRFSamplingMode", (int)bladerf_sampling.BLADERF_SAMPLING_INTERNAL);
        private GCHandle _gcHandle;
        private UnsafeBuffer _iqBuffer;
        private unsafe Complex* _iqPtr;
        private bool _isStreaming;
        private string _devSpec;
        private readonly SamplesAvailableEventArgs _eventArgs = new SamplesAvailableEventArgs();
        private static uint _readLength = 16384;
        private Thread _sampleThread = null;
        private static bladerf_version _version = NativeMethods.bladerf_version();
        private static bool _xb200_enabled = Utils.GetBooleanSetting("BladeRFXB200Enabled");
        private static bladerf_xb200_filter _xb200_filter = (bladerf_xb200_filter) (Utils.GetIntSetting("BladeRFXB200Filter", 0) - 1);

        private static readonly unsafe float* _lutPtr;
        private static readonly UnsafeBuffer _lutBuffer = UnsafeBuffer.Create(4096, sizeof(float));
        private UnsafeBuffer _samplesBuffer;
        private unsafe Int16* _samplesPtr;

        public bool XB200Enabled
        {
            get { return _xb200_enabled; }
            set
            {
                _xb200_enabled = value;
                if (value == true && _dev != IntPtr.Zero)
                {
                    NativeMethods.bladerf_xb200_attach(_dev);
                }
            }
        }

        public int XB200Filter
        {
            get { return (int) (_xb200_filter + 1); }
            set
            {
                switch (value - 1)
                {
                    case (int)bladerf_xb200_filter.BLADERF_XB200_AUTO:
                        _xb200_filter = bladerf_xb200_filter.BLADERF_XB200_AUTO;
                        break;
                    case (int)bladerf_xb200_filter.BLADERF_XB200_50M:
                        _xb200_filter = bladerf_xb200_filter.BLADERF_XB200_50M;
                        break;
                    case (int)bladerf_xb200_filter.BLADERF_XB200_144M:
                        _xb200_filter = bladerf_xb200_filter.BLADERF_XB200_144M;
                        break;
                    case (int)bladerf_xb200_filter.BLADERF_XB200_222M:
                        _xb200_filter = bladerf_xb200_filter.BLADERF_XB200_222M;
                        break;
                    case (int)bladerf_xb200_filter.BLADERF_XB200_AUTO_1DB:
                        _xb200_filter = bladerf_xb200_filter.BLADERF_XB200_AUTO_1DB;
                        break;
                    case (int)bladerf_xb200_filter.BLADERF_XB200_AUTO_3DB:
                        _xb200_filter = bladerf_xb200_filter.BLADERF_XB200_AUTO_3DB;
                        break;
                    default:
                        _xb200_filter = bladerf_xb200_filter.BLADERF_XB200_CUSTOM;
                        break;
                }
            }
        }

        public string FPGAImage
        {
            get { return _fpga_path; }
            set { _fpga_path = value; }
        }

        public int Sampling
        {
            get { return (int)_sampling; }
            set
            {
                switch (value)
                {
                    case (int)bladerf_sampling.BLADERF_SAMPLING_UNKNOWN:
                        _sampling = bladerf_sampling.BLADERF_SAMPLING_UNKNOWN;
                        break;
                    case (int)bladerf_sampling.BLADERF_SAMPLING_INTERNAL:
                        _sampling = bladerf_sampling.BLADERF_SAMPLING_INTERNAL;
                        break;
                    case (int)bladerf_sampling.BLADERF_SAMPLING_EXTERNAL:
                        _sampling = bladerf_sampling.BLADERF_SAMPLING_EXTERNAL;
                        break;
                    default:
                        _sampling = bladerf_sampling.BLADERF_SAMPLING_UNKNOWN;
                        break;
                }
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.bladerf_set_sampling(_dev, _sampling);
                }
            }
        }

        public uint Index
        {
            get
            {
                return 0;
            }
        }

        public string Serial
        {
            get { return _serial; }
        }

        public string Name
        {
            get
            {
                return DeviceName;
            }
        }

        public uint LNAGain
        {
            get
            {
                return (uint)_lnaGain;
            }
            set
            {
                switch (value)
                {
                    case (uint)bladerf_lna_gain.BLADERF_LNA_GAIN_BYPASS:
                        _lnaGain = bladerf_lna_gain.BLADERF_LNA_GAIN_BYPASS;
                        break;
                    case (uint)bladerf_lna_gain.BLADERF_LNA_GAIN_MID:
                        _lnaGain = bladerf_lna_gain.BLADERF_LNA_GAIN_MID;
                        break;
                    case (uint)bladerf_lna_gain.BLADERF_LNA_GAIN_MAX:
                        _lnaGain = bladerf_lna_gain.BLADERF_LNA_GAIN_MAX;
                        break;
                    default:
                        _lnaGain = bladerf_lna_gain.BLADERF_LNA_GAIN_UNKNOWN;
                        break;
                }
                if (_dev != IntPtr.Zero)
                    NativeMethods.bladerf_set_lna_gain(_dev, _lnaGain);
            }
        }

        public int VGA1Gain
        {
            get
            {
                return _vga1Gain;
            }
            set
            {
                _vga1Gain = value;
                if (_dev != IntPtr.Zero)
                    NativeMethods.bladerf_set_rxvga1(_dev, _vga1Gain);
            }
        }

        public int VGA2Gain
        {
            get
            {
                return _vga2Gain;
            }
            set
            {
                _vga2Gain = value;
                if (_dev != IntPtr.Zero)
                    NativeMethods.bladerf_set_rxvga2(_dev, _vga2Gain);
            }
        }

        public double SampleRate
        {
            get
            {
                return _sampleRate;
            }
            set
            {
                _sampleRate = value;
                if (_dev != IntPtr.Zero)
                {
                    double actual;
                    if (0 == NativeMethods.bladerf_set_sample_rate(_dev, bladerf_module.BLADERF_MODULE_RX, _sampleRate, out actual))
                        _sampleRate = actual;
                    adjustReadLength();
                    uint tmp = 0;
                    if (_bandwidth == 0)
                        NativeMethods.bladerf_set_bandwidth(_dev, bladerf_module.BLADERF_MODULE_RX, (uint)(_sampleRate * 0.75), out tmp);
                }
                OnSampleRateChanged();
            }
        }

        private void adjustReadLength()
        {
            lock (syncLock)
            {
                if (_sampleRate <= 1000000)
                    _readLength = 4096U;
                else if (_sampleRate <= 10000000)
                    _readLength = 16384U;
                else
                    _readLength = 32768U;
            }
        }

        public event EventHandler SampleRateChanged;

        public void OnSampleRateChanged()
        {
            if (SampleRateChanged != null)
                SampleRateChanged(this, EventArgs.Empty);
        }

        public int Bandwidth
        {
            get
            {
                if (_bandwidth != 0)
                    return _bandwidth;
                if (_dev != IntPtr.Zero)
                {
                    uint tmp;
                    if (0 == NativeMethods.bladerf_get_bandwidth(_dev, bladerf_module.BLADERF_MODULE_RX, out tmp))
                        return (int) tmp;
                }
                return Math.Min(MaxBandwidth, Math.Max((int)(0.75 * _sampleRate), MinBandwidth));
            }

            set
            {
                _bandwidth = value;
                if (_dev != IntPtr.Zero)
                {
                    uint tmp;
                    if (value == 0)
                        NativeMethods.bladerf_set_bandwidth(_dev, bladerf_module.BLADERF_MODULE_RX, (uint) (0.75 * _sampleRate), out tmp);
                    else
                        NativeMethods.bladerf_set_bandwidth(_dev, bladerf_module.BLADERF_MODULE_RX, (uint) value, out tmp);
                }
            }
        }

        public long Frequency
        {
            get
            {
                return _centerFrequency;
            }
            set
            {
                uint real_frequency;
                _centerFrequency = value;
                if (_centerFrequency < MinFrequency && _xb200_enabled == false)
                    _centerFrequency = MinFrequency;
                if (_centerFrequency > MaxFrequency)
                    _centerFrequency = MaxFrequency;
                if (_dev != IntPtr.Zero)
                {
                    if (_xb200_filter == bladerf_xb200_filter.BLADERF_XB200_AUTO)
                        XB200AdjustFilterBank();
                    NativeMethods.bladerf_set_frequency(_dev, bladerf_module.BLADERF_MODULE_RX, (uint)_centerFrequency);
                    NativeMethods.bladerf_get_frequency(_dev, bladerf_module.BLADERF_MODULE_RX, out real_frequency);
                    _centerFrequency = real_frequency;
                }
            }
        }

        public bool IsStreaming
        {
            get
            {
                return _isStreaming;
            }
        }

        static unsafe BladeRFDevice()
        {
            _lutPtr = (float*)_lutBuffer;
            const float scale = 1.0f / 2048.0f;
            for (int i = 0; i < 4096; ++i)
            {
                _lutPtr[i] = (((i + 2048) % 4096) - 2048) * scale;
            }
        }

        public BladeRFDevice(string serial = "")
        {
            _isFpgaLoaded = false;
            string devspec = "";
            if (serial != "")
                devspec = String.Format("*:serial={0}", serial);
            var rv = NativeMethods.bladerf_open(out _dev, devspec);
            if (rv != 0)
                throw new ApplicationException(String.Format("Cannot open BladeRF device. Is the device locked somewhere?. {0}", NativeMethods.bladerf_strerror(rv)));
            _devSpec = devspec;
            _gcHandle = GCHandle.Alloc(this);
            if (serial == "")
                if ((rv = NativeMethods.bladerf_get_serial(_dev, out serial)) != 0)
                    throw new ApplicationException(String.Format("bladerf_get_serial() error. {0}", NativeMethods.bladerf_strerror(rv)));
            _serial = serial;
            DeviceName = String.Format("BladeRF SN#{0}", serial);
            NativeMethods.bladerf_close(_dev);
            _dev = IntPtr.Zero;
#if DEBUG
            NativeMethods.bladerf_log_set_verbosity(bladerf_log_level.BLADERF_LOG_LEVEL_VERBOSE);
#endif
        }

        ~BladeRFDevice()
        {
            Dispose();
        }

        private void LoadFPGA()
        {
            if (!_isFpgaLoaded)
            {
                if (_dev == IntPtr.Zero)
                    if (0 != NativeMethods.bladerf_open(out _dev, _devSpec))
                        _dev = IntPtr.Zero;
                if (_dev != IntPtr.Zero)
                {
                    if (0 == NativeMethods.bladerf_is_fpga_configured(_dev))
                    {
                        if (_fpga_path == "")
                            throw new ApplicationException("FPGA image is not set");
                        if (0 == NativeMethods.bladerf_load_fpga(_dev, _fpga_path))
                        {
                            _isFpgaLoaded = true;
                        }
                    }
                    else
                        _isFpgaLoaded = true;
                }
                else
                {
                    _isFpgaLoaded = false;
                }
            }
        }

        public void Dispose()
        {
            Stop();
            if (_gcHandle.IsAllocated)
            {
                _gcHandle.Free();
            }
            _dev = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        public event SamplesAvailableDelegate SamplesAvailable;

        private unsafe void ReceiveSamples_sync()
        {
            int status = 0;
            while (_isStreaming)
            {
                uint cur_len, new_len;
                lock (syncLock)
                {
                    cur_len = new_len = _readLength;
                }
                if (_iqBuffer == null || _iqBuffer.Length != cur_len)
                {
                    _iqBuffer = UnsafeBuffer.Create((int)cur_len, sizeof(Complex));
                    _iqPtr = (Complex*)_iqBuffer;
                }
                if (_samplesBuffer == null || _samplesBuffer.Length != (2 * cur_len))
                {
                    _samplesBuffer = UnsafeBuffer.Create((int)(2 * cur_len), sizeof(Int16));
                    _samplesPtr = (Int16*)_samplesBuffer;
                }
                if ((status = NativeMethods.bladerf_sync_config(_dev, bladerf_module.BLADERF_MODULE_RX, bladerf_format.BLADERF_FORMAT_SC16_Q11, NumBuffers, cur_len, NumBuffers / 2, SampleTimeoutMs)) != 0)
                    _isStreaming = false;
                lock(syncLock)
                {
                    _RXConfigured = true;
                }
                while (status == 0 && cur_len == new_len)
                {
                    try
                    {
                        status = NativeMethods.bladerf_sync_rx(_dev, _samplesPtr, cur_len, IntPtr.Zero, SampleTimeoutMs);
                        if (status != 0)
                            throw new ApplicationException(String.Format("bladerf_rx() error. {0}", NativeMethods.bladerf_strerror(status)));
                        var ptrIq = _iqPtr;
                        var ptrSample = _samplesPtr;
                        for (int i = 0; i < cur_len; i++)
                        {
                            ptrIq->Imag = _lutPtr[*ptrSample & 0x0fff];
                            ptrSample++;
                            ptrIq->Real = _lutPtr[*ptrSample & 0x0fff];
                            ptrSample++;
                            ptrIq++;
                        }
                        ComplexSamplesAvailable(_iqPtr, _iqBuffer.Length);
                    }
                    catch
                    {
                        break;
                    }
                    lock (syncLock)
                    {
                        new_len = _readLength;
                    }
                }
            }
        }

        private void XB200AdjustFilterBank()
        {
            if (_dev == IntPtr.Zero)
                return;
            if (!_xb200_enabled)
                return;
            if (_centerFrequency >= MinFrequency)
                return;
            int error = 0;
            FrequencyRange filter50M_freqs = new FrequencyRange(50000000, 54000000);
            FrequencyRange filter144M_freqs = new FrequencyRange(149000000, 159000000);
            FrequencyRange filter222M_freqs = new FrequencyRange(206000000, 235000000);
            switch (_xb200_filter)
            {
                case bladerf_xb200_filter.BLADERF_XB200_AUTO_1DB:
                    filter50M_freqs.Min = 37774405;
                    filter50M_freqs.Max = 59535436;
                    filter144M_freqs.Min = 128326173;
                    filter144M_freqs.Max = 166711171;
                    filter222M_freqs.Min = 187593160;
                    filter222M_freqs.Max = 245346403;
                    break;
                case bladerf_xb200_filter.BLADERF_XB200_AUTO_3DB:
                    filter50M_freqs.Min = 34782924;
                    filter50M_freqs.Max = 61899260;
                    filter144M_freqs.Min = 121956957;
                    filter144M_freqs.Max = 178444099;
                    filter222M_freqs.Min = 177522675;
                    filter222M_freqs.Max = 260140935;
                    break;
            }
            if (filter50M_freqs.contains(_centerFrequency))
            {
                error = NativeMethods.bladerf_xb200_set_filterbank(_dev, bladerf_module.BLADERF_MODULE_RX, bladerf_xb200_filter.BLADERF_XB200_50M);
            }
            else if (filter144M_freqs.contains(_centerFrequency))
            {
                error = NativeMethods.bladerf_xb200_set_filterbank(_dev, bladerf_module.BLADERF_MODULE_RX, bladerf_xb200_filter.BLADERF_XB200_144M);
            }
            else if (filter222M_freqs.contains(_centerFrequency))
            {
                error = NativeMethods.bladerf_xb200_set_filterbank(_dev, bladerf_module.BLADERF_MODULE_RX, bladerf_xb200_filter.BLADERF_XB200_222M);
            }
            else
            {
                error = NativeMethods.bladerf_xb200_set_filterbank(_dev, bladerf_module.BLADERF_MODULE_RX, bladerf_xb200_filter.BLADERF_XB200_CUSTOM);
            }

            if (error != 0)
                throw new ApplicationException(String.Format("bladerf_xb200_set_filterbank() error. {0}", NativeMethods.bladerf_strerror(error)));
        }

        public unsafe void Start()
        {
            int error;
            if (_isStreaming)
                throw new ApplicationException("Start() Already running");
            if (_dev == IntPtr.Zero)
            {
                int ret;
                if ((ret = NativeMethods.bladerf_open(out _dev, _devSpec)) == 0)
                {
                    LoadFPGA();
                }
                else
                {
                    _dev = IntPtr.Zero;
                    throw new ApplicationException(String.Format("Masked bladerf_open() error. {0}", NativeMethods.bladerf_strerror(ret)));
                }
            }
            if (_dev == IntPtr.Zero)
                throw new ApplicationException("Cannot open device");
            if (_isFpgaLoaded == false)
                throw new ApplicationException("FPGA not loaded");
            double actual;
            if ((error = NativeMethods.bladerf_set_sample_rate(_dev, bladerf_module.BLADERF_MODULE_RX, _sampleRate, out actual)) != 0)
                throw new ApplicationException(String.Format("bladerf_sample_set_sample_rate() error. {0}", NativeMethods.bladerf_strerror(error)));
            _sampleRate = actual;
            adjustReadLength();
            uint tmp = 0;
            if (_bandwidth == 0)
            {
                if ((error = NativeMethods.bladerf_set_bandwidth(_dev, bladerf_module.BLADERF_MODULE_RX, (uint)(_sampleRate * 0.75), out tmp)) != 0)
                    throw new ApplicationException(String.Format("bladerf_set_bandwidth() error. {0}", NativeMethods.bladerf_strerror(error)));
            }
            else
            {
                if ((error = NativeMethods.bladerf_set_bandwidth(_dev, bladerf_module.BLADERF_MODULE_RX, (uint) _bandwidth, out tmp)) != 0)
                    throw new ApplicationException(String.Format("bladerf_set_bandwidth() error. {0}", NativeMethods.bladerf_strerror(error)));
            }
            if ((error = NativeMethods.bladerf_set_loopback(_dev, bladerf_loopback.BLADERF_LB_NONE)) != 0)
                throw new ApplicationException(String.Format("bladerf_set_loopback() error. {0}", NativeMethods.bladerf_strerror(error)));
            if ((error = NativeMethods.bladerf_set_sampling(_dev, _sampling)) != 0)
                throw new ApplicationException(String.Format("bladerf_sampling() error. {0}", NativeMethods.bladerf_strerror(error)));
            if ((error = NativeMethods.bladerf_set_lpf_mode(_dev, bladerf_module.BLADERF_MODULE_RX, bladerf_lpf_mode.BLADERF_LPF_NORMAL)) != 0)
                throw new ApplicationException(String.Format("bladerf_set_lpf_mode() error. {0}", NativeMethods.bladerf_strerror(error)));
            if ((error = NativeMethods.bladerf_set_lna_gain(_dev, _lnaGain)) != 0)
                throw new ApplicationException(String.Format("bladerf_set_lna_gain() error. {0}", NativeMethods.bladerf_strerror(error)));
            if ((error = NativeMethods.bladerf_set_rxvga1(_dev, _vga1Gain)) != 0)
                throw new ApplicationException(String.Format("bladerf_set_rxvga1() error. {0}", NativeMethods.bladerf_strerror(error)));
            if ((error = NativeMethods.bladerf_set_rxvga2(_dev, _vga2Gain)) != 0)
                throw new ApplicationException(String.Format("bladerf_set_rxvga2() error. {0}", NativeMethods.bladerf_strerror(error)));
            if (_xb200_enabled)
            {
                if ((error = NativeMethods.bladerf_xb200_attach(_dev)) != 0)
                    throw new ApplicationException(String.Format("bladerf_xb200_attach() error. {0}", NativeMethods.bladerf_strerror(error)));
                if (_xb200_filter == bladerf_xb200_filter.BLADERF_XB200_AUTO)
                {
                    XB200AdjustFilterBank();
                }
                else
                {
                    if ((error = NativeMethods.bladerf_xb200_set_filterbank(_dev, bladerf_module.BLADERF_MODULE_RX, _xb200_filter)) != 0)
                    {
                        if (_xb200_filter == bladerf_xb200_filter.BLADERF_XB200_AUTO_1DB || _xb200_filter == bladerf_xb200_filter.BLADERF_XB200_AUTO_3DB)
                            XB200AdjustFilterBank();
                        else
                            throw new ApplicationException(String.Format("bladerf_xb200_set_filterbank() error. {0}", NativeMethods.bladerf_strerror(error)));
                    }
                }
            }
            if ((error = NativeMethods.bladerf_set_frequency(_dev, bladerf_module.BLADERF_MODULE_RX, (uint)_centerFrequency)) != 0)
                throw new ApplicationException(String.Format("bladerf_set_frequency() error. {0}", NativeMethods.bladerf_strerror(error)));
            if (_version.major == 0 && _version.minor < 14)
            {
                throw new ApplicationException(String.Format("libbladerf is too old. Got {0} and expecting at least v0.14", _version.describe));
            }
            else
            {
                // new sync interface
                _sampleThread = new Thread(ReceiveSamples_sync);
                _sampleThread.Name = "bladerf_samples_rx";
                _sampleThread.Priority = ThreadPriority.Highest;
                _isStreaming = true;
                _sampleThread.Start();
                bool ready = false;
                while (!ready)
                {
                    lock (syncLock)
                    {
                        ready = _RXConfigured;
                    }
                }
                if ((error = NativeMethods.bladerf_enable_module(_dev, bladerf_module.BLADERF_MODULE_RX, 1)) != 0)
                    throw new ApplicationException(String.Format("bladerf_enable_module() error. {0}", NativeMethods.bladerf_strerror(error)));
            }
        }

        public void Stop()
        {
            if (_isStreaming)
            {
                int error;
                _isStreaming = false;
                if ((error = NativeMethods.bladerf_enable_module(_dev, bladerf_module.BLADERF_MODULE_RX, 0)) != 0)
                    throw new ApplicationException(String.Format("Disabling RX module failed: {0}", NativeMethods.bladerf_strerror(error)));
            }
            
            if (_sampleThread != null)
            {
                if (_sampleThread.ThreadState == ThreadState.Running)
                    _sampleThread.Join();
                _sampleThread = null;
            }

            if (_dev != IntPtr.Zero)
            {
                NativeMethods.bladerf_close(_dev);
                _dev = IntPtr.Zero;
            }
        }

        private unsafe void ComplexSamplesAvailable(Complex* buffer, int length)
        {
            if (SamplesAvailable != null)
            {
                _eventArgs.Buffer = buffer;
                _eventArgs.Length = length;
                SamplesAvailable(this, _eventArgs);
            }
        }

        private sealed class FrequencyRange
        {
            private long _min, _max;
            public FrequencyRange(long min, long max)
            {
                _min = min;
                _max = max;
            }

            public long Min
            {
                get { return _min; }
                set { _min = value; }
            }

            public long Max
            {
                get { return _max; }
                set { _max = value; }
            }

            public bool contains(long frequency)
            {
                return (Min <= frequency) && (frequency <= Max);
            }
        }
    }

    public delegate void SamplesAvailableDelegate(object sender, SamplesAvailableEventArgs e);

    public unsafe sealed class SamplesAvailableEventArgs : EventArgs
    {
        public int Length { get; set; }
        public Complex* Buffer { get; set; }
    }
}
