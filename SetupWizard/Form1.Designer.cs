namespace SetupWizard
{
    partial class SetupWizardScreen1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupWizardScreen1));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.introTab = new System.Windows.Forms.TabPage();
            this.introGroup = new System.Windows.Forms.GroupBox();
            this.introLabel = new System.Windows.Forms.Label();
            this.beginButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.lodestarTab = new System.Windows.Forms.TabPage();
            this.lodestarNext = new System.Windows.Forms.Button();
            this.lodestarPrevious = new System.Windows.Forms.Button();
            this.lodestarGroup = new System.Windows.Forms.GroupBox();
            this.lodestarNo = new System.Windows.Forms.RadioButton();
            this.lodestarYes = new System.Windows.Forms.RadioButton();
            this.lodestarLabel = new System.Windows.Forms.Label();
            this.mainCameraTab = new System.Windows.Forms.TabPage();
            this.mainCameraNext = new System.Windows.Forms.Button();
            this.mainCameraPrevous = new System.Windows.Forms.Button();
            this.mainGroup = new System.Windows.Forms.GroupBox();
            this.mainCameraNo = new System.Windows.Forms.RadioButton();
            this.mainCameraYes = new System.Windows.Forms.RadioButton();
            this.mainLabel = new System.Windows.Forms.Label();
            this.autoGuideTab = new System.Windows.Forms.TabPage();
            this.autoGuideNext = new System.Windows.Forms.Button();
            this.autoGuidePrevious = new System.Windows.Forms.Button();
            this.autoGuideGroup = new System.Windows.Forms.GroupBox();
            this.autoGuideNo = new System.Windows.Forms.RadioButton();
            this.autoGuideYes = new System.Windows.Forms.RadioButton();
            this.autoGuideLabel = new System.Windows.Forms.Label();
            this.confirmTab = new System.Windows.Forms.TabPage();
            this.finishButton = new System.Windows.Forms.Button();
            this.confirmPrevious = new System.Windows.Forms.Button();
            this.confirmGroup = new System.Windows.Forms.GroupBox();
            this.confirmText = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.introTab.SuspendLayout();
            this.introGroup.SuspendLayout();
            this.lodestarTab.SuspendLayout();
            this.lodestarGroup.SuspendLayout();
            this.mainCameraTab.SuspendLayout();
            this.mainGroup.SuspendLayout();
            this.autoGuideTab.SuspendLayout();
            this.autoGuideGroup.SuspendLayout();
            this.confirmTab.SuspendLayout();
            this.confirmGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.introTab);
            this.tabControl1.Controls.Add(this.lodestarTab);
            this.tabControl1.Controls.Add(this.mainCameraTab);
            this.tabControl1.Controls.Add(this.autoGuideTab);
            this.tabControl1.Controls.Add(this.confirmTab);
            this.tabControl1.Location = new System.Drawing.Point(-3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(223, 239);
            this.tabControl1.TabIndex = 6;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            // 
            // introTab
            // 
            this.introTab.Controls.Add(this.introGroup);
            this.introTab.Controls.Add(this.beginButton);
            this.introTab.Controls.Add(this.cancelButton);
            this.introTab.Location = new System.Drawing.Point(4, 22);
            this.introTab.Name = "introTab";
            this.introTab.Padding = new System.Windows.Forms.Padding(3);
            this.introTab.Size = new System.Drawing.Size(215, 213);
            this.introTab.TabIndex = 1;
            this.introTab.Text = "Introduction";
            this.introTab.UseVisualStyleBackColor = true;
            // 
            // introGroup
            // 
            this.introGroup.Controls.Add(this.introLabel);
            this.introGroup.Location = new System.Drawing.Point(0, 0);
            this.introGroup.Name = "introGroup";
            this.introGroup.Size = new System.Drawing.Size(215, 178);
            this.introGroup.TabIndex = 3;
            this.introGroup.TabStop = false;
            this.introGroup.Text = "Introduction";
            // 
            // introLabel
            // 
            this.introLabel.AutoSize = true;
            this.introLabel.Location = new System.Drawing.Point(9, 16);
            this.introLabel.Name = "introLabel";
            this.introLabel.Size = new System.Drawing.Size(203, 65);
            this.introLabel.TabIndex = 0;
            this.introLabel.Text = "Welcome to the sxASCOM Setup Wizard.\r\n\r\nYou will be asked a series of questions\r\n" +
                "that will allow the driver to be configured\r\nfor your equipement.";
            // 
            // beginButton
            // 
            this.beginButton.Location = new System.Drawing.Point(110, 184);
            this.beginButton.Name = "beginButton";
            this.beginButton.Size = new System.Drawing.Size(75, 23);
            this.beginButton.TabIndex = 2;
            this.beginButton.Text = "Begin";
            this.beginButton.UseVisualStyleBackColor = true;
            this.beginButton.Click += new System.EventHandler(this.nextTab);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(12, 184);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.handleCancelClick);
            // 
            // lodestarTab
            // 
            this.lodestarTab.Controls.Add(this.lodestarNext);
            this.lodestarTab.Controls.Add(this.lodestarPrevious);
            this.lodestarTab.Controls.Add(this.lodestarGroup);
            this.lodestarTab.Location = new System.Drawing.Point(4, 22);
            this.lodestarTab.Name = "lodestarTab";
            this.lodestarTab.Padding = new System.Windows.Forms.Padding(3);
            this.lodestarTab.Size = new System.Drawing.Size(215, 213);
            this.lodestarTab.TabIndex = 2;
            this.lodestarTab.Text = "Lodestar";
            this.lodestarTab.UseVisualStyleBackColor = true;
            // 
            // lodestarNext
            // 
            this.lodestarNext.Location = new System.Drawing.Point(110, 184);
            this.lodestarNext.Name = "lodestarNext";
            this.lodestarNext.Size = new System.Drawing.Size(75, 23);
            this.lodestarNext.TabIndex = 2;
            this.lodestarNext.Text = "Next";
            this.lodestarNext.UseVisualStyleBackColor = true;
            this.lodestarNext.Click += new System.EventHandler(this.nextTab);
            // 
            // lodestarPrevious
            // 
            this.lodestarPrevious.Location = new System.Drawing.Point(12, 184);
            this.lodestarPrevious.Name = "lodestarPrevious";
            this.lodestarPrevious.Size = new System.Drawing.Size(75, 23);
            this.lodestarPrevious.TabIndex = 1;
            this.lodestarPrevious.Text = "Previous";
            this.lodestarPrevious.UseVisualStyleBackColor = true;
            this.lodestarPrevious.Click += new System.EventHandler(this.previousTab);
            // 
            // lodestarGroup
            // 
            this.lodestarGroup.Controls.Add(this.lodestarNo);
            this.lodestarGroup.Controls.Add(this.lodestarYes);
            this.lodestarGroup.Controls.Add(this.lodestarLabel);
            this.lodestarGroup.Location = new System.Drawing.Point(3, 0);
            this.lodestarGroup.Name = "lodestarGroup";
            this.lodestarGroup.Size = new System.Drawing.Size(212, 178);
            this.lodestarGroup.TabIndex = 0;
            this.lodestarGroup.TabStop = false;
            this.lodestarGroup.Text = "LodeStar Configuration";
            // 
            // lodestarNo
            // 
            this.lodestarNo.AutoSize = true;
            this.lodestarNo.Location = new System.Drawing.Point(9, 120);
            this.lodestarNo.Name = "lodestarNo";
            this.lodestarNo.Size = new System.Drawing.Size(39, 17);
            this.lodestarNo.TabIndex = 2;
            this.lodestarNo.TabStop = true;
            this.lodestarNo.Text = "No";
            this.lodestarNo.UseVisualStyleBackColor = true;
            // 
            // lodestarYes
            // 
            this.lodestarYes.AutoSize = true;
            this.lodestarYes.Location = new System.Drawing.Point(9, 97);
            this.lodestarYes.Name = "lodestarYes";
            this.lodestarYes.Size = new System.Drawing.Size(43, 17);
            this.lodestarYes.TabIndex = 1;
            this.lodestarYes.TabStop = true;
            this.lodestarYes.Text = "Yes";
            this.lodestarYes.UseVisualStyleBackColor = true;
            // 
            // lodestarLabel
            // 
            this.lodestarLabel.AutoSize = true;
            this.lodestarLabel.Location = new System.Drawing.Point(6, 16);
            this.lodestarLabel.Name = "lodestarLabel";
            this.lodestarLabel.Size = new System.Drawing.Size(199, 65);
            this.lodestarLabel.TabIndex = 0;
            this.lodestarLabel.Text = "Do you have a LodeStar Guide Camera?\r\n\r\nThis is a small guide camera which \r\nplug" +
                "s into the computer via a USB\r\nplug.";
            // 
            // mainCameraTab
            // 
            this.mainCameraTab.Controls.Add(this.mainCameraNext);
            this.mainCameraTab.Controls.Add(this.mainCameraPrevous);
            this.mainCameraTab.Controls.Add(this.mainGroup);
            this.mainCameraTab.Location = new System.Drawing.Point(4, 22);
            this.mainCameraTab.Name = "mainCameraTab";
            this.mainCameraTab.Padding = new System.Windows.Forms.Padding(3);
            this.mainCameraTab.Size = new System.Drawing.Size(215, 213);
            this.mainCameraTab.TabIndex = 3;
            this.mainCameraTab.Text = "MainCamera";
            this.mainCameraTab.UseVisualStyleBackColor = true;
            // 
            // mainCameraNext
            // 
            this.mainCameraNext.Location = new System.Drawing.Point(110, 184);
            this.mainCameraNext.Name = "mainCameraNext";
            this.mainCameraNext.Size = new System.Drawing.Size(75, 23);
            this.mainCameraNext.TabIndex = 2;
            this.mainCameraNext.Text = "Next";
            this.mainCameraNext.UseVisualStyleBackColor = true;
            this.mainCameraNext.Click += new System.EventHandler(this.nextTab);
            // 
            // mainCameraPrevous
            // 
            this.mainCameraPrevous.Location = new System.Drawing.Point(12, 184);
            this.mainCameraPrevous.Name = "mainCameraPrevous";
            this.mainCameraPrevous.Size = new System.Drawing.Size(75, 23);
            this.mainCameraPrevous.TabIndex = 1;
            this.mainCameraPrevous.Text = "Previous";
            this.mainCameraPrevous.UseVisualStyleBackColor = true;
            this.mainCameraPrevous.Click += new System.EventHandler(this.previousTab);
            // 
            // mainGroup
            // 
            this.mainGroup.Controls.Add(this.mainCameraNo);
            this.mainGroup.Controls.Add(this.mainCameraYes);
            this.mainGroup.Controls.Add(this.mainLabel);
            this.mainGroup.Location = new System.Drawing.Point(0, 0);
            this.mainGroup.Name = "mainGroup";
            this.mainGroup.Size = new System.Drawing.Size(212, 178);
            this.mainGroup.TabIndex = 0;
            this.mainGroup.TabStop = false;
            this.mainGroup.Text = "Main Imaging Camera Configuration";
            // 
            // mainCameraNo
            // 
            this.mainCameraNo.AutoSize = true;
            this.mainCameraNo.Location = new System.Drawing.Point(12, 118);
            this.mainCameraNo.Name = "mainCameraNo";
            this.mainCameraNo.Size = new System.Drawing.Size(39, 17);
            this.mainCameraNo.TabIndex = 2;
            this.mainCameraNo.TabStop = true;
            this.mainCameraNo.Text = "No";
            this.mainCameraNo.UseVisualStyleBackColor = true;
            // 
            // mainCameraYes
            // 
            this.mainCameraYes.AutoSize = true;
            this.mainCameraYes.Location = new System.Drawing.Point(12, 95);
            this.mainCameraYes.Name = "mainCameraYes";
            this.mainCameraYes.Size = new System.Drawing.Size(43, 17);
            this.mainCameraYes.TabIndex = 1;
            this.mainCameraYes.TabStop = true;
            this.mainCameraYes.Text = "Yes";
            this.mainCameraYes.UseVisualStyleBackColor = true;
            // 
            // mainLabel
            // 
            this.mainLabel.AutoSize = true;
            this.mainLabel.Location = new System.Drawing.Point(9, 16);
            this.mainLabel.Name = "mainLabel";
            this.mainLabel.Size = new System.Drawing.Size(164, 65);
            this.mainLabel.TabIndex = 0;
            this.mainLabel.Text = "Do you have a  Starligt Xpress\r\nMain Imaging camera?\r\n\r\nThis is any USB camera ex" +
                "cept a\r\nLodeStar Guide Camera";
            // 
            // autoGuideTab
            // 
            this.autoGuideTab.Controls.Add(this.autoGuideNext);
            this.autoGuideTab.Controls.Add(this.autoGuidePrevious);
            this.autoGuideTab.Controls.Add(this.autoGuideGroup);
            this.autoGuideTab.Location = new System.Drawing.Point(4, 22);
            this.autoGuideTab.Name = "autoGuideTab";
            this.autoGuideTab.Padding = new System.Windows.Forms.Padding(3);
            this.autoGuideTab.Size = new System.Drawing.Size(215, 213);
            this.autoGuideTab.TabIndex = 4;
            this.autoGuideTab.Text = "AutoGuider";
            this.autoGuideTab.UseVisualStyleBackColor = true;
            // 
            // autoGuideNext
            // 
            this.autoGuideNext.Location = new System.Drawing.Point(110, 184);
            this.autoGuideNext.Name = "autoGuideNext";
            this.autoGuideNext.Size = new System.Drawing.Size(75, 23);
            this.autoGuideNext.TabIndex = 2;
            this.autoGuideNext.Text = "Next";
            this.autoGuideNext.UseVisualStyleBackColor = true;
            this.autoGuideNext.Click += new System.EventHandler(this.nextTab);
            // 
            // autoGuidePrevious
            // 
            this.autoGuidePrevious.Location = new System.Drawing.Point(12, 184);
            this.autoGuidePrevious.Name = "autoGuidePrevious";
            this.autoGuidePrevious.Size = new System.Drawing.Size(75, 23);
            this.autoGuidePrevious.TabIndex = 1;
            this.autoGuidePrevious.Text = "Previous";
            this.autoGuidePrevious.UseVisualStyleBackColor = true;
            this.autoGuidePrevious.Click += new System.EventHandler(this.previousTab);
            // 
            // autoGuideGroup
            // 
            this.autoGuideGroup.Controls.Add(this.autoGuideNo);
            this.autoGuideGroup.Controls.Add(this.autoGuideYes);
            this.autoGuideGroup.Controls.Add(this.autoGuideLabel);
            this.autoGuideGroup.Location = new System.Drawing.Point(0, 0);
            this.autoGuideGroup.Name = "autoGuideGroup";
            this.autoGuideGroup.Size = new System.Drawing.Size(212, 178);
            this.autoGuideGroup.TabIndex = 0;
            this.autoGuideGroup.TabStop = false;
            this.autoGuideGroup.Text = "Autoguide Camea Configuration";
            // 
            // autoGuideNo
            // 
            this.autoGuideNo.AutoSize = true;
            this.autoGuideNo.Location = new System.Drawing.Point(12, 155);
            this.autoGuideNo.Name = "autoGuideNo";
            this.autoGuideNo.Size = new System.Drawing.Size(39, 17);
            this.autoGuideNo.TabIndex = 2;
            this.autoGuideNo.TabStop = true;
            this.autoGuideNo.Text = "No";
            this.autoGuideNo.UseVisualStyleBackColor = true;
            // 
            // autoGuideYes
            // 
            this.autoGuideYes.AutoSize = true;
            this.autoGuideYes.Location = new System.Drawing.Point(11, 132);
            this.autoGuideYes.Name = "autoGuideYes";
            this.autoGuideYes.Size = new System.Drawing.Size(43, 17);
            this.autoGuideYes.TabIndex = 1;
            this.autoGuideYes.TabStop = true;
            this.autoGuideYes.Text = "Yes";
            this.autoGuideYes.UseVisualStyleBackColor = true;
            // 
            // autoGuideLabel
            // 
            this.autoGuideLabel.AutoSize = true;
            this.autoGuideLabel.Location = new System.Drawing.Point(9, 16);
            this.autoGuideLabel.Name = "autoGuideLabel";
            this.autoGuideLabel.Size = new System.Drawing.Size(205, 104);
            this.autoGuideLabel.TabIndex = 0;
            this.autoGuideLabel.Text = resources.GetString("autoGuideLabel.Text");
            // 
            // confirmTab
            // 
            this.confirmTab.Controls.Add(this.finishButton);
            this.confirmTab.Controls.Add(this.confirmPrevious);
            this.confirmTab.Controls.Add(this.confirmGroup);
            this.confirmTab.Location = new System.Drawing.Point(4, 22);
            this.confirmTab.Name = "confirmTab";
            this.confirmTab.Padding = new System.Windows.Forms.Padding(3);
            this.confirmTab.Size = new System.Drawing.Size(215, 213);
            this.confirmTab.TabIndex = 5;
            this.confirmTab.Text = "Confirmation";
            this.confirmTab.UseVisualStyleBackColor = true;
            // 
            // finishButton
            // 
            this.finishButton.Location = new System.Drawing.Point(110, 184);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 2;
            this.finishButton.Text = "Finish";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.handleFinishClick);
            // 
            // confirmPrevious
            // 
            this.confirmPrevious.Location = new System.Drawing.Point(12, 184);
            this.confirmPrevious.Name = "confirmPrevious";
            this.confirmPrevious.Size = new System.Drawing.Size(75, 23);
            this.confirmPrevious.TabIndex = 1;
            this.confirmPrevious.Text = "Previous";
            this.confirmPrevious.UseVisualStyleBackColor = true;
            this.confirmPrevious.Click += new System.EventHandler(this.previousTab);
            // 
            // confirmGroup
            // 
            this.confirmGroup.Controls.Add(this.confirmText);
            this.confirmGroup.Location = new System.Drawing.Point(0, 0);
            this.confirmGroup.Name = "confirmGroup";
            this.confirmGroup.Size = new System.Drawing.Size(219, 178);
            this.confirmGroup.TabIndex = 0;
            this.confirmGroup.TabStop = false;
            this.confirmGroup.Text = "Confirmtion";
            // 
            // confirmText
            // 
            this.confirmText.BackColor = System.Drawing.SystemColors.Window;
            this.confirmText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.confirmText.Location = new System.Drawing.Point(12, 20);
            this.confirmText.Multiline = true;
            this.confirmText.Name = "confirmText";
            this.confirmText.ReadOnly = true;
            this.confirmText.Size = new System.Drawing.Size(197, 132);
            this.confirmText.TabIndex = 0;
            // 
            // SetupWizardScreen1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 240);
            this.Controls.Add(this.tabControl1);
            this.Name = "SetupWizardScreen1";
            this.Text = "Setup Wizard";
            this.tabControl1.ResumeLayout(false);
            this.introTab.ResumeLayout(false);
            this.introGroup.ResumeLayout(false);
            this.introGroup.PerformLayout();
            this.lodestarTab.ResumeLayout(false);
            this.lodestarGroup.ResumeLayout(false);
            this.lodestarGroup.PerformLayout();
            this.mainCameraTab.ResumeLayout(false);
            this.mainGroup.ResumeLayout(false);
            this.mainGroup.PerformLayout();
            this.autoGuideTab.ResumeLayout(false);
            this.autoGuideGroup.ResumeLayout(false);
            this.autoGuideGroup.PerformLayout();
            this.confirmTab.ResumeLayout(false);
            this.confirmGroup.ResumeLayout(false);
            this.confirmGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage introTab;
        private System.Windows.Forms.Button beginButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TabPage lodestarTab;
        private System.Windows.Forms.GroupBox lodestarGroup;
        private System.Windows.Forms.RadioButton lodestarNo;
        private System.Windows.Forms.RadioButton lodestarYes;
        private System.Windows.Forms.Label lodestarLabel;
        private System.Windows.Forms.Button lodestarPrevious;
        private System.Windows.Forms.GroupBox introGroup;
        private System.Windows.Forms.Label introLabel;
        private System.Windows.Forms.Button lodestarNext;
        private System.Windows.Forms.TabPage mainCameraTab;
        private System.Windows.Forms.Button mainCameraNext;
        private System.Windows.Forms.Button mainCameraPrevous;
        private System.Windows.Forms.GroupBox mainGroup;
        private System.Windows.Forms.Label mainLabel;
        private System.Windows.Forms.RadioButton mainCameraYes;
        private System.Windows.Forms.RadioButton mainCameraNo;
        private System.Windows.Forms.TabPage autoGuideTab;
        private System.Windows.Forms.GroupBox autoGuideGroup;
        private System.Windows.Forms.Label autoGuideLabel;
        private System.Windows.Forms.RadioButton autoGuideNo;
        private System.Windows.Forms.RadioButton autoGuideYes;
        private System.Windows.Forms.Button autoGuideNext;
        private System.Windows.Forms.Button autoGuidePrevious;
        private System.Windows.Forms.TabPage confirmTab;
        private System.Windows.Forms.GroupBox confirmGroup;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.Button confirmPrevious;
        private System.Windows.Forms.TextBox confirmText;

    }
}

