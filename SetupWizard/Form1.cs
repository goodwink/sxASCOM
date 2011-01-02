using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SetupWizard
{
    public partial class SetupWizardScreen1 : Form
    {
        public SetupWizardScreen1()
        {
            InitializeComponent();
            lodestarNo.Checked = true;
            mainCameraNo.Checked = true;
            autoGuideNo.Checked = true;
        }

        private void nextTab(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex++;
        }

        private void previousTab(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex--;
        }

        private void handleCancelClick(object sender, EventArgs e)
        {
            Close();
        }

        private void handleFinishClick(object sender, EventArgs e)
        {
            string fullPath = String.Format("{0}", Application.ExecutablePath);
            int pos = fullPath.LastIndexOf(@"\");

            if (pos >= 0)
            {
                string dirPart = fullPath.Substring(0, pos);

                string serverPath = String.Format(@"{0}\{1}", dirPart, "ASCOM.SXCamera.exe");

                MessageBox.Show(String.Format("fullPath={0}\npos={1}\ndirpart={2}\nserverPath={3}\n", fullPath, pos, dirPart, serverPath));

                ProcessStartInfo si = new ProcessStartInfo();

                string args = "/register";

                if (lodestarYes.Checked)
                {
                    args += " /lodestar";
                }
                if (mainCameraYes.Checked)
                {
                    args += " /main";
                }
                if (autoGuideYes.Checked)
                {
                    args += " /autoguide";
                }

                si.Arguments = args;
                si.WorkingDirectory = Environment.CurrentDirectory;
                si.FileName = serverPath;

                try
                {
                    Process p = Process.Start(si);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error registering server: {0}", ex.ToString());
                }
            }
            Close();
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            string lodestar = "No", 
                   main = "No",
                   autoguide = "No";

            if (lodestarYes.Checked)
            {
                lodestar = "Yes";
            }

            if (mainCameraYes.Checked)
            {
                main = "Yes";
                autoGuideNo.Enabled = true;
                autoGuideYes.Enabled = true;
            }
            else
            {
                autoGuideNo.Enabled = false;
                autoGuideYes.Enabled = false;
                autoGuideNo.Checked = true;
            }

            if (autoGuideYes.Checked)
            {
                autoguide = "Yes";
            }
        
            confirmText.Text = String.Format("You have selected:\r\n\r\nLodestar:\t\t{0}\r\nMain Camera:\t{1}\r\nAutoguide Camera:\t{2}\r\n\r\nIf this is correct, press 'Finish' otherwise press 'Previous' and make any needed corrections.",
                lodestar, 
                main,
                autoguide);
        }
    }
}
