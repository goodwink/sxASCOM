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

namespace ASCOM.SXMain0
{
    //
    // Your driver's ID is ASCOM.SXCamera.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.SXCamera.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("c150cbaa-429d-4bad-84ff-27077b4156a0")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Camera : ASCOM.SXGeneric.Camera
    {
        private const UInt16 DEFAULT_CONTROLLER_NUMBER = 0;
        private const UInt16 DEFAULT_CAMERA_ID = 0;

        public Camera(UInt16 controllerNumber, UInt16 cameraId) :
            base(controllerNumber, cameraId)
        {
            Log.Write(String.Format("Main::Camera({0}, {1}) executing\n", cameraId, controllerNumber));
        }

        public Camera() :
            this(DEFAULT_CONTROLLER_NUMBER, DEFAULT_CAMERA_ID)
        {
            Log.Write(String.Format("Main::Camera() executing\n"));
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

                    Log.Write(String.Format("Main::CCDTemperature get returns {0}\n", dReturn));

                    return dReturn;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
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

                    Log.Write("Main::CanGetCoolerPower get returns false\n");

                    return false;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
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
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    bool bReturn = false;

                    if (!bHasGuideCamera)
                    {
                        bReturn = sxCamera.hasGuidePort;
                    }

                    Log.Write(String.Format("Main::CanPulseGuide get returns {0}\n", bReturn));

                    return bReturn;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
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

                    bool bReturn = sxCamera.hasCoolerControl;

                    Log.Write(String.Format("Main::CanSetCCDTemperature get returns {0}\n", bReturn));

                    return bReturn;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
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

                    Log.Write(String.Format("Main::CoolerOn get returns {0}\n", bReturn));

                    return bReturn;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
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

                    sxCamera.coolerEnabled = value;
                    Log.Write(String.Format("CoolerOn set to {0}\n", value));
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
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

                    Log.Write("Main::CoolerPower will throw exception\n");

                    throw new ASCOM.PropertyNotImplementedException(SetError(String.Format("Cooler Power Must throw exception if not supported.")), false);
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
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

                    Log.Write("Main::HeatSinkTemperature will throw exception\n");

                    throw new ASCOM.PropertyNotImplementedException(SetError("HeatSinkTemperature must throw exception if data unavailable"), true);
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
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

                    Log.Write(String.Format("Main::SetCCDTemperature get returns {0}\n", dReturn));

                    return dReturn;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
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

                    Log.Write(String.Format("Main::SetCCDTemperature set to {0}\n", value));

                    // These two range checks are required by conform, but not by the spec.
                    if (value >= 100 || value <= -273)
                    {
                        throw new ASCOM.InvalidValueException(SetError(String.Format("SetCCDTemperature request for {0} is not reasonable", value)), value.ToString(), "<= 100");
                    }

                    sxCamera.coolerSetPoint = (UInt16) ((value * 10) + 2732);
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

    }
}

