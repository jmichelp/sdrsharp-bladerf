using System;
using System.Runtime.InteropServices;

namespace SDRSharp.BladeRF
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate IntPtr bladerf_stream_cb(IntPtr dev, IntPtr stream, ref bladerf_metadata meta, Int16* samples, uint num_samples, IntPtr user_data);

    #region Internal enums
    public enum bladerf_backend
    {
        BLADERF_BACKEND_ANY = 0,    /**< "Don't Care" -- use any available backend */
        BLADERF_BACKEND_LINUX = 1,  /**< Linux kernel driver */
        BLADERF_BACKEND_LIBUSB = 2, /**< libusb */
        BLADERF_BACKEND_CYPRESS = 3, /**< CyAPI */
        BLADERF_BACKEND_DUMMY = 100, /**< Dummy used for development purposes */
    }

    public enum bladerf_dev_speed
    {
        BLADERF_DEVICE_SPEED_UNKNOWN = 0,
        BLADERF_DEVICE_SPEED_HIGH = 1,
        BLADERF_DEVICE_SPEED_SUPER = 2,
    }

    public enum bladerf_error_codes
    {
        BLADERF_ERR_UNEXPECTED = -1,  /**< An unexpected failure occurred */
        BLADERF_ERR_RANGE = -2,  /**< Provided parameter is out of range */
        BLADERF_ERR_INVAL = -3,  /**< Invalid operation/parameter */
        BLADERF_ERR_MEM = -4,  /**< Memory allocation error */
        BLADERF_ERR_IO = -5,  /**< File/Device I/O error */
        BLADERF_ERR_TIMEOUT = -6,  /**< Operation timed out */
        BLADERF_ERR_NODEV = -7,  /**< No device(s) available */
        BLADERF_ERR_UNSUPPORTED = -8,  /**< Operation not supported */
        BLADERF_ERR_MISALIGNED = -9,  /**< Misaligned flash access */
        BLADERF_ERR_CHECKSUM = -10, /**< Invalid checksum */
        BLADERF_ERR_NO_FILE = -11, /**< File not found */
        BLADERF_ERR_UPDATE_FPGA = -12, /**< An FPGA update is required */
        BLADERF_ERR_UPDATE_FW = -13, /**< A firmware update is requied */
        BLADERF_ERR_TIME_PAST = -14, /**< Requested timestamp is in the past */
        BLADERF_ERR_QUEUE_FULL = -15, /**< Could not enqueue data into full queue */
        BLADERF_ERR_FPGA_OP = -16, /**< An FPGA operation reported failure */
        BLADERF_ERR_PERMISSION = -17 /**< Insufficient permissions for the requested operation */
    }

    public enum bladerf_module
    {
        BLADERF_MODULE_RX = 0,  /**< Receive Module */
        BLADERF_MODULE_TX = 1   /**< Transmit Module */
    }

    public enum bladerf_loopback
    {

        /**
         * Firmware loopback inside of the FX3
         */
        BLADERF_LB_FIRMWARE = 1,

        /**
         * Baseband loopback. TXLPF output is connected to the RXVGA2 input.
         */
        BLADERF_LB_BB_TXLPF_RXVGA2 = 2,

        /**
         * Baseband loopback. TXVGA1 output is connected to the RXVGA2 input.
         */
        BLADERF_LB_BB_TXVGA1_RXVGA2 = 3,

        /**
         * Baseband loopback. TXLPF output is connected to the RXLPF input.
         */
        BLADERF_LB_BB_TXLPF_RXLPF = 4,

        /**
         * Baseband loopback. TXVGA1 output is connected to RXLPF input.
         */
        BLADERF_LB_BB_TXVGA1_RXLPF = 5,

        /**
         * RF loopback. The TXMIX output, through the AUX PA, is connected to the
         * output of LNA1.
         */
        BLADERF_LB_RF_LNA1 = 6,


        /**
         * RF loopback. The TXMIX output, through the AUX PA, is connected to the
         * output of LNA2.
         */
        BLADERF_LB_RF_LNA2 = 7,

        /**
         * RF loopback. The TXMIX output, through the AUX PA, is connected to the
         * output of LNA3.
         */
        BLADERF_LB_RF_LNA3 = 8,

        /**
         * Disables loopback and returns to normal operation.
         */
        BLADERF_LB_NONE = 9
    }

    public enum bladerf_sampling
    {
        BLADERF_SAMPLING_UNKNOWN = 0,  /**< Unable to determine connection type */
        BLADERF_SAMPLING_INTERNAL = 1, /**< Sample from RX/TX connector */
        BLADERF_SAMPLING_EXTERNAL = 2  /**< Sample from J60 or J61 */
    }

    public enum bladerf_lna_gain
    {
        BLADERF_LNA_GAIN_UNKNOWN = 0,    /**< Invalid LNA gain */
        BLADERF_LNA_GAIN_BYPASS = 1,     /**< LNA bypassed - 0dB gain */
        BLADERF_LNA_GAIN_MID = 2,        /**< LNA Mid Gain (MAX-6dB) */
        BLADERF_LNA_GAIN_MAX = 3         /**< LNA Max Gain */
    }

    public enum bladerf_lpf_mode
    {
        BLADERF_LPF_NORMAL = 0,     /**< LPF connected and enabled */
        BLADERF_LPF_BYPASSED = 1,   /**< LPF bypassed */
        BLADERF_LPF_DISABLED = 2    /**< LPF disabled */
    }

    public enum bladerf_cal_module
    {
        BLADERF_DC_CAL_LPF_TUNING = 0,
        BLADERF_DC_CAL_TX_LPF = 1,
        BLADERF_DC_CAL_RX_LPF = 2,
        BLADERF_DC_CAL_RXVGA2 = 3
    }

    public enum bladerf_correction
    {
        /**
         * Adjusts the in-phase DC offset via controls provided by the LMS6002D
         * front end. Valid values are [-2048, 2048], which are scaled to the
         * available control bits in the LMS device.
         */
        BLADERF_CORR_LMS_DCOFF_I = 0,

        /**
         * Adjusts the quadrature DC offset via controls provided the LMS6002D
         * front end. Valid values are [-2048, 2048], which are scaled to the
         * available control bits.
         */
        BLADERF_CORR_LMS_DCOFF_Q = 1,

        /**
         * Adjusts FPGA-based phase correction of [-10, 10] degrees, via a provided
         * count value of [-4096, 4096].
         */
        BLADERF_CORR_FPGA_PHASE = 2,

        /**
         * Adjusts FPGA-based gain correction of [0.0, 2.0], via provided
         * values in the range of [-4096, 4096], where a value of 0 corresponds to
         * a gain of 1.0.
         */
        BLADERF_CORR_FPGA_GAIN = 3
    }

    public enum bladerf_format
    {
        BLADERF_FORMAT_SC16_Q11 = 0, /**< Signed, Complex 16-bit Q11.*/
        BLADERF_FORMAT_SC16_Q11_META = 1
    }

    public enum bladerf_xb
    {
        BLADERF_XB_NONE = 0,
        BLADERF_XB_100 = 1,
        BLADERF_XB_200 = 2
    }

    public enum bladerf_xb200_filter
    {
        BLADERF_XB200_50M = 0,
        BLADERF_XB200_144M = 1,
        BLADERF_XB200_222M = 2,
        BLADERF_XB200_CUSTOM = 3,
        BLADERF_XB200_AUTO_1DB = 4,
        BLADERF_XB200_AUTO_3DB = 5,
        BLADERF_XB200_AUTO = -1
    }

    public enum bladerf_log_level
    {
        BLADERF_LOG_LEVEL_VERBOSE = 0,  /**< Verbose level logging */
        BLADERF_LOG_LEVEL_DEBUG = 1,    /**< Debug level logging */
        BLADERF_LOG_LEVEL_INFO = 2,     /**< Information level logging */
        BLADERF_LOG_LEVEL_WARNING = 3,  /**< Warning level logging */
        BLADERF_LOG_LEVEL_ERROR = 4,    /**< Error level logging */
        BLADERF_LOG_LEVEL_CRITICAL = 5, /**< Fatal error level logging */
        BLADERF_LOG_LEVEL_SILENT = 6    /**< No output */
    }

    public enum bladerf_xb200_path
    {
        BLADERF_XB200_BYPASS = 0, /**< Bypass the XB-200 mixer */
        BLADERF_XB200_MIX = 1 /**< Pass signals through the XB-200 mixer */
    }
    #endregion

    #region Internal structs
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct bladerf_version
    {
        public UInt16 major;
        public UInt16 minor;
        public UInt16 patch;
        private IntPtr describePtr;
        public string describe
        {
            get { return Marshal.PtrToStringAnsi(describePtr); }
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct bladerf_metadata
    {
        public UInt64 timestamp;
        public UInt32 flags;
        public UInt32 status;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct bladerf_devinfo
    {
        public bladerf_backend backend;
        public fixed sbyte serial[33];
        public byte usb_bus;
        public byte usb_addr;
        public uint instance;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct bladerf_rational_rate
    {
        public UInt64 integer;
        public UInt64 num;
        public UInt64 denom;
    }
    #endregion


    class NativeMethods
    {
        private const string LibBladeRF = "bladerf";

        [DllImport("bladerf", EntryPoint = "bladerf_version", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_version_native(ref bladerf_version version);

        public static bladerf_version bladerf_version()
        {
            bladerf_version ret = new bladerf_version();
            bladerf_version_native(ref ret);
            return ret;
        }

        [DllImport("bladerf", EntryPoint = "bladerf_fpga_version", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_fpga_version_native(IntPtr dev, ref bladerf_version version);

        public static bladerf_version bladerf_fpga_version(IntPtr dev)
        {
            bladerf_version ret = new bladerf_version();
            bladerf_fpga_version_native(dev, ref ret);
            return ret;
        }

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_calibrate_dc(IntPtr dev, bladerf_cal_module module);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_init_stream(ref IntPtr stream, IntPtr dev, bladerf_stream_cb callback, out IntPtr buffers, uint num_buffers, bladerf_format format, uint num_samples, uint num_transfers, IntPtr user_data);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_stream(IntPtr stream, bladerf_module module);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_deinit_stream(IntPtr stream);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_sync_config(IntPtr dev, bladerf_module module, bladerf_format format, uint num_buffers, uint buffer_size, uint num_transfers, uint stream_timeout);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_sync_rx(IntPtr dev, Int16* samples, uint num_samples, IntPtr metadata, uint timeout_ms);

        [DllImport("bladerf", EntryPoint = "bladerf_rx", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_rx_native(IntPtr dev, bladerf_format format, ref Int16[] samples, uint num_samples, ref bladerf_metadata metadata);

        public static int bladerf_rx(IntPtr dev, ref Int16[] samples, uint num_samples, uint timeout_ms)
        {
            bladerf_metadata meta = new bladerf_metadata();
            return bladerf_rx_native(dev, bladerf_format.BLADERF_FORMAT_SC16_Q11, ref samples, num_samples, ref meta);
        }

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_device_list(out IntPtr devices);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_free_device_list(IntPtr devices);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_open(out IntPtr device, string device_identifier);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_close(IntPtr device);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_enable_module(IntPtr dev, bladerf_module m, int enable);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_loopback(IntPtr dev, bladerf_loopback l);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int bladerf_get_loopback(IntPtr dev, out bladerf_loopback l);

        public static int bladerf_set_sample_rate(IntPtr dev, bladerf_module module, double rate, out double actual)
        {
            bladerf_rational_rate rational_rate = new bladerf_rational_rate();
            bladerf_rational_rate rational_actual = new bladerf_rational_rate();
            rational_rate.integer = (UInt32) rate;
            rational_rate.denom = 10000;
            rational_rate.num = (UInt64) ((rate - rational_rate.integer) * rational_rate.denom);
            actual = rational_rate.integer + rational_rate.num / (double)rational_rate.denom;
            int ret = bladerf_set_rational_sample_rate(dev, module, ref rational_rate, out rational_actual);
            if (ret == 0)
            {
                actual = rational_actual.integer + rational_actual.num / (double)rational_actual.denom;
            }
            return ret;
        }

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int bladerf_set_rational_sample_rate(IntPtr dev, bladerf_module module, ref bladerf_rational_rate rate, out bladerf_rational_rate actual);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_sampling(IntPtr dev, bladerf_sampling sampling);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int bladerf_get_sampling(IntPtr dev, out bladerf_sampling sampling);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_sample_rate(IntPtr dev, bladerf_module module, out uint rate);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int bladerf_get_rational_sample_rate(IntPtr dev, bladerf_module module, out bladerf_rational_rate rate);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_correction(IntPtr dev, bladerf_module module, bladerf_correction corr, Int16 value);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_correction(IntPtr dev, bladerf_module module, bladerf_correction corr, out Int16 value);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_lna_gain(IntPtr dev, bladerf_lna_gain gain);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_lna_gain(IntPtr dev, out bladerf_lna_gain gain);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_rxvga1(IntPtr dev, int gain);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rxvga1(IntPtr dev, out int gain);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_rxvga2(IntPtr dev, int gain);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rxvga2(IntPtr dev, out int gain);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_bandwidth(IntPtr dev, bladerf_module module, uint bandwidth, out uint actual);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_bandwidth(IntPtr dev, bladerf_module module, out uint bandwidth);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_lpf_mode(IntPtr dev, bladerf_module module, bladerf_lpf_mode mode);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_lpf_mode(IntPtr dev, bladerf_module module, out bladerf_lpf_mode mode);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_select_band(IntPtr dev, bladerf_module module, uint frequency);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_frequency(IntPtr dev, bladerf_module module, uint frequency);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_frequency(IntPtr dev, bladerf_module module, out uint frequency);

        [DllImport("bladerf", EntryPoint = "bladerf_get_serial", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_get_serial(IntPtr dev, out string serial);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_load_fpga(IntPtr dev, string fpga);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_is_fpga_configured(IntPtr dev);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "bladerf_strerror")]
        public static extern IntPtr bladerf_strerror_native(int error);

        public static string bladerf_strerror(int error)
        {
            IntPtr ret = bladerf_strerror_native(error);
            if (ret != IntPtr.Zero)
                return Marshal.PtrToStringAnsi(ret);
            return String.Empty;
        }

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern bladerf_dev_speed bladerf_device_speed(IntPtr dev);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_expansion_attach(IntPtr dev, bladerf_xb xb);

        public static int bladerf_xb200_attach(IntPtr dev)
        {
            return bladerf_expansion_attach(dev, bladerf_xb.BLADERF_XB_200);
        }

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb200_set_filterbank(IntPtr dev, bladerf_module mod, bladerf_xb200_filter filter);

        [DllImport("bladerf", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_log_set_verbosity(bladerf_log_level level);

        public static string backend_to_str(bladerf_backend backend)
        {
            switch (backend)
            {
                case bladerf_backend.BLADERF_BACKEND_CYPRESS:
                    return "CyUSB";
                case bladerf_backend.BLADERF_BACKEND_DUMMY:
                    return "Dummy";
                case bladerf_backend.BLADERF_BACKEND_LIBUSB:
                    return "libusb";
                case bladerf_backend.BLADERF_BACKEND_LINUX:
                    return "Linux";
                case bladerf_backend.BLADERF_BACKEND_ANY:
                    return "any";
            }
            return "Unknown";
        }

        public static string speed_to_str(bladerf_dev_speed speed)
        {
            switch (speed)
            {
                case bladerf_dev_speed.BLADERF_DEVICE_SPEED_HIGH:
                    return "H";
                case bladerf_dev_speed.BLADERF_DEVICE_SPEED_SUPER:
                    return "S";
                case bladerf_dev_speed.BLADERF_DEVICE_SPEED_UNKNOWN:
                    return "?";
            }
            return "-";
        }
    }
}
