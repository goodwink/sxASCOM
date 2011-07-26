namespace ASCOM.SXCamera
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
<<<<<<< HEAD
            this.label2 = new System.Windows.Forms.Label();
            this.fixedBin = new System.Windows.Forms.NumericUpDown();
            this.fixedBinning = new System.Windows.Forms.CheckBox();
            this.asymetricBinning = new System.Windows.Forms.CheckBox();
            this.maxYBin = new System.Windows.Forms.NumericUpDown();
=======
            this.maxYBin = new System.Windows.Forms.NumericUpDown();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.symetricBinning = new System.Windows.Forms.RadioButton();
>>>>>>> Setup dialog changes
            this.xBinLabel = new System.Windows.Forms.Label();
            this.maxXBin = new System.Windows.Forms.NumericUpDown();
            this.binLabel = new System.Windows.Forms.Label();
            this.dumpDataEnabled = new System.Windows.Forms.CheckBox();
<<<<<<< HEAD
            this.useDumpedData = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.squareLodestarPixels = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.doubleExposureThreshold = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.gaussianBlurRadius = new System.Windows.Forms.NumericUpDown();
            this.doubleExposeShort = new System.Windows.Forms.CheckBox();
            this.equalizeFrames = new System.Windows.Forms.CheckBox();
            this.gaussianBlur = new System.Windows.Forms.CheckBox();
            this.waitForCooldown = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.hardwareExposureThreshold = new System.Windows.Forms.NumericUpDown();
            this.colorBinning = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.usbGroup.SuspendLayout();
            this.binGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fixedBin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxYBin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxXBin)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.doubleExposureThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gaussianBlurRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hardwareExposureThreshold)).BeginInit();
=======
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.usbGroup.SuspendLayout();
            this.binGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxYBin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxXBin)).BeginInit();
>>>>>>> Setup dialog changes
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(497, 74);
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
            this.cmdCancel.Location = new System.Drawing.Point(497, 104);
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
            this.picASCOM.Image = global::ASCOM.SXCamera.Properties.Resources.ASCOM;
            this.picASCOM.Location = new System.Drawing.Point(508, 12);
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
            // PID
            // 
            this.PID.Location = new System.Drawing.Point(43, 116);
            this.PID.Mask = "99990";
            this.PID.Name = "PID";
            this.PID.PromptChar = ' ';
            this.PID.Size = new System.Drawing.Size(36, 20);
            this.PID.TabIndex = 6;
            this.PID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // selectionExcludeModel
            // 
            this.selectionExcludeModel.AutoSize = true;
            this.selectionExcludeModel.Location = new System.Drawing.Point(12, 65);
            this.selectionExcludeModel.Name = "selectionExcludeModel";
            this.selectionExcludeModel.Size = new System.Drawing.Size(95, 17);
            this.selectionExcludeModel.TabIndex = 2;
            this.selectionExcludeModel.Text = "Exclude Model";
            this.selectionExcludeModel.UseVisualStyleBackColor = true;
            // 
            // VID
            // 
            this.VID.Location = new System.Drawing.Point(43, 90);
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
            this.pidLabel.Location = new System.Drawing.Point(9, 116);
            this.pidLabel.Name = "pidLabel";
            this.pidLabel.Size = new System.Drawing.Size(28, 13);
            this.pidLabel.TabIndex = 5;
            this.pidLabel.Text = "PID:";
            // 
            // selectionExactModel
            // 
            this.selectionExactModel.AutoSize = true;
            this.selectionExactModel.Location = new System.Drawing.Point(12, 42);
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
            this.selectionAllowAny.Location = new System.Drawing.Point(12, 19);
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
            this.vidLabel.Location = new System.Drawing.Point(9, 93);
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
<<<<<<< HEAD
            this.usbGroup.Location = new System.Drawing.Point(383, 292);
=======
            this.usbGroup.Location = new System.Drawing.Point(20, 292);
            this.usbGroup.Margin = new System.Windows.Forms.Padding(4);
>>>>>>> Setup dialog changes
            this.usbGroup.Name = "usbGroup";
            this.usbGroup.Size = new System.Drawing.Size(161, 168);
            this.usbGroup.TabIndex = 4;
            this.usbGroup.TabStop = false;
            this.usbGroup.Text = "Advanced USB Parameters";
            // 
            // advancedUSBParmsEnabled
            // 
            this.advancedUSBParmsEnabled.AutoSize = true;
<<<<<<< HEAD
            this.advancedUSBParmsEnabled.Location = new System.Drawing.Point(15, 215);
=======
            this.advancedUSBParmsEnabled.Location = new System.Drawing.Point(20, 265);
            this.advancedUSBParmsEnabled.Margin = new System.Windows.Forms.Padding(4);
>>>>>>> Setup dialog changes
            this.advancedUSBParmsEnabled.Name = "advancedUSBParmsEnabled";
            this.advancedUSBParmsEnabled.Size = new System.Drawing.Size(192, 17);
            this.advancedUSBParmsEnabled.TabIndex = 104;
            this.advancedUSBParmsEnabled.Text = "Enable Advanced USB Parameters";
            this.advancedUSBParmsEnabled.UseVisualStyleBackColor = true;
            this.advancedUSBParmsEnabled.CheckedChanged += new System.EventHandler(this.handleAdvancedUsbPropertiesChange);
            // 
            // binGroup
            // 
<<<<<<< HEAD
            this.binGroup.Controls.Add(this.colorBinning);
            this.binGroup.Controls.Add(this.label2);
            this.binGroup.Controls.Add(this.fixedBin);
            this.binGroup.Controls.Add(this.fixedBinning);
            this.binGroup.Controls.Add(this.asymetricBinning);
            this.binGroup.Controls.Add(this.maxYBin);
            this.binGroup.Controls.Add(this.xBinLabel);
            this.binGroup.Controls.Add(this.maxXBin);
            this.binGroup.Controls.Add(this.binLabel);
            this.binGroup.Location = new System.Drawing.Point(15, 292);
            this.binGroup.Margin = new System.Windows.Forms.Padding(2);
            this.binGroup.Name = "binGroup";
            this.binGroup.Padding = new System.Windows.Forms.Padding(2);
            this.binGroup.Size = new System.Drawing.Size(171, 169);
=======
            this.binGroup.Controls.Add(this.maxYBin);
            this.binGroup.Controls.Add(this.radioButton2);
            this.binGroup.Controls.Add(this.symetricBinning);
            this.binGroup.Controls.Add(this.xBinLabel);
            this.binGroup.Controls.Add(this.maxXBin);
            this.binGroup.Controls.Add(this.binLabel);
            this.binGroup.Location = new System.Drawing.Point(223, 292);
            this.binGroup.Name = "binGroup";
            this.binGroup.Size = new System.Drawing.Size(204, 187);
>>>>>>> Setup dialog changes
            this.binGroup.TabIndex = 105;
            this.binGroup.TabStop = false;
            this.binGroup.Text = "Binning Control";
            // 
<<<<<<< HEAD
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Fixed Bin:";
            // 
            // fixedBin
            // 
            this.fixedBin.Location = new System.Drawing.Point(80, 139);
            this.fixedBin.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.fixedBin.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.fixedBin.Name = "fixedBin";
            this.fixedBin.Size = new System.Drawing.Size(39, 20);
            this.fixedBin.TabIndex = 10;
            this.fixedBin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.fixedBin.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // fixedBinning
            // 
            this.fixedBinning.AutoSize = true;
            this.fixedBinning.Location = new System.Drawing.Point(6, 115);
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
            this.asymetricBinning.Location = new System.Drawing.Point(6, 45);
            this.asymetricBinning.Name = "asymetricBinning";
            this.asymetricBinning.Size = new System.Drawing.Size(153, 17);
            this.asymetricBinning.TabIndex = 8;
            this.asymetricBinning.Text = "Enable Asymmetric Binning";
            this.asymetricBinning.UseVisualStyleBackColor = true;
            this.asymetricBinning.CheckedChanged += new System.EventHandler(this.asymetricBinning_CheckedChanged);
            // 
            // maxYBin
            // 
            this.maxYBin.Location = new System.Drawing.Point(68, 69);
            this.maxYBin.Margin = new System.Windows.Forms.Padding(2);
=======
            // maxYBin
            // 
            this.maxYBin.Location = new System.Drawing.Point(82, 86);
>>>>>>> Setup dialog changes
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
<<<<<<< HEAD
            this.maxYBin.Size = new System.Drawing.Size(51, 20);
=======
            this.maxYBin.Size = new System.Drawing.Size(68, 22);
>>>>>>> Setup dialog changes
            this.maxYBin.TabIndex = 7;
            this.maxYBin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maxYBin.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
<<<<<<< HEAD
=======
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
>>>>>>> Setup dialog changes
            // 
            // xBinLabel
            // 
            this.xBinLabel.AutoSize = true;
            this.xBinLabel.Location = new System.Drawing.Point(3, 93);
            this.xBinLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.xBinLabel.Name = "xBinLabel";
            this.xBinLabel.Size = new System.Drawing.Size(55, 13);
            this.xBinLabel.TabIndex = 4;
            this.xBinLabel.Text = "Max X Bin";
            // 
            // maxXBin
            // 
            this.maxXBin.Location = new System.Drawing.Point(68, 93);
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
            // binLabel
            // 
            this.binLabel.AutoSize = true;
            this.binLabel.Location = new System.Drawing.Point(3, 71);
            this.binLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.binLabel.Name = "binLabel";
            this.binLabel.Size = new System.Drawing.Size(45, 13);
            this.binLabel.TabIndex = 2;
            this.binLabel.Text = "Max Bin";
            // 
            // dumpDataEnabled
            // 
            this.dumpDataEnabled.AutoSize = true;
<<<<<<< HEAD
            this.dumpDataEnabled.Location = new System.Drawing.Point(15, 170);
            this.dumpDataEnabled.Name = "dumpDataEnabled";
            this.dumpDataEnabled.Size = new System.Drawing.Size(207, 17);
            this.dumpDataEnabled.TabIndex = 106;
            this.dumpDataEnabled.Text = "Dump Data to File (for debugging only)";
            this.dumpDataEnabled.UseVisualStyleBackColor = true;
            // 
            // useDumpedData
            // 
            this.useDumpedData.AutoSize = true;
            this.useDumpedData.Location = new System.Drawing.Point(15, 192);
            this.useDumpedData.Name = "useDumpedData";
            this.useDumpedData.Size = new System.Drawing.Size(210, 17);
            this.useDumpedData.TabIndex = 107;
            this.useDumpedData.Text = "Use Dumped Data (for debugging only)";
            this.useDumpedData.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.squareLodestarPixels);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.doubleExposureThreshold);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.gaussianBlurRadius);
            this.groupBox2.Controls.Add(this.doubleExposeShort);
            this.groupBox2.Controls.Add(this.equalizeFrames);
            this.groupBox2.Controls.Add(this.gaussianBlur);
            this.groupBox2.Location = new System.Drawing.Point(202, 292);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(160, 168);
            this.groupBox2.TabIndex = 108;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Interlaced Adjustments";
            // 
            // squareLodestarPixels
            // 
            this.squareLodestarPixels.AutoSize = true;
            this.squareLodestarPixels.Location = new System.Drawing.Point(7, 42);
            this.squareLodestarPixels.Name = "squareLodestarPixels";
            this.squareLodestarPixels.Size = new System.Drawing.Size(134, 17);
            this.squareLodestarPixels.TabIndex = 8;
            this.squareLodestarPixels.Text = "Square Lodestar Pixels";
            this.squareLodestarPixels.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(127, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "ms";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Radius:";
            // 
            // doubleExposureThreshold
            // 
            this.doubleExposureThreshold.Location = new System.Drawing.Point(63, 139);
            this.doubleExposureThreshold.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.doubleExposureThreshold.Name = "doubleExposureThreshold";
            this.doubleExposureThreshold.Size = new System.Drawing.Size(57, 20);
            this.doubleExposureThreshold.TabIndex = 3;
            this.doubleExposureThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Cutoff:";
            // 
            // gaussianBlurRadius
            // 
            this.gaussianBlurRadius.DecimalPlaces = 1;
            this.gaussianBlurRadius.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.gaussianBlurRadius.Location = new System.Drawing.Point(63, 89);
            this.gaussianBlurRadius.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            65536});
            this.gaussianBlurRadius.Name = "gaussianBlurRadius";
            this.gaussianBlurRadius.Size = new System.Drawing.Size(57, 20);
            this.gaussianBlurRadius.TabIndex = 5;
            this.gaussianBlurRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // doubleExposeShort
            // 
            this.doubleExposeShort.AutoSize = true;
            this.doubleExposeShort.Location = new System.Drawing.Point(6, 115);
            this.doubleExposeShort.Name = "doubleExposeShort";
            this.doubleExposeShort.Size = new System.Drawing.Size(126, 17);
            this.doubleExposeShort.TabIndex = 1;
            this.doubleExposeShort.Text = "Double Expose Short";
            this.doubleExposeShort.UseVisualStyleBackColor = true;
            this.doubleExposeShort.CheckedChanged += new System.EventHandler(this.doubleExposeShort_CheckedChanged);
            // 
            // equalizeFrames
            // 
            this.equalizeFrames.AutoSize = true;
            this.equalizeFrames.Location = new System.Drawing.Point(7, 20);
            this.equalizeFrames.Name = "equalizeFrames";
            this.equalizeFrames.Size = new System.Drawing.Size(103, 17);
            this.equalizeFrames.TabIndex = 0;
            this.equalizeFrames.Text = "Equalize Frames";
            this.equalizeFrames.UseVisualStyleBackColor = true;
            // 
            // gaussianBlur
            // 
            this.gaussianBlur.AutoSize = true;
            this.gaussianBlur.Location = new System.Drawing.Point(6, 65);
            this.gaussianBlur.Name = "gaussianBlur";
            this.gaussianBlur.Size = new System.Drawing.Size(91, 17);
            this.gaussianBlur.TabIndex = 4;
            this.gaussianBlur.Text = "Gaussian Blur";
            this.gaussianBlur.UseVisualStyleBackColor = true;
            this.gaussianBlur.CheckedChanged += new System.EventHandler(this.gaussianBlur_CheckedChanged);
            // 
            // waitForCooldown
            // 
            this.waitForCooldown.AutoSize = true;
            this.waitForCooldown.Location = new System.Drawing.Point(15, 239);
            this.waitForCooldown.Name = "waitForCooldown";
            this.waitForCooldown.Size = new System.Drawing.Size(192, 17);
            this.waitForCooldown.TabIndex = 109;
            this.waitForCooldown.Text = "Wait for Cooldown before exposure";
            this.waitForCooldown.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 263);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(203, 13);
            this.label6.TabIndex = 110;
            this.label6.Text = "Max exposure seconds for hardware timer";
            // 
            // hardwareExposureThreshold
            // 
            this.hardwareExposureThreshold.DecimalPlaces = 2;
            this.hardwareExposureThreshold.Location = new System.Drawing.Point(224, 261);
            this.hardwareExposureThreshold.Name = "hardwareExposureThreshold";
            this.hardwareExposureThreshold.Size = new System.Drawing.Size(57, 20);
            this.hardwareExposureThreshold.TabIndex = 111;
            this.hardwareExposureThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.hardwareExposureThreshold.Value = new decimal(new int[] {
            20,
            0,
            0,
            65536});
            // 
            // colorBinning
            // 
            this.colorBinning.AutoSize = true;
            this.colorBinning.Location = new System.Drawing.Point(6, 19);
            this.colorBinning.Name = "colorBinning";
            this.colorBinning.Size = new System.Drawing.Size(137, 17);
            this.colorBinning.TabIndex = 12;
            this.colorBinning.Text = "M26C 2x2 color binning";
            this.colorBinning.UseVisualStyleBackColor = true;
=======
            this.dumpDataEnabled.Location = new System.Drawing.Point(20, 237);
            this.dumpDataEnabled.Name = "dumpDataEnabled";
            this.dumpDataEnabled.Size = new System.Drawing.Size(275, 21);
            this.dumpDataEnabled.TabIndex = 106;
            this.dumpDataEnabled.Text = "Dump Data to File (for debugging only)";
            this.dumpDataEnabled.UseVisualStyleBackColor = true;
>>>>>>> Setup dialog changes
            // 
            // SetupDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
<<<<<<< HEAD
            this.ClientSize = new System.Drawing.Size(569, 472);
            this.Controls.Add(this.hardwareExposureThreshold);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.waitForCooldown);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.useDumpedData);
=======
            this.ClientSize = new System.Drawing.Size(449, 496);
>>>>>>> Setup dialog changes
            this.Controls.Add(this.dumpDataEnabled);
            this.Controls.Add(this.binGroup);
            this.Controls.Add(this.advancedUSBParmsEnabled);
            this.Controls.Add(this.usbGroup);
            this.Controls.Add(this.Copyright);
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
<<<<<<< HEAD
            ((System.ComponentModel.ISupportInitialize)(this.fixedBin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxYBin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxXBin)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.doubleExposureThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gaussianBlurRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hardwareExposureThreshold)).EndInit();
=======
            ((System.ComponentModel.ISupportInitialize)(this.maxYBin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxXBin)).EndInit();
>>>>>>> Setup dialog changes
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
        public System.Windows.Forms.NumericUpDown maxYBin;
        public System.Windows.Forms.CheckBox asymetricBinning;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.CheckBox fixedBinning;
        public System.Windows.Forms.NumericUpDown fixedBin;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.CheckBox equalizeFrames;
        public System.Windows.Forms.CheckBox gaussianBlur;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.CheckBox dumpDataEnabled;
        public System.Windows.Forms.CheckBox useDumpedData;
        public System.Windows.Forms.GroupBox binGroup;
        public System.Windows.Forms.CheckBox doubleExposeShort;
        public System.Windows.Forms.NumericUpDown doubleExposureThreshold;
        public System.Windows.Forms.NumericUpDown gaussianBlurRadius;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.CheckBox squareLodestarPixels;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.CheckBox waitForCooldown;
        public System.Windows.Forms.NumericUpDown hardwareExposureThreshold;
        public System.Windows.Forms.CheckBox colorBinning;
    }
}
