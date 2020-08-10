namespace SDRSharp.BladeRF
{
    partial class BladeRFControllerDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.closeButton = new System.Windows.Forms.Button();
            this.deviceComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rxVga2GainTrackBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.samplerateComboBox = new System.Windows.Forms.ComboBox();
            this.rxVga2GainLabel = new System.Windows.Forms.Label();
            this.bladeRFTypeLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.samplingModeComboBox = new System.Windows.Forms.ComboBox();
            this.rxVga1GainTrackBar = new System.Windows.Forms.TrackBar();
            this.rxVga1GainLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lnaGainLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lnaGainTrackBar = new System.Windows.Forms.TrackBar();
            this.fpgaOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.fpgaTextBox = new System.Windows.Forms.TextBox();
            this.fpgaButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.xb200Checkbox = new System.Windows.Forms.CheckBox();
            this.xb200FilterCombobox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.bandwidthComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.overallGainLabel = new System.Windows.Forms.Label();
            this.overallGainTrackBar = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.rxVga2GainTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rxVga1GainTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lnaGainTrackBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overallGainTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 1000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(183, 603);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 8;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // deviceComboBox
            // 
            this.deviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceComboBox.FormattingEnabled = true;
            this.deviceComboBox.Location = new System.Drawing.Point(12, 26);
            this.deviceComboBox.Name = "deviceComboBox";
            this.deviceComboBox.Size = new System.Drawing.Size(247, 21);
            this.deviceComboBox.TabIndex = 0;
            this.deviceComboBox.SelectedIndexChanged += new System.EventHandler(this.deviceComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Device";
            // 
            // rxVga2GainTrackBar
            // 
            this.rxVga2GainTrackBar.LargeChange = 3;
            this.rxVga2GainTrackBar.Location = new System.Drawing.Point(14, 212);
            this.rxVga2GainTrackBar.Maximum = 30;
            this.rxVga2GainTrackBar.Name = "rxVga2GainTrackBar";
            this.rxVga2GainTrackBar.Size = new System.Drawing.Size(216, 45);
            this.rxVga2GainTrackBar.SmallChange = 3;
            this.rxVga2GainTrackBar.TabIndex = 6;
            this.rxVga2GainTrackBar.Value = 20;
            this.rxVga2GainTrackBar.Scroll += new System.EventHandler(this.rxVga2GainTrackBar_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 196);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "RxVGA2 Gain";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Sample Rate";
            // 
            // samplerateComboBox
            // 
            this.samplerateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.samplerateComboBox.FormattingEnabled = true;
            this.samplerateComboBox.Location = new System.Drawing.Point(12, 124);
            this.samplerateComboBox.Name = "samplerateComboBox";
            this.samplerateComboBox.Size = new System.Drawing.Size(103, 21);
            this.samplerateComboBox.TabIndex = 1;
            this.samplerateComboBox.SelectedIndexChanged += new System.EventHandler(this.samplerateComboBox_SelectedIndexChanged);
            // 
            // rxVga2GainLabel
            // 
            this.rxVga2GainLabel.Location = new System.Drawing.Point(162, 198);
            this.rxVga2GainLabel.Name = "rxVga2GainLabel";
            this.rxVga2GainLabel.Size = new System.Drawing.Size(68, 13);
            this.rxVga2GainLabel.TabIndex = 26;
            this.rxVga2GainLabel.Text = "30dB";
            this.rxVga2GainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // bladeRFTypeLabel
            // 
            this.bladeRFTypeLabel.Location = new System.Drawing.Point(75, 9);
            this.bladeRFTypeLabel.Name = "bladeRFTypeLabel";
            this.bladeRFTypeLabel.Size = new System.Drawing.Size(184, 13);
            this.bladeRFTypeLabel.TabIndex = 29;
            this.bladeRFTypeLabel.Text = "BladeRF";
            this.bladeRFTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Sampling Mode";
            // 
            // samplingModeComboBox
            // 
            this.samplingModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.samplingModeComboBox.FormattingEnabled = true;
            this.samplingModeComboBox.Items.AddRange(new object[] {
            "Unknown",
            "RX/TX SMA",
            "J60/J61 connectors"});
            this.samplingModeComboBox.Location = new System.Drawing.Point(12, 74);
            this.samplingModeComboBox.Name = "samplingModeComboBox";
            this.samplingModeComboBox.Size = new System.Drawing.Size(247, 21);
            this.samplingModeComboBox.TabIndex = 2;
            this.samplingModeComboBox.SelectedIndexChanged += new System.EventHandler(this.samplingModeComboBox_SelectedIndexChanged);
            // 
            // rxVga1GainTrackBar
            // 
            this.rxVga1GainTrackBar.LargeChange = 1;
            this.rxVga1GainTrackBar.Location = new System.Drawing.Point(14, 160);
            this.rxVga1GainTrackBar.Maximum = 30;
            this.rxVga1GainTrackBar.Minimum = 5;
            this.rxVga1GainTrackBar.Name = "rxVga1GainTrackBar";
            this.rxVga1GainTrackBar.Size = new System.Drawing.Size(216, 45);
            this.rxVga1GainTrackBar.TabIndex = 32;
            this.rxVga1GainTrackBar.Value = 20;
            this.rxVga1GainTrackBar.Scroll += new System.EventHandler(this.rxVga1GainTrackBar_Scroll);
            // 
            // rxVga1GainLabel
            // 
            this.rxVga1GainLabel.Location = new System.Drawing.Point(162, 144);
            this.rxVga1GainLabel.Name = "rxVga1GainLabel";
            this.rxVga1GainLabel.Size = new System.Drawing.Size(68, 13);
            this.rxVga1GainLabel.TabIndex = 34;
            this.rxVga1GainLabel.Text = "30dB";
            this.rxVga1GainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "RxVGA1 Gain";
            // 
            // lnaGainLabel
            // 
            this.lnaGainLabel.Location = new System.Drawing.Point(162, 94);
            this.lnaGainLabel.Name = "lnaGainLabel";
            this.lnaGainLabel.Size = new System.Drawing.Size(68, 13);
            this.lnaGainLabel.TabIndex = 37;
            this.lnaGainLabel.Text = "6dB";
            this.lnaGainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 94);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 36;
            this.label7.Text = "LNA Gain";
            // 
            // lnaGainTrackBar
            // 
            this.lnaGainTrackBar.LargeChange = 1;
            this.lnaGainTrackBar.Location = new System.Drawing.Point(14, 110);
            this.lnaGainTrackBar.Maximum = 6;
            this.lnaGainTrackBar.Name = "lnaGainTrackBar";
            this.lnaGainTrackBar.Size = new System.Drawing.Size(216, 45);
            this.lnaGainTrackBar.TabIndex = 35;
            this.lnaGainTrackBar.Value = 2;
            this.lnaGainTrackBar.Scroll += new System.EventHandler(this.lnaGainTrackBar_Scroll);
            // 
            // fpgaOpenFileDialog
            // 
            this.fpgaOpenFileDialog.AddExtension = false;
            this.fpgaOpenFileDialog.FileName = "openFileDialog1";
            this.fpgaOpenFileDialog.Filter = "rbf files|*.rbf";
            this.fpgaOpenFileDialog.ReadOnlyChecked = true;
            this.fpgaOpenFileDialog.Title = "Choose FPGA file";
            // 
            // fpgaTextBox
            // 
            this.fpgaTextBox.Location = new System.Drawing.Point(11, 562);
            this.fpgaTextBox.Name = "fpgaTextBox";
            this.fpgaTextBox.ReadOnly = true;
            this.fpgaTextBox.Size = new System.Drawing.Size(160, 20);
            this.fpgaTextBox.TabIndex = 38;
            // 
            // fpgaButton
            // 
            this.fpgaButton.Location = new System.Drawing.Point(183, 560);
            this.fpgaButton.Name = "fpgaButton";
            this.fpgaButton.Size = new System.Drawing.Size(75, 23);
            this.fpgaButton.TabIndex = 39;
            this.fpgaButton.Text = "Browse...";
            this.fpgaButton.UseVisualStyleBackColor = true;
            this.fpgaButton.Click += new System.EventHandler(this.fpgaButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 541);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 40;
            this.label4.Text = "FPGA File";
            // 
            // labelVersion
            // 
            this.labelVersion.Location = new System.Drawing.Point(8, 603);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(168, 23);
            this.labelVersion.TabIndex = 41;
            this.labelVersion.Text = "libbladeRF Version";
            // 
            // xb200Checkbox
            // 
            this.xb200Checkbox.AutoSize = true;
            this.xb200Checkbox.Location = new System.Drawing.Point(6, 23);
            this.xb200Checkbox.Name = "xb200Checkbox";
            this.xb200Checkbox.Size = new System.Drawing.Size(184, 17);
            this.xb200Checkbox.TabIndex = 42;
            this.xb200Checkbox.Text = "Enable XB-200 Transverter board";
            this.xb200Checkbox.UseVisualStyleBackColor = true;
            this.xb200Checkbox.CheckedChanged += new System.EventHandler(this.xb200Checkbox_CheckedChanged);
            // 
            // xb200FilterCombobox
            // 
            this.xb200FilterCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.xb200FilterCombobox.Enabled = false;
            this.xb200FilterCombobox.FormattingEnabled = true;
            this.xb200FilterCombobox.Location = new System.Drawing.Point(6, 64);
            this.xb200FilterCombobox.Name = "xb200FilterCombobox";
            this.xb200FilterCombobox.Size = new System.Drawing.Size(235, 21);
            this.xb200FilterCombobox.TabIndex = 43;
            this.xb200FilterCombobox.SelectedIndexChanged += new System.EventHandler(this.xb200FilterCombobox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.xb200Checkbox);
            this.groupBox1.Controls.Add(this.xb200FilterCombobox);
            this.groupBox1.Location = new System.Drawing.Point(12, 433);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(247, 100);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "XB-200 configuration";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 13);
            this.label8.TabIndex = 44;
            this.label8.Text = "Filter bank";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(131, 107);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 46;
            this.label9.Text = "Bandwidth";
            // 
            // bandwidthComboBox
            // 
            this.bandwidthComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.bandwidthComboBox.FormattingEnabled = true;
            this.bandwidthComboBox.Location = new System.Drawing.Point(134, 124);
            this.bandwidthComboBox.Name = "bandwidthComboBox";
            this.bandwidthComboBox.Size = new System.Drawing.Size(125, 21);
            this.bandwidthComboBox.TabIndex = 45;
            this.bandwidthComboBox.SelectedIndexChanged += new System.EventHandler(this.bandwidthComboBox_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.overallGainLabel);
            this.groupBox2.Controls.Add(this.overallGainTrackBar);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.lnaGainLabel);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.rxVga2GainLabel);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.rxVga1GainLabel);
            this.groupBox2.Controls.Add(this.lnaGainTrackBar);
            this.groupBox2.Controls.Add(this.rxVga1GainTrackBar);
            this.groupBox2.Controls.Add(this.rxVga2GainTrackBar);
            this.groupBox2.Location = new System.Drawing.Point(12, 162);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(247, 262);
            this.groupBox2.TabIndex = 47;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "RX Gain controls";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 23);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 13);
            this.label13.TabIndex = 39;
            this.label13.Text = "Overall Gain";
            // 
            // overallGainLabel
            // 
            this.overallGainLabel.Location = new System.Drawing.Point(162, 23);
            this.overallGainLabel.Name = "overallGainLabel";
            this.overallGainLabel.Size = new System.Drawing.Size(68, 13);
            this.overallGainLabel.TabIndex = 41;
            this.overallGainLabel.Text = "60dB";
            this.overallGainLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // overallGainTrackBar
            // 
            this.overallGainTrackBar.LargeChange = 1;
            this.overallGainTrackBar.Location = new System.Drawing.Point(14, 42);
            this.overallGainTrackBar.Maximum = 60;
            this.overallGainTrackBar.Minimum = -1;
            this.overallGainTrackBar.Name = "overallGainTrackBar";
            this.overallGainTrackBar.Size = new System.Drawing.Size(216, 45);
            this.overallGainTrackBar.TabIndex = 38;
            this.overallGainTrackBar.Value = 2;
            this.overallGainTrackBar.Scroll += new System.EventHandler(this.overallGainTrackBar_Scroll);
            // 
            // BladeRFControllerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(268, 638);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.bandwidthComboBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.fpgaButton);
            this.Controls.Add(this.fpgaTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.samplingModeComboBox);
            this.Controls.Add(this.bladeRFTypeLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.samplerateComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.deviceComboBox);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BladeRFControllerDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BladeRF Controller";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BladeRFControllerDialog_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.BladeRFControllerDialog_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.rxVga2GainTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rxVga1GainTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lnaGainTrackBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overallGainTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ComboBox deviceComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar rxVga2GainTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox samplerateComboBox;
        private System.Windows.Forms.Label rxVga2GainLabel;
        private System.Windows.Forms.Label bladeRFTypeLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox samplingModeComboBox;
        private System.Windows.Forms.TrackBar rxVga1GainTrackBar;
        private System.Windows.Forms.Label rxVga1GainLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lnaGainLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar lnaGainTrackBar;
        private System.Windows.Forms.OpenFileDialog fpgaOpenFileDialog;
        private System.Windows.Forms.TextBox fpgaTextBox;
        private System.Windows.Forms.Button fpgaButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.CheckBox xb200Checkbox;
        private System.Windows.Forms.ComboBox xb200FilterCombobox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox bandwidthComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label overallGainLabel;
        private System.Windows.Forms.TrackBar overallGainTrackBar;
    }
}

