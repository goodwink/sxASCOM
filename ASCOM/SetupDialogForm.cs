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

        private void symetricBinning_CheckedChanged(object sender, EventArgs e)
        {
            if (symetricBinning.Checked)
            {
                binLabel.Text = "Max Bin";
                xBinLabel.Visible = false;
                maxXBin.Visible = false;
            }
            else
            {
                binLabel.Text = "Max Y Bin";
                xBinLabel.Visible = true;
                maxXBin.Visible = true;
            }
        }
    }
}
