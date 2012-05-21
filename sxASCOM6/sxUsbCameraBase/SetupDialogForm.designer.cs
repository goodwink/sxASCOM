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
            this.fixedBin = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.fixedBinning = new System.Windows.Forms.CheckBox();
            this.asymetricBinning = new System.Windows.Forms.CheckBox();
            this.maxYBin = new System.Windows.Forms.NumericUpDown();
            this.xBinLabel = new System.Windows.Forms.Label();
            this.maxXBin = new System.Windows.Forms.NumericUpDown();
            this.yBinLabel = new System.Windows.Forms.Label();
            this.useDumpedData = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gaussianBlurRadius = new System.Windows.Forms.NumericUpDown();
            this.interlacedDoubleExposureThreshold = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.gaussianBlur = new System.Windows.Forms.CheckBox();
            this.equalizeFrames = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.doubleExposeShort = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.usbGroup.SuspendLayout();
            this.binGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fixedBin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxYBin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxXBin)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gaussianBlurRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.interlacedDoubleExposureThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(433, 71);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(59, 24);
            this.cmdOK.TabIndex = 6;
            this.cmdOK.Text = "OK";
            this.toolTip1.SetToolTip(this.cmdOK, "Close this dialog saving changes.");
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(433, 101);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(59, 25);
            this.cmdCancel.TabIndex = 0;
            this.cmdCancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.cmdCancel, "Close this dialog with no changes.");
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(254, 21);
            this.label1.TabIndex = 101;
            this.label1.Text = "ASCOM Driver for SX/SXV/SXVF/SXVR Cameras\r\n\r\n";
            // 
            // picASCOM
            // 
            this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picASCOM.Image = global::ASCOM.sxUsbCameraBase.Properties.Resources.ASCOM;
            this.picASCOM.Location = new System.Drawing.Point(438, 9);
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
            this.EnableLoggingCheckBox.Location = new System.Drawing.Point(15, 123);
            this.EnableLoggingCheckBox.Name = "EnableLoggingCheckBox";
            this.EnableLoggingCheckBox.Size = new System.Drawing.Size(100, 17);
            this.EnableLoggingCheckBox.TabIndex = 1;
            this.EnableLoggingCheckBox.Text = "Enable Logging";
            this.toolTip1.SetToolTip(this.EnableLoggingCheckBox, "Enable the creation of a debug log file.");
            this.EnableLoggingCheckBox.UseVisualStyleBackColor = true;
            // 
            // EnableUntestedCheckBox
            // 
            this.EnableUntestedCheckBox.AutoSize = true;
            this.EnableUntestedCheckBox.Location = new System.Drawing.Point(15, 146);
            this.EnableUntestedCheckBox.Name = "EnableUntestedCheckBox";
            this.EnableUntestedCheckBox.Size = new System.Drawing.Size(195, 17);
            this.EnableUntestedCheckBox.TabIndex = 2;
            this.EnableUntestedCheckBox.Text = "Enable Untested Cameras/Features";
            this.toolTip1.SetToolTip(this.EnableUntestedCheckBox, "Allowed untested features and untested cameras to be used.");
            this.EnableUntestedCheckBox.UseVisualStyleBackColor = true;
            // 
            // Version
            // 
            this.Version.AutoSize = true;
            this.Version.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Version.Location = new System.Drawing.Point(12, 30);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(80, 15);
            this.Version.TabIndex = 102;
            this.Version.Text = "Version: a.b.c";
            // 
            // dumpDataEnabled
            // 
            this.dumpDataEnabled.AutoSize = true;
            this.dumpDataEnabled.Location = new System.Drawing.Point(15, 169);
            this.dumpDataEnabled.Name = "dumpDataEnabled";
            this.dumpDataEnabled.Size = new System.Drawing.Size(207, 17);
            this.dumpDataEnabled.TabIndex = 3;
            this.dumpDataEnabled.Text = "Dump Data to File (for debugging only)";
            this.toolTip1.SetToolTip(this.dumpDataEnabled, "Divide the requested exposure by 1000.  Useful if you need faster exposures than " +
                    "your caputure software allows for.");
            this.dumpDataEnabled.UseVisualStyleBackColor = true;
            // 
            // PID
            // 
            this.PID.Location = new System.Drawing.Point(37, 128);
            this.PID.Mask = "9990";
            this.PID.Name = "PID";
            this.PID.PromptChar = ' ';
            this.PID.Size = new System.Drawing.Size(36, 20);
            this.PID.TabIndex = 6;
            this.PID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // selectionExcludeModel
            // 
            this.selectionExcludeModel.AutoSize = true;
            this.selectionExcludeModel.Location = new System.Drawing.Point(6, 77);
            this.selectionExcludeModel.Name = "selectionExcludeModel";
            this.selectionExcludeModel.Size = new System.Drawing.Size(95, 17);
            this.selectionExcludeModel.TabIndex = 2;
            this.selectionExcludeModel.Text = "Exclude Model";
            this.selectionExcludeModel.UseVisualStyleBackColor = true;
            // 
            // VID
            // 
            this.VID.Location = new System.Drawing.Point(37, 102);
            this.VID.Mask = "9990";
            this.VID.Name = "VID";
            this.VID.PromptChar = ' ';
            this.VID.Size = new System.Drawing.Size(36, 20);
            this.VID.TabIndex = 4;
            this.VID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // pidLabel
            // 
            this.pidLabel.AutoSize = true;
            this.pidLabel.Location = new System.Drawing.Point(3, 128);
            this.pidLabel.Name = "pidLabel";
            this.pidLabel.Size = new System.Drawing.Size(28, 13);
            this.pidLabel.TabIndex = 5;
            this.pidLabel.Text = "PID:";
            // 
            // selectionExactModel
            // 
            this.selectionExactModel.AutoSize = true;
            this.selectionExactModel.Location = new System.Drawing.Point(6, 54);
            this.selectionExactModel.Name = "selectionExactModel";
            this.selectionExactModel.Size = new System.Drawing.Size(87, 17);
            this.selectionExactModel.TabIndex = 1;
            this.selectionExactModel.Text = "Select Model";
            this.selectionExactModel.UseVisualStyleBackColor = true;
            // 
            // selectionAllowAny
            // 
            this.selectionAllowAny.AutoSize = true;
            this.selectionAllowAny.Checked = true;
            this.selectionAllowAny.Location = new System.Drawing.Point(6, 31);
            this.selectionAllowAny.Name = "selectionAllowAny";
            this.selectionAllowAny.Size = new System.Drawing.Size(75, 17);
            this.selectionAllowAny.TabIndex = 0;
            this.selectionAllowAny.TabStop = true;
            this.selectionAllowAny.Text = "Any Model";
            this.selectionAllowAny.UseVisualStyleBackColor = true;
            this.selectionAllowAny.CheckedChanged += new System.EventHandler(this.camera1SelectionAllowAny_CheckedChanged);
            // 
            // vidLabel
            // 
            this.vidLabel.AutoSize = true;
            this.vidLabel.Location = new System.Drawing.Point(3, 105);
            this.vidLabel.Name = "vidLabel";
            this.vidLabel.Size = new System.Drawing.Size(28, 13);
            this.vidLabel.TabIndex = 3;
            this.vidLabel.Text = "VID:";
            // 
            // Copyright
            // 
            this.Copyright.Location = new System.Drawing.Point(12, 55);
            this.Copyright.Name = "Copyright";
            this.Copyright.Size = new System.Drawing.Size(254, 57);
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
            this.usbGroup.Location = new System.Drawing.Point(339, 238);
            this.usbGroup.Name = "usbGroup";
            this.usbGroup.Size = new System.Drawing.Size(147, 163);
            this.usbGroup.TabIndex = 4;
            this.usbGroup.TabStop = false;
            this.usbGroup.Text = "Advanced USB Parameters";
            // 
            // advancedUSBParmsEnabled
            // 
            this.advancedUSBParmsEnabled.AutoSize = true;
            this.advancedUSBParmsEnabled.Location = new System.Drawing.Point(15, 215);
            this.advancedUSBParmsEnabled.Name = "advancedUSBParmsEnabled";
            this.advancedUSBParmsEnabled.Size = new System.Drawing.Size(192, 17);
            this.advancedUSBParmsEnabled.TabIndex = 104;
            this.advancedUSBParmsEnabled.Text = "Enable Advanced USB Parameters";
            this.advancedUSBParmsEnabled.UseVisualStyleBackColor = true;
            this.advancedUSBParmsEnabled.CheckedChanged += new System.EventHandler(this.handleAdvancedUsbPropertiesChange);
            // 
            // binGroup
            // 
            this.binGroup.Controls.Add(this.fixedBin);
            this.binGroup.Controls.Add(this.label5);
            this.binGroup.Controls.Add(this.fixedBinning);
            this.binGroup.Controls.Add(this.asymetricBinning);
            this.binGroup.Controls.Add(this.maxYBin);
            this.binGroup.Controls.Add(this.xBinLabel);
            this.binGroup.Controls.Add(this.maxXBin);
            this.binGroup.Controls.Add(this.yBinLabel);
            this.binGroup.Location = new System.Drawing.Point(15, 237);
            this.binGroup.Margin = new System.Windows.Forms.Padding(2);
            this.binGroup.Name = "binGroup";
            this.binGroup.Padding = new System.Windows.Forms.Padding(2);
            this.binGroup.Size = new System.Drawing.Size(153, 164);
            this.binGroup.TabIndex = 105;
            this.binGroup.TabStop = false;
            this.binGroup.Text = "Binning Control";
            // 
            // fixedBin
            // 
            this.fixedBin.Location = new System.Drawing.Point(59, 109);
            this.fixedBin.Name = "fixedBin";
            this.fixedBin.Size = new System.Drawing.Size(51, 20);
            this.fixedBin.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Fixed Bin:";
            // 
            // fixedBinning
            // 
            this.fixedBinning.AutoSize = true;
            this.fixedBinning.Location = new System.Drawing.Point(6, 85);
            this.fixedBinning.Name = "fixedBinning";
            this.fixedBinning.Size = new System.Drawing.Size(89, 17);
            this.fixedBinning.TabIndex = 9;
            this.fixedBinning.Text = "Fixed Binning";
            this.fixedBinning.UseVisualStyleBackColor = true;
            this.fixedBinning.CheckedChanged += new System.EventHandler(this.fixedBinning_CheckedChanged);
            // 
            // asymetricBinning
            // 
            this.asymetricBinning.AutoSize = true;
            this.asymetricBinning.Location = new System.Drawing.Point(6, 19);
            this.asymetricBinning.Name = "asymetricBinning";
            this.asymetricBinning.Size = new System.Drawing.Size(153, 17);
            this.asymetricBinning.TabIndex = 8;
            this.asymetricBinning.Text = "Enable Asymmetric Binning";
            this.asymetricBinning.UseVisualStyleBackColor = true;
            this.asymetricBinning.CheckedChanged += new System.EventHandler(this.asymetricBinning_CheckedChanged);
            // 
            // maxYBin
            // 
            this.maxYBin.Location = new System.Drawing.Point(59, 64);
            this.maxYBin.Margin = new System.Windows.Forms.Padding(2);
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
            this.maxYBin.Size = new System.Drawing.Size(51, 20);
            this.maxYBin.TabIndex = 7;
            this.maxYBin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maxYBin.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // xBinLabel
            // 
            this.xBinLabel.AutoSize = true;
            this.xBinLabel.Location = new System.Drawing.Point(1, 42);
            this.xBinLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.xBinLabel.Name = "xBinLabel";
            this.xBinLabel.Size = new System.Drawing.Size(45, 13);
            this.xBinLabel.TabIndex = 4;
            this.xBinLabel.Text = "Max Bin";
            // 
            // maxXBin
            // 
            this.maxXBin.Location = new System.Drawing.Point(59, 40);
            this.maxXBin.Margin = new System.Windows.Forms.Padding(2);
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
            this.maxXBin.Size = new System.Drawing.Size(51, 20);
            this.maxXBin.TabIndex = 3;
            this.maxXBin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maxXBin.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // yBinLabel
            // 
            this.yBinLabel.AutoSize = true;
            this.yBinLabel.Location = new System.Drawing.Point(1, 64);
            this.yBinLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.yBinLabel.Name = "yBinLabel";
            this.yBinLabel.Size = new System.Drawing.Size(55, 13);
            this.yBinLabel.TabIndex = 2;
            this.yBinLabel.Text = "Max Y Bin";
            // 
            // useDumpedData
            // 
            this.useDumpedData.AutoSize = true;
            this.useDumpedData.Location = new System.Drawing.Point(15, 193);
            this.useDumpedData.Margin = new System.Windows.Forms.Padding(2);
            this.useDumpedData.Name = "useDumpedData";
            this.useDumpedData.Size = new System.Drawing.Size(210, 17);
            this.useDumpedData.TabIndex = 106;
            this.useDumpedData.Text = "Use Dumped Data (for debugging only)";
            this.useDumpedData.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gaussianBlurRadius);
            this.groupBox1.Controls.Add(this.interlacedDoubleExposureThreshold);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.gaussianBlur);
            this.groupBox1.Controls.Add(this.equalizeFrames);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.doubleExposeShort);
            this.groupBox1.Location = new System.Drawing.Point(180, 238);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(147, 163);
            this.groupBox1.TabIndex = 107;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Interlaced Adjustments";
            // 
            // gaussianBlurRadius
            // 
            this.gaussianBlurRadius.DecimalPlaces = 1;
            this.gaussianBlurRadius.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.gaussianBlurRadius.Location = new System.Drawing.Point(52, 103);
            this.gaussianBlurRadius.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            65536});
            this.gaussianBlurRadius.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.gaussianBlurRadius.Name = "gaussianBlurRadius";
            this.gaussianBlurRadius.Size = new System.Drawing.Size(54, 20);
            this.gaussianBlurRadius.TabIndex = 9;
            this.gaussianBlurRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.gaussianBlurRadius.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // interlacedDoubleExposureThreshold
            // 
            this.interlacedDoubleExposureThreshold.Location = new System.Drawing.Point(45, 60);
            this.interlacedDoubleExposureThreshold.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.interlacedDoubleExposureThreshold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.interlacedDoubleExposureThreshold.Name = "interlacedDoubleExposureThreshold";
            this.interlacedDoubleExposureThreshold.Size = new System.Drawing.Size(61, 20);
            this.interlacedDoubleExposureThreshold.TabIndex = 8;
            this.interlacedDoubleExposureThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.interlacedDoubleExposureThreshold.ThousandsSeparator = true;
            this.interlacedDoubleExposureThreshold.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Radius:";
            // 
            // gaussianBlur
            // 
            this.gaussianBlur.AutoSize = true;
            this.gaussianBlur.Location = new System.Drawing.Point(6, 85);
            this.gaussianBlur.Name = "gaussianBlur";
            this.gaussianBlur.Size = new System.Drawing.Size(91, 17);
            this.gaussianBlur.TabIndex = 5;
            this.gaussianBlur.Text = "Gaussian Blur";
            this.gaussianBlur.UseVisualStyleBackColor = true;
            this.gaussianBlur.CheckedChanged += new System.EventHandler(this.gaussianBlur_CheckedChanged);
            // 
            // equalizeFrames
            // 
            this.equalizeFrames.AutoSize = true;
            this.equalizeFrames.Location = new System.Drawing.Point(6, 19);
            this.equalizeFrames.Name = "equalizeFrames";
            this.equalizeFrames.Size = new System.Drawing.Size(103, 17);
            this.equalizeFrames.TabIndex = 4;
            this.equalizeFrames.Text = "Equalize Frames";
            this.equalizeFrames.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(112, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "ms";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Cutoff:";
            // 
            // doubleExposeShort
            // 
            this.doubleExposeShort.AutoSize = true;
            this.doubleExposeShort.Location = new System.Drawing.Point(6, 36);
            this.doubleExposeShort.Name = "doubleExposeShort";
            this.doubleExposeShort.Size = new System.Drawing.Size(126, 17);
            this.doubleExposeShort.TabIndex = 0;
            this.doubleExposeShort.Text = "Double Expose Short";
            this.doubleExposeShort.UseVisualStyleBackColor = true;
            this.doubleExposeShort.CheckedChanged += new System.EventHandler(this.doubleExposeShort_CheckedChanged);
            // 
            // SetupDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 417);
            this.Controls.Add(this.groupBox1);
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
            ((System.ComponentModel.ISupportInitialize)(this.fixedBin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxYBin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxXBin)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gaussianBlurRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.interlacedDoubleExposureThreshold)).EndInit();
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
        public System.Windows.Forms.Label yBinLabel;
        public System.Windows.Forms.NumericUpDown maxYBin;
        public System.Windows.Forms.CheckBox useDumpedData;
        public System.Windows.Forms.GroupBox binGroup;
        public System.Windows.Forms.CheckBox dumpDataEnabled;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.CheckBox asymetricBinning;
        public System.Windows.Forms.CheckBox fixedBinning;
        public System.Windows.Forms.NumericUpDown fixedBin;
        public System.Windows.Forms.CheckBox equalizeFrames;
        public System.Windows.Forms.CheckBox doubleExposeShort;
        public System.Windows.Forms.CheckBox gaussianBlur;
        public System.Windows.Forms.NumericUpDown interlacedDoubleExposureThreshold;
        public System.Windows.Forms.NumericUpDown gaussianBlurRadius;
    }
}
