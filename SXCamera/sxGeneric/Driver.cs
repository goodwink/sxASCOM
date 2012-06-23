//tabs=4
// --------------------------------------------------------------------------------
//
// ASCOM Camera driver for SXCamera
//
// Description:	
//
// This file contains the implementation for the generic SX camera.  This is 
// the base class for both the guide and main cameras, and provides the vast majority
// of the functionality.
//
// Implements:	ASCOM Camera interface version: 1.0
// Author:		Bret McKee <bretm@daddog.com>
//

using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

using ASCOM;
using ASCOM.Helper;
using ASCOM.Helper2;
using ASCOM.Interface;

using Logging;


namespace ASCOM.SXGeneric
{
    //
    // Your driver's ID is ASCOM.SXCamera.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.SXCamera.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [ClassInterface(ClassInterfaceType.None)]
    abstract public class Camera :
        ASCOM.SXCamera.ReferenceCountedObjectBase, 
        ICamera
    {
        // Constants
        private const double ImageCommandTime = 0.001; // this is about how long it takes to send the "Download Image" command

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
        protected ASCOM.SXCamera.Configuration m_config;
        private UInt16 m_vid, m_pid;
        private bool m_skip;
        // elminiate lots of log chatter
        private CameraStates m_lastLoggedState;
        private bool m_lastLoggedConnected;
        // values that back properties: property foo is in m_foo
        private bool m_Connected;
        private int m_NumX, m_NumY;
        private int m_StartX, m_StartY;

        #region Camera Constructor
         //
        // Constructor - Must be public for COM registration!
        //
        protected Camera(UInt16 whichController, UInt16 whichCamera)
        {
            try
            {
                Log.Write(String.Format("Generic::Camera({0}, {1}) begins\n", whichController, whichCamera));
                                
                m_cameraId = whichCamera;
                m_lastLoggedConnected = true;
                m_config = new ASCOM.SXCamera.Configuration(whichController, whichCamera);
                m_controller = ASCOM.SXCamera.SharedResources.controllers[whichController];
                
                if (m_config.selectionMethod == ASCOM.SXCamera.Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_ANY)
                {
                    m_vid = m_pid = 0;
                }
                else
                {
                    m_vid = m_config.VID;
                    m_pid = m_config.PID;
                }
                m_skip = (m_config.selectionMethod == ASCOM.SXCamera.Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL);

                oCameraStateLock = new Object();
                oGuideStateLock = new Object();

                Log.Write(String.Format("Generic::Camera() ends for vid={0} pid={1} skip={2}\n", m_vid, m_pid, m_skip));
            }
            catch (ASCOM.DriverException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new ASCOM.DriverException(SetError(String.Format("Camera constructor caught exception {0}\n", ex.ToString())), ex);
            }
        }
        #endregion

        //
        // PUBLIC COM INTERFACE ICamera IMPLEMENTATION
        //

        #region ICamera Members


        protected string SetError(string errorMessage)
        {
            bLastErrorValid = true;
            lastErrorMessage = errorMessage;

            Log.Write(String.Format("Generic::SetError({0})\n", errorMessage));
            return errorMessage;
        }

        protected void verifyConnected(string caller)
        {
            if (!Connected)
            {
                throw new ASCOM.NotConnectedException(SetError(String.Format("{0}: Camera not connected", caller)));
            }
        }

        /// <summary>
        /// Aborts the current exposure, if any, and returns the camera to Idle state.
        /// Must throw exception if camera is not idle and abort is
        ///  unsuccessful (or not possible, e.g. during download).
        /// Must throw exception if hardware or communications error
        ///  occurs.
        /// Must NOT throw an exception if the camera is already idle.
        /// </summary>
        public void AbortExposure()
        {
            try
            {
                verifyConnected(MethodBase.GetCurrentMethod().Name);

                Log.Write("Generic::AbortExposure() begins\n");

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
            catch (ASCOM.DriverException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
            }
        }

        /// <summary>
        /// Sets the binning factor for the X axis.  Also returns the current value.  Should
        /// default to 1 when the camera link is established.  Note:  driver does not check
        /// for compatible subframe values when this value is set; rather they are checked
        /// upon StartExposure.
        /// </summary>
        /// <value>BinX sets/gets the X binning value</value>
        /// <exception>Must throw an exception for illegal binning values</exception>
        private short BinXActual
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    short ret = sxCamera.xBin;

                    Log.Write(String.Format("Generic::BinXActual get returns {0}\n", ret));

                    return ret;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
            set
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (value > MaxBinXActual)
                    {
                        throw new ASCOM.InvalidValueException(SetError(String.Format("BinXActual set value out of range")), value.ToString(), MaxBinXActual.ToString());
                    }

                    sxCamera.xBin = (byte)value;
                    Log.Write(String.Format("Generic::BinXActual set to {0}\n", value));
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    SetError(ex.ToString());
                    throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, value.ToString(), "1-" + MaxBinXActual.ToString(), ex);
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        public short BinX
        {
            get
            {
                short ret = BinXActual;

                if (m_config.fixedBinning)
                {
                    ret = 1;
                }

                Log.Write(String.Format("sxCameraBase::BinX get returns {0}\n", ret));

                return ret;
            }
            set
            {
                if (m_config.fixedBinning)
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (value != 1)
                    {
                        throw new ASCOM.InvalidValueException(SetError(String.Format("BinX set value out of range")), value.ToString(), MaxBinXActual.ToString());
                    }
                    BinXActual = m_config.fixedBin;
                }
                else
                {
                    BinXActual = value;
                }
                Log.Write(String.Format("sxCameraBase::BinX set to {0}\n", value));
            }
        }


        /// <summary>
        /// Sets the binning factor for the Y axis  Also returns the current value.  Should
        /// default to 1 when the camera link is established.  Note:  driver does not check
        /// for compatible subframe values when this value is set; rather they are checked
        /// upon StartExposure.
        /// </summary>
        /// <exception>Must throw an exception for illegal binning values</exception>
        private short BinYActual
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    short ret = sxCamera.yBin;

                    Log.Write(String.Format("Generic::BinY get returns {0}\n", ret));

                    return ret;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
            set
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (value > MaxBinYActual)
                    {
                        throw new ASCOM.InvalidValueException(SetError(String.Format("BinY set value out of range")), value.ToString(), MaxBinYActual.ToString());
                    }

                    sxCamera.yBin = (byte)value;
                    Log.Write(String.Format("Generic::BinY set to {0}\n", value));
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    SetError(ex.ToString());
                    throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, value.ToString(), "1-" + MaxBinYActual.ToString(), ex);
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        public short BinY
        {
            get
            {
                short ret = BinYActual;

                if (m_config.fixedBinning)
                {
                    ret = 1;
                }

                Log.Write(String.Format("sxCameraBase::BinY get returns {0}\n", ret));

                return ret;
            }
            set
            {
                if (m_config.fixedBinning)
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (value != 1)
                    {
                        throw new ASCOM.InvalidValueException(SetError(String.Format("BinY set value out of range")), value.ToString(), MaxBinYActual.ToString());
                    }
                    BinYActual = m_config.fixedBin;
                }
                else
                {
                    BinYActual = value;
                }
                Log.Write(String.Format("sxCameraBase::BinY set to {0}\n", value));
            }
        }


        /// <summary>
        /// Returns the current CCD temperature in degrees Celsius. Only valid if
        /// CanControlTemperature is True.
        /// </summary>
        /// <exception>Must throw exception if data unavailable.</exception>
        abstract public double CCDTemperature
        {
            get;
        }

        /// <summary>
        /// Returns one of the following status information:
        /// <list type="bullet">
        ///  <listheader>
        ///   <description>Value  State          Meaning</description>
        ///  </listheader>
        ///  <item>
        ///   <description>0      CameraIdle      At idle state, available to start exposure</description>
        ///  </item>
        ///  <item>
        ///   <description>1      CameraWaiting   Exposure started but waiting (for shutter, trigger,
        ///                        filter wheel, etc.)</description>
        ///  </item>
        ///  <item>
        ///   <description>2      CameraExposing  Exposure currently in progress</description>
        ///  </item>
        ///  <item>
        ///   <description>3      CameraReading   CCD array is being read out (digitized)</description>
        ///  </item>
        ///  <item>
        ///   <description>4      CameraDownload  Downloading data to PC</description>
        ///  </item>
        ///  <item>
        ///   <description>5      CameraError     Camera error condition serious enough to prevent
        ///                        further operations (link fail, etc.).</description>
        ///  </item>
        /// </list>
        /// </summary>
        /// <exception cref="System.Exception">Must return an exception if the camera status is unavailable.</exception>
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
                            Log.Write(String.Format("Generic::CameraState() called from state {0}\n", state));
                            m_lastLoggedState = state;
                        }
                        return state;
                    }
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Returns the width of the CCD camera chip in unbinned pixels.
        /// </summary>
        /// <exception cref="System.Exception">Must throw exception if the value is not known</exception>
        public int CameraXSize
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    int ret = sxCamera.frameWidth;

                    if (m_config.fixedBinning)
                    {
                        ret /= m_config.fixedBin;
                    }

                    Log.Write(String.Format("Generic::CameraXSize get returns m_CameraXSize {0}\n", ret));

                    return ret;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Returns the height of the CCD camera chip in unbinned pixels.
        /// </summary>
        /// <exception cref="System.Exception">Must throw exception if the value is not known</exception>
        public int CameraYSize
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    int ret = sxCamera.frameHeight;

                    if (m_config.fixedBinning)
                    {
                        ret /= m_config.fixedBin;
                    }

                    Log.Write(String.Format("Generic::CameraYSize get returns CameraYSize {0}\n", ret));

                    return ret;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Returns True if the camera can abort exposures; False if not.
        /// </summary>
        public bool CanAbortExposure
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("Generic::CanAbortExposure get returns true\n");

                    return true;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// If True, the camera can have different binning on the X and Y axes, as
        /// determined by BinX and BinY. If False, the binning must be equal on the X and Y
        /// axes.
        /// </summary>
        /// <exception cref="System.Exception">Must throw exception if the value is not known (n.b. normally only
        ///            occurs if no link established and camera must be queried)</exception>
        public bool CanAsymmetricBin
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    bool bRet = m_config.asymetricBinning;

                    // The SX cameras can actualy do asymmetric binning, but with bayer color cameras it makes things weird, 
                    // and I don't need it, so I'm disallowing it.

                    Log.Write(String.Format("Generic::CanAsymetricBin get returns {0}\n", bRet));

                    return bRet;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// If True, the camera's cooler power setting can be read.
        /// </summary>
        abstract public bool CanGetCoolerPower
        {
            get;
        }

        /// <summary>
        /// Returns True if the camera can send autoguider pulses to the telescope mount; 
        /// False if not.  (Note: this does not provide any indication of whether the
        /// autoguider cable is actually connected.)
        /// </summary>
        abstract public bool CanPulseGuide
        {
            get;
        }


        /// <summary>
        /// If True, the camera's cooler setpoint can be adjusted. If False, the camera
        /// either uses open-loop cooling or does not have the ability to adjust temperature
        /// from software, and setting the TemperatureSetpoint property has no effect.
        /// </summary>
        abstract public bool CanSetCCDTemperature
        {
            get;
        }

        /// <summary>
        /// Some cameras support StopExposure, which allows the exposure to be terminated
        /// before the exposure timer completes, but will still read out the image.  Returns
        /// True if StopExposure is available, False if not.
        /// </summary>
        /// <exception cref=" System.Exception">not supported</exception>
        /// <exception cref=" System.Exception">an error condition such as link failure is present</exception>
        public bool CanStopExposure
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("Generic::CanStopExposure get returns true\n");

                    return true;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Controls the link between the driver and the camera. Set True to enable the
        /// link. Set False to disable the link (this does not switch off the cooler).
        /// You can also read the property to check whether it is connected.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if unsuccessful.</exception>
        public bool Connected
        {
            get
            {
                try
                {
                    if (m_lastLoggedConnected != m_Connected)
                    {
                        Log.Write(String.Format("Generic::Connected get returning {0}\n", m_Connected));
                        m_lastLoggedConnected = m_Connected;
                    }

                    return  m_Connected;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
            set
            {
                Log.Write(String.Format("Generic::Connected set: Current Value is {0}, requested value is {1}\n", m_Connected, value));

                try
                {
                    if (value)
                    {
#if DEBUG
                        DateTime expirationDate = new DateTime(2012, 8, 15);
                        DateTime currentDate = DateTime.Now;
                        Log.Write(String.Format("this version expires on {0}, it is currently {1}", expirationDate, currentDate));
                        if (currentDate.CompareTo(expirationDate) > 0)
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
                            if (!m_controller.Connected && !m_config.bUseDumpedData)
                            {
                                try
                                {
                                    m_controller.connect(m_vid, m_pid, m_skip);
                                }
                                catch (Exception ex)
                                {
                                    throw new ASCOM.DriverException(SetError(String.Format("SharedResources().controllerConnect(): caught an exception {0}\n", ex.ToString())), ex);
                                }
                            }

                            if (m_config.bUseDumpedData)
                            {
                                OpenFileDialog dlg = new OpenFileDialog();

                                dlg.ValidateNames = true;
                                dlg.Multiselect = false;
                                dlg.Title = "Select a dumped model file";

                                if (dlg.ShowDialog(ASCOM.SXCamera.SXCamera.m_MainForm) == DialogResult.OK)
                                {
                                    sxCamera = new sx.Camera(m_controller, m_cameraId, m_config.enableUntested, dlg.FileName);
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                sxCamera = new sx.Camera(m_controller, m_cameraId, m_config.enableUntested, m_config.bDumpData);
                            }

                            m_Connected = true;
                            // set properties to defaults. These all talk to the camera, and having them here saves
                            // a lot of try/catch blocks in other places

                            if (m_config.fixedBinning)
                            {
                                BinXActual = m_config.fixedBin;
                                BinYActual = m_config.fixedBin;
                                NumX = sxCamera.frameWidth / m_config.fixedBin;
                                NumY = sxCamera.frameHeight / m_config.fixedBin;
                            }
                            else
                            {
                                BinXActual = 1;
                                BinYActual = 1;
                                NumX = sxCamera.frameWidth;
                                NumY = sxCamera.frameHeight;
                            }
                            StartX = 0;
                            StartY = 0;
                            bHasGuideCamera = sxCamera.hasGuideCamera;

                            sxCamera.bInterlacedEqualization = m_config.interlacedEqualizeFrames;
                            sxCamera.bSquareLodestarPixels = m_config.squareLodestarPixels;

                            if (m_config.interlacedDoubleExposeShortExposures)
                            {
                                sxCamera.interlacedDoubleExposureThreshold = m_config.interlacedDoubleExposureThreshold;
                            }
                            else
                            {
                                sxCamera.interlacedDoubleExposureThreshold = 0;
                            }

                            if (m_config.interlacedGaussianBlur)
                            {
                                sxCamera.interlacedGaussianBlurRadius = m_config.interlacedGaussianBlurRadius;
                            }
                            else
                            {
                                sxCamera.interlacedGaussianBlurRadius = 0;
                            }
                            
                        }
                        catch (System.Exception ex)
                        {
                            sxCamera = null;
                            m_Connected = true;
                            throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
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
                        sxCamera = null;
                        m_Connected = false;
                    }

                    // set the last logged values to something that they are not, so that 
                    // the next time the are gotten we log them
                    m_lastLoggedConnected = !m_Connected;
                    m_lastLoggedState = CameraStates.cameraError;

                    Log.Write("Generic::conneted set ends\n");
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw new ASCOM.DriverException(SetError("Generic::Camera::connected set caught an exception: " + ex.ToString()), ex);
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
        abstract public bool CoolerOn
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the present cooler power level, in percent.  Returns zero if CoolerOn is
        /// False.
        /// </summary>
        /// <exception cref=" System.Exception">not supported</exception>
        /// <exception cref=" System.Exception">an error condition such as link failure is present</exception>
        abstract public double CoolerPower
        {
            get;
        }

        /// <summary>
        /// Returns a description of the camera model, such as manufacturer and model
        /// number. Any ASCII characters may be used. The string shall not exceed 68
        /// characters (for compatibility with FITS headers).
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if description unavailable</exception>
        public virtual string Description
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    string ret = sxCamera.description;

                    Log.Write(String.Format("Generic::Description get returns {0}\n", ret));

                    return ret;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }

            }
        }

        /// <summary>
        /// Returns the gain of the camera in photoelectrons per A/D unit. (Some cameras have
        /// multiple gain modes; these should be selected via the SetupDialog and thus are
        /// static during a session.)
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        public double ElectronsPerADU
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    double dRet = sxCamera.electronsPerADU;

                    Log.Write(String.Format("Generic::ElectronsPerADU get returns {0}\n", dRet));

                    return dRet;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw new ASCOM.PropertyNotImplementedException(SetError(String.Format("ElectronsPerADU Must throw exception if data unavailable.")), false, ex);
                }
            }
        }

        /// <summary>
        /// Reports the full well capacity of the camera in electrons, at the current camera
        /// settings (binning, SetupDialog settings, etc.)
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        public double FullWellCapacity
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    double dRet =  MaxADU * ElectronsPerADU / (BinXActual * BinYActual);

                    Log.Write(String.Format("Generic::FullWellCapacity get returns {0}\n", dRet));

                    return dRet;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// If True, the camera has a mechanical shutter. If False, the camera does not have
        /// a shutter.  If there is no shutter, the StartExposure command will ignore the
        /// Light parameter.
        /// </summary>
        public bool HasShutter
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("Generic::HasShutter get returns hard coded false\n");

                    return false;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Returns the current heat sink temperature (called "ambient temperature" by some
        /// manufacturers) in degrees Celsius. Only valid if CanControlTemperature is True.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        abstract public double HeatSinkTemperature
        {
            get;
        }

        /// <summary>
        /// Returns a safearray of int of size NumX * NumY containing the pixel values from
        /// the last exposure. The application must inspect the Safearray parameters to
        /// determine the dimensions. Note: if NumX or NumY is changed after a call to
        /// StartExposure it will have no effect on the size of this array. This is the
        /// preferred method for programs (not scripts) to download images since it requires
        /// much less memory.
        ///
        /// For color or multispectral cameras, will produce an array of NumX * NumY *
        /// NumPlanes.  If the application cannot handle multispectral images, it should use
        /// just the first plane.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        public object ImageArray
        {
            get 
            {
                try
                {
                    Log.Write("Generic::ImageArray get\n");

                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!bImageValid)
                    {
                        throw new ASCOM.ValueNotSetException(SetError("ImageArray get called when the image is not valid."));
                    }

                    try
                    {
                        return sxCamera.ImageArray;
                    }
                    catch (System.Exception ex)
                    {
                        SetError(ex.ToString());
                        throw new ASCOM.ValueNotSetException(MethodBase.GetCurrentMethod().Name, ex);
                    }
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Returns a safearray of Variant of size NumX * NumY containing the pixel values
        /// from the last exposure. The application must inspect the Safearray parameters to
        /// determine the dimensions. Note: if NumX or NumY is changed after a call to
        /// StartExposure it will have no effect on the size of this array. This property
        /// should only be used from scripts due to the extremely high memory utilization on
        /// large image arrays (26 bytes per pixel). Pixels values should be in Short, int,
        /// or Double format.
        ///
        /// For color or multispectral cameras, will produce an array of NumX * NumY *
        /// NumPlanes.  If the application cannot handle multispectral images, it should use
        /// just the first plane.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        public object ImageArrayVariant
        {
            get
            { 
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("Generic::ImageArrayVariant get\n");

                    if (!bImageValid)
                    {
                        throw new ASCOM.ValueNotSetException(SetError("ImageArrayVarient get called when the image is not valid."));
                    }

                    Int32[,] data = (Int32[,])ImageArray;

                    Log.Write(String.Format("got data value {0}\n", data));

                    Int32 width = data.GetLength(0);
                    Int32 height = data.GetLength(1);
                    object[,] oReturn = new object[width, height];

                    Log.Write(String.Format("Generic::ImageArrayVariant get: width={0} height={1}\n", width, height));

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            oReturn[x, y] = data[x, y];
                        }
                    }

                    return oReturn;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    SetError(ex.ToString());
                    throw new ASCOM.DriverException(String.Format("Caught an exception in {0}", MethodBase.GetCurrentMethod().Name), ex);
                }
            }
        }

        /// <summary>
        /// If True, there is an image from the camera available. If False, no image
        /// is available and attempts to use the ImageArray method will produce an
        /// exception.
        /// </summary>
        /// <exception cref=" System.Exception">hardware or communications link error has occurred.</exception>
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
                                throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request due to previous error\n", MethodBase.GetCurrentMethod().Name)));
                        }
                    }

                    Log.Write(String.Format("Generic::ImageReady get returns {0}\n", bImageValid));

                    return bImageValid;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// If True, pulse guiding is in progress. Required if the PulseGuide() method
        /// (which is non-blocking) is implemented. See the PulseGuide() method.
        /// </summary>
        /// <exception cref=" System.Exception">hardware or communications link error has occurred.</exception>
        public bool IsPulseGuiding
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write(String.Format("Generic::IsPulseGuiding get returns {0}\n", bGuiding));

                    lock (oGuideStateLock)
                    {
                        return bGuiding;
                    }
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Reports the last error condition reported by the camera hardware or communications
        /// link.  The string may contain a text message or simply an error code.  The error
        /// value is cleared the next time any method is called.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if no error condition.</exception>
        public string LastError
        {
            get 
            {
                try
                {
                    if (!bLastErrorValid)
                    {
                        throw new ASCOM.InvalidOperationException("LastError called when there was no last error");
                    }

                    Log.Write(String.Format("Generic::LastError get returns {0}\n", lastErrorMessage));

                    return lastErrorMessage;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Reports the actual exposure duration in seconds (i.e. shutter open time).  This
        /// may differ from the exposure time requested due to shutter latency, camera timing
        /// precision, etc.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if not supported or no exposure has been taken</exception>
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

                    Log.Write(String.Format("Generic::LastExposureDuration get returns {0}\n", actualExposureLength.TotalSeconds));

                    return actualExposureLength.TotalSeconds;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Reports the actual exposure start in the FITS-standard
        /// CCYY-MM-DDThh:mm:ss[.sss...] format.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if not supported or no exposure has been taken</exception>
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

                    Log.Write(String.Format("Generic::LastExposureStartTime get returns {0}\n", sRet));

                    return sRet;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Reports the maximum ADU value the camera can produce.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        public int MaxADU
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    int ret = sxCamera.fullWellCapacity;

                    Log.Write(String.Format("Generic::MaxADU get returns {0}\n", ret));

                    return ret;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// If AsymmetricBinning = False, returns the maximum allowed binning factor. If
        /// AsymmetricBinning = True, returns the maximum allowed binning factor for the X axis.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        ///
        private short MaxBinXActual
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

                    Log.Write(String.Format("Generic::MaxBinXActual get returns {0}\n", ret));

                    return ret;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        public short MaxBinX
        {
            get
            {
                short ret = MaxBinXActual;

                if (m_config.fixedBinning)
                {
                    ret = 1;
                }

                Log.Write(String.Format("sxCameraBase::MaxBinX get returns {0}\n", ret));

                return ret;
            }
        }

        /// <summary>
        /// If AsymmetricBinning = False, equals MaxBinXActual. If AsymmetricBinning = True,
        /// returns the maximum allowed binning factor for the Y axis.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        /// 
        public short MaxBinYActual
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

                    Log.Write(String.Format("Generic::MaxBinYActual get returns {0}\n", ret));

                    return ret;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        public short MaxBinY
        {
            get
            {
                short ret = MaxBinYActual;

                if (m_config.fixedBinning)
                {
                    ret = 1;
                }

                Log.Write(String.Format("sxCameraBase::MaxBinY get returns {0}\n", ret));

                return ret;
            }
        }
 

        /// <summary>
        /// Sets the subframe width. Also returns the current value.  If binning is active,
        /// value is in binned pixels.  No error check is performed when the value is set.
        /// Should default to CameraXSize.
        /// </summary>
        public int NumX
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write(String.Format("Generic::NumX get returns {0}\n",  m_NumX));

                    return m_NumX;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }

            set
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    m_NumX = value;
                    Log.Write(String.Format("Generic::NumX set to {0}\n", m_NumX));
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Sets the subframe height. Also returns the current value.  If binning is active,
        /// value is in binned pixels.  No error check is performed when the value is set.
        /// Should default to CameraYSize.
        /// </summary>
        public int NumY
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write(String.Format("Generic::NumY get returns {0}\n",  m_NumY));

                    return m_NumY;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
            set
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    m_NumY = value;

                    Log.Write(String.Format("Generic::NumY set to {0}\n", m_NumY));
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Returns the width of the CCD chip pixels in microns, as provided by the camera
        /// driver.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        public double PixelSizeX
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    double ret = sxCamera.pixelWidth;

                    if (m_config.fixedBinning)
                    {
                        ret *= m_config.fixedBin;
                    }

                    Log.Write(String.Format("Generic::PixelSizeX get returns {0}\n", ret));

                    return ret;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Returns the height of the CCD chip pixels in microns, as provided by the camera
        /// driver.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        public double PixelSizeY
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    double ret = sxCamera.pixelHeight;

                    if (m_config.fixedBinning)
                    {
                        ret *= m_config.fixedBin;
                    }

                    Log.Write(String.Format("Generic::PixelSizeY get returns {0}\n", ret));

                    return ret;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// This method returns only after the move has completed.
        ///
        /// symbolic Constants
        /// The (symbolic) values for GuideDirections are:
        /// Constant     Value      Description
        /// --------     -----      -----------
        /// guideNorth     0        North (+ declination/elevation)
        /// guideSouth     1        South (- declination/elevation)
        /// guideEast      2        East (+ right ascension/azimuth)
        /// guideWest      3        West (+ right ascension/azimuth)
        ///
        /// Note: directions are nominal and may depend on exact mount wiring.  guideNorth
        /// must be opposite guideSouth, and guideEast must be opposite guideWest.
        /// </summary>
        /// <param name="Direction">Direction of guide command</param>
        /// <param name="Duration">Duration of guide in milliseconds</param>
        /// <exception cref=" System.Exception">PulseGuide command is unsupported</exception>
        /// <exception cref=" System.Exception">PulseGuide command is unsuccessful</exception>
        public void PulseGuide(GuideDirections Direction, int Duration)
        {
            try
            {
                verifyConnected(MethodBase.GetCurrentMethod().Name);

                if (!CanPulseGuide)
                {
                    throw new ASCOM.InvalidOperationException(SetError(String.Format("PulseGuide() cannot be called if CanPuluseGuide == false")));
                }

                Log.Write(String.Format("Generic::PulseGuide({0}, {1})\n",  Direction, Duration));

                bLastErrorValid = false;

                lock (oGuideStateLock)
                {
                    bGuiding = true;
                }

                switch (Direction)
                {
                    case GuideDirections.guideNorth:
                        sxCamera.guideNorth(Duration);
                        break;
                    case GuideDirections.guideSouth:
                        sxCamera.guideSouth(Duration);
                        break;
                    case GuideDirections.guideEast:
                        sxCamera.guideEast(Duration);
                        break;
                    case GuideDirections.guideWest:
                        sxCamera.guideWest(Duration);
                        break;
                }
            }
            catch (ASCOM.DriverException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                SetError(ex.ToString());
                throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
            }
            finally
            {
                lock (oGuideStateLock)
                {
                    bGuiding = false;
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
        abstract public double SetCCDTemperature
        {
            get;
            set;
        }

        /// <summary>
        /// Launches a configuration dialog box for the driver.  The call will not return
        /// until the user clicks OK or cancel manually.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw an exception if Setup dialog is unavailable.</exception>
        public void SetupDialog()
        {
            try
            {
                m_config.SetupDialog();
            }
            catch (ASCOM.DriverException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                SetError(ex.ToString());
                throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
            }
        }

        internal void hardwareCapture(double Duration, bool Light)
        {
            try
            {
                Log.Write(String.Format("Generic::hardwareCapture({0}, {1}): begins\n", Duration, Light));
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

                Log.Write("Generic::hardwareCapture(): ends successfully\n");

                bImageValid = true;
            }
            catch (ASCOM.DriverException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                lock (oCameraStateLock)
                {
                    state = CameraStates.cameraError;
                }
                throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
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
                Log.Write(String.Format("Generic::softwareCapture({0}, {1}): begins\n", Duration, Light));

                sxCamera.clearCCDAndRegisters(); // For exposures > 1 second we will clear the registers again just before
                                                    // the exposure ends to clear any accumulated noise.
                bool bRegistersCleareded = false;

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
                    sxCamera.clearRegisters();
                    // For short exposures we don't do the clear the registers inside the loop
                    bRegistersCleareded = true;
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
                    
                    if (remainingExposureTime.TotalSeconds < 2.0 && !bRegistersCleareded)
                    {
                        Log.Write("softwareCapture(): doing clearAllRegisters() inside of loop, remaining exposure=" + remainingExposureTime.TotalSeconds + "\n");
                        sxCamera.clearRegisters();
                        bRegistersCleareded = true;
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

                Log.Write(String.Format("Generic::softwareCapture(): delay ends, actualExposureLength={0}, requested={1}\n", actualExposureLength.TotalSeconds, Duration));
                
                bImageValid = true;
            }
            catch (ASCOM.DriverException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                lock (oCameraStateLock)
                {
                    state = CameraStates.cameraError;
                }
                throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
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

        /// <summary>
        /// Starts an exposure. Use ImageReady to check when the exposure is complete.
        /// </summary>
        /// <exception cref=" System.Exception">NumX, NumY, XBin, YBin, StartX, StartY, or Duration parameters are invalid.</exception>
        /// <exception cref=" System.Exception">CanAsymmetricBin is False and BinX != BinY</exception>
        /// <exception cref=" System.Exception">the exposure cannot be started for any reason, such as a hardware or communications error</exception>
        virtual public void StartExposure(double Duration, bool Light)
        {
            try
            {
                Log.Write(String.Format("Generic::StartExposure({0}, {1}) begins\n", Duration, Light));

                // because of timing accuracy, we do all short exposures with the HW timer
                if (Duration <= 1.1)
                {
                    StartExposure(Duration, Light, true);
                }
                else
                {
                    StartExposure(Duration, Light, false);
                }
            }
            catch (ASCOM.DriverException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
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

                Log.Write(String.Format("Generic::StartExposure({0}, {1}, {2}) begins\n", Duration, Light, useHardwareTimer));

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
                        sxCamera.width = (UInt16)(NumX * BinXActual);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        SetError(ex.ToString());
                        throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, NumX.ToString(), "1-" + (CameraXSize/BinXActual).ToString(), ex);
                    }

                    try
                    {
                        sxCamera.height = (UInt16)(NumY * BinYActual);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        SetError(ex.ToString());
                        throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, NumY.ToString(), "1-" + (CameraYSize/BinYActual).ToString(), ex);
                    }
                    
                    try
                    {
                        sxCamera.xOffset = (UInt16)(StartX * BinXActual);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        SetError(ex.ToString());
                        throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, StartX.ToString(), "1-" + (CameraYSize/BinY).ToString(), ex);
                    }

                    try
                    {
                        sxCamera.yOffset = (UInt16)(StartY * BinYActual);
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
            catch (ASCOM.DriverException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
            }
        }

        /// <summary>
        /// Sets the subframe start position for the X axis (0 based). Also returns the
        /// current value.  If binning is active, value is in binned pixels.
        /// </summary>
        public int StartX
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write(String.Format("Generic::StartX get returns {0}\n", m_StartX));

                    return m_StartX;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }

            set
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    m_StartX = value;
                    Log.Write(String.Format("Generic::StartX set to {0}\n", m_StartX));
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Sets the subframe start position for the Y axis (0 based). Also returns the
        /// current value.  If binning is active, value is in binned pixels.
        /// </summary>
        public int StartY
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write(String.Format("Generic::StartY get returns {0}\n", m_StartY));

                    return m_StartY;
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }

            set
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    m_StartY = value;
                    Log.Write(String.Format("Generic::StartY set to {0}\n", m_StartY));
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
                }
            }
        }

        /// <summary>
        /// Stops the current exposure, if any.  If an exposure is in progress, the readout
        /// process is initiated.  Ignored if readout is already in process.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw an exception if CanStopExposure is False</exception>
        /// <exception cref=" System.Exception">Must throw an exception if no exposure is in progress</exception>
        /// <exception cref=" System.Exception">Must throw an exception if the camera or link has an error condition</exception>
        /// <exception cref=" System.Exception">Must throw an exception if for any reason no image readout will be available.</exception>
        public void StopExposure()
        {
            try
            {
                verifyConnected(MethodBase.GetCurrentMethod().Name);

                Log.Write("Generic::StopExposure() requested when in state " + state + "\n");

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
            catch (ASCOM.DriverException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
            }
        }
        #endregion
    }
}
