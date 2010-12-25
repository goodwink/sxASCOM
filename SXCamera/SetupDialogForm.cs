using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ASCOM.SXCamera
{
    [ComVisible(false)]					// Form not registered for COM!
    public partial class SetupDialogForm : Form
    {
        public SetupDialogForm()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void BrowseToAscom(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://ascom-standards.org/");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void camera1SelectionAllowAny_CheckedChanged(object sender, EventArgs e)
        {
            if (camera0SelectionAllowAny.Checked)
            {
                vid0Label.Visible = false;
                pid0Label.Visible = false;
                camera0VID.Visible = false;
                camera0PID.Visible = false;
            }
            else
            {
                vid0Label.Visible = true;
                pid0Label.Visible = true;
                camera0VID.Visible = true;
                camera0PID.Visible = true;
            }
        }

        private void camera2SelectionAllowAny_CheckedChanged(object sender, EventArgs e)
        {
            if (camera1SelectionAllowAny.Checked)
            {
                vid1Label.Visible = false;
                pid1Label.Visible = false;
                camera1VID.Visible = false;
                camera1PID.Visible = false;
            }
            else
            {
                vid1Label.Visible = true;
                pid1Label.Visible = true;
                camera1VID.Visible = true;
                camera1PID.Visible = true;
            }
        }

        private void handleAdvancedUsbPropertiesChange(object sender, EventArgs e)
        {
            if (advancedUSBParmsEnabled.Checked)
            {
                camera0Group.Enabled = true;
                camera1Group.Enabled = true;
            }
            else
            {
                camera0Group.Enabled = false;
                camera1Group.Enabled = false;
            }
        }
    }
}
