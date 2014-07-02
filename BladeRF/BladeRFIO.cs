using System;
using System.Windows.Forms;
using System.ComponentModel;

using SDRSharp.Common;
using SDRSharp.Radio;

namespace SDRSharp.BladeRF
{
    public class BladeRFIO:  IFrontendController, IDisposable
    {
        private const string _displayName = "BladeRF";
        private readonly BladeRFControllerDialog _gui;
        private BladeRFDevice _bladeRFDevice;
        private long _frequency;
        private double _frequencyCorrection;
        private SDRSharp.Radio.SamplesAvailableDelegate _callback;

        public BladeRFIO()
        {
            _frequency = 405500000L;
            _gui = new BladeRFControllerDialog(this);
            bladerf_version v = NativeMethods.bladerf_version();
            if (v.major == 0 && v.minor < 14)
            {
                MessageBox.Show(String.Format("Your bladerf.dll is outdated. Upgrade to v0.14+ (now using {0})", v.describe));
                //throw new ApplicationException("outdated bladerf.dll");
            }
        }

        ~BladeRFIO()
        {
            Dispose();
        }

        public void Dispose()
        {
            Close();
            _gui.Close();
            _gui.Dispose();
            GC.SuppressFinalize(this);
        }

        public void SelectDevice(string serial)
        {
            Close();
            _bladeRFDevice = new BladeRFDevice(serial);
            _bladeRFDevice.SamplesAvailable += BladeRFDevice_SamplesAvailable;
            _bladeRFDevice.Frequency = _frequency;
            _gui.ConfigureGUI();
            _gui.ConfigureDevice();
        }

        public BladeRFDevice Device
        {
            get
            {
                return _bladeRFDevice;
            }
        }

        public void Open()
        {
            DeviceDisplay[] activeDevices = DeviceDisplay.GetActiveDevices();
            if (null == activeDevices)
                throw new ApplicationException("No compatible devices found");
            foreach (DeviceDisplay deviceDisplay in activeDevices)
            {
                try
                {
                    SelectDevice(deviceDisplay.Serial);
                    return;
                }
                catch
                {
                }
            }
            if (activeDevices.Length > 0)
                throw new ApplicationException(activeDevices.Length + " compatible devices have been found but are all busy");
            else
                throw new ApplicationException("No compatible devices found");
        }

        public void Close()
        {
            if (_bladeRFDevice == null)
                return;
            _bladeRFDevice.SamplesAvailable -= BladeRFDevice_SamplesAvailable;
            _bladeRFDevice.Dispose();
            _bladeRFDevice = null;
        }

        public void Start(SDRSharp.Radio.SamplesAvailableDelegate callback)
        {
            if (_bladeRFDevice == null)
                throw new ApplicationException("No device selected");
            _callback = callback;
            _bladeRFDevice.Start();
        }

        public void Stop()
        {
            _bladeRFDevice.Stop();
        }

        public bool IsSoundCardBased
        {
          get
          {
            return false;
          }
        }

        public string SoundCardHint
        {
          get
          {
            return string.Empty;
          }
        }

        public void ShowSettingGUI(IWin32Window parent)
        {
            if (this._gui.IsDisposed)
                return;
            _gui.Show();
            _gui.Activate();
        }

        public void HideSettingGUI()
        {
            if (_gui.IsDisposed)
                return;
            _gui.Hide();
        }

        public double Samplerate
        {
          get
          {
            if (_bladeRFDevice != null)
              return _bladeRFDevice.SampleRate;
            else
              return 0.0;
          }
        }

        public long Frequency
        {
          get
          {
            return _frequency;
          }
          set
          {
            if (this._bladeRFDevice == null)
              return;
            this._bladeRFDevice.Frequency = (long)(value * (1.0 + this._frequencyCorrection * 1E-06));
            this._frequency = value;
          }
        }

        public double FrequencyCorrection
        {
          get
          {
            return this._frequencyCorrection;
          }
          set
          {
            this._frequencyCorrection = value;
            this.Frequency = this._frequency;
          }
        }

        private unsafe void BladeRFDevice_SamplesAvailable(object sender, SamplesAvailableEventArgs e)
        {
          _callback(this, e.Buffer, e.Length);
        }
    }
}
