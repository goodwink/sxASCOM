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
            this.camera1SelectionExcludeModel = new System.Windows.Forms.RadioButton();
            this.camera1SelectionExactModel = new System.Windows.Forms.RadioButton();
            this.camera1SelectionAllowAny = new System.Windows.Forms.RadioButton();
            this.model1SelectionGroup = new System.Windows.Forms.GroupBox();
            this.camera1PID = new System.Windows.Forms.MaskedTextBox();
            this.camera1VID = new System.Windows.Forms.MaskedTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Copyright = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.camera2SelectionExcludeModel = new System.Windows.Forms.RadioButton();
            this.camera2SelectionExactModel = new System.Windows.Forms.RadioButton();
            this.camera2SelectionAllowAny = new System.Windows.Forms.RadioButton();
            this.model2SelectionGroup = new System.Windows.Forms.GroupBox();
            this.camera2PID = new System.Windows.Forms.MaskedTextBox();
            this.camera2VID = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.model1SelectionGroup.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.model2SelectionGroup.SuspendLayout();
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
            this.groupBox1.Controls.Add(this.camera1SelectionExcludeModel);
            this.groupBox1.Controls.Add(this.camera1SelectionExactModel);
            this.groupBox1.Controls.Add(this.camera1SelectionAllowAny);
            this.groupBox1.Location = new System.Drawing.Point(15, 192);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(147, 89);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Main Camera Selection";
            // 
            // camera1SelectionExcludeModel
            // 
            this.camera1SelectionExcludeModel.AutoSize = true;
            this.camera1SelectionExcludeModel.Location = new System.Drawing.Point(21, 65);
            this.camera1SelectionExcludeModel.Name = "camera1SelectionExcludeModel";
            this.camera1SelectionExcludeModel.Size = new System.Drawing.Size(95, 17);
            this.camera1SelectionExcludeModel.TabIndex = 7;
            this.camera1SelectionExcludeModel.Text = "Exclude Model";
            this.camera1SelectionExcludeModel.UseVisualStyleBackColor = true;
            // 
            // camera1SelectionExactModel
            // 
            this.camera1SelectionExactModel.AutoSize = true;
            this.camera1SelectionExactModel.Location = new System.Drawing.Point(21, 42);
            this.camera1SelectionExactModel.Name = "camera1SelectionExactModel";
            this.camera1SelectionExactModel.Size = new System.Drawing.Size(87, 17);
            this.camera1SelectionExactModel.TabIndex = 6;
            this.camera1SelectionExactModel.Text = "Select Model";
            this.camera1SelectionExactModel.UseVisualStyleBackColor = true;
            // 
            // camera1SelectionAllowAny
            // 
            this.camera1SelectionAllowAny.AutoSize = true;
            this.camera1SelectionAllowAny.Checked = true;
            this.camera1SelectionAllowAny.Location = new System.Drawing.Point(21, 19);
            this.camera1SelectionAllowAny.Name = "camera1SelectionAllowAny";
            this.camera1SelectionAllowAny.Size = new System.Drawing.Size(71, 17);
            this.camera1SelectionAllowAny.TabIndex = 5;
            this.camera1SelectionAllowAny.TabStop = true;
            this.camera1SelectionAllowAny.Text = "Allow Any";
            this.camera1SelectionAllowAny.UseVisualStyleBackColor = true;
            this.camera1SelectionAllowAny.CheckedChanged += new System.EventHandler(this.camera1SelectionAllowAny_CheckedChanged);
            // 
            // model1SelectionGroup
            // 
            this.model1SelectionGroup.Controls.Add(this.camera1PID);
            this.model1SelectionGroup.Controls.Add(this.camera1VID);
            this.model1SelectionGroup.Controls.Add(this.label4);
            this.model1SelectionGroup.Controls.Add(this.label3);
            this.model1SelectionGroup.Location = new System.Drawing.Point(18, 296);
            this.model1SelectionGroup.Name = "model1SelectionGroup";
            this.model1SelectionGroup.Size = new System.Drawing.Size(144, 100);
            this.model1SelectionGroup.TabIndex = 8;
            this.model1SelectionGroup.TabStop = false;
            this.model1SelectionGroup.Text = "Main Camera Model";
            // 
            // camera1PID
            // 
            this.camera1PID.Location = new System.Drawing.Point(69, 58);
            this.camera1PID.Mask = "9990";
            this.camera1PID.Name = "camera1PID";
            this.camera1PID.Size = new System.Drawing.Size(36, 20);
            this.camera1PID.TabIndex = 112;
            this.camera1PID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // camera1VID
            // 
            this.camera1VID.Location = new System.Drawing.Point(69, 29);
            this.camera1VID.Mask = "9990";
            this.camera1VID.Name = "camera1VID";
            this.camera1VID.Size = new System.Drawing.Size(36, 20);
            this.camera1VID.TabIndex = 111;
            this.camera1VID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.camera2SelectionExcludeModel);
            this.groupBox2.Controls.Add(this.camera2SelectionExactModel);
            this.groupBox2.Controls.Add(this.camera2SelectionAllowAny);
            this.groupBox2.Location = new System.Drawing.Point(205, 192);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(147, 89);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Second Camera Selection";
            // 
            // camera2SelectionExcludeModel
            // 
            this.camera2SelectionExcludeModel.AutoSize = true;
            this.camera2SelectionExcludeModel.Location = new System.Drawing.Point(21, 65);
            this.camera2SelectionExcludeModel.Name = "camera2SelectionExcludeModel";
            this.camera2SelectionExcludeModel.Size = new System.Drawing.Size(95, 17);
            this.camera2SelectionExcludeModel.TabIndex = 7;
            this.camera2SelectionExcludeModel.Text = "Exclude Model";
            this.camera2SelectionExcludeModel.UseVisualStyleBackColor = true;
            // 
            // camera2SelectionExactModel
            // 
            this.camera2SelectionExactModel.AutoSize = true;
            this.camera2SelectionExactModel.Location = new System.Drawing.Point(21, 42);
            this.camera2SelectionExactModel.Name = "camera2SelectionExactModel";
            this.camera2SelectionExactModel.Size = new System.Drawing.Size(87, 17);
            this.camera2SelectionExactModel.TabIndex = 6;
            this.camera2SelectionExactModel.Text = "Select Model";
            this.camera2SelectionExactModel.UseVisualStyleBackColor = true;
            // 
            // camera2SelectionAllowAny
            // 
            this.camera2SelectionAllowAny.AutoSize = true;
            this.camera2SelectionAllowAny.Checked = true;
            this.camera2SelectionAllowAny.Location = new System.Drawing.Point(21, 19);
            this.camera2SelectionAllowAny.Name = "camera2SelectionAllowAny";
            this.camera2SelectionAllowAny.Size = new System.Drawing.Size(71, 17);
            this.camera2SelectionAllowAny.TabIndex = 5;
            this.camera2SelectionAllowAny.TabStop = true;
            this.camera2SelectionAllowAny.Text = "Allow Any";
            this.camera2SelectionAllowAny.UseVisualStyleBackColor = true;
            this.camera2SelectionAllowAny.CheckedChanged += new System.EventHandler(this.camera2SelectionAllowAny_CheckedChanged);
            // 
            // model2SelectionGroup
            // 
            this.model2SelectionGroup.Controls.Add(this.camera2PID);
            this.model2SelectionGroup.Controls.Add(this.camera2VID);
            this.model2SelectionGroup.Controls.Add(this.label2);
            this.model2SelectionGroup.Controls.Add(this.label5);
            this.model2SelectionGroup.Location = new System.Drawing.Point(208, 296);
            this.model2SelectionGroup.Name = "model2SelectionGroup";
            this.model2SelectionGroup.Size = new System.Drawing.Size(144, 100);
            this.model2SelectionGroup.TabIndex = 113;
            this.model2SelectionGroup.TabStop = false;
            this.model2SelectionGroup.Text = "Second Camera Model";
            // 
            // camera2PID
            // 
            this.camera2PID.Location = new System.Drawing.Point(69, 58);
            this.camera2PID.Mask = "9990";
            this.camera2PID.Name = "camera2PID";
            this.camera2PID.Size = new System.Drawing.Size(36, 20);
            this.camera2PID.TabIndex = 112;
            this.camera2PID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // camera2VID
            // 
            this.camera2VID.Location = new System.Drawing.Point(69, 29);
            this.camera2VID.Mask = "9990";
            this.camera2VID.Name = "camera2VID";
            this.camera2VID.Size = new System.Drawing.Size(36, 20);
            this.camera2VID.TabIndex = 111;
            this.camera2VID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 109;
            this.label2.Text = "PID:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(35, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 13);
            this.label5.TabIndex = 110;
            this.label5.Text = "VID:";
            // 
            // SetupDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 424);
            this.Controls.Add(this.model2SelectionGroup);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.Copyright);
            this.Controls.Add(this.model1SelectionGroup);
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
            this.model1SelectionGroup.ResumeLayout(false);
            this.model1SelectionGroup.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.model2SelectionGroup.ResumeLayout(false);
            this.model2SelectionGroup.PerformLayout();
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
        internal System.Windows.Forms.RadioButton camera1SelectionAllowAny;
        internal System.Windows.Forms.RadioButton camera1SelectionExcludeModel;
        internal System.Windows.Forms.RadioButton camera1SelectionExactModel;
        internal System.Windows.Forms.GroupBox model1SelectionGroup;
        internal System.Windows.Forms.MaskedTextBox camera1PID;
        internal System.Windows.Forms.MaskedTextBox camera1VID;
        private System.Windows.Forms.Label Copyright;
        private System.Windows.Forms.GroupBox groupBox2;
        internal System.Windows.Forms.RadioButton camera2SelectionExcludeModel;
        internal System.Windows.Forms.RadioButton camera2SelectionExactModel;
        internal System.Windows.Forms.RadioButton camera2SelectionAllowAny;
        internal System.Windows.Forms.GroupBox model2SelectionGroup;
        internal System.Windows.Forms.MaskedTextBox camera2PID;
        internal System.Windows.Forms.MaskedTextBox camera2VID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
    }
}
