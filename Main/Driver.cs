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
        private const UInt16 DEFAULT_CAMERA_ID = 0;
        private const UInt16 DEFAULT_CONTROLLER_NUMBER = 0;

        public Camera(UInt16 cameraId, UInt16 controllerNumber) :
            base(cameraId, controllerNumber)
        {
            Log.Write(String.Format("Camera({0}, {1}) called\n", cameraId, controllerNumber));
        }

        public Camera() :
            this(DEFAULT_CAMERA_ID, DEFAULT_CONTROLLER_NUMBER)
        {
            Log.Write(String.Format("Camera() called\n"));
        }

        /// <summary>
        /// Returns the current CCD temperature in degrees Celsius. Only valid if
        /// CanControlTemperature is True.
        /// </summary>
        /// <exception>Must throw exception if data unavailable.</exception>
        override public double CCDTemperature
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!CanSetCCDTemperature)
                    {
                        throw new ASCOM.PropertyNotImplementedException(SetError("CCDTemperature get: must throw exception if data unavailable"), false);
                    }

                    double dReturn =  ((double)(sxCamera.coolerTemp - 2732)) / 10;

                    Log.Write(String.Format("CCDTemperature get: returns {0}\n", dReturn));

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
        }

        /// <summary>
        /// If True, the camera's cooler power setting can be read.
        /// </summary>
        override public bool CanGetCoolerPower
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("CanGetCoolerPower get: false\n");

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
        override public bool CanPulseGuide
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
        override public bool CanSetCCDTemperature
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    bool bReturn = sxCamera.hasCoolerControl && config.enableUntested;

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
        override public bool CoolerOn
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!CanSetCCDTemperature)
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

                    if (!CanSetCCDTemperature)
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
        override public double CoolerPower
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
        /// Returns the current heat sink temperature (called "ambient temperature" by some
        /// manufacturers) in degrees Celsius. Only valid if CanControlTemperature is True.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        override public double HeatSinkTemperature
        {
            get
            { 
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    throw new ASCOM.PropertyNotImplementedException(SetError("HeatSinkTemperature must throw exception if data unavailable"), true);
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
        override public double SetCCDTemperature
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!CanSetCCDTemperature)
                    {
                        throw new ASCOM.PropertyNotImplementedException(String.Format("SetCCDTemperature get: must throw exception if CanSetCCDTemperature is False."), false);
                    }

                    double dReturn =  ((double)(sxCamera.coolerSetPoint - 2732)) / 10;

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

                    if (!CanSetCCDTemperature)
                    {
                        throw new ASCOM.PropertyNotImplementedException(String.Format("SetCCDTemperature set: must throw exception if CanSetCCDTemperature is False."), false);
                    }

                    Log.Write(String.Format("SetCCDTemperature set to {0}\n", value));

                    sxCamera.coolerSetPoint = (UInt16) ((value * 10) + 2732);
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

