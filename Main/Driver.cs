//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM Camera driver for SXCamera
//
// Description:	Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam 
//				nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam 
//				erat, sed diam voluptua. At vero eos et accusam et justo duo 
//				dolores et ea rebum. Stet clita kasd gubergren, no sea takimata 
//				sanctus est Lorem ipsum dolor sit amet.
//
// Implements:	ASCOM Camera interface version: 1.0
// Author:		(XXX) Your N. Here <your@email.here>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// dd-mmm-yyyy	XXX	1.0.0	Initial edit, from ASCOM Camera Driver template
// --------------------------------------------------------------------------------
//
using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ASCOM;
using ASCOM.Helper;
using ASCOM.Helper2;
using ASCOM.Interface;

using Logging;

namespace ASCOM.SXMain
{
    //
    // Your driver's ID is ASCOM.SXCamera.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.SXCamera.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("cfa2c985-9251-4b62-9146-99a52bf47701")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Camera : ASCOM.SXGeneric.Camera
    {
        public Camera() :
            base(0, "Main")
        {
        }

        /// <summary>
        /// Launches a configuration dialog box for the driver.  The call will not return
        /// until the user clicks OK or cancel manually.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw an exception if Setup dialog is unavailable.</exception>
        public override void SetupDialog()
        {
            Log.Write("Main Camera: SetupDialog()\n");
            SetupDialogForm F = new SetupDialogForm();

            F.EnableLoggingCheckBox.Checked = config.enableLogging;
            F.EnableUntestedCheckBox.Checked = config.enableUntested;
            F.Version.Text = String.Format("Version: {0}.{1}.{2}", SXCamera.SharedResources.versionMajor,
                SXCamera.SharedResources.versionMinor, SXCamera.SharedResources.versionMaintenance);

            if (F.ShowDialog() == DialogResult.OK)
            {
                if (config.enableLogging != F.EnableLoggingCheckBox.Checked)
                {
                    config.enableLogging = F.EnableLoggingCheckBox.Checked;

                    if (false && F.EnableLoggingCheckBox.Checked)
                    {
                        SaveFileDialog DebugOutputFileName = new SaveFileDialog();

                        DebugOutputFileName.Title = "Log File Name";
                        DebugOutputFileName.AddExtension = true;
                        DebugOutputFileName.FileName = config.logFileName;
                        DebugOutputFileName.InitialDirectory = ".";
                        DebugOutputFileName.RestoreDirectory = true;
                        DebugOutputFileName.CheckFileExists = false;
                        DebugOutputFileName.DefaultExt = "log";
                        DebugOutputFileName.Filter = "Log Files|*.log|All Files|*.*";

                        if (DebugOutputFileName.ShowDialog() == DialogResult.OK)
                        {
                            config.logFileName = DebugOutputFileName.FileName;
                        }
                        else
                        {
                            MessageBox.Show("Logging disabled since no file was selected", "Logging Disabled");
                            config.enableLogging = false;
                        }
                        DebugOutputFileName.Dispose();
                    }
                }

                if (config.enableUntested != F.EnableUntestedCheckBox.Checked)
                {
                    config.enableUntested = F.EnableUntestedCheckBox.Checked;
                }
            }
        }

        public override bool CanPulseGuide
        {
            get
            {
                bool bReturn = false;

                if (!Connected)
                {
                    throw new ASCOM.NotConnectedException(SetError("Camera not connected"));
                }

                if (!bHasGuideCamera)
                    bReturn = sxCamera.hasGuidePort;

                Log.Write("Main Camera: CanPulseGuide returns " + bReturn + "\n");

                return bReturn;
            }
        }

    }
}

