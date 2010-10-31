//tabs=4
// --------------------------------------------------------------------------------
//
// ASCOM Camera driver for SXCamera
//
// Description:
// 
// This file contains the implementation for the SX guide camera
//
// Implements:	ASCOM Camera interface version: 1.0
// Author:		Bret McKee <bretm@daddog.com>

using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

using ASCOM;
using ASCOM.Helper;
using ASCOM.Helper2;
using ASCOM.Interface;

using Logging;

namespace ASCOM.SXGuide
{
    //
    // Your driver's ID is ASCOM.SXCamera.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.SXCamera.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("c150cbaa-429d-4bad-84ff-27077b4156aa")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Camera : ASCOM.SXGeneric.Camera
    {
        public Camera() :
            base(1, "Guide")
        {
        }

        public override void SetupDialog()
        {
            try
            {
                Log.Write("Guide Camera SetupDialog()\n");
                SetupDialogForm F = new SetupDialogForm();
                F.ShowDialog();
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
        /// <summary>
        /// Returns a description of the camera model, such as manufacturer and model
        /// number. Any ASCII characters may be used. The string shall not exceed 68
        /// characters (for compatibility with FITS headers).
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if description unavailable</exception>
        public override string Description
        {
            get
            {
                try
                {
                    string sReturn = "SX-Guider (" + base.Description + ")";
                    Log.Write("Guide Camera Description" + sReturn + "\n");
                    return sReturn;
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

        public override bool CanPulseGuide
        {
            get
            {
                try
                {
                    bool bReturn = bHasGuideCamera;

                    Log.Write("Guide Camera CanPulseGuide returns " + bReturn+ "\n");

                    if (!Connected)
                    {
                        throw new ASCOM.NotConnectedException(SetError("Camera not connected"));
                    }

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

        public override void StartExposure(double Duration, bool Light)
        {
            try
            {
                bool useHardwareTimer = false;

                if (Duration <= 5.0)
                {
                    useHardwareTimer = true;
                }

                Log.Write(String.Format("Guide Camera StartExposure({0}, {1}) useHardwareTimer = {2}\n", Duration, Light, useHardwareTimer));

                base.StartExposure(Duration, Light, useHardwareTimer);
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
