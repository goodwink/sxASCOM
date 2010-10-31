namespace ASCOM.SXMain
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
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(293, 88);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(59, 24);
            this.cmdOK.TabIndex = 0;
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
            this.cmdCancel.TabIndex = 1;
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
            this.label1.Size = new System.Drawing.Size(286, 76);
            this.label1.TabIndex = 2;
            this.label1.Text = "ASCOM Driver for SX/SXV/SXVF/SXVR Cameras\r\n\r\nCopyright (C) 2010 Dad Dog Developme" +
                "nt Ltd.\r\nAll Rights Reserved";
            // 
            // picASCOM
            // 
            this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picASCOM.Image = global::ASCOM.SXMain.Properties.Resources.ASCOM;
            this.picASCOM.Location = new System.Drawing.Point(304, 12);
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
            this.EnableLoggingCheckBox.TabIndex = 4;
            this.EnableLoggingCheckBox.Text = "Enable Logging";
            this.toolTip1.SetToolTip(this.EnableLoggingCheckBox, "Enable the creation of a debug log file.");
            this.EnableLoggingCheckBox.UseVisualStyleBackColor = true;
            // 
            // EnableUntestedCheckBox
            // 
            this.EnableUntestedCheckBox.AutoSize = true;
            this.EnableUntestedCheckBox.Location = new System.Drawing.Point(15, 155);
            this.EnableUntestedCheckBox.Name = "EnableUntestedCheckBox";
            this.EnableUntestedCheckBox.Size = new System.Drawing.Size(149, 17);
            this.EnableUntestedCheckBox.TabIndex = 5;
            this.EnableUntestedCheckBox.Text = "Enable Untested Cameras";
            this.toolTip1.SetToolTip(this.EnableUntestedCheckBox, "Attempt to control untested cameras.");
            this.EnableUntestedCheckBox.UseVisualStyleBackColor = true;
            // 
            // Version
            // 
            this.Version.AutoSize = true;
            this.Version.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Version.Location = new System.Drawing.Point(15, 88);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(80, 15);
            this.Version.TabIndex = 6;
            this.Version.Text = "Version: a.b.c";
            // 
            // secondsAreMiliseconds
            // 
            this.secondsAreMiliseconds.AutoSize = true;
            this.secondsAreMiliseconds.Location = new System.Drawing.Point(15, 188);
            this.secondsAreMiliseconds.Name = "secondsAreMiliseconds";
            this.secondsAreMiliseconds.Size = new System.Drawing.Size(147, 17);
            this.secondsAreMiliseconds.TabIndex = 7;
            this.secondsAreMiliseconds.Text = "Seconds Are Milliseconds";
            this.toolTip1.SetToolTip(this.secondsAreMiliseconds, "Divide the requested exposure by 1000.  Useful if you need faster exposures than " +
                    "your caputure software allows for.");
            this.secondsAreMiliseconds.UseVisualStyleBackColor = true;
            // 
            // SetupDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 229);
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
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).EndInit();
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
    }
}
