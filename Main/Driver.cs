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
        /// If True, the camera's cooler power setting can be read.
        /// </summary>
        public override bool CanGetCoolerPower
        {
            get
            {
                try
                {
                    Log.Write("CanGetCoolerPower get: false\n");

                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    return false;
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

        /// <summary>
        /// Returns True if the camera can send autoguider pulses to the telescope mount; 
        /// False if not.  (Note: this does not provide any indication of whether the
        /// autoguider cable is actually connected.)
        /// </summary>
        public override bool CanPulseGuide
        {
            get
            {
                try
                {
                    bool bReturn = false;

                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!bHasGuideCamera)
                    {
                        bReturn = sxCamera.hasGuidePort;
                    }

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

        /// <summary>
        /// If True, the camera's cooler setpoint can be adjusted. If False, the camera
        /// either uses open-loop cooling or does not have the ability to adjust temperature
        /// from software, and setting the TemperatureSetpoint property has no effect.
        /// </summary>
        public override bool CanSetCCDTemperature
        {
            get
            {
                try
                {
                    bool bReturn = sxCamera.hasCoolerControl && config.enableUntested;

                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write(String.Format("CanSetCCDTemperature get: {0}\n", bReturn));

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

        /// <summary>
        /// Turns on and off the camera cooler, and returns the current on/off state.
        /// Warning: turning the cooler off when the cooler is operating at high delta-T
        /// (typically >20C below ambient) may result in thermal shock.  Repeated thermal
        /// shock may lead to damage to the sensor or cooler stack.  Please consult the
        /// documentation supplied with the camera for further information.
        /// </summary>
        /// <exception cref=" System.Exception">not supported</exception>
        /// <exception cref=" System.Exception">an error condition such as link failure is present</exception>
        public override bool CoolerOn
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!(sxCamera.hasCoolerControl && config.enableUntested))
                    {
                        throw new ASCOM.PropertyNotImplementedException(SetError("CoolerOn get is not supported"), true);
                    }

                    bool bReturn = sxCamera.coolerEnabled;

                    Log.Write(String.Format("CoolerOn get: {0}\n", bReturn));

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
            set
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!(sxCamera.hasCoolerControl && config.enableUntested))
                    {
                        throw new ASCOM.PropertyNotImplementedException(SetError("CoolerOn set is not supported"), true);
                    }

                    Log.Write("CoolerOn set to " + value + "\n");

                    sxCamera.coolerEnabled = value;

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

        /// <summary>
        /// Returns the present cooler power level, in percent.  Returns zero if CoolerOn is
        /// False.
        /// </summary>
        /// <exception cref=" System.Exception">not supported</exception>
        /// <exception cref=" System.Exception">an error condition such as link failure is present</exception>
        public override double CoolerPower
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);
                    throw new ASCOM.PropertyNotImplementedException(String.Format("Cooler Power Must throw exception if not supported."), false);
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

        /// <summary>
        /// Sets the camera cooler setpoint in degrees Celsius, and returns the current
        /// setpoint.
        /// Note:  camera hardware and/or driver should perform cooler ramping, to prevent
        /// thermal shock and potential damage to the CCD array or cooler stack.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if command not successful.</exception>
        /// <exception cref=" System.Exception">Must throw exception if CanSetCCDTemperature is False.</exception>
        public override double SetCCDTemperature
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!(sxCamera.hasCoolerControl && config.enableUntested))
                    {
                        throw new ASCOM.PropertyNotImplementedException(String.Format("SetCCDTemperature get: must throw exception if CanSetCCDTemperature is False."), false);
                    }

                    double dReturn = sxCamera.coolerTemp/10 - 273.2;

                    Log.Write(String.Format("SetCCDTemperature get returns {0}\n", dReturn));

                    return dReturn;
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
            set
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!(sxCamera.hasCoolerControl && config.enableUntested))
                    {
                        throw new ASCOM.PropertyNotImplementedException(String.Format("SetCCDTemperature set: must throw exception if CanSetCCDTemperature is False."), false);
                    }

                    Log.Write(String.Format("SetCCDTemperature set to {0}\n", value));

                    sxCamera.coolerTemp = (UInt16) ((value * 10) + 2732);
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
    }
}

