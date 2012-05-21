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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ASCOM.sxUsbCameraBase
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
            vidLabel.Visible = !selectionAllowAny.Checked;
            pidLabel.Visible = !selectionAllowAny.Checked;
            VID.Visible = !selectionAllowAny.Checked;
            PID.Visible = !selectionAllowAny.Checked;
        }

        private void handleAdvancedUsbPropertiesChange(object sender, EventArgs e)
        {
            usbGroup.Enabled = advancedUSBParmsEnabled.Checked;
        }

        private void asymetricBinning_CheckedChanged(object sender, EventArgs e)
        {
            if (asymetricBinning.Checked)
            {
                xBinLabel.Text = "Max X Bin";
            }
            else
            {
                xBinLabel.Text = "Max Bin";
            }
            yBinLabel.Visible = asymetricBinning.Checked;
            maxYBin.Visible = asymetricBinning.Checked;
        }

        private void fixedBinning_CheckedChanged(object sender, EventArgs e)
        {
            fixedBin.Enabled = fixedBinning.Checked;
        }

        private void gaussianBlur_CheckedChanged(object sender, EventArgs e)
        {
            gaussianBlurRadius.Enabled = gaussianBlur.Checked;
        }

        private void doubleExposeShort_CheckedChanged(object sender, EventArgs e)
        {
            interlacedDoubleExposureThreshold.Enabled = doubleExposeShort.Checked;
        }
    }
}
