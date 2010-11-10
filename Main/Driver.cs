//tabs=4
// --------------------------------------------------------------------------------
//
// ASCOM Camera driver for SXCamera
//
// Description:
//
// This file contains the implementation for the SX main camera
//
// Implements:	ASCOM Camera interface version: 1.0
// Author:		      Bret McKee <bretm@daddog.com>
//

using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;

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
            try
            {
                Log.Write("Main Camera: SetupDialog()\n");
                SetupDialogForm F = new SetupDialogForm();

                F.EnableLoggingCheckBox.Checked = config.enableLogging;
                F.EnableUntestedCheckBox.Checked = config.enableUntested;
                F.secondsAreMiliseconds.Checked = config.secondsAreMilliseconds;
                F.Version.Text = String.Format("Version: {0}.{1}.{2}", SXCamera.SharedResources.versionMajor,
                    SXCamera.SharedResources.versionMinor, SXCamera.SharedResources.versionMaintenance);

                if (F.ShowDialog() == DialogResult.OK)
                {
                    Log.Write("ShowDialog returned OK - saving parameters\n");
 
                    config.enableLogging = F.EnableLoggingCheckBox.Checked;
                    config.enableUntested = F.EnableUntestedCheckBox.Checked;
                    config.secondsAreMilliseconds = F.secondsAreMiliseconds.Checked;
                }
            }
            catch (ASCOM.DriverException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                throw new ASCOM.DriverException(SetError("Unable to complete " + MethodBase.GetCurrentMethod().Name + " request"), ex);
            }
        }

        public override bool CanPulseGuide
        {
            get
            {
                try
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
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError("Unable to complete " + MethodBase.GetCurrentMethod().Name + " request"), ex);
                }
            }
        }

    }
}

