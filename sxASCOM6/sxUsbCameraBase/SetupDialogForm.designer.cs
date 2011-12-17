// tabs=4
// Copyright 2010-2010 by Dad Dog Development, Ltd
//
// This work is licensed under the Creative Commons Attribution-No Derivative 
// Works 3.0 License. 
//
// A copy of the license should have been included with this software. If
// not, you can also view a copy of this license, at:
//
// http://creativecommons.org/licenses/by-nd/3.0/ or 
// send a letter to:
//
// Creative Commons
// 171 Second Street
// Suite 300
// San Francisco, California, 94105, USA.
// 
// If this license is not suitable for your purposes, it is possible to 
// obtain it under a different license. 
//
// For more information please contact bretm@daddog.com

namespace ASCOM.sxUsbCameraBase
{
    partial class SetupDialogForm
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
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.picASCOM = new System.Windows.Forms.PictureBox();
            this.EnableLoggingCheckBox = new System.Windows.Forms.CheckBox();
            this.EnableUntestedCheckBox = new System.Windows.Forms.CheckBox();
            this.Version = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.dumpDataEnabled = new System.Windows.Forms.CheckBox();
            this.PID = new System.Windows.Forms.MaskedTextBox();
            this.selectionExcludeModel = new System.Windows.Forms.RadioButton();
            this.VID = new System.Windows.Forms.MaskedTextBox();
            this.pidLabel = new System.Windows.Forms.Label();
            this.selectionExactModel = new System.Windows.Forms.RadioButton();
            this.selectionAllowAny = new System.Windows.Forms.RadioButton();
            this.vidLabel = new System.Windows.Forms.Label();
            this.Copyright = new System.Windows.Forms.Label();
            this.usbGroup = new System.Windows.Forms.GroupBox();
            this.advancedUSBParmsEnabled = new System.Windows.Forms.CheckBox();
            this.binGroup = new System.Windows.Forms.GroupBox();
            this.maxYBin = new System.Windows.Forms.NumericUpDown();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.symetricBinning = new System.Windows.Forms.RadioButton();
            this.xBinLabel = new System.Windows.Forms.Label();
            this.maxXBin = new System.Windows.Forms.NumericUpDown();
            this.binLabel = new System.Windows.Forms.Label();
            this.useDumpedData = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.usbGroup.SuspendLayout();
            this.binGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxYBin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxXBin)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(363, 93);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(4);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(79, 30);
            this.cmdOK.TabIndex = 6;
            this.cmdOK.Text = "OK";
            this.toolTip1.SetToolTip(this.cmdOK, "Close this dialog saving changes.");
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(363, 141);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(4);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(79, 31);
            this.cmdCancel.TabIndex = 0;
            this.cmdCancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.cmdCancel, "Close this dialog with no changes.");
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(339, 26);
            this.label1.TabIndex = 101;
            this.label1.Text = "ASCOM Driver for SX/SXV/SXVF/SXVR Cameras\r\n\r\n";
            // 
            // picASCOM
            // 
            this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picASCOM.Image = global::ASCOM.sxUsbCameraBase.Properties.Resources.ASCOM;
            this.picASCOM.Location = new System.Drawing.Point(363, 11);
            this.picASCOM.Margin = new System.Windows.Forms.Padding(4);
            this.picASCOM.Name = "picASCOM";
            this.picASCOM.Size = new System.Drawing.Size(48, 56);
            this.picASCOM.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picASCOM.TabIndex = 3;
            this.picASCOM.TabStop = false;
            this.toolTip1.SetToolTip(this.picASCOM, "Click to visit the ASCOM web site.");
            this.picASCOM.DoubleClick += new System.EventHandler(this.BrowseToAscom);
            this.picASCOM.Click += new System.EventHandler(this.BrowseToAscom);
            // 
            // EnableLoggingCheckBox
            // 
            this.EnableLoggingCheckBox.AutoSize = true;
            this.EnableLoggingCheckBox.Location = new System.Drawing.Point(20, 151);
            this.EnableLoggingCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.EnableLoggingCheckBox.Name = "EnableLoggingCheckBox";
            this.EnableLoggingCheckBox.Size = new System.Drawing.Size(129, 21);
            this.EnableLoggingCheckBox.TabIndex = 1;
            this.EnableLoggingCheckBox.Text = "Enable Logging";
            this.toolTip1.SetToolTip(this.EnableLoggingCheckBox, "Enable the creation of a debug log file.");
            this.EnableLoggingCheckBox.UseVisualStyleBackColor = true;
            // 
            // EnableUntestedCheckBox
            // 
            this.EnableUntestedCheckBox.AutoSize = true;
            this.EnableUntestedCheckBox.Location = new System.Drawing.Point(20, 180);
            this.EnableUntestedCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.EnableUntestedCheckBox.Name = "EnableUntestedCheckBox";
            this.EnableUntestedCheckBox.Size = new System.Drawing.Size(255, 21);
            this.EnableUntestedCheckBox.TabIndex = 2;
            this.EnableUntestedCheckBox.Text = "Enable Untested Cameras/Features";
            this.toolTip1.SetToolTip(this.EnableUntestedCheckBox, "Allowed untested features and untested cameras to be used.");
            this.EnableUntestedCheckBox.UseVisualStyleBackColor = true;
            // 
            // Version
            // 
            this.Version.AutoSize = true;
            this.Version.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Version.Location = new System.Drawing.Point(16, 37);
            this.Version.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(98, 18);
            this.Version.TabIndex = 102;
            this.Version.Text = "Version: a.b.c";
            // 
            // dumpDataEnabled
            // 
            this.dumpDataEnabled.AutoSize = true;
            this.dumpDataEnabled.Location = new System.Drawing.Point(20, 208);
            this.dumpDataEnabled.Margin = new System.Windows.Forms.Padding(4);
            this.dumpDataEnabled.Name = "dumpDataEnabled";
            this.dumpDataEnabled.Size = new System.Drawing.Size(275, 21);
            this.dumpDataEnabled.TabIndex = 3;
            this.dumpDataEnabled.Text = "Dump Data to File (for debugging only)";
            this.toolTip1.SetToolTip(this.dumpDataEnabled, "Divide the requested exposure by 1000.  Useful if you need faster exposures than " +
                    "your caputure software allows for.");
            this.dumpDataEnabled.UseVisualStyleBackColor = true;
            // 
            // PID
            // 
            this.PID.Location = new System.Drawing.Point(57, 143);
            this.PID.Margin = new System.Windows.Forms.Padding(4);
            this.PID.Mask = "9990";
            this.PID.Name = "PID";
            this.PID.PromptChar = ' ';
            this.PID.Size = new System.Drawing.Size(47, 22);
            this.PID.TabIndex = 6;
            this.PID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // selectionExcludeModel
            // 
            this.selectionExcludeModel.AutoSize = true;
            this.selectionExcludeModel.Location = new System.Drawing.Point(16, 80);
            this.selectionExcludeModel.Margin = new System.Windows.Forms.Padding(4);
            this.selectionExcludeModel.Name = "selectionExcludeModel";
            this.selectionExcludeModel.Size = new System.Drawing.Size(120, 21);
            this.selectionExcludeModel.TabIndex = 2;
            this.selectionExcludeModel.Text = "Exclude Model";
            this.selectionExcludeModel.UseVisualStyleBackColor = true;
            // 
            // VID
            // 
            this.VID.Location = new System.Drawing.Point(57, 111);
            this.VID.Margin = new System.Windows.Forms.Padding(4);
            this.VID.Mask = "9990";
            this.VID.Name = "VID";
            this.VID.PromptChar = ' ';
            this.VID.Size = new System.Drawing.Size(47, 22);
            this.VID.TabIndex = 4;
            this.VID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // pidLabel
            // 
            this.pidLabel.AutoSize = true;
            this.pidLabel.Location = new System.Drawing.Point(12, 143);
            this.pidLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.pidLabel.Name = "pidLabel";
            this.pidLabel.Size = new System.Drawing.Size(34, 17);
            this.pidLabel.TabIndex = 5;
            this.pidLabel.Text = "PID:";
            // 
            // selectionExactModel
            // 
            this.selectionExactModel.AutoSize = true;
            this.selectionExactModel.Location = new System.Drawing.Point(16, 52);
            this.selectionExactModel.Margin = new System.Windows.Forms.Padding(4);
            this.selectionExactModel.Name = "selectionExactModel";
            this.selectionExactModel.Size = new System.Drawing.Size(110, 21);
            this.selectionExactModel.TabIndex = 1;
            this.selectionExactModel.Text = "Select Model";
            this.selectionExactModel.UseVisualStyleBackColor = true;
            // 
            // selectionAllowAny
            // 
            this.selectionAllowAny.AutoSize = true;
            this.selectionAllowAny.Checked = true;
            this.selectionAllowAny.Location = new System.Drawing.Point(16, 23);
            this.selectionAllowAny.Margin = new System.Windows.Forms.Padding(4);
            this.selectionAllowAny.Name = "selectionAllowAny";
            this.selectionAllowAny.Size = new System.Drawing.Size(95, 21);
            this.selectionAllowAny.TabIndex = 0;
            this.selectionAllowAny.TabStop = true;
            this.selectionAllowAny.Text = "Any Model";
            this.selectionAllowAny.UseVisualStyleBackColor = true;
            this.selectionAllowAny.CheckedChanged += new System.EventHandler(this.camera1SelectionAllowAny_CheckedChanged);
            // 
            // vidLabel
            // 
            this.vidLabel.AutoSize = true;
            this.vidLabel.Location = new System.Drawing.Point(12, 114);
            this.vidLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.vidLabel.Name = "vidLabel";
            this.vidLabel.Size = new System.Drawing.Size(34, 17);
            this.vidLabel.TabIndex = 3;
            this.vidLabel.Text = "VID:";
            // 
            // Copyright
            // 
            this.Copyright.Location = new System.Drawing.Point(16, 68);
            this.Copyright.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Copyright.Name = "Copyright";
            this.Copyright.Size = new System.Drawing.Size(339, 70);
            this.Copyright.TabIndex = 103;
            this.Copyright.Text = "Copyright (C) 2010 Dad Dog Development Ltd.\r\nThis work is licensed under the Crea" +
                "tive Commons Attribution-No Derivative Works 3.0 License.";
            // 
            // usbGroup
            // 
            this.usbGroup.Controls.Add(this.PID);
            this.usbGroup.Controls.Add(this.vidLabel);
            this.usbGroup.Controls.Add(this.selectionAllowAny);
            this.usbGroup.Controls.Add(this.pidLabel);
            this.usbGroup.Controls.Add(this.selectionExactModel);
            this.usbGroup.Controls.Add(this.VID);
            this.usbGroup.Controls.Add(this.selectionExcludeModel);
            this.usbGroup.Location = new System.Drawing.Point(20, 292);
            this.usbGroup.Margin = new System.Windows.Forms.Padding(4);
            this.usbGroup.Name = "usbGroup";
            this.usbGroup.Padding = new System.Windows.Forms.Padding(4);
            this.usbGroup.Size = new System.Drawing.Size(196, 188);
            this.usbGroup.TabIndex = 4;
            this.usbGroup.TabStop = false;
            this.usbGroup.Text = "Advanced USB Parameters";
            // 
            // advancedUSBParmsEnabled
            // 
            this.advancedUSBParmsEnabled.AutoSize = true;
            this.advancedUSBParmsEnabled.Location = new System.Drawing.Point(20, 265);
            this.advancedUSBParmsEnabled.Margin = new System.Windows.Forms.Padding(4);
            this.advancedUSBParmsEnabled.Name = "advancedUSBParmsEnabled";
            this.advancedUSBParmsEnabled.Size = new System.Drawing.Size(250, 21);
            this.advancedUSBParmsEnabled.TabIndex = 104;
            this.advancedUSBParmsEnabled.Text = "Enable Advanced USB Parameters";
            this.advancedUSBParmsEnabled.UseVisualStyleBackColor = true;
            this.advancedUSBParmsEnabled.CheckedChanged += new System.EventHandler(this.handleAdvancedUsbPropertiesChange);
            // 
            // binGroup
            // 
            this.binGroup.Controls.Add(this.maxYBin);
            this.binGroup.Controls.Add(this.radioButton2);
            this.binGroup.Controls.Add(this.symetricBinning);
            this.binGroup.Controls.Add(this.xBinLabel);
            this.binGroup.Controls.Add(this.maxXBin);
            this.binGroup.Controls.Add(this.binLabel);
            this.binGroup.Location = new System.Drawing.Point(223, 292);
            this.binGroup.Name = "binGroup";
            this.binGroup.Size = new System.Drawing.Size(204, 187);
            this.binGroup.TabIndex = 105;
            this.binGroup.TabStop = false;
            this.binGroup.Text = "Binning Control";
            // 
            // maxYBin
            // 
            this.maxYBin.Location = new System.Drawing.Point(82, 86);
            this.maxYBin.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.maxYBin.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxYBin.Name = "maxYBin";
            this.maxYBin.Size = new System.Drawing.Size(68, 22);
            this.maxYBin.TabIndex = 7;
            this.maxYBin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maxYBin.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(9, 49);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(141, 21);
            this.radioButton2.TabIndex = 6;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Asymetric Binning";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // symetricBinning
            // 
            this.symetricBinning.AutoSize = true;
            this.symetricBinning.Location = new System.Drawing.Point(9, 21);
            this.symetricBinning.Name = "symetricBinning";
            this.symetricBinning.Size = new System.Drawing.Size(134, 21);
            this.symetricBinning.TabIndex = 5;
            this.symetricBinning.TabStop = true;
            this.symetricBinning.Text = "Symetric Binning";
            this.symetricBinning.UseVisualStyleBackColor = true;
            this.symetricBinning.CheckedChanged += new System.EventHandler(this.symetricBinning_CheckedChanged);
            // 
            // xBinLabel
            // 
            this.xBinLabel.AutoSize = true;
            this.xBinLabel.Location = new System.Drawing.Point(6, 117);
            this.xBinLabel.Name = "xBinLabel";
            this.xBinLabel.Size = new System.Drawing.Size(70, 17);
            this.xBinLabel.TabIndex = 4;
            this.xBinLabel.Text = "Max X Bin";
            // 
            // maxXBin
            // 
            this.maxXBin.Location = new System.Drawing.Point(82, 115);
            this.maxXBin.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.maxXBin.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxXBin.Name = "maxXBin";
            this.maxXBin.Size = new System.Drawing.Size(68, 22);
            this.maxXBin.TabIndex = 3;
            this.maxXBin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maxXBin.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // binLabel
            // 
            this.binLabel.AutoSize = true;
            this.binLabel.Location = new System.Drawing.Point(6, 86);
            this.binLabel.Name = "binLabel";
            this.binLabel.Size = new System.Drawing.Size(57, 17);
            this.binLabel.TabIndex = 2;
            this.binLabel.Text = "Max Bin";
            // 
            // useDumpedData
            // 
            this.useDumpedData.AutoSize = true;
            this.useDumpedData.Location = new System.Drawing.Point(20, 237);
            this.useDumpedData.Name = "useDumpedData";
            this.useDumpedData.Size = new System.Drawing.Size(278, 21);
            this.useDumpedData.TabIndex = 106;
            this.useDumpedData.Text = "Use Dumped Data (for debugging only)";
            this.useDumpedData.UseVisualStyleBackColor = true;
            // 
            // SetupDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 496);
            this.Controls.Add(this.useDumpedData);
            this.Controls.Add(this.binGroup);
            this.Controls.Add(this.advancedUSBParmsEnabled);
            this.Controls.Add(this.usbGroup);
            this.Controls.Add(this.Copyright);
            this.Controls.Add(this.dumpDataEnabled);
            this.Controls.Add(this.Version);
            this.Controls.Add(this.EnableUntestedCheckBox);
            this.Controls.Add(this.EnableLoggingCheckBox);
            this.Controls.Add(this.picASCOM);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetupDialogForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SXCamera Setup";
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).EndInit();
            this.usbGroup.ResumeLayout(false);
            this.usbGroup.PerformLayout();
            this.binGroup.ResumeLayout(false);
            this.binGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxYBin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxXBin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox picASCOM;
        public System.Windows.Forms.CheckBox EnableLoggingCheckBox;
        public System.Windows.Forms.CheckBox EnableUntestedCheckBox;
        public System.Windows.Forms.Label Version;
        private System.Windows.Forms.ToolTip toolTip1;
        internal System.Windows.Forms.RadioButton selectionAllowAny;
        internal System.Windows.Forms.RadioButton selectionExcludeModel;
        internal System.Windows.Forms.RadioButton selectionExactModel;
        internal System.Windows.Forms.MaskedTextBox PID;
        internal System.Windows.Forms.MaskedTextBox VID;
        private System.Windows.Forms.Label Copyright;
        internal System.Windows.Forms.Label vidLabel;
        internal System.Windows.Forms.Label pidLabel;
        internal System.Windows.Forms.CheckBox advancedUSBParmsEnabled;
        internal System.Windows.Forms.GroupBox usbGroup;
        public System.Windows.Forms.Label xBinLabel;
        public System.Windows.Forms.NumericUpDown maxXBin;
        public System.Windows.Forms.Label binLabel;
        public System.Windows.Forms.RadioButton radioButton2;
        public System.Windows.Forms.RadioButton symetricBinning;
        public System.Windows.Forms.NumericUpDown maxYBin;
        public System.Windows.Forms.CheckBox useDumpedData;
        public System.Windows.Forms.GroupBox binGroup;
        public System.Windows.Forms.CheckBox dumpDataEnabled;
    }
}
