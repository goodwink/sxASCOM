// tabs=4
// Copyright 2010-2012 by Dad Dog Development, Ltd
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
// This file contains the implementation for the SX camera base class.  This class
// provides the common functionality for SX cameras.  
//
// There are two directly derived classes, sxUsbCameraBase for cameras that have
// a direct USB connection, and sxGuideCameraBase for guide cameras that plug
// into another camera instead of into a USB port
//
// The majority of the camera code is in this file - the number of differences between
// USB and guide cameras is pretty small
//
// Implements:	ASCOM Camera interface version: 2.0
// Author:		Bret McKee <bretm@daddog.com>
//

using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using ASCOM.DeviceInterface;
using ASCOM.Utilities;

using Logging;

namespace ASCOM.sxCameraBase
{
    //
    // Your driver's ID is ASCOM.sxCameraBase.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.sxCameraBase.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("54040c6b-884e-4375-86d8-6f051560e79a")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    abstract public class Camera : 
        ASCOM.StarlightXpress.ReferenceCountedObjectBase,
        ICameraV2
    {
        #region Camera Constants
        // Constants
        private const double ImageCommandTime = 0.001; // this is about how long it takes to send the "Download Image" command
        #endregion

        #region Camera member variables
        // members
        protected sx.Controller m_controller;
        protected sx.Camera sxCamera;
        private DateTimeOffset exposureStart;
        private TimeSpan desiredExposureLength;
        private TimeSpan actualExposureLength;
        private delegate void CaptureDelegate(double Duration, bool Light);
        private bool bImageValid;
        private volatile Object oCameraStateLock;
        private volatile CameraStates state;
        private volatile bool bAbortRequested;
        private volatile bool bStopRequested;
        private volatile Object oGuideStateLock;
        private volatile bool bGuiding;
        private static UInt16 m_cameraId;
        protected bool bLastErrorValid;
        protected string lastErrorMessage;
        protected bool bHasGuideCamera;
        protected ASCOM.sxCameraBase.Configuration m_config;
        private UInt16 m_vid, m_pid;
        private bool m_skip;
        // elminiate lots of log chatter
        private CameraStates m_lastLoggedState;
        private bool m_lastLoggedConnected;
        // values that back properties: property foo is in m_foo
        private bool m_Connected;
        private int m_NumX, m_NumY;
        private int m_StartX, m_StartY;
        #endregion

        #region Camera Constructor
        //
        // Constructor
        //
        protected Camera(UInt16 whichController, UInt16 whichCamera)
        {
            try
            {
                Log.Write(String.Format("sxCameraBase::Camera({0}, {1}) begins\n", whichController, whichCamera));
                                
                m_cameraId = whichCamera;
                m_lastLoggedConnected = true;
                m_config = new ASCOM.sxCameraBase.Configuration(whichController, whichCamera);
                m_controller = ASCOM.StarlightXpress.SharedResources.controllers[whichController];
                
                if (m_config.selectionMethod == ASCOM.sxCameraBase.Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_ANY)
                {
                    m_vid = m_pid = 0;
                }
                else
                {
                    m_vid = m_config.VID;
                    m_pid = m_config.PID;
                }
                m_skip = (m_config.selectionMethod == ASCOM.sxCameraBase.Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL);

                oCameraStateLock = new Object();
                oGuideStateLock = new Object();

                Log.Write(String.Format("sxCameraBase::Camera() ends for vid={0} pid={1} skip={2}\n", m_vid, m_pid, m_skip));
            }
            catch (System.Exception ex)
            {
                String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                Log.Write(msg);
                throw ex;
            }
        }
        #endregion


        #region Utility Routines
        protected string SetError(string errorMessage)
        {
            bLastErrorValid = true;
            lastErrorMessage = errorMessage;

            Log.Write(String.Format("sxCameraBase::SetError({0})\n", errorMessage));
            return errorMessage;
        }

        protected void verifyConnected(string caller)
        {
            if (!Connected)
            {
                throw new ASCOM.NotConnectedException(SetError(String.Format("{0}: Camera not connected", caller)));
            }
        }

        internal void hardwareCapture(double Duration, bool Light)
        {
            try
            {
                Log.Write(String.Format("sxCameraBase::hardwareCapture({0}, {1}): begins\n", Duration, Light));
                exposureStart = DateTime.Now;
                DateTimeOffset exposureEnd;
                
                lock (oCameraStateLock)
                {
                    if (bAbortRequested)
                    {
                        return;
                    }
                    state = CameraStates.cameraExposing;
                }

                sxCamera.recordPixels(true, out exposureEnd);

                actualExposureLength = DateTime.Now - exposureStart;
                Log.Write(String.Format("hardwareCapture(): exposure + download took {0}, requested {1}\n", actualExposureLength.TotalSeconds, Duration));
                actualExposureLength = new TimeSpan(0, 0, 0, 0, (int)(1000*Duration));

                Log.Write("sxCameraBase::hardwareCapture(): ends successfully\n");

                bImageValid = true;
            }
            catch (System.Exception ex)
            {
                String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                Log.Write(msg);

                lock (oCameraStateLock)
                {
                    state = CameraStates.cameraError;
                }

                throw ex;
            }
            finally
            {
                lock (oCameraStateLock)
                {
                    if (state != CameraStates.cameraError)
                    {
                        state = CameraStates.cameraIdle;
                    }
                }
            }
        }

        internal void softwareCapture(double Duration, bool Light)
        {
            try
            {
                Log.Write(String.Format("sxCameraBase::softwareCapture({0}, {1}): begins\n", Duration, Light));

                sxCamera.clearCCDAndRegisters(); // For exposures > 1 second we will clear the registers again just before
                                                    // the exposure ends to clear any accumulated noise.
                bool bAllRegistersCleareded = false;
                bool bVerticalRegistersCleareded = false;

                if (Duration > ImageCommandTime)
                {
                    Duration -= ImageCommandTime;
                }
                else
                {
                    Duration = 0;
                }
                desiredExposureLength = TimeSpan.FromSeconds(Duration);

                if (desiredExposureLength.TotalSeconds < 2.0)
                {
                    sxCamera.clearAllRegisters();
                    // For short exposures we don't do the clear the registers inside the loop
                    bAllRegistersCleareded = true;
                    bVerticalRegistersCleareded = true; 
                }
                
                exposureStart = DateTime.Now;
                DateTimeOffset exposureEnd = exposureStart + desiredExposureLength;

                // We sleep for most of the exposure, then spin for the last little bit
                // because this helps us end closer to the right time

                Log.Write("softwareCapture(): about to begin loop, exposureEnd=" + exposureEnd + "\n");

                for (TimeSpan remainingExposureTime = desiredExposureLength;
                    remainingExposureTime.TotalMilliseconds > 0;
                    remainingExposureTime = exposureEnd - DateTime.Now)
                {
                    
                    if (remainingExposureTime.TotalSeconds < 2.0 && !bAllRegistersCleareded)
                    {
                        Log.Write("softwareCapture(): doing clearAllRegisters() inside of loop, remaining exposure=" + remainingExposureTime.TotalSeconds + "\n");
                        sxCamera.clearAllRegisters();
                        bAllRegistersCleareded = true;
                    }
                    else if (remainingExposureTime.TotalSeconds < 1.0 && !bVerticalRegistersCleareded)
                    {
                        Log.Write("softwareCapture(): doing clearVerticalRegisters() inside of loop, remaining exposure=" + remainingExposureTime.TotalSeconds + "\n");
                        sxCamera.clearVerticalRegisters();
                        bVerticalRegistersCleareded = true;
                    }
                    else if (remainingExposureTime.TotalMilliseconds > 75)
                    {
                        // sleep in small chunks so that we are responsive to abort and stop requests
                        //Log.Write("Before sleep, remaining exposure=" + remainingExposureTime.TotalSeconds + "\n");
                        Thread.Sleep(50);
                    }

                    if (bAbortRequested || bStopRequested)
                    {
                        break;
                    }
                }

                lock (oCameraStateLock)
                {
                    if (bAbortRequested)
                    {
                        return;
                    }
                    state = CameraStates.cameraDownload;
                }

                sxCamera.recordPixels(false, out exposureEnd);

                actualExposureLength = exposureEnd - exposureStart;

                Log.Write(String.Format("sxCameraBase::softwareCapture(): delay ends, actualExposureLength={0}, requested={1}\n", actualExposureLength.TotalSeconds, Duration));
                
                bImageValid = true;
            }
            catch (System.Exception ex)
            {
                String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                Log.Write(msg);

                lock (oCameraStateLock)
                {
                    state = CameraStates.cameraError;
                }
                throw ex;
            }
            finally
            {
                lock (oCameraStateLock)
                {
                    if (state != CameraStates.cameraError)
                    {
                        state = CameraStates.cameraIdle;
                    }
                }
            }
        }

        protected void StartExposure(double Duration, bool Light, bool useHardwareTimer)
        {
            try
            {
                verifyConnected(MethodBase.GetCurrentMethod().Name);

                if (Duration < 0)
                {
                    SetError("Exposure duration < 0");
                    throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, Duration.ToString(), ">= 0");
                }

                Log.Write(String.Format("sxCameraBase::StartExposure({0}, {1}, {2}) begins\n", Duration, Light, useHardwareTimer));

                bLastErrorValid = false;

                lock (oCameraStateLock)
                {
                    if (state != CameraStates.cameraIdle)
                    {
                        throw new ASCOM.InvalidOperationException(SetError(String.Format("StartExposure called while in state {0}", state)));
                    }
                    state = CameraStates.cameraExposing;
                }

                try
                {
                    bAbortRequested = false;
                    bStopRequested = false;
                    bImageValid = false;

                    try
                    {
                        sxCamera.width = (UInt16)(NumX * BinX);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        SetError(ex.ToString());
                        throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, NumX.ToString(), "1-" + (CameraXSize/BinX).ToString(), ex);
                    }

                    try
                    {
                        sxCamera.height = (UInt16)(NumY * BinY);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        SetError(ex.ToString());
                        throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, NumY.ToString(), "1-" + (CameraYSize/BinY).ToString(), ex);
                    }
                    
                    try
                    {
                        sxCamera.xOffset = (UInt16)(StartX * BinX);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        SetError(ex.ToString());
                        throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, StartX.ToString(), "1-" + (CameraYSize/BinY).ToString(), ex);
                    }

                    try
                    {
                        sxCamera.yOffset = (UInt16)(StartY * BinY);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        SetError(ex.ToString());
                        throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, StartY.ToString(), "1-" + (CameraYSize/BinY).ToString(), ex);
                    }

                    CaptureDelegate captureDelegate;

                    if (useHardwareTimer || sxCamera.mustUseHardwareTimer)
                    {
                        sxCamera.delayMs = (uint)(1000 * Duration);
                        captureDelegate = new CaptureDelegate(hardwareCapture);
                    }
                    else
                    {
                        captureDelegate = new CaptureDelegate(softwareCapture);
                    }

                    // we have passed all the parameter sanity checsks, and are still 
                    // running on the calling thread.  Tell the camera to freeze the
                    // parameters for this exposure
                    sxCamera.freezeParameters();

                    Log.Write("StartExposure() before captureDelegate.BeginInvode()\n");

                    captureDelegate.BeginInvoke(Duration, Light, null, null);

                    Log.Write("StartExposure() after captureDelegate.BeginInvode()\n");
                }
                catch (System.Exception ex)
                {
                    lock (oCameraStateLock)
                    {
                        if (state != CameraStates.cameraError)
                        {
                            state = CameraStates.cameraIdle;
                        }
                    }
                    throw ex;
                }
            }
            catch (System.Exception ex)
            {
                String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                Log.Write(msg);
                throw ex;
            }
        }
        #endregion

        #region Implementation of ICameraV2

        abstract public void SetupDialog();

        public string Action(string actionName, string actionParameters)
        {
            throw new ASCOM.MethodNotImplementedException("Action");
        }

        public void CommandBlind(string command, bool raw)
        {
            throw new ASCOM.MethodNotImplementedException("CommandBlind");
        }

        public bool CommandBool(string command, bool raw)
        {
            throw new ASCOM.MethodNotImplementedException("CommandBool");
        }

        public string CommandString(string command, bool raw)
        {
            throw new ASCOM.MethodNotImplementedException("CommandString");
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void AbortExposure()
        {
            try
            {
                verifyConnected(MethodBase.GetCurrentMethod().Name);

                Log.Write("sxCameraBase::AbortExposure() begins\n");

                bLastErrorValid = false;


                lock (oCameraStateLock)
                {
                    switch (state)
                    {
                        case CameraStates.cameraIdle:
                            // nothing to do
                            break;
                        case CameraStates.cameraExposing:
                            bAbortRequested = true;
                            break;
                        default:
                            throw new ASCOM.InvalidOperationException(SetError(String.Format("Abort not possible when camera is in state {0}", state)));
                    }
                }
            }
            catch (System.Exception ex)
            {
                String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                Log.Write(msg);
                throw ex;
            }
        }

        public void PulseGuide(GuideDirections direction, int duration)
        {
            try
            {
                verifyConnected(MethodBase.GetCurrentMethod().Name);

                if (!CanPulseGuide)
                {
                    throw new ASCOM.InvalidOperationException(SetError(String.Format("PulseGuide() cannot be called if CanPuluseGuide == false")));
                }

                Log.Write(String.Format("sxCameraBase::PulseGuide({0}, {1})\n",  direction, duration));

                bLastErrorValid = false;

                lock (oGuideStateLock)
                {
                    bGuiding = true;
                }

                switch (direction)
                {
                    case GuideDirections.guideNorth:
                        sxCamera.guideNorth(duration);
                        break;
                    case GuideDirections.guideSouth:
                        sxCamera.guideSouth(duration);
                        break;
                    case GuideDirections.guideEast:
                        sxCamera.guideEast(duration);
                        break;
                    case GuideDirections.guideWest:
                        sxCamera.guideWest(duration);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                Log.Write(msg);
                throw ex;
            }
            finally
            {
                lock (oGuideStateLock)
                {
                    bGuiding = false;
                }
            }
        }

        virtual public void StartExposure(double duration, bool light)
        {
            try
            {
                Log.Write(String.Format("sxCameraBase::StartExposure({0}, {1}) begins\n", duration, light));

                if (m_config.secondsAreMilliseconds)
                {
                    duration /= 1000;
                    Log.Write(String.Format("sxCameraBase::StartExposure(): after secondsAreMilliseconds adjustment, duration={0}\n", duration));
                }

                // because of timing accuracy, we do all short exposures with the HW timer
                if (duration <= 1.1)
                {
                    StartExposure(duration, light, true);
                }
                else
                {
                    StartExposure(duration, light, false);
                }
            }
            catch (System.Exception ex)
            {
                String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                Log.Write(msg);
                throw ex;
            }
        }

        public void StopExposure()
        {
            try
            {
                verifyConnected(MethodBase.GetCurrentMethod().Name);

                Log.Write("sxCameraBase::StopExposure() requested when in state " + state + "\n");

                bLastErrorValid = false;

                lock (oCameraStateLock)
                {
                    switch (state)
                    {
                        case CameraStates.cameraExposing:
                        case CameraStates.cameraDownload:
                            bStopRequested = true;
                            break;
                        default:
#if false
                            if (bStopRequested)
                                break; // they asked when it was legal and are just asking again.
                            throw new ASCOM.InvalidOperationException(String.Format("Stop not possible when camera is in state {0}", state));
#endif
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                Log.Write(msg);
                throw ex;
            }
        }

        public bool Connected
        {
            get
            {
                try
                {
                    if (m_lastLoggedConnected != m_Connected)
                    {
                        Log.Write(String.Format("sxCameraBase::Connected get returning {0}\n", m_Connected));
                        m_lastLoggedConnected = m_Connected;
                    }

                    return  m_Connected;
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
                Log.Write(String.Format("sxCameraBase::Connected set: Current Value is {0}, requested value is {1}\n", m_Connected, value));

                try
                {
                    if (value)
                    {
#if DEBUG
                        if (DateTime.Now.CompareTo(new DateTime(2012,01,15)) > 0)
                        {
                            MessageBox.Show("This debug release has expired.  Please update your bits", "Expired");
                            throw new ASCOM.PropertyNotImplementedException(SetError("connected: non-production release expired"), true);
                        }
#endif

                        if (m_Connected)
                        {
                            throw new ASCOM.InvalidOperationException(SetError("Attempt to connect when already connected"));
                        }

                        try
                        {
                            Log.Write(String.Format("m_controller.Connected={0}\n", m_controller.Connected));
                            if (!m_controller.Connected && !sx.Camera.m_useDumped)
                            {
                                try
                                {
                                    m_controller.connect(m_vid, m_pid, m_skip);
                                }
                                catch (System.Exception ex)
                                {
                                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                                    Log.Write(msg);
                                    throw ex;
                                }
                            }
                            sxCamera = new sx.Camera(m_controller, m_cameraId, m_config.enableUntested, m_config.dumpDataEnabled);
                            m_Connected = true;
                            // set properties to defaults. These all talk to the camera, and having them here saves
                            // a lot of try/catch blocks in other places
                            BinX = 1;
                            BinY = 1;
                            NumX = sxCamera.frameWidth;
                            NumY = sxCamera.frameHeight;
                            StartX = 0;
                            StartY = 0;
                            bHasGuideCamera = sxCamera.hasGuideCamera;
                        }
                        catch (System.Exception ex)
                        {
                            sxCamera = null;
                            m_Connected = true;
                            String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                            Log.Write(msg);
                            throw ex;
                        }
                        // setup state variables
                        state = CameraStates.cameraIdle;
                        bImageValid = false;
                        bAbortRequested = false;
                        bStopRequested = false;
                        bGuiding = false;
                        bLastErrorValid = false;
                        lastErrorMessage = "Uninitialized Last Error";
                    }
                    else
                    {
                        m_controller.disconnect();
                        sxCamera = null;
                        m_Connected = false;
                    }

                    // set the last logged values to something that they are not, so that 
                    // the next time the are gotten we log them
                    m_lastLoggedConnected = !m_Connected;
                    m_lastLoggedState = CameraStates.cameraError;

                    Log.Write("sxCameraBase::conneted set ends\n");
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public virtual string Description
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    string ret = sxCamera.description;

                    Log.Write(String.Format("sxCameraBase::Description get returns {0}\n", ret));

                    return ret;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public string DriverInfo
        {
            get 
            { 
                return "sxASCOM Driver version " + ASCOM.StarlightXpress.SharedResources.versionNumber + "\r\n" +
                       "Copyright (C) 2011, 2012 Dad Dog Developtment Ltd\r\n" +
                       "Author: Bret McKee <bretm@daddog.com>\r\n" +
                       "\r\n" +
                       "This work is licensed under the Creative Commons Attribution-No Derivative Works 3.0 License.\r\n" +
                       "To view a copy of this license, visit http://creativecommons.org/licenses/by-nd/3.0/ or\r\n" +
                       "send a letter to Creative Commons, 171 Second Street, Suite 300, San Francisco, California, 94105, USA.\r\n";
            }
        }

        public string DriverVersion
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
            }
        }

        public short InterfaceVersion
        {
            get { return 2; }
        }

        public string Name
        {
            get
            {
                return "sxASCOM";
            }
        }

        public ArrayList SupportedActions
        {
            get { return new ArrayList(); }
        }

        public short BinX
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    short ret = sxCamera.xBin;

                    Log.Write(String.Format("sxCameraBase::BinX get returns {0}\n", ret));

                    return ret;
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

                    if (value > MaxBinX)
                    {
                        throw new ASCOM.InvalidValueException(SetError(String.Format("BinX set value out of range")), value.ToString(), MaxBinX.ToString());
                    }

                    sxCamera.xBin = (byte)value;
                    Log.Write(String.Format("sxCameraBase::BinX set to {0}\n", value));
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    SetError(ex.ToString());
                    throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, value.ToString(), "1-" + MaxBinX.ToString(), ex);
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public short BinY
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    short ret = sxCamera.yBin;

                    Log.Write(String.Format("sxCameraBase::BinY get returns {0}\n", ret));

                    return ret;
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

                    if (value > MaxBinY)
                    {
                        throw new ASCOM.InvalidValueException(SetError(String.Format("BinY set value out of range")), value.ToString(), MaxBinY.ToString());
                    }

                    sxCamera.yBin = (byte)value;
                    Log.Write(String.Format("sxCameraBase::BinY set to {0}\n", value));
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    SetError(ex.ToString());
                    throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, value.ToString(), "1-" + MaxBinY.ToString(), ex);
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public CameraStates CameraState
        {
            get 
            { 
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    lock (oCameraStateLock)
                    {
                        if (m_lastLoggedState != state)
                        {
                            Log.Write(String.Format("sxCameraBase::CameraState() called from state {0}\n", state));
                            m_lastLoggedState = state;
                        }
                        return state;
                    }
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public int CameraXSize
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    int ret = sxCamera.frameWidth;

                    Log.Write(String.Format("sxCameraBase::CameraXSize get returns m_CameraXSize {0}\n", ret));

                    return ret;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public int CameraYSize
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    int ret = sxCamera.frameHeight;

                    Log.Write(String.Format("sxCameraBase::CameraYSize get returns CameraYSize {0}\n", ret));

                    return ret;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public bool CanAbortExposure
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("sxCameraBase::CanAbortExposure get returns true\n");

                    return true;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public bool CanAsymmetricBin
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    bool bRet = ! m_config.symetricBinning;

                    // The SX cameras can actualy do asymmetric binning, but with bayer color cameras it makes things weird, 
                    // and I don't need it, so I'm disallowing it.

                    Log.Write(String.Format("sxCameraBase::CanAsymetricBin get returns {0}\n", bRet));

                    return bRet;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        abstract public bool CanGetCoolerPower
        {
            get;
        }

        abstract public bool CanPulseGuide
        {
            get;
        }

        abstract public bool CanSetCCDTemperature
        {
            get;
        }

        public bool CanStopExposure
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("sxCameraBase::CanStopExposure get returns true\n");

                    return true;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        abstract public double CCDTemperature
        {
            get;
        }

        abstract public bool CoolerOn
        {
            get;
            set;
        }

        abstract public double CoolerPower
        {
            get;
        }

        public double ElectronsPerADU
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    double dRet = sxCamera.electronsPerADU;

                    Log.Write(String.Format("sxCameraBase::ElectronsPerADU get returns {0}\n", dRet));

                    return dRet;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public double FullWellCapacity
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    double dRet =  MaxADU * ElectronsPerADU / (BinX * BinY);

                    Log.Write(String.Format("sxCameraBase::FullWellCapacity get returns {0}\n", dRet));

                    return dRet;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public bool HasShutter
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("sxCameraBase::HasShutter get returns hard coded false\n");

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

        abstract public double HeatSinkTemperature
        {
            get;
        }

        public object ImageArray
        {
            get 
            {
                try
                {
                    Log.Write("sxCameraBase::ImageArray get\n");

                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!bImageValid)
                    {
                        throw new ASCOM.ValueNotSetException(SetError("ImageArray get called when the image is not valid."));
                    }

                    return sxCamera.ImageArray;
                }
                catch (System.ArgumentException ex)
                {
                    String msg = SetError(String.Format("{0} caught exception {1} and is throwing ASCOM.ValueNotSetException()", MethodBase.GetCurrentMethod().Name, ex));
                    throw new ASCOM.ValueNotSetException(msg, ex);
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public object ImageArrayVariant
        {
            get
            { 
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("sxCameraBase::ImageArrayVariant get\n");

                    if (!bImageValid)
                    {
                        throw new ASCOM.ValueNotSetException(SetError("ImageArrayVarient get called when the image is not valid."));
                    }

                    Int32[,] data = (Int32[,])ImageArray;

                    Log.Write(String.Format("got data value {0}\n", data));

                    Int32 width = data.GetLength(0);
                    Int32 height = data.GetLength(1);
                    object[,] oReturn = new object[width, height];

                    Log.Write(String.Format("sxCameraBase::ImageArrayVariant get: width={0} height={1}\n", width, height));

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            oReturn[x, y] = data[x, y];
                        }
                    }

                    return oReturn;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public bool ImageReady
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    lock (oCameraStateLock)
                    {
                        if (state == CameraStates.cameraError)
                        {
                                throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request due to previous error", MethodBase.GetCurrentMethod().Name)));
                        }
                    }

                    Log.Write(String.Format("sxCameraBase::ImageReady get returns {0}\n", bImageValid));

                    return bImageValid;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public bool IsPulseGuiding
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write(String.Format("sxCameraBase::IsPulseGuiding get returns {0}\n", bGuiding));

                    lock (oGuideStateLock)
                    {
                        return bGuiding;
                    }
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public double LastExposureDuration
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!bImageValid)
                    {
                        throw new ASCOM.ValueNotSetException(SetError("The image is not valid."));
                    }

                    Log.Write(String.Format("sxCameraBase::LastExposureDuration get returns {0}\n", actualExposureLength.TotalSeconds));

                    return actualExposureLength.TotalSeconds;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public string LastExposureStartTime
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!bImageValid)
                    {
                        throw new ASCOM.ValueNotSetException(SetError("The image is not valid."));
                    }

                    string sRet = exposureStart.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff");

                    Log.Write(String.Format("sxCameraBase::LastExposureStartTime get returns {0}\n", sRet));

                    return sRet;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public int MaxADU
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    int ret = sxCamera.fullWellCapacity;

                    Log.Write(String.Format("sxCameraBase::MaxADU get returns {0}\n", ret));

                    return ret;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public short MaxBinX
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    short ret = sxCamera.maxXBin;
                        
                    if (ret > m_config.maxXBin)
                    {
                        ret = m_config.maxXBin;
                    }

                    Log.Write(String.Format("sxCameraBase::MaxBinX get returns {0}\n", ret));

                    return ret;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public short MaxBinY
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    short ret = sxCamera.maxYBin;
                        
                    if (ret > m_config.maxYBin)
                    {
                        ret = m_config.maxYBin;
                    }

                    Log.Write(String.Format("sxCameraBase::MaxBinY get returns {0}\n", ret));

                    return ret;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public int NumX
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write(String.Format("sxCameraBase::NumX get returns {0}\n",  m_NumX));

                    return m_NumX;
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

                    m_NumX = value;
                    Log.Write(String.Format("sxCameraBase::NumX set to {0}\n", m_NumX));
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public int NumY
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write(String.Format("sxCameraBase::NumY get returns {0}\n",  m_NumY));

                    return m_NumY;
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

                    m_NumY = value;

                    Log.Write(String.Format("sxCameraBase::NumY set to {0}\n", m_NumY));
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public double PixelSizeX
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    double ret = sxCamera.pixelWidth;

                    Log.Write(String.Format("sxCameraBase::PixelSizeX get returns {0}\n", ret));

                    return ret;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public double PixelSizeY
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    double ret = sxCamera.pixelHeight;

                    Log.Write(String.Format("sxCameraBase::PixelSizeY get returns {0}\n", ret));

                    return ret;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        abstract public double SetCCDTemperature
        {
            get;
            set;
        }

        public int StartX
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write(String.Format("sxCameraBase::StartX get returns {0}\n", m_StartX));

                    return m_StartX;
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

                    m_StartX = value;
                    Log.Write(String.Format("sxCameraBase::StartX set to {0}\n", m_StartX));
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public int StartY
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write(String.Format("sxCameraBase::StartY get returns {0}\n", m_StartY));

                    return m_StartY;
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

                    m_StartY = value;
                    Log.Write(String.Format("sxCameraBase::StartY set to {0}\n", m_StartY));
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public short BayerOffsetX
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (sxCamera.isMonochrome)
                    {
                        throw new ASCOM.InvalidValueException("BayerOffsetX is undefined for monochrome cameras");
                    }
                    Log.Write(String.Format("sxCameraBase::BayerOffsetX get returns hard coded 0"));

                    return 0;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public short BayerOffsetY
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (sxCamera.isMonochrome)
                    {
                        throw new ASCOM.InvalidValueException("BayerOffsetY is undefined for monochrome cameras");
                    }
                    Log.Write(String.Format("sxCameraBase::BayerOffsetY get returns hard coded 0"));

                    return 0;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public bool CanFastReadout
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write(String.Format("sxCameraBase::CanFastReadout get returns hard coded false"));

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

        public double ExposureMax
        {
            get { throw new PropertyNotImplementedException("ExposureMax", false); }
        }

        public double ExposureMin
        {
            get { throw new PropertyNotImplementedException("ExposureMin", false); }
        }

        public double ExposureResolution
        {
            get { return 0.001; }
        }

        public bool FastReadout
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    string msg = "sxCameraBase::FastReadout Get must throw and exception if CanFastReadout returns false";
                    Log.Write(msg);

                    throw new PropertyNotImplementedException(msg, false); 
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

                    string msg = "sxCameraBase::FastReadout Set must throw and exception if CanFastReadout returns false";
                    Log.Write(msg);

                    throw new PropertyNotImplementedException(msg, true); 
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public short Gain
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    string msg = "sxCameraBase::Gain get throws an exception because gain is not supported";
                    Log.Write(msg);

                    throw new PropertyNotImplementedException(msg, false); 
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

                    string msg = "sxCameraBase::Gain set throws an exception because gain is not supported";
                    Log.Write(msg);

                    throw new PropertyNotImplementedException(msg, true); 
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public short GainMax
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    string msg = "sxCameraBase::GainMax throws an exception because gain is not supported";
                    Log.Write(msg);

                    throw new PropertyNotImplementedException(msg, false); 
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public short GainMin
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    string msg = "sxCameraBase::GainMin throws an exception because gain is not supported";
                    Log.Write(msg);

                    throw new PropertyNotImplementedException(msg, false); 
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public ArrayList Gains
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    string msg = "sxCameraBase::Gains throws an exception because gain is not supported";
                    Log.Write(msg);

                    throw new PropertyNotImplementedException(msg, false); 
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public short PercentCompleted
        {
            get { throw new PropertyNotImplementedException("PercentCompleted", false); }
        }

        public short ReadoutMode
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    string msg = "sxCameraBase::ReadoutMode get throws an exception because readoutmodes are not supported";
                    Log.Write(msg);

                    throw new PropertyNotImplementedException(msg, false); 
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

                    string msg = "sxCameraBase::ReadoutMode set throws an exception because readoutmodes are not supported";
                    Log.Write(msg);

                    throw new PropertyNotImplementedException(msg, true); 
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public ArrayList ReadoutModes
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    string msg = "sxCameraBase::ReadoutModes throws an exception because readoutmodes are not supported";
                    Log.Write(msg);

                    throw new PropertyNotImplementedException(msg, false); 
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public string SensorName
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    string ret = sxCamera.sensorName;

                    Log.Write("SensorName get returns " + ret);

                    return ret;

                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        public SensorType SensorType
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    SensorType ret;

                    if (sxCamera.isMonochrome)
                    {
                        ret = SensorType.Monochrome;
                    }
                    else if (sxCamera.isRGGB)
                    {
                        ret = SensorType.RGGB;
                    }
                    else
                    {
                        throw new ASCOM.DriverException(SetError(String.Format("SensorType get was unable to determine sensor type")));
                    }

                    Log.Write(String.Format("SensorType get returns {0}", ret));

                    return ret;

                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        #endregion
    }
}
