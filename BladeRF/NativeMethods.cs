using System;
using System.Runtime.InteropServices;
using System.Text;

namespace libbladeRF_wrapper
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate IntPtr bladerf_stream_cb(IntPtr dev, IntPtr stream, ref bladerf_metadata meta, Int16* samples, uint num_samples, IntPtr user_data);

    #region enums

    #region libbladeRF.h enums
    public enum bladerf_backend
    {
        BLADERF_BACKEND_ANY     = 0,  /* "Don't Care" -- use any available backend */
        BLADERF_BACKEND_LINUX   = 1,  /* Linux kernel driver */
        BLADERF_BACKEND_LIBUSB  = 2,  /* libusb */
        BLADERF_BACKEND_CYPRESS = 3,  /* CyAPI */
        BLADERF_BACKEND_DUMMY   = 100 /* Dummy used for development purposes */
    }
    public enum bladerf_fpga_size
    {
        BLADERF_FPGA_UNKNOWN  = 0,    /* Unable to determine FPGA variant */
        BLADERF_FPGA_40KLE    = 40,   /* 40 kLE FPGA */
        BLADERF_FPGA_115KLE   = 115,  /* 115 kLE FPGA */
        BLADERF_FPGA_A4       = 49,   /* 49 kLE FPGA (A4) */
        BLADERF_FPGA_A9       = 301   /* 301 kLE FPGA (A9) */
    }
    public enum bladerf_dev_speed
    {
        BLADERF_DEVICE_SPEED_UNKNOWN = 0,
        BLADERF_DEVICE_SPEED_HIGH    = 1,
        BLADERF_DEVICE_SPEED_SUPER   = 2
    }
    public enum bladerf_fpga_source
    {
        BLADERF_FPGA_SOURCE_UNKNOWN = 0, /* Uninitialized/invalid */
        BLADERF_FPGA_SOURCE_FLASH   = 1, /* Last FPGA load was from flash */
        BLADERF_FPGA_SOURCE_HOST    = 2  /* Last FPGA load was from host */
    }
    public enum bladerf_direction
    {
        BLADERF_RX = 0, /* Receive direction */
        BLADERF_TX = 1  /* Transmit direction */
    }
    public enum bladerf_channel_layout
    {
        BLADERF_RX_X1 = 0, /* x1 RX (SISO) */
        BLADERF_TX_X1 = 1, /* x1 TX (SISO) */
        BLADERF_RX_X2 = 2, /* x2 RX (MIMO) */
        BLADERF_TX_X2 = 3  /* x2 TX (MIMO) */
    }
    public enum bladerf_gain_mode
    {
        /** Device-specific default (automatic, when available)
         *
         * On the bladeRF x40 and x115 with FPGA versions >= v0.7.0, this is
         * automatic gain control.
         *
         * On the bladeRF 2.0 Micro, this is BLADERF_GAIN_SLOWATTACK_AGC with
         * reasonable default settings.
         */
        BLADERF_GAIN_DEFAULT        = 0, /* Automatic Gain Control */
        BLADERF_GAIN_AUTOMATIC      = 0,

        /** Manual gain control
         *
         * Available on all bladeRF models.
         */
        BLADERF_GAIN_MGC            = 1, /* Manual Gain Control */

        /** Automatic gain control, fast attack (advanced)
         *
         * Only available on the bladeRF 2.0 Micro. This is an advanced option, and
         * typically requires additional configuration for ideal performance.
         */
        BLADERF_GAIN_FASTATTACK_AGC = 2, /* Only available on bladeRF 2.0 Micro */

        /** Automatic gain control, slow attack (advanced)
         *
         * Only available on the bladeRF 2.0 Micro. This is an advanced option, and
         * typically requires additional configuration for ideal performance.
         */
        BLADERF_GAIN_SLOWATTACK_AGC = 3, /* Only available on bladeRF 2.0 Micro */

        /** Automatic gain control, hybrid attack (advanced)
         *
         * Only available on the bladeRF 2.0 Micro. This is an advanced option, and
         * typically requires additional configuration for ideal performance.
         */
        BLADERF_GAIN_HYBRID_AGC     = 4 /* Only available on bladeRF 2.0 Micro */
    }
    public enum bladerf_loopback
    {
        BLADERF_LB_NONE             = 0, /* Disables loopback and returns to normal operation. */
        BLADERF_LB_FIRMWARE         = 1, /* Firmware loopback inside of the FX3 */
        BLADERF_LB_BB_TXLPF_RXVGA2  = 2, /* Baseband loopback. TXLPF output is connected to the RXVGA2 input. */
        BLADERF_LB_BB_TXVGA1_RXVGA2 = 3, /* Baseband loopback. TXVGA1 output is connected to the RXVGA2 input. */
        BLADERF_LB_BB_TXLPF_RXLPF   = 4, /* Baseband loopback. TXLPF output is connected to the RXLPF input. */
        BLADERF_LB_BB_TXVGA1_RXLPF  = 5, /* Baseband loopback. TXVGA1 output is connected to RXLPF input. */
        BLADERF_LB_RF_LNA1          = 6, /* RF loopback. The TXMIX output, through the AUX PA, is connected to the output of LNA1.*/
        BLADERF_LB_RF_LNA2          = 7, /* RF loopback. The TXMIX output, through the AUX PA, is connected to the output of LNA2. */
        BLADERF_LB_RF_LNA3          = 8, /* RF loopback. The TXMIX output, through the AUX PA, is connected to the output of LNA3. */
        BLADERF_LB_RFIC_BIST        = 9  /* RFIC digital loopback (built-in self-test). */
    }
    public enum bladerf_trigger_role
    {
        BLADERF_TRIGGER_ROLE_INVALID = -1,  /* Invalid role selection */

        BLADERF_TRIGGER_ROLE_DISABLED = 0,  /* Triggering functionality is
                                             *   disabled on this device. Samples
                                             *   are not gated and the trigger
                                             *   signal is an input.
                                             */

        BLADERF_TRIGGER_ROLE_MASTER = 1,    /* This device is the trigger master.
                                             *   Its trigger signal will be an
                                             *   output and this device will
                                             *   determine when all devices shall
                                             *   trigger.
                                             */

        BLADERF_TRIGGER_ROLE_SLAVE = 2      /* This device is the trigger slave.
                                             *   This device's trigger signal will
                                             *   be an input and this devices will
                                             *   wait for the master's trigger
                                             *   signal assertion.
                                             */
    }
    public enum bladerf_trigger_signal
    {
        BLADERF_TRIGGER_INVALID     = -1, /* Invalid selection */
        BLADERF_TRIGGER_J71_4       = 0, /* J71 pin 4, mini_exp[1] on x40/x115 */
        BLADERF_TRIGGER_J51_1       = 1, /* J51 pin 1, mini_exp[1] on xA4/xA9 */
        BLADERF_TRIGGER_MINI_EXP_1  = 2, /* mini_exp[1], hardware-independent */

        BLADERF_TRIGGER_USER_0      = 128, /* Reserved for user SW/HW customizations */
        BLADERF_TRIGGER_USER_1      = 129, /* Reserved for user SW/HW customizations */
        BLADERF_TRIGGER_USER_2      = 130, /* Reserved for user SW/HW customizations */
        BLADERF_TRIGGER_USER_3      = 131, /* Reserved for user SW/HW customizations */
        BLADERF_TRIGGER_USER_4      = 132, /* Reserved for user SW/HW customizations */
        BLADERF_TRIGGER_USER_5      = 133, /* Reserved for user SW/HW customizations */
        BLADERF_TRIGGER_USER_6      = 134, /* Reserved for user SW/HW customizations */
        BLADERF_TRIGGER_USER_7      = 135  /* Reserved for user SW/HW customizations */
    }
    public enum bladerf_rx_mux
    {
        BLADERF_RX_MUX_INVALID          = -1, /* Invalid RX Mux mode selection */
        BLADERF_RX_MUX_BASEBAND_LMS     = 0, /* Read baseband samples from the LMS6002D. This is the default mode of operation. */
        BLADERF_RX_MUX_12BIT_COUNTER    = 1, /* Read samples from 12 bit counters. The I channel counts up while the Q channel counts down. */
        BLADERF_RX_MUX_32BIT_COUNTER    = 2, /* Read samples from a 32 bit up-counter. I and Q form a little-endian value. */
        BLADERF_RX_MUX_DIGITAL_LOOPBACK = 4 /* RX_MUX setting 0x3 is reserved for future use. Read samples from the baseband TX input to the FPGA (from the host). */
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
        /**
         * Signed, Complex 16-bit Q11. This is the native format of the DAC data.
         *
         * Values in the range [-2048, 2048) are used to represent [-1.0, 1.0).
         * Note that the lower bound here is inclusive, and the upper bound is
         * exclusive. Ensure that provided samples stay within [-2048, 2047].
         *
         * Samples consist of interleaved IQ value pairs, with I being the first
         * value in the pair. Each value in the pair is a right-aligned,
         * little-endian int16_t. The FPGA ensures that these values are
         * sign-extended.
         *
         * <pre>
         *  .--------------.--------------.
         *  | Bits 31...16 | Bits 15...0  |
         *  +--------------+--------------+
         *  |   Q[15..0]   |   I[15..0]   |
         *  `--------------`--------------`
         * </pre>
         *
         * When using this format the minimum required buffer size, in bytes, is:
         *
         * \f$
         *  buffer\_size\_min = (2 \times num\_samples \times num\_channels \times
         *                      sizeof(int16\_t))
         * \f$
         *
         * For example, to hold 2048 samples for one channel, a buffer must be at
         * least 8192 bytes large.
         *
         * When a multi-channel ::bladerf_channel_layout is selected, samples
         * will be interleaved per channel. For example, with ::BLADERF_RX_X2
         * or ::BLADERF_TX_X2 (x2 MIMO), the buffer is structured like:
         *
         * <pre>
         *  .-------------.--------------.--------------.------------------.
         *  | Byte offset | Bits 31...16 | Bits 15...0  |    Description   |
         *  +-------------+--------------+--------------+------------------+
         *  |    0x00     |     Q0[0]    |     I0[0]    |  Ch 0, sample 0  |
         *  |    0x04     |     Q1[0]    |     I1[0]    |  Ch 1, sample 0  |
         *  |    0x08     |     Q0[1]    |     I0[1]    |  Ch 0, sample 1  |
         *  |    0x0c     |     Q1[1]    |     I1[1]    |  Ch 1, sample 1  |
         *  |    ...      |      ...     |      ...     |        ...       |
         *  |    0xxx     |     Q0[n]    |     I0[n]    |  Ch 0, sample n  |
         *  |    0xxx     |     Q1[n]    |     I1[n]    |  Ch 1, sample n  |
         *  `-------------`--------------`--------------`------------------`
         * </pre>
         *
         * Per the `buffer_size_min` formula above, 2048 samples for two channels
         * will generate 4096 total samples, and require at least 16384 bytes.
         *
         * Implementors may use the interleaved buffers directly, or may use
         * bladerf_deinterleave_stream_buffer() / bladerf_interleave_stream_buffer()
         * if contiguous blocks of samples are desired.
         */
        BLADERF_FORMAT_SC16_Q11      = 0,
        /**
         * This format is the same as the ::BLADERF_FORMAT_SC16_Q11 format, except
         * the first 4 samples in every <i>block*</i> of samples are replaced with
         * metadata organized as follows. All fields are little-endian byte order.
         *
         * <pre>
         *  .-------------.------------.----------------------------------.
         *  | Byte offset |   Type     | Description                      |
         *  +-------------+------------+----------------------------------+
         *  |    0x00     | uint32_t   | Reserved                         |
         *  |    0x04     | uint64_t   | 64-bit Timestamp                 |
         *  |    0x0c     | uint32_t   | BLADERF_META_FLAG_* flags        |
         *  |  0x10..end  |            | Payload                          |
         *  `-------------`------------`----------------------------------`
         * </pre>
         *
         * <i>*</i>The number of samples in a <i>block</i> is dependent upon
         * the USB speed being used:
         *  - USB 2.0 Hi-Speed: 256 samples
         *  - USB 3.0 SuperSpeed: 512 samples
         *
         * When using the bladerf_sync_rx() and bladerf_sync_tx() functions, the
         * above details are entirely transparent; the caller need not be concerned
         * with these details. These functions take care of packing/unpacking the
         * metadata into/from the underlying stream and convey this information
         * through the ::bladerf_metadata structure.
         *
         * However, when using the \ref FN_STREAMING_ASYNC interface, the user is
         * responsible for manually packing/unpacking the above metadata into/from
         * their samples.
         *
         * @see STREAMING_FORMAT_METADATA
         * @see The `src/streaming/metadata.h` header in the libbladeRF codebase.
         */
        BLADERF_FORMAT_SC16_Q11_META = 1
    }
    public enum bladerf_image_type
    {
        BLADERF_IMAGE_TYPE_INVALID = -1,      /* Used to denote invalid value */
        BLADERF_IMAGE_TYPE_RAW = 0,           /* Misc. raw data */
        BLADERF_IMAGE_TYPE_FIRMWARE = 1,      /* Firmware data */
        BLADERF_IMAGE_TYPE_FPGA_40KLE = 2,    /* FPGA bitstream for 40 KLE device */
        BLADERF_IMAGE_TYPE_FPGA_115KLE = 3,   /* FPGA bitstream for 115  KLE device */
        BLADERF_IMAGE_TYPE_FPGA_A4 = 4,       /* FPGA bitstream for A4 device */
        BLADERF_IMAGE_TYPE_FPGA_A9 = 5,       /* FPGA bitstream for A9 device */
        BLADERF_IMAGE_TYPE_CALIBRATION = 6,   /* Board calibration */
        BLADERF_IMAGE_TYPE_RX_DC_CAL = 7,     /* RX DC offset calibration table */
        BLADERF_IMAGE_TYPE_TX_DC_CAL = 8,     /* TX DC offset calibration table */
        BLADERF_IMAGE_TYPE_RX_IQ_CAL = 9,     /* RX IQ balance calibration table */
        BLADERF_IMAGE_TYPE_TX_IQ_CAL = 10,    /* TX IQ balance calibration table */
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
    public enum bladerf_tuning_mode
    { 
        /** Indicates an invalid mode is set */
        BLADERF_TUNING_MODE_INVALID = -1,

        /** Perform tuning algorithm on the host. This is slower, but provides easier accessiblity to diagnostic information. */
        BLADERF_TUNING_MODE_HOST = 0,

        /** Perform tuning algorithm on the FPGA for faster tuning. */
        BLADERF_TUNING_MODE_FPGA = 1
    }
    public enum bladerf_xb
    {
        BLADERF_XB_NONE = 0, /* No expansion boards attached */
        BLADERF_XB_100  = 1, /* XB-100 GPIO expansion board.
                              *   This device is not yet supported in
                              *   libbladeRF, and is here as a placeholder
                              *   for future support. */
        BLADERF_XB_200  = 2, /* XB-200 Transverter board */
        BLADERF_XB_300  = 3  /* XB-300 Amplifier board */
    }
    public enum bladerf_log_level
    {
        BLADERF_LOG_LEVEL_VERBOSE = 0,  /* Verbose level logging */
        BLADERF_LOG_LEVEL_DEBUG = 1,    /* Debug level logging */
        BLADERF_LOG_LEVEL_INFO = 2,     /* Information level logging */
        BLADERF_LOG_LEVEL_WARNING = 3,  /* Warning level logging */
        BLADERF_LOG_LEVEL_ERROR = 4,    /* Error level logging */
        BLADERF_LOG_LEVEL_CRITICAL = 5, /* Fatal error level logging */
        BLADERF_LOG_LEVEL_SILENT = 6    /* No output */
    }

    public enum bladerf_error_codes
    {
        BLADERF_ERR_UNEXPECTED      = -1,  /* An unexpected failure occurred */
        BLADERF_ERR_RANGE           = -2,  /* Provided parameter is out of range */
        BLADERF_ERR_INVAL           = -3,  /* Invalid operation/parameter */
        BLADERF_ERR_MEM             = -4,  /* Memory allocation error */
        BLADERF_ERR_IO              = -5,  /* File/Device I/O error */
        BLADERF_ERR_TIMEOUT         = -6,  /* Operation timed out */
        BLADERF_ERR_NODEV           = -7,  /* No device(s) available */
        BLADERF_ERR_UNSUPPORTED     = -8,  /* Operation not supported */
        BLADERF_ERR_MISALIGNED      = -9,  /* Misaligned flash access */
        BLADERF_ERR_CHECKSUM        = -10, /* Invalid checksum */
        BLADERF_ERR_NO_FILE         = -11, /* File not found */
        BLADERF_ERR_UPDATE_FPGA     = -12, /* An FPGA update is required */
        BLADERF_ERR_UPDATE_FW       = -13, /* A firmware update is requied */
        BLADERF_ERR_TIME_PAST       = -14, /* Requested timestamp is in the past */
        BLADERF_ERR_QUEUE_FULL      = -15, /* Could not enqueue data into full queue */
        BLADERF_ERR_FPGA_OP         = -16, /* An FPGA operation reported failure */
        BLADERF_ERR_PERMISSION      = -17, /* Insufficient permissions for the requested operation */
        BLADERF_ERR_WOULD_BLOCK     = -18, /* Operation would block, but has been
                                            *   requested to be non-blocking. This
                                            *   indicates to a caller that it may
                                            *   need to retry the operation later. */
        BLADERF_ERR_NOT_INIT        = -19, /* Devce insufficiently initialized for operation */
    }
        
    public enum bladerf_channel 
    {
        BLADERF_RX_X1 = 0,
        BLADERF_TX_X1 = 1,
        BLADERF_RX_X2 = 2,
        BLADERF_TX_X2 = 3,
        BLADERF_RX = 0,
        BLADERF_TX = 1,
    }
    #endregion

    #region libbladeRF1.h enums
    public enum bladerf_sampling
    {
        BLADERF_SAMPLING_UNKNOWN  = 0, /* Unable to determine connection type */
        BLADERF_SAMPLING_INTERNAL = 1, /* Sample from RX/TX connector */
        BLADERF_SAMPLING_EXTERNAL = 2  /* Sample from J60 or J61 */
    }
    public enum bladerf_lpf_mode
    {
        BLADERF_LPF_NORMAL   = 0,   /* LPF connected and enabled */
        BLADERF_LPF_BYPASSED = 1,   /* LPF bypassed */
        BLADERF_LPF_DISABLED = 2    /* LPF disabled */
    }
    public enum bladerf_smb_mode
    {
        BLADERF_SMB_MODE_INVALID = -1,    /* Invalid selection */
        BLADERF_SMB_MODE_DISABLED = 0,    /* Not in use. Device operates from its onboard
                                           * clock and does not use J62.
                                           */
        BLADERF_SMB_MODE_OUTPUT = 1,      /* Device outputs a 38.4 MHz reference clock on
                                           * J62. This may be used to drive another device
                                           * that is configured with
                                           * ::BLADERF_SMB_MODE_INPUT.
                                           */
        BLADERF_SMB_MODE_INPUT = 2,       /* Device configures J62 as an input and expects a
                                           * 38.4 MHz reference to be available when this
                                           * setting is applied.
                                           */
        BLADERF_SMB_MODE_UNAVAILABLE = 3,  /* SMB port is unavailable for use due to the
                                            * underlying clock being used elsewhere (e.g.,
                                            * for an expansion board).
                                            */
    }
    public enum bladerf_xb200_filter
    {
        BLADERF_XB200_AUTO = -1, //deprecated!! TODO

        /** 50-54 MHz (6 meter band) filterbank */
        BLADERF_XB200_50M = 0,

        /** 144-148 MHz (2 meter band) filterbank */
        BLADERF_XB200_144M = 1,

        /**
         * 222-225 MHz (1.25 meter band) filterbank.
         *
         * Note that this filter option is technically wider, covering 206-235 MHz.
         */
        BLADERF_XB200_222M = 2,

        /**
         * This option enables the RX/TX channel's custom filter bank path across
         * the associated FILT and FILT-ANT SMA connectors on the XB-200 board.
         *
         * For reception, it is often possible to simply connect the RXFILT and
         * RXFILT-ANT connectors with an SMA cable (effectively, "no filter"). This
         * allows for reception of signals outside of the frequency range of the
         * on-board filters, with some potential trade-off in signal quality.
         *
         * For transmission, <b>always</b> use an appropriate filter on the custom
         * filter path to avoid spurious emissions.
         *
         */
        BLADERF_XB200_CUSTOM = 3,

        /**
         * When this option is selected, the other filter options are automatically
         * selected depending on the RX or TX channel's current frequency, based
         * upon the 1dB points of the on-board filters.  For frequencies outside
         * the range of the on-board filters, the custom path is selected.
         */
        BLADERF_XB200_AUTO_1DB =4,

        /**
         * When this option is selected, the other filter options are automatically
         * selected depending on the RX or TX channel's current frequency, based
         * upon the 3dB points of the on-board filters. For frequencies outside the
         * range of the on-board filters, the custom path is selected.
         */
        BLADERF_XB200_AUTO_3DB = 5
    }
    public enum bladerf_xb200_path
    {
        BLADERF_XB200_BYPASS = 0, /* Bypass the XB-200 mixer */
        BLADERF_XB200_MIX    = 1  /* Pass signals through the XB-200 mixer */
    }
    public enum bladerf_xb300_trx
    {
        BLADERF_XB300_TRX_INVAL = -1,  /* Invalid TRX selection */
        BLADERF_XB300_TRX_TX = 0,      /* TRX antenna operates as TX */
        BLADERF_XB300_TRX_RX = 1,      /* TRX antenna operates as RX */
        BLADERF_XB300_TRX_UNSET = 2    /* TRX antenna unset */
    }
    public enum bladerf_xb300_amplifier
    {
        BLADERF_XB300_AMP_INVAL = -1,   /* Invalid amplifier selection */
        BLADERF_XB300_AMP_PA = 0,       /* TX Power amplifier */
        BLADERF_XB300_AMP_LNA = 1,      /* RX LNA */
        BLADERF_XB300_AMP_PA_AUX = 2    /* Auxillary Power amplifier */
    }
    public enum bladerf_cal_module
    {
        BLADERF_DC_CAL_INVALID = -1,
        BLADERF_DC_CAL_LPF_TUNING = 0,
        BLADERF_DC_CAL_TX_LPF = 1,
        BLADERF_DC_CAL_RX_LPF = 2,
        BLADERF_DC_CAL_RXVGA2 = 3
    }

    public enum bladerf_gpio : long
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
        BLADERF_XB_GPIO_32 = 0x80000000L,

        /* XB-200 GPIO */
        BLADERF_XB200_PIN_J7_1 = BLADERF_XB_GPIO_10,
        BLADERF_XB200_PIN_J7_2 = BLADERF_XB_GPIO_11,
        BLADERF_XB200_PIN_J7_5 = BLADERF_XB_GPIO_08,
        BLADERF_XB200_PIN_J7_6 = BLADERF_XB_GPIO_09,
        BLADERF_XB200_PIN_J13_1 = BLADERF_XB_GPIO_17,
        BLADERF_XB200_PIN_J13_2 = BLADERF_XB_GPIO_18,
        /* XB-200 J13 Pin 6 is actually reserved for SPI */
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
        /*  XB-100 header J12, pins 3 and 4 are reserved for SPI */
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

    #region libbladeRF2.h enums
    public enum bladerf_rfic_rxfir
    {
        BLADERF_RFIC_RXFIR_BYPASS = 0, /* No filter */
        BLADERF_RFIC_RXFIR_CUSTOM = 1, /* Custom FIR filter (currently unused) */
        BLADERF_RFIC_RXFIR_DEC1 = 2,   /* Decimate by 1 (default) */
        BLADERF_RFIC_RXFIR_DEC2 = 3,   /* Decimate by 2 */
        BLADERF_RFIC_RXFIR_DEC4 = 4    /* Decimate by 4 */
    }
    public enum bladerf_rfic_txfir
    {
        BLADERF_RFIC_TXFIR_BYPASS = 0, /* No filter (default) */
        BLADERF_RFIC_TXFIR_CUSTOM = 1, /* Custom FIR filter (currently unused) */
        BLADERF_RFIC_TXFIR_DEC1 = 2,   /* Decimate by 1 */
        BLADERF_RFIC_TXFIR_DEC2 = 3,   /* Decimate by 2 */
        BLADERF_RFIC_TXFIR_DEC4 = 4    /* Decimate by 4 */
    }
    public enum bladerf_power_sources
    {
        BLADERF_UNKNOWN     = 0, /* Unknown; manual observation may be required */
        BLADERF_PS_DC       = 1, /* DC Barrel Plug */
        BLADERF_PS_USB_VBUS = 2  /* USB Bus */
    }
    public enum bladerf_clock_select
    {
        CLOCK_SELECT_ONBOARD  = 0, /* Use onboard VCTCXO */
        CLOCK_SELECT_EXTERNAL = 1  /* Use external clock input */
    }
    public enum bladerf_pmic_register
    {
        BLADERF_PMIC_CONFIGURATION  = 0, /* Configuration register (uint16_t) */
        BLADERF_PMIC_VOLTAGE_SHUNT  = 1, /* Shunt voltage (float) */
        BLADERF_PMIC_VOLTAGE_BUS    = 2, /* Bus voltage (float) */
        BLADERF_PMIC_POWER          = 3, /* Load power (float) */
        BLADERF_PMIC_CURRENT        = 4, /* Load current (float) */
        BLADERF_PMIC_CALIBRATION    = 5, /* Calibration (uint16_t) */
    }
    #endregion

    #endregion

    #region structs

    #region libbladeRF.h structs
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct bladerf_devinfo
    {
        public bladerf_backend backend;                                             /* Backend to use when connecting to device */
        public fixed sbyte _serial[Constants.BLADERF_SERIAL_LENGTH];                 /* Device serial number string */
        public byte usb_bus;                                                        /* Bus # device is attached to */
        public byte usb_addr;                                                       /* Device address on bus */
        public uint instance;                                                       /* Device instance or ID */
        public fixed sbyte _manufacturer[Constants.BLADERF_DESCRIPTION_LENGTH];      /* Manufacturer description string */
        public fixed sbyte _product[Constants.BLADERF_DESCRIPTION_LENGTH];           /* Product description string */

        public string serial
        { get { fixed (sbyte* sbp = _serial) { return Marshal.PtrToStringAnsi((IntPtr)sbp); } } }
        public string manufacturer
        { get { fixed (sbyte* sbp = _manufacturer) { return Marshal.PtrToStringAnsi((IntPtr)sbp); } } }
        public string product
        { get { fixed (sbyte* sbp = _product) { return Marshal.PtrToStringAnsi((IntPtr)sbp); } } }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct bladerf_range
    {
        public Int64 min;
        public Int64 max;
        public Int64 step;
        public float scale;
    }
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct bladerf_serial
    {
        public fixed sbyte _serial[Constants.BLADERF_SERIAL_LENGTH];
        public string serial
        { get { fixed (sbyte* sbp = _serial) { return Marshal.PtrToStringAnsi((IntPtr)sbp); } } }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct bladerf_version
    {
        public UInt16 major;        /* Major version */
        public UInt16 minor;        /* Minor version */
        public UInt16 patch;        /* Patch version */
        private IntPtr _description; /* Version string with any additional suffix information */
        public string description
        {
            get { return Marshal.PtrToStringAnsi(_description); }
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct bladerf_gain_modes
    {
        private IntPtr name_ptr;
        public bladerf_gain_mode mode;
        public string name
        {
            get { return Marshal.PtrToStringAnsi(name_ptr); }
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct bladerf_rational_rate
    {
        public UInt64 integer;  /* Integer portion */
        public UInt64 num;      /* Numerator in fractional portion */
        public UInt64 denom;    /* Denominator in fractional portion. This must be greater than 0. */
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct bladerf_loopback_modes
    {
        IntPtr name_ptr;        /* Name of loopback mode */
        bladerf_loopback mode;  /* Loopback mode enumeration */
        public string name
        {
            get { return Marshal.PtrToStringAnsi(name_ptr); }
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct bladerf_trigger
    {
        bladerf_channel channel;        /* RX/TX channel associated with trigger */
        bladerf_trigger_role role;      /* Role of the device in a trigger chain */
        bladerf_trigger_signal signal;  /* Pin or signal being used */
        UInt64 options;                 /* Reserved field for future options. This is unused and should be set to 0. */
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct bladerf_quick_tune
    {
        // BladeRF 1
        [FieldOffset(0)]
        public byte freqsel;           /* Choice of VCO and VCO division factor */
        [FieldOffset(1)]
        public byte vcocap;            /* VCOCAP value */
        [FieldOffset(2)]
        public UInt16 nint;            /* Integer portion of LO frequency value */
        [FieldOffset(4)]
        public UInt32 nfrac;           /* Fractional portion of LO frequency value */
        [FieldOffset(8)]
        public byte flags;             /* Flag bits used internally by libbladeRF */
        [FieldOffset(9)]
        public byte xb_gpio;           /* Flag bits used to configure XB */

        // BladeRF 2
        [FieldOffset(0)]
        public UInt16 nios_profile;    /* Profile number in Nios */
        [FieldOffset(2)]
        public byte rffe_profile;      /* Profile number in RFFE */
        [FieldOffset(3)]
        public byte port;              /* RFFE port settings */
        [FieldOffset(4)]
        public byte spdt;              /* External SPDT settings */
    }
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct bladerf_metadata
    {
        /**
         * Free-running FPGA counter that monotonically increases at the sample rate
         * of the associated channel.
         */
        public UInt64 timestamp;

        /**
         * Input bit field to control the behavior of the call that the metadata
         * structure is passed to. API calls read this field from the provided data
         * structure, and do not modify it.
         *
         * Valid flags include
         *  ::BLADERF_META_FLAG_TX_BURST_START,
         *  ::BLADERF_META_FLAG_TX_BURST_END,
         *  ::BLADERF_META_FLAG_TX_NOW,
         *  ::BLADERF_META_FLAG_TX_UPDATE_TIMESTAMP, and
         *  ::BLADERF_META_FLAG_RX_NOW
         */
        public UInt32 flags;

        /**
         * Output bit field to denoting the status of transmissions/receptions. API
         * calls will write this field.
         *
         * Possible status flags include ::BLADERF_META_STATUS_OVERRUN and
         * ::BLADERF_META_STATUS_UNDERRUN.
         */
        public UInt32 status;

        /**
          * This output parameter is updated to reflect the actual number of
          * contiguous samples that have been populated in an RX buffer during a
          * bladerf_sync_rx() call.
          *
          * This will not be equal to the requested count in the event of a
          * discontinuity (i.e., when the status field has the
          * ::BLADERF_META_STATUS_OVERRUN flag set). When an overrun occurs, it is
          * important not to read past the number of samples specified by this value,
          * as the remaining contents of the buffer are undefined.
          *
          * @note This parameter is not currently used by bladerf_sync_tx().
          */
        public uint actual_count;

        /**
           * Reserved for future use. This is not used by any functions. It is
           * recommended that users zero out this field.
           */
        public fixed byte reserved[32];
    }
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct bladerf_image
    {
       /**
        * Magic value used to identify image file format.
        *
        * Note that an extra character is added to store a `NUL`-terminator,
        * to allow this field to be printed. This `NUL`-terminator is *NOT*
        * written in the serialized image.
        */
        fixed byte magic[Constants.BLADERF_IMAGE_MAGIC_LEN + 1];

       /**
        * SHA256 checksum of the flash image. This is computed over the entire
        * image, with this field filled with 0's.
        */
        fixed byte checksum[Constants.BLADERF_IMAGE_CHECKSUM_LEN];

       /**
        * Image format version. Only the major, minor, and patch fields are
        * written to the disk; the describe field is not used. The version is
        * serialized as: [major | minor | patch]
        */
        bladerf_version version;

       /** UTC image timestamp, in seconds since the Unix Epoch */
        UInt64 timestamp;

       /**
        * Serial number of the device that the image was obtained from. This
        * field should be all '\0' if irrelevant.
        *
        * The +1 here is actually extraneous; ::BLADERF_SERIAL_LENGTH already
        * accounts for a `NUL` terminator. However, this is left here to avoid
        * breaking backwards compatibility.
        */
        fixed byte serial[Constants.BLADERF_SERIAL_LENGTH + 1];

       /** Reserved for future metadata. Should be 0's. */
        fixed byte reserved[Constants.BLADERF_IMAGE_RESERVED_LEN];

       /** Type of data contained in the image. Serialized as a uint32_t. */
        bladerf_image_type type;

       /**
        * Address of the flash data in this image. A value of `0xffffffff`
        * implies that this field is left unspecified (i.e., "don't care").
        */
        UInt32 address;

       /** Length of the data contained in the image */
        UInt32 length;

       /** Image data */
        byte* data;

    }
    #endregion

    #region libbladeRF1.h structs
    [StructLayout(LayoutKind.Sequential)]
    public struct bladerf_lms_dc_cals
    {
        Int16 lpf_tuning; /* LPF tuning module */
        Int16 tx_lpf_i;   /* TX LPF I filter */
        Int16 tx_lpf_q;   /* TX LPF Q filter */
        Int16 rx_lpf_i;   /* RX LPF I filter */
        Int16 rx_lpf_q;   /* RX LPF Q filter */
        Int16 dc_ref;     /* RX VGA2 DC reference module */
        Int16 rxvga2a_i;  /* RX VGA2, I channel of first gain stage */
        Int16 rxvga2a_q;  /* RX VGA2, Q channel of first gain stage */
        Int16 rxvga2b_i;  /* RX VGA2, I channel of second gain stage */
        Int16 rxvga2b_q;  /* RX VGA2, Q channel of second gain stage */
    }
    #endregion

    #region libbladeRF2.h structs
    [StructLayout(LayoutKind.Sequential)]
    public struct bladerf_rf_switch_config
    {
        UInt32 tx1_rfic_port; /* Active TX1 output from RFIC */
        UInt32 tx1_spdt_port; /* RF switch configuration for the TX1 path */
        UInt32 tx2_rfic_port; /* Active TX2 output from RFIC */
        UInt32 tx2_spdt_port; /* RF switch configuration for the TX2 path */
        UInt32 rx1_rfic_port; /* Active RX1 input to RFIC */
        UInt32 rx1_spdt_port; /* RF switch configuration for the RX1 path */
        UInt32 rx2_rfic_port; /* Active RX2 input to RFIC */
        UInt32 rx2_spdt_port; /* RF switch configuration for the RX2 path */
    }
    #endregion

    #endregion

    public static class Constants
    {
        /** Length of device description string, including NUL-terminator */
        public const int BLADERF_DESCRIPTION_LENGTH = 33;
        /** Length of device serial number string, including NUL-terminator */
        public const int BLADERF_SERIAL_LENGTH = 33;
        /** Size of the magic signature at the beginning of bladeRF image files */
        public const int BLADERF_IMAGE_MAGIC_LEN = 7;
        /** Size of bladeRF flash image checksum */
        public const int BLADERF_IMAGE_CHECKSUM_LEN = 32;
        /** Size of reserved region of flash image */
        public const int BLADERF_IMAGE_RESERVED_LEN = 128; 
    }

    public static class NativeMethods
    {
        #if PLATFORM_X86
                    private const string LibBladeRF = "..\\..\\..\\..\\libbladeRF\\x86\\bladeRF.dll";
        #elif PLATFORM_X64
                    private const string LibBladeRF = "..\\..\\..\\..\\libbladeRF\\x64\\bladeRF.dll";
        #elif PLATFORM_ANYCPU
                    private const string Platform = "AnyCPU";
        #else
                private const string LibBladeRF = "bladeRF";
        #endif

        #region Initialization
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_open(out IntPtr device, string device_identifier);                 // struct bladerf **device - hence the out keyword, it is initialised by the called method
                                                                                                            // strings are reference types in C#, in this declaration the pointer is passed by value
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_close(IntPtr device);                                             // struct bladerf *device

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_open_with_devinfo(out IntPtr device, ref bladerf_devinfo devinfo); // structs are value types in C#, to pass a pointer to them the ref keyword must be used

        [DllImport(LibBladeRF, EntryPoint = "bladerf_get_device_list", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_device_list(out bladerf_devinfo[] devices);                    // struct bladerf_devinfo **devices
                                                                                                            // if declared "out IntPtr devices" null could be passed, the variable is either initialised by the method or not used
        [DllImport(LibBladeRF, EntryPoint = "bladerf_get_device_list", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_device_count();

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_free_device_list(bladerf_devinfo[] devices);                      // This function doesn't work, it crashes

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_init_devinfo(ref bladerf_devinfo info);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_devinfo(IntPtr dev, ref bladerf_devinfo info);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_get_devinfo_from_str(string devstr, ref bladerf_devinfo info);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bladerf_devinfo_matches(ref bladerf_devinfo a, ref bladerf_devinfo b);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern bool bladerf_devstr_matches(string dev_str, ref bladerf_devinfo info);

        [DllImport(LibBladeRF, EntryPoint = "bladerf_backend_str", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr bladerf_backend_str_native(bladerf_backend backend);
        public static string bladerf_backend_str(bladerf_backend backend)
        {
            IntPtr strptr = bladerf_backend_str_native(backend);
            return Marshal.PtrToStringAnsi(strptr);
        }

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_set_usb_reset_on_open(bool enabled);
        #endregion

        #region Device properties
        // bladerf_get_serial() will be deprecated, replace with bladerf_get_serial_struct()
        [DllImport(LibBladeRF, EntryPoint = "bladerf_get_serial", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_get_serial_native(IntPtr dev, StringBuilder serial);
        public static int bladerf_get_serial(IntPtr dev, out string serial)
        {
            StringBuilder sbserial = new StringBuilder(Constants.BLADERF_SERIAL_LENGTH);
            int rv = bladerf_get_serial_native(dev, sbserial);
            serial = sbserial.ToString();
            return rv;
        }

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_serial_struct(IntPtr dev, ref bladerf_serial serial);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_fpga_size(IntPtr dev, out bladerf_fpga_size size);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_fpga_bytes(IntPtr dev, out uint size);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_flash_size(IntPtr dev, out UInt32 size, out bool is_guess);

        [DllImport(LibBladeRF, EntryPoint = "bladerf_fw_version", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_fw_version_native(IntPtr dev, ref bladerf_version version); // out works as well, no need to initialise version

        public static bladerf_version bladerf_fw_version(IntPtr dev)
        {
            bladerf_version ret = new bladerf_version();
            bladerf_fw_version_native(dev, ref ret);
            return ret;
        }

        [DllImport(LibBladeRF, EntryPoint = "bladerf_is_fpga_configured", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_is_fpga_configured_native(IntPtr dev);
        public static bool bladerf_is_fpga_configured(IntPtr dev)
        {
            int rv = bladerf_is_fpga_configured_native(dev);
            switch (rv)
            {
                case 1:
                    return true;
                case 0:
                    return false;
                default:
                    //throw error and return false
                    return false;
            }
        }


        [DllImport(LibBladeRF, EntryPoint = "bladerf_fpga_version", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_fpga_version_native(IntPtr dev, ref bladerf_version version); // out works as well, no need to initialise version

        public static bladerf_version bladerf_fpga_version(IntPtr dev)
        {
            bladerf_version ret = new bladerf_version();
            bladerf_fpga_version_native(dev, ref ret);
            return ret;
        }

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_fpga_source(IntPtr dev, out bladerf_fpga_source source);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern bladerf_dev_speed bladerf_device_speed(IntPtr dev);

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

        [DllImport(LibBladeRF, EntryPoint="bladerf_get_board_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bladerf_get_board_name_native(IntPtr dev);

        public static string bladerf_get_board_name(IntPtr dev)
        {
            IntPtr ret = bladerf_get_board_name_native(dev);
            if (ret != IntPtr.Zero)
                return Marshal.PtrToStringAnsi(ret);
            return String.Empty;
        }

        #endregion

        #region Channel Control
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_channel_count(IntPtr dev, bladerf_direction dir);

        #region Gain
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_gain(IntPtr dev, bladerf_channel ch, int gain);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_gain(IntPtr dev, bladerf_channel ch, out int gain);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_gain_mode(IntPtr dev, bladerf_channel ch, bladerf_gain_mode mode);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_gain_mode(IntPtr dev, bladerf_channel ch, out bladerf_gain_mode mode);

        [DllImport(LibBladeRF, EntryPoint = "bladerf_get_gain_modes", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_get_gain_modes_native(IntPtr dev, bladerf_channel ch, bladerf_gain_mode* modes);

        public static unsafe bladerf_gain_mode[] bladerf_get_gain_modes(IntPtr dev, bladerf_channel ch)
        {
            int num_gain = bladerf_get_gain_modes_native(dev, ch, null);
            if (num_gain == 0)
            {
                return new bladerf_gain_mode[0];
            }
            bladerf_gain_mode[] rv = new bladerf_gain_mode[num_gain];
            fixed (bladerf_gain_mode* buffer = rv)
            {
                bladerf_get_gain_modes_native(dev, ch, buffer);
            }
            return rv;
        }

        [DllImport(LibBladeRF, EntryPoint = "bladerf_get_gain_range", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_gain_range_native(IntPtr dev, bladerf_channel ch, out bladerf_range range); 
        //This may vary depending on the configured frequency, so it should be checked after setting the desired frequency.

        public static bladerf_range bladerf_get_gain_range(IntPtr dev, bladerf_channel ch)
        {
            bladerf_range range;
            bladerf_get_gain_range_native(dev, ch, out range);
            return range;
        }

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_set_gain_stage(IntPtr dev, bladerf_channel ch, string stage, int gain);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_get_gain_stage(IntPtr dev, bladerf_channel ch, string stage, out int gain);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_get_gain_stage_range(IntPtr dev, bladerf_channel ch, string stage, out bladerf_range range);

        [DllImport(LibBladeRF, EntryPoint = "bladerf_get_gain_stages", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_get_gain_stages_native(IntPtr dev, bladerf_channel ch, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3, ArraySubType = UnmanagedType.LPStr)] out string[] stages, int count); // This function doesn't work, it crashes

        public static string[] bladerf_get_gain_stages(IntPtr dev, bladerf_channel ch)
        {
            int num_stages = 0;
            string[] rv = new string[0];
            num_stages = bladerf_get_gain_stages_native(dev, ch, out rv, num_stages);
            if (num_stages <= 0)
            {
                return rv;
            }
            rv = new string[num_stages];
            bladerf_get_gain_stages_native(dev, ch, out rv, num_stages);
            return rv;
        }

        #endregion

        #region Sample rate
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_sample_rate(IntPtr dev, bladerf_channel ch, uint rate, out uint actual);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_rational_sample_rate(IntPtr dev, bladerf_channel ch, ref bladerf_rational_rate rate, out bladerf_rational_rate actual);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_sample_rate(IntPtr dev, bladerf_channel ch, out uint rate);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_sample_rate_range(IntPtr dev, bladerf_channel ch, out bladerf_range range);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rational_sample_rate(IntPtr dev, bladerf_channel ch, out bladerf_rational_rate rate);

        #endregion

        #region Bandwidth
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_bandwidth(IntPtr dev, bladerf_channel ch, uint bandwidth, out uint actual);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_bandwidth(IntPtr dev, bladerf_channel ch, out uint bandwidth);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_bandwidth_range(IntPtr dev, bladerf_channel ch, out bladerf_range range);

        #endregion

        #region Frequency
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_select_band(IntPtr dev, bladerf_channel ch, UInt64 frequency);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_frequency(IntPtr dev, bladerf_channel ch, UInt64 frequency);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_frequency(IntPtr dev, bladerf_channel ch, out UInt64 frequency);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_frequency_range(IntPtr dev, bladerf_channel ch, out bladerf_range range);

        #endregion

        #region Internal loopback
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_loopback_modes(IntPtr dev, out bladerf_loopback_modes modes); // This doesn't work

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bladerf_is_loopback_mode_supported(IntPtr dev, bladerf_loopback mode);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_loopback(IntPtr dev, bladerf_loopback lb);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_loopback(IntPtr dev, out bladerf_loopback lb);

        #endregion

        #region Triggers
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_trigger_init(IntPtr dev, bladerf_channel ch, bladerf_trigger_signal signal, out bladerf_trigger trigger);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_trigger_arm(IntPtr dev, ref bladerf_trigger trigger, bool arm, UInt64 resv1, UInt64 resv2);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_trigger_fire(IntPtr dev, ref bladerf_trigger trigger);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_trigger_state(IntPtr dev, ref bladerf_trigger trigger, out bool is_armed, out bool has_fired, out bool fire_requested, out UInt64 resv1, out UInt64 resv2);

        #endregion

        #region Receive Mux
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_rx_mux(IntPtr dev, bladerf_rx_mux mux);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rx_mux(IntPtr dev, out bladerf_rx_mux mode);

        #endregion

        #region Scheduled Tuning
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_schedule_retune(IntPtr dev, bladerf_channel ch, UInt64 timestamp, UInt64 frequency, ref bladerf_quick_tune quick_tune);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_cancel_scheduled_retunes(IntPtr dev, bladerf_channel ch);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_quick_tune(IntPtr dev, bladerf_channel ch, out bladerf_quick_tune quick_tune);

        #endregion

        #region Correction
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_correction(IntPtr dev, bladerf_channel ch, bladerf_correction corr, Int16 value);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_correction(IntPtr dev, bladerf_channel ch, bladerf_correction corr, out Int16 value);

        #endregion

        #endregion

        #region Streaming
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_interleave_stream_buffer(bladerf_channel ch, bladerf_format format, uint buffer_size, IntPtr samples);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_deinterleave_stream_buffer(bladerf_channel ch, bladerf_format format, uint buffer_siwe, IntPtr samples);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_enable_module(IntPtr dev, bladerf_channel ch, int enable);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_timestamp(IntPtr dev, bladerf_direction dir, out UInt64 timestamp);

        #region Synchronous API
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_sync_config(IntPtr dev, bladerf_channel ch, bladerf_format format, uint num_buffers, uint buffer_size, uint num_transfers, uint stream_timeout);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_sync_tx(IntPtr dev, Int16* samples, uint num_samples, IntPtr metadata, uint timeout_ms);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_sync_rx(IntPtr dev, Int16* samples, uint num_samples, IntPtr metadata, uint timeout_ms);

        #endregion

        #region Asynchronous API
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_init_stream(ref IntPtr stream, IntPtr dev, bladerf_stream_cb callback, out IntPtr buffers, uint num_buffers, bladerf_format format, uint samples_per_buffer, uint num_transfers, IntPtr user_data);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_stream(IntPtr stream, bladerf_channel ch);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_submit_stream_buffer(IntPtr stream, IntPtr buffer, uint timeout_ms);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_submit_stream_buffer_nb(IntPtr stream, IntPtr buffer);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_deinit_stream(IntPtr stream);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_stream_timeout(IntPtr dev, bladerf_direction dir, uint timeout);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_stream_timemout(IntPtr dev, bladerf_direction dir, out uint timeout);

        #endregion

        #endregion

        #region Firmware and FPGA
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_flash_firmware(IntPtr dev, string firmware);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_load_fpga(IntPtr dev, string fpga);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_flash_fpga(IntPtr dev, string fpga_image);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_erase_stored_fpga(IntPtr dev);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_device_reset(IntPtr dev);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_get_fw_log(IntPtr dev, string filename);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_jump_to_bootloader(IntPtr dev);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_bootloader_list(out bladerf_devinfo[] list);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_load_fw_from_bootloader(string device_identifier, bladerf_backend backend, byte bus, byte addr, string file);

        #endregion

        #region Flash image format
        // TODO: bladerf_image functions

        #endregion

        #region Low-level functions

        #region Tamer mode
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_vctcxo_tamer_mode(IntPtr dev, bladerf_vctcxo_tamer_mode mode);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_vctcxo_tamer_mode(IntPtr dev, out bladerf_vctcxo_tamer_mode mode);

        #endregion

        #region VCTCXO Trim DAC
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_vctcxo_trim(IntPtr dev, out UInt16 trim);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_trim_dac_write(IntPtr dev, UInt16 val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_trim_dac_read(IntPtr dev, out UInt16 val);

        #endregion

        #region Tuning mode
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_tuning_mode(IntPtr dev, bladerf_tuning_mode mode);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_tuning_mode(IntPtr dev, out bladerf_tuning_mode mode);

        #endregion

        #region Trigger Control
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_read_trigger(IntPtr dev, bladerf_channel ch, bladerf_trigger_signal signal, out byte val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_write_trigger(IntPtr dev, bladerf_channel ch, bladerf_trigger_signal signal, byte val);

        #endregion

        #region Configuration GPIO
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_config_gpio_read(IntPtr dev, out UInt32 val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_config_gpio_write(IntPtr dev, UInt32 val);

        #endregion

        #region SPI Flash
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_erase_flash(IntPtr dev, UInt32 erase_block, UInt32 count);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_erase_flash_bytes(IntPtr dev, UInt32 address, UInt32 length);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int bladerf_read_flash(IntPtr dev, byte* buf, UInt32 page, UInt32 count);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_read_flash_bytes(IntPtr dev, out byte[] buf, UInt32 address, UInt32 bytes);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_write_flash(IntPtr dev, byte[] buf, UInt32 page, UInt32 count);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_write_flash_bytes(IntPtr dev, byte[] buf, UInt32 address, UInt32 length);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_lock_otp(IntPtr dev);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_read_otp(IntPtr dev, out byte[] buf);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_write_otp(IntPtr dev, byte[] buf);

        #endregion

        #region RF Ports
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_set_rf_port(IntPtr dev, bladerf_channel ch, string port);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_get_rf_port(IntPtr dev, bladerf_channel ch, out string port);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int bladerf_get_rf_ports(IntPtr dev, bladerf_channel ch, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3, ArraySubType = UnmanagedType.LPStr)] out string[] ports, uint count);

        #endregion

        #endregion

        #region Expansion board support
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_expansion_attach(IntPtr dev, bladerf_xb xb);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
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

        #endregion

        #region Logging
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_log_set_verbosity(bladerf_log_level level);

        #endregion

        #region Library version
        [DllImport(LibBladeRF, EntryPoint = "bladerf_version", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bladerf_version_native(ref bladerf_version version);

        public static bladerf_version bladerf_version()
        {
            bladerf_version ret = new bladerf_version();
            bladerf_version_native(ref ret);
            return ret;
        }

        #endregion

        #region Error codes
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl, EntryPoint = "bladerf_strerror")]
        public static extern IntPtr bladerf_strerror_native(int error);

        public static string bladerf_strerror(int error)
        {
            IntPtr ret = bladerf_strerror_native(error);
            if (ret != IntPtr.Zero)
                return Marshal.PtrToStringAnsi(ret);
            return String.Empty;
        }

        #endregion

        #region BladeRF1 specific APIs
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_sampling(IntPtr dev, bladerf_sampling sampling);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_sampling(IntPtr dev, out bladerf_sampling sampling);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_lpf_mode(IntPtr dev, bladerf_channel ch, bladerf_lpf_mode mode);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_lpf_mode(IntPtr dev, bladerf_channel ch, out bladerf_lpf_mode mode);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_smb_mode(IntPtr dev, bladerf_smb_mode mode);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_smb_mode(IntPtr dev, out bladerf_smb_mode mode);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_rational_smb_frequency(IntPtr dev, ref bladerf_rational_rate rate, out bladerf_rational_rate actual);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_smb_frequency(IntPtr dev, UInt32 rate, out UInt32 actual);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rational_smb_frequency(IntPtr dev, out bladerf_rational_rate rate);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_smb_frequency(IntPtr dev, out UInt32 rate);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_expansion_gpio_read(IntPtr dev, out UInt32 val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_expansion_gpio_write(IntPtr dev, UInt32 val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_expansion_gpio_masked_write(IntPtr dev, UInt32 mask, UInt32 value);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_expansion_gpio_dir_read(IntPtr dev, out UInt32 outputs);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_expansion_gpio_dir_write(IntPtr dev, UInt32 outputs);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_expansion_gpio_dir_masked_write(IntPtr dev, UInt32 mask, UInt32 outputs);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb200_set_filterbank(IntPtr dev, bladerf_channel ch, bladerf_xb200_filter filter);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb200_get_filterbank(IntPtr dev, bladerf_channel ch, out bladerf_xb200_filter filter);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb200_set_path(IntPtr dev, bladerf_channel ch, bladerf_xb200_path path);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb200_get_path(IntPtr dev, bladerf_channel ch, out bladerf_xb200_path path);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb300_set_trx(IntPtr dev, bladerf_xb300_trx trx);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb300_get_trx(IntPtr dev, out bladerf_xb300_trx trx);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb300_set_amplifier_enable(IntPtr dev, bladerf_xb300_amplifier amp, bool enable);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb300_get_amplifier_enable(IntPtr dev, bladerf_xb300_amplifier amp, out bool enable);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb300_get_output_power(IntPtr dev, out float val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_calibrate_dc(IntPtr dev, bladerf_cal_module module);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_si5338_read(IntPtr dev, byte address, out byte val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_si5338_write(IntPtr dev, byte address, byte val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_lms_read(IntPtr dev, byte address, out byte val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_lms_write(IntPtr dev, byte address, byte val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_lms_set_dc_cals(IntPtr dev, ref bladerf_lms_dc_cals dc_cals);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_lms_get_dc_cals(IntPtr dev, out bladerf_lms_dc_cals dc_cals);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_xb_spi_write(IntPtr dev, UInt32 val);

        #endregion

        #region BladeRF2 specific APIs
        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_bias_tee(IntPtr dev, bladerf_channel ch, out bool enable);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_bias_tee(IntPtr dev, bladerf_channel ch, bool enable);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rfic_register(IntPtr dev, UInt16 address, out byte val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_rfic_register(IntPtr dev, UInt16 address, byte val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rfic_temperature(IntPtr dev, out float val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rfic_rssi(IntPtr dev, bladerf_channel ch, out Int32 pre_rssi, out Int32 sym_rssi);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rfic_ctrl_out(IntPtr dev, out byte ctrl_out);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rfic_rx_fir(IntPtr dev, out bladerf_rfic_rxfir rxfir);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_rfic_rx_fir(IntPtr dev, bladerf_rfic_rxfir rxfir);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rfic_tx_fir(IntPtr dev, out bladerf_rfic_txfir txfir);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_rfic_tx_fir(IntPtr dev, bladerf_rfic_txfir txfir);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_pll_lock_state(IntPtr dev, out bool locked);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_pll_enable(IntPtr dev, out bool enabled);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_pll_enable(IntPtr dev, bool enable);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_pll_refclk_range(IntPtr dev, out bladerf_range range);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_pll_refclk(IntPtr dev, out UInt64 frequency);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_pll_refclk(IntPtr dev, UInt64 frequency);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_pll_register(IntPtr dev, byte address, out UInt32 val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_pll_register(IntPtr dev, byte address, UInt32 val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_power_source(IntPtr dev, out bladerf_power_sources val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_clock_select(IntPtr dev, out bladerf_clock_select sel);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_clock_select(IntPtr dev, bladerf_clock_select sel);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_clock_output(IntPtr dev, out bool state);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_set_clock_output(IntPtr dev, bool enable);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_pmic_register(IntPtr dev, bladerf_pmic_register reg, out IntPtr val);

        [DllImport(LibBladeRF, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bladerf_get_rf_switch_config(IntPtr dev, out bladerf_rf_switch_config config);

        #endregion

    }
}
