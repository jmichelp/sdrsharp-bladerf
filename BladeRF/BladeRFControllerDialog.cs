using System;
using System.Globalization;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp.BladeRF
{
    public partial class BladeRFControllerDialog : Form
    {
        private readonly BladeRFIO _owner;
        private bool _initialized;

        public BladeRFControllerDialog(BladeRFIO owner)
        {
            InitializeComponent();

            InitSampleRates();
            _owner = owner;
            var devices = DeviceDisplay.GetActiveDevices();
            deviceComboBox.Items.Clear();
            if (devices != null)
                deviceComboBox.Items.AddRange(devices);

            samplerateComboBox.SelectedIndex = Utils.GetIntSetting("BladeRFSampleRate", 3);
            samplingModeComboBox.SelectedIndex = Utils.GetIntSetting("BladeRFSamplingMode", (int) bladerf_sampling.BLADERF_SAMPLING_INTERNAL);
            rxVga1GainTrackBar.Value = Utils.GetIntSetting("BladeRFVGA1Gain", 20);
            rxVga2GainTrackBar.Value = Utils.GetIntSetting("BladeRFVGA2Gain", 20);
            lnaGainTrackBar.Value = Utils.GetIntSetting("BladeRFLNAGain", (int) bladerf_lna_gain.BLADERF_LNA_GAIN_MID);
            fpgaTextBox.Text = Utils.GetStringSetting("BladeRFFPGA", "");

            labelVersion.Text = "libbladerf " + NativeMethods.bladerf_version().describe;

            rxVga1gainLabel.Text = rxVga1GainTrackBar.Value + " dB";
            rxVga2gainLabel.Text = rxVga2GainTrackBar.Value + " dB";
            lnaGainLabel.Text = String.Format("{0} dB", 3 * (lnaGainTrackBar.Value - 1)); ;

            _initialized = true;
        }

        private void InitSampleRates()
        {
            for (int i = 40; i > 0; i--)
                samplerateComboBox.Items.Add(String.Format("{0} MSPS", i));
            for (int i = 900; i > 0; i -= 300)
                samplerateComboBox.Items.Add(String.Format("0.{0} MSPS", i));
            samplerateComboBox.Items.Add("0.200 MSPS");
            samplerateComboBox.Items.Add("0.160 MSPS");
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BladeRFControllerDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void BladeRFControllerDialog_VisibleChanged(object sender, EventArgs e)
        {
            refreshTimer.Enabled = Visible;
            if (Visible)
            {
                samplerateComboBox.Enabled = !_owner.Device.IsStreaming;
                deviceComboBox.Enabled = !_owner.Device.IsStreaming;
                samplingModeComboBox.Enabled = !_owner.Device.IsStreaming;
                rxVga1GainTrackBar.Enabled = !_owner.Device.IsStreaming;
                rxVga2GainTrackBar.Enabled = !_owner.Device.IsStreaming;
                lnaGainTrackBar.Enabled = !_owner.Device.IsStreaming;

                if (!_owner.Device.IsStreaming)
                {
                    var devices = DeviceDisplay.GetActiveDevices();
                    deviceComboBox.Items.Clear();
                    if (devices != null)
                    {
                        deviceComboBox.Items.AddRange(devices);

                        for (var i = 0; i < devices.Length; i++)
                        {
                            if (devices[i].Index == ((DeviceDisplay)(deviceComboBox.Items[i])).Index)
                            {
                                _initialized = false;
                                deviceComboBox.SelectedIndex = i;
                                _initialized = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            samplerateComboBox.Enabled = !_owner.Device.IsStreaming;
            deviceComboBox.Enabled = !_owner.Device.IsStreaming;
            samplingModeComboBox.Enabled = !_owner.Device.IsStreaming;
            rxVga1GainTrackBar.Enabled = !_owner.Device.IsStreaming;
            rxVga2GainTrackBar.Enabled = !_owner.Device.IsStreaming;
            lnaGainTrackBar.Enabled = !_owner.Device.IsStreaming;
        }

        private void deviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            var deviceDisplay = (DeviceDisplay) deviceComboBox.SelectedItem;
            if (deviceDisplay != null)
            {
                try
                {
                    _owner.SelectDevice(deviceDisplay.Serial);
                }
                catch (Exception ex)
                {
                    deviceComboBox.SelectedIndex = -1;
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void samplerateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            var samplerateString = samplerateComboBox.Items[samplerateComboBox.SelectedIndex].ToString().Split(' ')[0];
            var sampleRate = double.Parse(samplerateString, CultureInfo.InvariantCulture);
            _owner.Device.SampleRate = (uint) (sampleRate * 1000000.0);
            Utils.SaveSetting("BladeRFSampleRate", samplerateComboBox.SelectedIndex);
        }

        private void samplingModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            _owner.Device.Sampling = samplingModeComboBox.SelectedIndex;
            Utils.SaveSetting("BladeRFSamplingMode", samplingModeComboBox.SelectedIndex);
        }

        public void ConfigureGUI()
        {
            bladeRFTypeLabel.Text = _owner.Device.Name;

            for (var i = 0; i < deviceComboBox.Items.Count; i++)
            {
                var deviceDisplay = (DeviceDisplay) deviceComboBox.Items[i];
                if (deviceDisplay.Serial == _owner.Device.Serial)
                {
                    deviceComboBox.SelectedIndex = i;
                    break;
                }
            }
        }

        public void ConfigureDevice()
        {
            samplerateComboBox_SelectedIndexChanged(null, null);
            samplingModeComboBox_SelectedIndexChanged(null, null);
        }

        private void rxVga1GainTrackBar_Scroll(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            _owner.Device.VGA1Gain = rxVga1GainTrackBar.Value;
            rxVga1gainLabel.Text = rxVga1GainTrackBar.Value + " dB";
            Utils.SaveSetting("BladeRFVGA1Gain", rxVga1GainTrackBar.Value);
        }

        private void rxVga2GainTrackBar_Scroll(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            _owner.Device.VGA2Gain = rxVga2GainTrackBar.Value;
            rxVga2gainLabel.Text = rxVga2GainTrackBar.Value + " dB";
            Utils.SaveSetting("BladeRFVGA2Gain", rxVga2GainTrackBar.Value);
        }

        private void lnaGainTrackBar_Scroll(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            _owner.Device.LNAGain = (uint) lnaGainTrackBar.Value;
            lnaGainLabel.Text = String.Format("{0} dB", 3 * (lnaGainTrackBar.Value - 1));
            Utils.SaveSetting("BladeRFLNAGain", lnaGainTrackBar.Value);
        }

        private void fpgaButton_Click(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            if (fpgaOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                fpgaTextBox.Text = fpgaOpenFileDialog.FileName;
                Utils.SaveSetting("BladeRFFPGA", fpgaTextBox.Text);
                _owner.Device.FPGAImage = fpgaTextBox.Text;
            }
        }
    }

    public class DeviceDisplay
    {
        public int Index { get; private set; }
        public string Name { get; set; }
        public string Serial { get; private set; }
        public int Bus { get; set; }
        public int Address { get; set; }

        public unsafe static DeviceDisplay[] GetActiveDevices()
        {
            IntPtr _tmp;
            bladerf_devinfo* devlist;
            int count = NativeMethods.bladerf_get_device_list(out _tmp);
            if (_tmp == IntPtr.Zero)
            {
                return null;
            }
            DeviceDisplay[] result = new DeviceDisplay[count];
            devlist = (bladerf_devinfo*)_tmp;

            for (int i = 0; i < count; i++)
            {
                int bus = (int)(devlist[i].usb_bus);
                int address = (int)(devlist[i].usb_addr);
                string serial = new String(devlist[i].serial, 0, 32, System.Text.Encoding.ASCII);
                string name = String.Format("BladeRF SN#{0} ({1}:{2})", serial, bus, address);
                result[i] = new DeviceDisplay { Index = i, Name = name, Serial = serial, Address = address, Bus = bus };
            }
            NativeMethods.bladerf_free_device_list(_tmp);
            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
