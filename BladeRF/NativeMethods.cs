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
        BLADERF_ERR_PERMISSION = -17, /**< Insufficient permissions for the requested operation */
        BLADERF_ERR_WOULD_BLOCK = -18 /**< Operation would block, but has been
                                       *   requested to be non-blocking. This
                                       *   indicates to a caller that it may
                                       *   need to retry the operation later.
                                       */
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
        BLADERF_XB_200 = 2,
        BLADERF_XB_300 = 3
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

    public enum bladerf_rx_mux
    {
        /**
         * Invalid RX Mux mode selection
         */
        BLADERF_RX_MUX_INVALID = -1,

        /**
         * Read baseband samples from the LMS6002D. This is the default mode
         * of operation.
         */
        BLADERF_RX_MUX_BASEBAND_LMS = 0,

        /**
         * Read samples from 12 bit counters.
         *
         * The I channel counts up while the Q channel counts down.
         */
        BLADERF_RX_MUX_12BIT_COUNTER = 1,

        /**
         * Read samples from a 32 bit up-counter.
         *
         * I and Q form a little-endian value.
         */
        BLADERF_RX_MUX_32BIT_COUNTER = 2,

        /* RX_MUX setting 0x3 is reserved for future use */

        /**
         * Read samples from the baseband TX input to the FPGA (from the host)
         */
        BLADERF_RX_MUX_DIGITAL_LOOPBACK = 4
    }

    public enum bladerf_xb300_trx
    {
        BLADERF_XB300_TRX_INVAL = -1,  /**< Invalid TRX selection */
        BLADERF_XB300_TRX_TX = 0,      /**< TRX antenna operates as TX */
        BLADERF_XB300_TRX_RX = 1,      /**< TRX antenna operates as RX */
        BLADERF_XB300_TRX_UNSET = 2    /**< TRX antenna unset */
    }

    public enum bladerf_xb300_amplifier
    {
        BLADERF_XB300_AMP_INVAL = -1,   /**< Invalid amplifier selection */
        BLADERF_XB300_AMP_PA = 0,       /**< TX Power amplifier */
        BLADERF_XB300_AMP_LNA = 1,      /**< RX LNA */
        BLADERF_XB300_AMP_PA_AUX = 2    /**< Auxillary Power amplifier */
    }

    public enum bladerf_vctcxo_tamer_mode
    {
        /** Denotes an invalid selection or state */
        BLADERF_VCTCXO_TAMER_INVALID = -1,

        /** Do not attempt to tame the VCTCXO with an input source. */
        BLADERF_VCTCXO_TAMER_DISABLED = 0,

        /** Use a 1 pps input source to tame the VCTCXO. */
        BLADERF_VCTCXO_TAMER_1_PPS = 1,

        /** Use a 10 MHz input source to tame the VCTCXO. */
        BLADERF_VCTCXO_TAMER_10_MHZ = 2
    }

    public enum bladerf_gpio
    {
        BLADERF_XB_GPIO_01 = 0x00000001,
        BLADERF_XB_GPIO_02 = 0x00000002,
        BLADERF_XB_GPIO_03 = 0x00000004,
        BLADERF_XB_GPIO_04 = 0x00000008,
        BLADERF_XB_GPIO_05 = 0x00000010,
        BLADERF_XB_GPIO_06 = 0x00000020,
        BLADERF_XB_GPIO_07 = 0x00000040,
        BLADERF_XB_GPIO_08 = 0x00000080,
        BLADERF_XB_GPIO_09 = 0x00000100,
        BLADERF_XB_GPIO_10 = 0x00000200,
        BLADERF_XB_GPIO_11 = 0x00000400,
        BLADERF_XB_GPIO_12 = 0x00000800,
        BLADERF_XB_GPIO_13 = 0x00001000,
        BLADERF_XB_GPIO_14 = 0x00002000,
        BLADERF_XB_GPIO_15 = 0x00004000,
        BLADERF_XB_GPIO_16 = 0x00008000,
        BLADERF_XB_GPIO_17 = 0x00010000,
        BLADERF_XB_GPIO_18 = 0x00020000,
        BLADERF_XB_GPIO_19 = 0x00040000,
        BLADERF_XB_GPIO_20 = 0x00080000,
        BLADERF_XB_GPIO_21 = 0x00100000,
        BLADERF_XB_GPIO_22 = 0x00200000,
        BLADERF_XB_GPIO_23 = 0x00400000,
        BLADERF_XB_GPIO_24 = 0x00800000,
        BLADERF_XB_GPIO_25 = 0x01000000,
        BLADERF_XB_GPIO_26 = 0x02000000,
        BLADERF_XB_GPIO_27 = 0x04000000,
        BLADERF_XB_GPIO_28 = 0x08000000,
        BLADERF_XB_GPIO_29 = 0x10000000,
        BLADERF_XB_GPIO_30 = 0x20000000,
        BLADERF_XB_GPIO_31 = 0x40000000,
        BLADERF_XB_GPIO_32 = 0x80000000,

        /* XB-200 GPIO */
        BLADERF_XB200_PIN_J7_1 = BLADERF_XB_GPIO_10,
        BLADERF_XB200_PIN_J7_2 = BLADERF_XB_GPIO_11,
        BLADERF_XB200_PIN_J7_5 = BLADERF_XB_GPIO_08,
        BLADERF_XB200_PIN_J7_6 = BLADERF_XB_GPIO_09,
        BLADERF_XB200_PIN_J13_1 = BLADERF_XB_GPIO_17,
        BLADERF_XB200_PIN_J13_2 = BLADERF_XB_GPIO_18,
        BLADERF_XB200_PIN_J16_1 = BLADERF_XB_GPIO_31,
        BLADERF_XB200_PIN_J16_2 = BLADERF_XB_GPIO_32,
        BLADERF_XB200_PIN_J16_3 = BLADERF_XB_GPIO_19,
        BLADERF_XB200_PIN_J16_4 = BLADERF_XB_GPIO_20,
        BLADERF_XB200_PIN_J16_5 = BLADERF_XB_GPIO_21,
        BLADERF_XB200_PIN_J16_6 = BLADERF_XB_GPIO_24,

        /* XB-100 GPIO */
        BLADERF_XB100_PIN_J2_3 = BLADERF_XB_GPIO_07,
        BLADERF_XB100_PIN_J2_4 = BLADERF_XB_GPIO_08,
        BLADERF_XB100_PIN_J3_3 = BLADERF_XB_GPIO_09,
        BLADERF_XB100_PIN_J3_4 = BLADERF_XB_GPIO_10,
        BLADERF_XB100_PIN_J4_3 = BLADERF_XB_GPIO_11,
        BLADERF_XB100_PIN_J4_4 = BLADERF_XB_GPIO_12,
        BLADERF_XB100_PIN_J5_3 = BLADERF_XB_GPIO_13,
        BLADERF_XB100_PIN_J5_4 = BLADERF_XB_GPIO_14,
        BLADERF_XB100_PIN_J11_2 = BLADERF_XB_GPIO_05,
        BLADERF_XB100_PIN_J11_3 = BLADERF_XB_GPIO_04,
        BLADERF_XB100_PIN_J11_4 = BLADERF_XB_GPIO_03,
        BLADERF_XB100_PIN_J11_5 = BLADERF_XB_GPIO_06,
        BLADERF_XB100_PIN_J12_2 = BLADERF_XB_GPIO_01,
        BLADERF_XB100_PIN_J12_5 = BLADERF_XB_GPIO_02,
        BLADERF_XB100_LED_D1 = BLADERF_XB_GPIO_24,
        BLADERF_XB100_LED_D2 = BLADERF_XB_GPIO_32,
        BLADERF_XB100_LED_D3 = BLADERF_XB_GPIO_30,
        BLADERF_XB100_LED_D4 = BLADERF_XB_GPIO_28,
        BLADERF_XB100_LED_D5 = BLADERF_XB_GPIO_23,
        BLADERF_XB100_LED_D6 = BLADERF_XB_GPIO_25,
        BLADERF_XB100_LED_D7 = BLADERF_XB_GPIO_31,
        BLADERF_XB100_LED_D8 = BLADERF_XB_GPIO_29,
        BLADERF_XB100_TLED_RED = BLADERF_XB_GPIO_22,
        BLADERF_XB100_TLED_GREEN = BLADERF_XB_GPIO_21,
        BLADERF_XB100_TLED_BLUE = BLADERF_XB_GPIO_20,
        BLADERF_XB100_DIP_SW1 = BLADERF_XB_GPIO_27,
        BLADERF_XB100_DIP_SW2 = BLADERF_XB_GPIO_26,
        BLADERF_XB100_DIP_SW3 = BLADERF_XB_GPIO_16,
        BLADERF_XB100_DIP_SW4 = BLADERF_XB_GPIO_15,
        BLADERF_XB100_BTN_J6 = BLADERF_XB_GPIO_19,
        BLADERF_XB100_BTN_J7 = BLADERF_XB_GPIO_18,
        BLADERF_XB100_BTN_J8 = BLADERF_XB_GPIO_17
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
    public unsafe struct bladerf_quick_tune
    {
        byte freqsel;   /**< Choice of VCO and VCO division factor */
        byte vcocap;    /**< VCOCAP value */
        UInt16 nint;    /**< Integer portion of LO frequency value */
        UInt32 nfrac;   /**< Fractional portion of LO frequency value */
        byte flags;     /**< Flag bits used internally by libbladeRF */
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct bladerf_metadata
    {
        public UInt64 timestamp;
        public UInt32 flags;
        public UInt32 status;
        public UInt32 actual_count;
        public fixed byte reserved[32];
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
        private const string LibBladeRF = "bladeRF";

        [DllImport("bladeRF", EntryPoint = "bladerf_version", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_version_native(ref bladerf_version version);

        public static bladerf_version bladerf_version()
        {
            bladerf_version ret = new bladerf_version();
            bladerf_version_native(ref ret);
            return ret;
        }

        [DllImport("bladeRF", EntryPoint = "bladerf_fw_version", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_fw_version_native(IntPtr dev, ref bladerf_version version);

        public static bladerf_version bladerf_fw_version(IntPtr dev)
        {
            bladerf_version ret = new bladerf_version();
            bladerf_fw_version_native(dev, ref ret);
            return ret;
        }

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_rx_mux(IntPtr dev, bladerf_rx_mux mux);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_get_rx_mux(IntPtr dec, out bladerf_rx_mux mode);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_set_rational_smb_frequency(IntPtr dev, ref bladerf_rational_rate rate, out bladerf_rational_rate actual);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_set_smb_frequency(IntPtr dev, UInt32 rate, out UInt32 actual);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_get_rational_smb_frequency(IntPtr dev, out bladerf_rational_rate rate);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_get_smb_frequency(IntPtr dev, out UInt32 rate);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_set_usb_reset_on_open(bool enabled);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_device_reset(IntPtr dev);

        [DllImport("bladeRF", EntryPoint = "bladerf_fpga_version", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_fpga_version_native(IntPtr dev, ref bladerf_version version);

        public static bladerf_version bladerf_fpga_version(IntPtr dev)
        {
            bladerf_version ret = new bladerf_version();
            bladerf_fpga_version_native(dev, ref ret);
            return ret;
        }

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_calibrate_dc(IntPtr dev, bladerf_cal_module module);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_init_stream(ref IntPtr stream, IntPtr dev, bladerf_stream_cb callback, out IntPtr buffers, uint num_buffers, bladerf_format format, uint num_samples, uint num_transfers, IntPtr user_data);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_stream(IntPtr stream, bladerf_module module);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_deinit_stream(IntPtr stream);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_sync_config(IntPtr dev, bladerf_module module, bladerf_format format, uint num_buffers, uint buffer_size, uint num_transfers, uint stream_timeout);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_sync_rx(IntPtr dev, Int16* samples, uint num_samples, IntPtr metadata, uint timeout_ms);

        [DllImport("bladeRF", EntryPoint = "bladerf_rx", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_rx_native(IntPtr dev, bladerf_format format, ref Int16[] samples, uint num_samples, ref bladerf_metadata metadata);

        public static int bladerf_rx(IntPtr dev, ref Int16[] samples, uint num_samples, uint timeout_ms)
        {
            bladerf_metadata meta = new bladerf_metadata();
            return bladerf_rx_native(dev, bladerf_format.BLADERF_FORMAT_SC16_Q11, ref samples, num_samples, ref meta);
        }

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_device_list(out IntPtr devices);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_free_device_list(IntPtr devices);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_open(out IntPtr device, string device_identifier);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_close(IntPtr device);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_enable_module(IntPtr dev, bladerf_module m, int enable);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_loopback(IntPtr dev, bladerf_loopback l);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
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

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int bladerf_set_rational_sample_rate(IntPtr dev, bladerf_module module, ref bladerf_rational_rate rate, out bladerf_rational_rate actual);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_sampling(IntPtr dev, bladerf_sampling sampling);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int bladerf_get_sampling(IntPtr dev, out bladerf_sampling sampling);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_sample_rate(IntPtr dev, bladerf_module module, out uint rate);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int bladerf_get_rational_sample_rate(IntPtr dev, bladerf_module module, out bladerf_rational_rate rate);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_correction(IntPtr dev, bladerf_module module, bladerf_correction corr, Int16 value);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_correction(IntPtr dev, bladerf_module module, bladerf_correction corr, out Int16 value);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_lna_gain(IntPtr dev, bladerf_lna_gain gain);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_lna_gain(IntPtr dev, out bladerf_lna_gain gain);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_rxvga1(IntPtr dev, int gain);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rxvga1(IntPtr dev, out int gain);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_rxvga2(IntPtr dev, int gain);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rxvga2(IntPtr dev, out int gain);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_bandwidth(IntPtr dev, bladerf_module module, uint bandwidth, out uint actual);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_bandwidth(IntPtr dev, bladerf_module module, out uint bandwidth);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_lpf_mode(IntPtr dev, bladerf_module module, bladerf_lpf_mode mode);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_lpf_mode(IntPtr dev, bladerf_module module, out bladerf_lpf_mode mode);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_select_band(IntPtr dev, bladerf_module module, uint frequency);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_frequency(IntPtr dev, bladerf_module module, uint frequency);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_frequency(IntPtr dev, bladerf_module module, out uint frequency);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_get_quick_tune(IntPtr dev, bladerf_module module, out bladerf_quick_tune quick_tune);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_get_serial(IntPtr dev, out string serial);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_load_fpga(IntPtr dev, string fpga);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_is_fpga_configured(IntPtr dev);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl, EntryPoint = "bladerf_strerror")]
        public static extern IntPtr bladerf_strerror_native(int error);

        public static string bladerf_strerror(int error)
        {
            IntPtr ret = bladerf_strerror_native(error);
            if (ret != IntPtr.Zero)
                return Marshal.PtrToStringAnsi(ret);
            return String.Empty;
        }

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern bladerf_dev_speed bladerf_device_speed(IntPtr dev);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_expansion_attach(IntPtr dev, bladerf_xb xb);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_expansion_get_attached(IntPtr dev, out bladerf_xb xb);

        public static int bladerf_xb100_attach(IntPtr dev)
        {
            return bladerf_expansion_attach(dev, bladerf_xb.BLADERF_XB_100);
        }

        public static int bladerf_xb200_attach(IntPtr dev)
        {
            return bladerf_expansion_attach(dev, bladerf_xb.BLADERF_XB_200);
        }

        public static int bladerf_xb300_attach(IntPtr dev)
        {
            return bladerf_expansion_attach(dev, bladerf_xb.BLADERF_XB_300);
        }

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb200_set_filterbank(IntPtr dev, bladerf_module mod, bladerf_xb200_filter filter);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_xb200_get_filterbank(IntPtr dev, bladerf_module module, out bladerf_xb200_filter filter);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb200_set_path(IntPtr dev, bladerf_module module, bladerf_xb200_path path);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_xb200_get_path(IntPtr dev, bladerf_module module, out bladerf_xb200_path path);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb300_set_trx(IntPtr dev, bladerf_xb300_trx trx);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_xb300_get_trx(IntPtr dev, bladerf_xb300_trx trx);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb300_set_amplifier_enable(IntPtr dev, bladerf_xb300_amplifier amp, int enable);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb300_get_amplifier_enable(IntPtr dev, bladerf_xb300_amplifier amp, out int enable);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_xb300_get_output_power(IntPtr dev, out float val);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_vctcxo_tamer_mode(IntPtr dev, bladerf_vctcxo_tamer_mode mode);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_get_vctcxo_tamer_mode(IntPtr dev, out bladerf_vctcxo_tamer_mode mode);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_log_set_verbosity(bladerf_log_level level);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_expansion_gpio_read(IntPtr dev, out UInt32 val);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_expansion_gpio_write(IntPtr dev, UInt32 val);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_expansion_gpio_masked_write(IntPtr dev, UInt32 mask, UInt32 value);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_expansion_gpio_dir_read(IntPtr dev, out UInt32 outputs);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_expansion_gpio_dir_write(IntPtr dev, UInt32 outputs);

        [DllImport("bladeRF", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_expansion_gpio_dir_masked_write(IntPtr dev, UInt32 mask, UInt32 outputs);

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
