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
            if (selectionAllowAny.Checked)
            {
                vidLabel.Visible = false;
                pidLabel.Visible = false;
                VID.Visible = false;
                PID.Visible = false;
            }
            else
            {
                vidLabel.Visible = true;
                pidLabel.Visible = true;
                VID.Visible = true;
                PID.Visible = true;
            }
        }

        private void handleAdvancedUsbPropertiesChange(object sender, EventArgs e)
        {
            if (advancedUSBParmsEnabled.Checked)
            {
                usbGroup.Enabled = true;
            }
            else
            {
                usbGroup.Enabled = false;
            }
        }

        private void asymetricBinning_CheckedChanged(object sender, EventArgs e)
        {
            if (asymetricBinning.Checked)
            {
                binLabel.Text = "Max Y Bin";
                xBinLabel.Visible = true;
                maxXBin.Visible = true;

            }
            else
            {
                binLabel.Text = "Max Bin";
                xBinLabel.Visible = false;
                maxXBin.Visible = false;
            }
        }

        private void fixedBinning_CheckedChanged(object sender, EventArgs e)
        {
            fixedBin.Enabled = fixedBinning.Checked;
        }

        private void doubleExposeShort_CheckedChanged(object sender, EventArgs e)
        {
            doubleExposureThreshold.Enabled = doubleExposeShort.Checked;
        }

        private void gaussianBlur_CheckedChanged(object sender, EventArgs e)
        {
            gaussianBlurRadius.Enabled = gaussianBlur.Checked;
        }
    }
}

