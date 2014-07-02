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
        private string DeviceName = "BladeRF";
        private string _serial;
        private static readonly unsafe float* _lutPtr;
        private static readonly UnsafeBuffer _lutBuffer = UnsafeBuffer.Create(4096, sizeof(float));
        private IntPtr _dev;
        private long _centerFrequency = DefaultFrequency;
        private double _sampleRate = DefaultSamplerate;
        private bool _isFpgaLoaded = false;
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
        private static readonly uint _readLength = (uint)Utils.GetIntSetting("BladeRFBufferLength", 16 * 1024);
        private Thread _sampleThread = null;
        private static bladerf_version _version = NativeMethods.bladerf_version();

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
                    uint tmp = 0;
                    NativeMethods.bladerf_set_bandwidth(_dev, bladerf_module.BLADERF_MODULE_RX, (uint)(_sampleRate * 0.75), out tmp);

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
                _centerFrequency = value;
                if (_dev != IntPtr.Zero)
                {
                    NativeMethods.bladerf_set_frequency(_dev, bladerf_module.BLADERF_MODULE_RX, (uint)_centerFrequency);
                    uint real_freq;
                    NativeMethods.bladerf_get_frequency(_dev, bladerf_module.BLADERF_MODULE_RX, out real_freq);
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
            const float scale = 1.0f / 2047.0f;
            for (int i = 0; i < 4096; ++i)
            {
                _lutPtr[i] = (i - 2048) * scale;
            }
        }

        public BladeRFDevice(string serial = "")
        {
            _isFpgaLoaded = false;
            string devspec = "";
            if (serial != "")
                devspec = String.Format("libusb:serial={0}", serial);
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
            Int16[] samples = new Int16[2 * _readLength];
            if (_iqBuffer == null || _iqBuffer.Length != _readLength)
            {
                _iqBuffer = UnsafeBuffer.Create((int)_readLength, sizeof(Complex));
                _iqPtr = (Complex*)_iqBuffer;
            }
            int status = 0;
            while (status == 0 && this._isStreaming)
            {
                try
                {
                    status = NativeMethods.bladerf_sync_rx(_dev, ref samples, _readLength, (uint)3500);
                    if (status != 0)
                        throw new ApplicationException(String.Format("bladerf_rx() error. {0}", NativeMethods.bladerf_strerror(status)));
                    var ptrIq = _iqPtr;
                    for (int i = 0; i < 2 * _readLength; )
                    {
                        ptrIq->Imag = _lutPtr[samples[i] & 0x0fff];
                        i++;
                        ptrIq->Real = _lutPtr[samples[i] & 0x0fff];
                        i++;
                        ptrIq++;
                    }
                    ComplexSamplesAvailable(_iqPtr, _iqBuffer.Length);
                }
                catch
                {
                    _isStreaming = false;
                    break;
                }
            }
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
            uint tmp = 0;
            if ((error = NativeMethods.bladerf_set_bandwidth(_dev, bladerf_module.BLADERF_MODULE_RX, (uint) (_sampleRate * 0.75), out tmp)) != 0)
                throw new ApplicationException(String.Format("bladerf_set_bandwidth() error. {0}", NativeMethods.bladerf_strerror(error)));
            if ((error = NativeMethods.bladerf_set_loopback(_dev, bladerf_loopback.BLADERF_LB_NONE)) != 0)
                throw new ApplicationException(String.Format("bladerf_set_loopback() error. {0}", NativeMethods.bladerf_strerror(error)));
            if ((error = NativeMethods.bladerf_set_sampling(_dev, _sampling)) != 0)
                throw new ApplicationException(String.Format("bladerf_sampling() error. {0}", NativeMethods.bladerf_strerror(error)));
            if ((error = NativeMethods.bladerf_set_lna_gain(_dev, _lnaGain)) != 0)
                throw new ApplicationException(String.Format("bladerf_set_lna_gain() error. {0}", NativeMethods.bladerf_strerror(error)));
            if ((error = NativeMethods.bladerf_set_rxvga1(_dev, _vga1Gain)) != 0)
                throw new ApplicationException(String.Format("bladerf_set_rxvga1() error. {0}", NativeMethods.bladerf_strerror(error)));
            if ((error = NativeMethods.bladerf_set_rxvga2(_dev, _vga2Gain)) != 0)
                throw new ApplicationException(String.Format("bladerf_set_rxvga2() error. {0}", NativeMethods.bladerf_strerror(error)));
            if ((error = NativeMethods.bladerf_set_frequency(_dev, bladerf_module.BLADERF_MODULE_RX, (uint)_centerFrequency)) != 0)
                throw new ApplicationException(String.Format("bladerf_set_frequency() error. {0}", NativeMethods.bladerf_strerror(error)));
            if (_version.major == 0 && _version.minor < 14)
            {
                throw new ApplicationException(String.Format("libbladerf is too old. Got {0} and expecting at least v0.14", _version.describe));
            }
            else
            {
                // new sync interface
                if ((error = NativeMethods.bladerf_sync_config(_dev, bladerf_module.BLADERF_MODULE_RX, bladerf_format.BLADERF_FORMAT_SC16_Q11, 64, 4 * _readLength, 16, 3500)) != 0)
                    throw new ApplicationException(String.Format("bladerf_sync_config() error. {0}", NativeMethods.bladerf_strerror(error)));
                _sampleThread = new Thread(ReceiveSamples_sync);
                _sampleThread.Name = "bladerf_samples_rx";
                _sampleThread.Start();
                if ((error = NativeMethods.bladerf_enable_module(_dev, bladerf_module.BLADERF_MODULE_RX, 1)) != 0)
                    throw new ApplicationException(String.Format("bladerf_enable_module() error. {0}", NativeMethods.bladerf_strerror(error)));
            }

            _isStreaming = true;
        }

        public void Stop()
        {
            if (_isStreaming)
            {
                int error;
                if ((error = NativeMethods.bladerf_enable_module(_dev, bladerf_module.BLADERF_MODULE_RX, 0)) != 0)
                    throw new ApplicationException(String.Format("Disabling RX module failed: {0}", NativeMethods.bladerf_strerror(error)));
            }
            _isStreaming = false;
            
            if (_sampleThread != null)
            {
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
    }

    public delegate void SamplesAvailableDelegate(object sender, SamplesAvailableEventArgs e);

    public unsafe sealed class SamplesAvailableEventArgs : EventArgs
    {
        public int Length { get; set; }
        public Complex* Buffer { get; set; }
    }
}
