using System;
using System.Globalization;
using System.Windows.Forms;
using SDRSharp.Common;
using SDRSharp.Radio;
using libbladeRF_wrapper;

namespace SDRSharp.BladeRF
{
    public partial class BladeRFControllerDialog : Form
    {
        private readonly BladeRFIO _owner;
        private bool _initialized;

        public BladeRFControllerDialog(BladeRFIO owner)
        {
            InitializeComponent();

            _owner = owner;

            InitSampleRates();
            InitBandwidths();
            InitXB200Filters();
            var devices = DeviceDisplay.GetActiveDevices();
            deviceComboBox.Items.Clear();
            if (devices != null)
            {
                deviceComboBox.Items.AddRange(devices);
            }
            samplerateComboBox.SelectedIndex = Utils.GetIntSetting("BladeRFSampleRate", 3);
            samplingModeComboBox.SelectedIndex = Utils.GetIntSetting("BladeRFSamplingMode", (int) bladerf_sampling.BLADERF_SAMPLING_INTERNAL);
            overallGainTrackBar.Value = Utils.GetIntSetting("BladeRFOverallGain", 10);
            lnaGainTrackBar.Value = Utils.GetIntSetting("BladeRFLNAGain", 3);
            rxVga1GainTrackBar.Value = Utils.GetIntSetting("BladeRFRXVGA1Gain", 23);
            rxVga2GainTrackBar.Value = Utils.GetIntSetting("BladeRFRXVGA2Gain", 0);
            fpgaTextBox.Text = Utils.GetStringSetting("BladeRFFPGA", "");
            bandwidthComboBox.SelectedIndex = Utils.GetIntSetting("BladeRFBandwidth", 0);

            xb200Checkbox.Checked = Utils.GetBooleanSetting("BladeRFXB200Enabled");
            xb200FilterCombobox.SelectedIndex = Utils.GetIntSetting("BladeRFXB200Filter", 0);

            labelVersion.Text = "libbladerf " + NativeMethods.bladerf_version().description;

            rxVga1GainLabel.Text = rxVga1GainTrackBar.Value + " dB";
            rxVga2GainLabel.Text = rxVga2GainTrackBar.Value + " dB";
            lnaGainLabel.Text = String.Format("{0} dB", 3 * (lnaGainTrackBar.Value - 1)); ;

            _initialized = true;
        }

        private bool Initialized
        {
            get
            {
                return _initialized && _owner.Device != null;
            }
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

        private void InitBandwidths()
        {
            bandwidthComboBox.Items.Clear();
            bandwidthComboBox.DisplayMember = "Text";
            bandwidthComboBox.ValueMember = "Value";
            bandwidthComboBox.Items.Add(new ComboboxItem("auto", 0));
            bandwidthComboBox.Items.Add(new ComboboxItem("28 MHz", 28000000));
            bandwidthComboBox.Items.Add(new ComboboxItem("20 MHz", 20000000));
            bandwidthComboBox.Items.Add(new ComboboxItem("14 MHz", 14000000));
            bandwidthComboBox.Items.Add(new ComboboxItem("12 MHz", 12000000));
            bandwidthComboBox.Items.Add(new ComboboxItem("10 MHz", 10000000));
            bandwidthComboBox.Items.Add(new ComboboxItem("8.75 MHz", 8750000));
            bandwidthComboBox.Items.Add(new ComboboxItem("7 MHz", 7000000));
            bandwidthComboBox.Items.Add(new ComboboxItem("6 MHz", 6000000));
            bandwidthComboBox.Items.Add(new ComboboxItem("5.5 MHz", 5500000));
            bandwidthComboBox.Items.Add(new ComboboxItem("5 MHz", 5000000));
            bandwidthComboBox.Items.Add(new ComboboxItem("3.84 MHz", 3840000));
            bandwidthComboBox.Items.Add(new ComboboxItem("3 MHz", 3000000));
            bandwidthComboBox.Items.Add(new ComboboxItem("2.75 MHz", 2750000));
            bandwidthComboBox.Items.Add(new ComboboxItem("2.5 MHz", 2500000));
            bandwidthComboBox.Items.Add(new ComboboxItem("1.75 MHz", 1750000));
            bandwidthComboBox.Items.Add(new ComboboxItem("1.5 MHz", 1500000));
        }

        private void InitXB200Filters()
        {
            xb200FilterCombobox.Items.Clear();
            xb200FilterCombobox.Items.Add("auto");
            xb200FilterCombobox.Items.Add("50 MHz");
            xb200FilterCombobox.Items.Add("144 MHz");
            xb200FilterCombobox.Items.Add("222 MHz");
            xb200FilterCombobox.Items.Add("Custom");
            xb200FilterCombobox.Items.Add("Auto (1dB)");
            xb200FilterCombobox.Items.Add("Auto (3dB)");
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
                deviceComboBox.Enabled = Initialized && !_owner.Device.IsStreaming;
                xb200Checkbox.Enabled = Initialized && !_owner.Device.IsStreaming;

                if (Initialized && !_owner.Device.IsStreaming)
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
            deviceComboBox.Enabled = Initialized && !_owner.Device.IsStreaming;
            xb200Checkbox.Enabled = Initialized && !_owner.Device.IsStreaming;
        }

        private void deviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Initialized)
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
            if (!Initialized)
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
            if (!Initialized)
            {
                return;
            }
            _owner.Device.Sampling = samplingModeComboBox.SelectedIndex;
            Utils.SaveSetting("BladeRFSamplingMode", samplingModeComboBox.SelectedIndex);
        }

        public void ConfigureGUI()
        {
            if (!Initialized)
                return;

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
            rxVga1GainTrackBar_Scroll(null, null);
            rxVga2GainTrackBar_Scroll(null, null);
            lnaGainTrackBar_Scroll(null, null);
            overallGainTrackBar_Scroll(null, null);
            xb200Checkbox_CheckedChanged(null, null);
            xb200FilterCombobox_SelectedIndexChanged(null, null);
            bandwidthComboBox_SelectedIndexChanged(null, null);
        }

        private void rxVga1GainTrackBar_Scroll(object sender, EventArgs e)
        {
            if (!Initialized)
            {
                return;
            }
            _owner.Device.VGA1Gain = rxVga1GainTrackBar.Value;
            rxVga1GainLabel.Text = rxVga1GainTrackBar.Value + " dB";
            Utils.SaveSetting("BladeRFRXVGA1Gain", rxVga1GainTrackBar.Value);
        }

        private void rxVga2GainTrackBar_Scroll(object sender, EventArgs e)
        {
            if (!Initialized)
            {
                return;
            }
            _owner.Device.VGA2Gain = rxVga2GainTrackBar.Value;
            rxVga2GainLabel.Text = rxVga2GainTrackBar.Value + " dB";
            Utils.SaveSetting("BladeRFRXVGA2Gain", rxVga2GainTrackBar.Value);
        }

        private void lnaGainTrackBar_Scroll(object sender, EventArgs e)
        {
            if (!Initialized)
            {
                return;
            }
            _owner.Device.LNAGain = lnaGainTrackBar.Value;
            lnaGainLabel.Text = lnaGainTrackBar.Value + " dB";
            Utils.SaveSetting("BladeRFLNAGain", lnaGainTrackBar.Value);
        }

        private void overallGainTrackBar_Scroll(object sender, EventArgs e)
        {
            if (!Initialized)
            {
                return;
            }
            _owner.Device.OverallGain = overallGainTrackBar.Value;
            overallGainLabel.Text = overallGainTrackBar.Value + " dB";
            Utils.SaveSetting("BladeRFOverallGain", overallGainTrackBar.Value);
        }


        private void fpgaButton_Click(object sender, EventArgs e)
        {
            if (!Initialized)
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

        private void xb200Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initialized)
                return;
            xb200FilterCombobox.Enabled = xb200Checkbox.Checked;
            Utils.SaveSetting("BladeRFXB200Enabled", xb200Checkbox.Checked);
            _owner.Device.XB200Enabled = xb200Checkbox.Checked;
        }

        private void xb200FilterCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Initialized)
                return;
            Utils.SaveSetting("BladeRFXB200Filter", xb200FilterCombobox.SelectedIndex);
            _owner.Device.XB200Filter = xb200FilterCombobox.SelectedIndex;
        }

        private void bandwidthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Initialized)
                return;
            Utils.SaveSetting("BladeRFBandwidth", bandwidthComboBox.SelectedIndex);
            try
            {
                _owner.Device.Bandwidth = (bandwidthComboBox.SelectedItem as ComboboxItem).Value;
            }
            catch
            {
                _owner.Device.Bandwidth = 0;
            }
        }

        private class ComboboxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }

            public ComboboxItem(string text, int value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        public ISharpControl Control
        {
            get;
            set;
        }
    }

    public class DeviceDisplay
    {
        public int Index { get; private set; }
        public string Name { get; set; }
        public string Serial { get; private set; }
        public int Bus { get; set; }
        public int Address { get; set; }
        public string Backend { get; set; }

        public unsafe static DeviceDisplay[] GetActiveDevices()
        {
            bladerf_devinfo[] devlist;
            int count = NativeMethods.bladerf_get_device_list(out devlist);
            if (count <= 0)
            {
                return null;
            }
            DeviceDisplay[] result = new DeviceDisplay[count];
            for (int i = 0; i < count; i++)
            {
                bladerf_devinfo device = devlist[i];
                int bus = (int)(device.usb_bus);
                int address = (int)(device.usb_addr);
                string backend = NativeMethods.bladerf_backend_str(device.backend);
                string serial = device.serial;
                string name = String.Format("BladeRF ({0}) SN#{1}..{2} ({3}:{4})", backend, serial.Substring(0, 4), serial.Substring(27, 4), bus, address);
                result[i] = new DeviceDisplay { Index = i, Name = name, Serial = serial, Address = address, Bus = bus, Backend = backend };
            }
            //NativeMethods.bladerf_free_device_list(devlist); //Crashes
            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
