namespace ASCOM.SXGeneric
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
            this.secondsAreMiliseconds = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cameraSelectionExcludeModel = new System.Windows.Forms.RadioButton();
            this.cameraSelectionExactModel = new System.Windows.Forms.RadioButton();
            this.cameraSelectionAllowAny = new System.Windows.Forms.RadioButton();
            this.modelSelectionGroup = new System.Windows.Forms.GroupBox();
            this.modelPID = new System.Windows.Forms.MaskedTextBox();
            this.modelVID = new System.Windows.Forms.MaskedTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Copyright = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.modelSelectionGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(293, 88);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(59, 24);
            this.cmdOK.TabIndex = 11;
            this.cmdOK.Text = "OK";
            this.toolTip1.SetToolTip(this.cmdOK, "Close this dialog saving changes.");
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(293, 123);
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
            this.label1.Size = new System.Drawing.Size(286, 21);
            this.label1.TabIndex = 101;
            this.label1.Text = "ASCOM Driver for SX/SXV/SXVF/SXVR Cameras\r\n\r\n";
            // 
            // picASCOM
            // 
            this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picASCOM.Image = global::ASCOM.SXGeneric.Properties.Resources.ASCOM;
            this.picASCOM.Location = new System.Drawing.Point(304, 13);
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
            // secondsAreMiliseconds
            // 
            this.secondsAreMiliseconds.AutoSize = true;
            this.secondsAreMiliseconds.Location = new System.Drawing.Point(15, 169);
            this.secondsAreMiliseconds.Name = "secondsAreMiliseconds";
            this.secondsAreMiliseconds.Size = new System.Drawing.Size(147, 17);
            this.secondsAreMiliseconds.TabIndex = 3;
            this.secondsAreMiliseconds.Text = "Seconds Are Milliseconds";
            this.toolTip1.SetToolTip(this.secondsAreMiliseconds, "Divide the requested exposure by 1000.  Useful if you need faster exposures than " +
                    "your caputure software allows for.");
            this.secondsAreMiliseconds.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cameraSelectionExcludeModel);
            this.groupBox1.Controls.Add(this.cameraSelectionExactModel);
            this.groupBox1.Controls.Add(this.cameraSelectionAllowAny);
            this.groupBox1.Location = new System.Drawing.Point(15, 192);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 89);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera Selection";
            // 
            // cameraSelectionExcludeModel
            // 
            this.cameraSelectionExcludeModel.AutoSize = true;
            this.cameraSelectionExcludeModel.Location = new System.Drawing.Point(21, 65);
            this.cameraSelectionExcludeModel.Name = "cameraSelectionExcludeModel";
            this.cameraSelectionExcludeModel.Size = new System.Drawing.Size(95, 17);
            this.cameraSelectionExcludeModel.TabIndex = 7;
            this.cameraSelectionExcludeModel.Text = "Exclude Model";
            this.cameraSelectionExcludeModel.UseVisualStyleBackColor = true;
            // 
            // cameraSelectionExactModel
            // 
            this.cameraSelectionExactModel.AutoSize = true;
            this.cameraSelectionExactModel.Location = new System.Drawing.Point(21, 42);
            this.cameraSelectionExactModel.Name = "cameraSelectionExactModel";
            this.cameraSelectionExactModel.Size = new System.Drawing.Size(87, 17);
            this.cameraSelectionExactModel.TabIndex = 6;
            this.cameraSelectionExactModel.Text = "Select Model";
            this.cameraSelectionExactModel.UseVisualStyleBackColor = true;
            // 
            // cameraSelectionAllowAny
            // 
            this.cameraSelectionAllowAny.AutoSize = true;
            this.cameraSelectionAllowAny.Checked = true;
            this.cameraSelectionAllowAny.Location = new System.Drawing.Point(21, 19);
            this.cameraSelectionAllowAny.Name = "cameraSelectionAllowAny";
            this.cameraSelectionAllowAny.Size = new System.Drawing.Size(71, 17);
            this.cameraSelectionAllowAny.TabIndex = 5;
            this.cameraSelectionAllowAny.TabStop = true;
            this.cameraSelectionAllowAny.Text = "Allow Any";
            this.cameraSelectionAllowAny.UseVisualStyleBackColor = true;
            this.cameraSelectionAllowAny.CheckedChanged += new System.EventHandler(this.cameraSelectionAllowAny_CheckedChanged);
            // 
            // modelSelectionGroup
            // 
            this.modelSelectionGroup.Controls.Add(this.modelPID);
            this.modelSelectionGroup.Controls.Add(this.modelVID);
            this.modelSelectionGroup.Controls.Add(this.label4);
            this.modelSelectionGroup.Controls.Add(this.label3);
            this.modelSelectionGroup.Location = new System.Drawing.Point(18, 296);
            this.modelSelectionGroup.Name = "modelSelectionGroup";
            this.modelSelectionGroup.Size = new System.Drawing.Size(200, 100);
            this.modelSelectionGroup.TabIndex = 8;
            this.modelSelectionGroup.TabStop = false;
            this.modelSelectionGroup.Text = "Model Selection";
            // 
            // modelPID
            // 
            this.modelPID.Location = new System.Drawing.Point(69, 58);
            this.modelPID.Mask = "9990";
            this.modelPID.Name = "modelPID";
            this.modelPID.Size = new System.Drawing.Size(36, 20);
            this.modelPID.TabIndex = 112;
            this.modelPID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // modelVID
            // 
            this.modelVID.Location = new System.Drawing.Point(69, 29);
            this.modelVID.Mask = "9990";
            this.modelVID.Name = "modelVID";
            this.modelVID.Size = new System.Drawing.Size(36, 20);
            this.modelVID.TabIndex = 111;
            this.modelVID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 109;
            this.label4.Text = "PID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 110;
            this.label3.Text = "VID:";
            // 
            // Copyright
            // 
            this.Copyright.Location = new System.Drawing.Point(12, 55);
            this.Copyright.Name = "Copyright";
            this.Copyright.Size = new System.Drawing.Size(272, 57);
            this.Copyright.TabIndex = 103;
            this.Copyright.Text = "Copyright (C) 2010 Dad Dog Development Ltd.\r\nThis work is licensed under the Crea" +
                "tive Commons Attribution-No Derivative Works 3.0 License.";
            // 
            // SetupDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 424);
            this.Controls.Add(this.Copyright);
            this.Controls.Add(this.modelSelectionGroup);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.secondsAreMiliseconds);
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
            this.Load += new System.EventHandler(this.SetupDialogForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.modelSelectionGroup.ResumeLayout(false);
            this.modelSelectionGroup.PerformLayout();
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
        public System.Windows.Forms.CheckBox secondsAreMiliseconds;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.RadioButton cameraSelectionAllowAny;
        internal System.Windows.Forms.RadioButton cameraSelectionExcludeModel;
        internal System.Windows.Forms.RadioButton cameraSelectionExactModel;
        internal System.Windows.Forms.GroupBox modelSelectionGroup;
        internal System.Windows.Forms.MaskedTextBox modelPID;
        internal System.Windows.Forms.MaskedTextBox modelVID;
        private System.Windows.Forms.Label Copyright;
    }
}
