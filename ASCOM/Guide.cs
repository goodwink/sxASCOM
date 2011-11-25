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

namespace ASCOM.SXGuide0
{
    //
    // Your driver's ID is ASCOM.SXCamera.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.SXCamera.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("c150cbaa-429d-4bad-84ff-27077b4156a8")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Camera : ASCOM.SXGeneric.Camera
    {
        private const UInt16 DEFAULT_CONTROLLER_NUMBER = 0;
        private const UInt16 DEFAULT_CAMERAID = 1;

        public Camera(UInt16 controllerNumber, UInt16 cameraId) :
            base(controllerNumber, cameraId)
        {
            Log.Write(String.Format("Guide::Camera({0}, {1}) executing\n", cameraId, controllerNumber));
        }

        public Camera() :
            this(DEFAULT_CONTROLLER_NUMBER, DEFAULT_CAMERAID)
        {
            Log.Write(String.Format("Guide::Camera() executing\n"));
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

                    Log.Write("Guide Camera CCDTemperature get will throw an exception\n");

                    throw new ASCOM.PropertyNotImplementedException(SetError("CCDTemperature: must throw exception if data unavailable"), false);
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
        /// Returns a description of the camera model, such as manufacturer and model
        /// number. Any ASCII characters may be used. The string shall not exceed 68
        /// characters (for compatibility with FITS headers).
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if description unavailable</exception>
        override public string Description
        {
            get
            {
                try
                {
                    string sReturn = String.Format("SX-Guider ({0})", base.Description);

                    Log.Write(String.Format("Guide::Descripton get returns {0}\n", sReturn));

                    return sReturn;
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

                    Log.Write("Guide::CanGetCoolerPower get returns: false\n");

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

                    bool bReturn = bHasGuideCamera;

                    Log.Write(String.Format("Guide::CanPulseGuide returns {0}\n",bReturn));

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

                    Log.Write("Guide::CanSetCCDTemperature get returns false\n");

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

                    Log.Write("Guide::CoolerOn get will throw an exception\n");

                    throw new ASCOM.PropertyNotImplementedException(SetError("CoolerOn get is not supported"), true);
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

                    Log.Write("Guide Camera CoolerOn set will throw an exception\n");

                    throw new ASCOM.PropertyNotImplementedException(SetError("CoolerOn set is not supported"), true);
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

                    Log.Write("Guide::CoolerPower will throw an exception\n");

                    throw new ASCOM.PropertyNotImplementedException(SetError("CoolerPower get is not supported"), true);
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

                    Log.Write("Guide::HeatSinkTemperature get will throw an exception\n");

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

                    Log.Write("Giide::SetCCDTemperature get will throw an exception\n");

                    throw new ASCOM.PropertyNotImplementedException(String.Format("SetCCDTemperature must throw exception if CanSetCCDTemperature is False."), false);
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

                    Log.Write("Giide Camera SetCCDTemperature set will throw an exception\n");

                    throw new ASCOM.PropertyNotImplementedException(String.Format("SetCCDTemperature must throw exception if CanSetCCDTemperature is False."), true);
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
        /// Starts an exposure. Use ImageReady to check when the exposure is complete.
        /// </summary>
        /// <exception cref=" System.Exception">NumX, NumY, XBin, YBin, StartX, StartY, or Duration parameters are invalid.</exception>
        /// <exception cref=" System.Exception">CanAsymmetricBin is False and BinX != BinY</exception>
        /// <exception cref=" System.Exception">the exposure cannot be started for any reason, such as a hardware or communications error</exception>
        override public void StartExposure(double Duration, bool Light)
        {
            try
            {
                bool useHardwareTimer = false;

                if (Duration <= 5.0)
                {
                    useHardwareTimer = true;
                }

                Log.Write(String.Format("Guide::StartExposure({0}, {1}) useHardwareTimer = {2}\n", Duration, Light, useHardwareTimer));

                base.StartExposure(Duration, Light, useHardwareTimer);
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

namespace ASCOM.SXGuide1
{
    //
    // Your driver's ID is ASCOM.SXCamera.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.SXCamera.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("c150cbaa-429d-4bad-84ff-27077b4156a7")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Camera : ASCOM.SXGuide0.Camera
    {
        private const UInt16 DEFAULT_CONTROLLER_NUMBER = 1;
        private const UInt16 DEFAULT_CAMERAID = 1;

        public Camera() :
            base(DEFAULT_CONTROLLER_NUMBER, DEFAULT_CAMERAID)
        {
            Log.Write(String.Format("Guide1::Camera() executing\n"));
        }
    }
}
