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
using System.Threading;
using System.Reflection;
using System.Windows.Forms;

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
        private static UInt16 cameraId;
        protected bool bLastErrorValid;
        protected string lastErrorMessage;
        protected bool bHasGuideCamera;
        protected ASCOM.SXCamera.Configuration config;
        // values that back properties: property foo is in m_foo
        private bool m_Connected;
        private short m_BinX, m_BinY;
        private short m_MaxBinX, m_MaxBinY;
        private int m_CameraXSize, m_CameraYSize;
        protected string m_Description;
        private int m_MaxADU;
        private int m_NumX, m_NumY;
        private double m_PixelSizeX, m_PixelSizeY;
        private int m_StartX, m_StartY;

        #region Camera Constructor
         //
        // Constructor - Must be public for COM registration!
        //
        protected Camera(UInt16 whichCamera, string cameraType)
        {
            try
            {
                //Thread.Sleep(15000);
                Log.Write(String.Format("Camera({0},{1})\n", whichCamera, cameraType));

                cameraId = whichCamera;

                oCameraStateLock = new Object();
                oGuideStateLock = new Object();

                config = new ASCOM.SXCamera.Configuration(cameraType);
            }
            catch (ASCOM.DriverException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                String msg = String.Format("Camera constructor caught exception {0}\n", ex.ToString());

                Log.Write(msg);
                throw new ASCOM.DriverException(msg, ex);
            }

            Log.Write("Camera() constructor returns\n");

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

            Log.Write("SetError(" + errorMessage + ")\n");
            return errorMessage;
        }

        private void verifyConnected(string caller)
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
            Log.Write("AbortExposure()\n");

            bLastErrorValid = false;

            verifyConnected(MethodBase.GetCurrentMethod().Name);

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

        /// <summary>
        /// Sets the binning factor for the X axis.  Also returns the current value.  Should
        /// default to 1 when the camera link is established.  Note:  driver does not check
        /// for compatible subframe values when this value is set; rather they are checked
        /// upon StartExposure.
        /// </summary>
        /// <value>BinX sets/gets the X binning value</value>
        /// <exception>Must throw an exception for illegal binning values</exception>
        public short BinX
        {
            get
            {
                Log.Write("BinX get: m_Binx =" + m_BinX + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_BinX;
            }
            set
            {
                Log.Write("BinX set to " + value + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                try
                {
                    m_BinX = value;
                    sxCamera.xBin = (byte)value;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    SetError(ex.ToString());
                    throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, value.ToString(), "1-" + m_MaxBinX.ToString(), ex);
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError("Unable to complete " + MethodBase.GetCurrentMethod().Name + " request"), ex);
                }
            }
        }

        /// <summary>
        /// Sets the binning factor for the Y axis  Also returns the current value.  Should
        /// default to 1 when the camera link is established.  Note:  driver does not check
        /// for compatible subframe values when this value is set; rather they are checked
        /// upon StartExposure.
        /// </summary>
        /// <exception>Must throw an exception for illegal binning values</exception>
        public short BinY
        {
            get
            {
                Log.Write("BinY get: m_BinY =" + m_BinY +"\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_BinY;
            }
            set
            {
                Log.Write("BinY set to " + value +" \n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                try
                {
                    m_BinY = value;
                    sxCamera.yBin = (byte)value;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    SetError(ex.ToString());
                    throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, value.ToString(), "1-" + MaxBinY.ToString(), ex);
                }
                catch (System.Exception ex)
                {
                    throw new ASCOM.DriverException(SetError("Unable to complete " + MethodBase.GetCurrentMethod().Name + " request"), ex);
                }
            }
        }

        /// <summary>
        /// Returns the current CCD temperature in degrees Celsius. Only valid if
        /// CanControlTemperature is True.
        /// </summary>
        /// <exception>Must throw exception if data unavailable.</exception>
        public double CCDTemperature
        {
            get
            {
                Log.Write("CCDTemperature: will throw excpetion\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);
                throw new ASCOM.PropertyNotImplementedException(SetError("CCDTemperature: must throw exception if data unavailable"), false);
            }
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
                Log.Write("CameraState() called from state " + state +"\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                lock (oCameraStateLock)
                {
                    return state;
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
                Log.Write("CameraXSize get: m_CameraXSize = " + m_CameraXSize + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_CameraXSize;
            }
            private set
            {
                m_CameraXSize = value;
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
                Log.Write("CameraYSize get: m_CameraYSize = " + m_CameraYSize +"\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_CameraYSize;
            }

            private set
            {
                m_CameraYSize = value;
            }
        }

        /// <summary>
        /// Returns True if the camera can abort exposures; False if not.
        /// </summary>
        public bool CanAbortExposure
        {
            get
            {
                Log.Write("CanAbortExposure get\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return true;
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
                Log.Write("CanAsymetricBin get: false\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                // The SX cameras can actualy do asymmetric binning, but with bayer color cameras it makes things weird, 
                // and I don't need it, so I'm disallowing it.

                return false;
            }
        }

        /// <summary>
        /// If True, the camera's cooler power setting can be read.
        /// </summary>
        public bool CanGetCoolerPower
        {
            get
            {
                Log.Write("CanGetCoolerPower get: false\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return false;
            }
        }

        /// <summary>
        /// Returns True if the camera can send autoguider pulses to the telescope mount; 
        public abstract bool CanPulseGuide
        {
            get;
        }

        /// False if not.  (Note: this does not provide any indication of whether the
        /// autoguider cable is actually connected.)
        /// </summary>


        /// <summary>
        /// If True, the camera's cooler setpoint can be adjusted. If False, the camera
        /// either uses open-loop cooling or does not have the ability to adjust temperature
        /// from software, and setting the TemperatureSetpoint property has no effect.
        /// </summary>
        public bool CanSetCCDTemperature
        {
            get
            {
                Log.Write("CanSetCCDTemperature get: false\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return false;
            }
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
                Log.Write("CanStopExposure get: true\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return true;
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
                Log.Write("Connected get: returning " + m_Connected +"\n");

                return  m_Connected;
            }
            set
            {
                Log.Write(String.Format("Connected set: Current Value is {0}, requested value is {1}\n", m_Connected, value));

                try
                {
                    if (value)
                    {
                        if (DateTime.Now.CompareTo(DateTime.Parse("10/1/2010")) > 0)
                        {
                            MessageBox.Show("This Beta Release has expired.  Please update your bits", "Expired");
                            throw new ASCOM.PropertyNotImplementedException(SetError("connected: Beta release expired"), true);
                        }

                        if (m_Connected)
                        {
                            throw new ASCOM.InvalidOperationException(SetError("Attempt to connect when already connected"));
                        }

                        try
                        {
                            sxCamera = new sx.Camera(SXCamera.SharedResources.controller, cameraId);
                            m_Connected = true;
                            // set properties to defaults. These all talk to the camera, and having them here saves
                            // a lot of try/catch blocks in other places
                            MaxBinX = sxCamera.xBinMax;
                            MaxBinY = sxCamera.yBinMax;
                            CameraXSize = sxCamera.ccdWidth;
                            CameraYSize = sxCamera.ccdHeight;
                            BinX = 1;
                            BinY = 1;
                            NumX = sxCamera.ccdWidth;
                            NumY = sxCamera.ccdHeight;
                            Description = sxCamera.description;
                            MaxADU = (1 << sxCamera.bitsPerPixel) - 1;
                            PixelSizeX = sxCamera.pixelWidth;
                            PixelSizeY = sxCamera.pixelHeight;
                            StartX = 0;
                            StartY = 0;
                            bHasGuideCamera = sxCamera.hasGuideCamera;
                        }
                        catch (System.Exception ex)
                        {
                            sxCamera = null;
                            m_Connected = true;
                            throw new ASCOM.DriverException(SetError("Unable to complete " + MethodBase.GetCurrentMethod().Name + " request: " + ex.ToString()), ex);
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
                }
                catch (ASCOM.DriverException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw new ASCOM.DriverException("Camera::connected set caught an exception: " + ex.ToString(), ex);
                }

                Log.Write("Camera::conneted set ends\n");
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
        public bool CoolerOn
        {
            get
            {
                Log.Write("CoolerOn get: true\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return true;
            }
            set
            {
                Log.Write("CoolerOn set to " + value + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                throw new ASCOM.PropertyNotImplementedException(SetError("CoolerOn is not supported"), true);
            }
        }

        /// <summary>
        /// Returns the present cooler power level, in percent.  Returns zero if CoolerOn is
        /// False.
        /// </summary>
        /// <exception cref=" System.Exception">not supported</exception>
        /// <exception cref=" System.Exception">an error condition such as link failure is present</exception>
        public double CoolerPower
        {
            get 
            {
                Log.Write("CoolerPower get returns hard coded 100\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return 100;
            }
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
                Log.Write("Generic Description get: " + m_Description + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_Description;

            }
            protected set
            {
                Log.Write("Generic Description set: " + value + "\n");
                m_Description = value;
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
                double dRet;

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                try
                {
                    dRet = sxCamera.electronsPerADU;
                    Log.Write("ElectronsPerADU get returns " + dRet + "\n");
                }
                catch (Exception ex)
                {
                    Log.Write("ElectronsPerADU: value not known\n");
                    throw new ASCOM.PropertyNotImplementedException(String.Format("ElectronsPerADU Must throw exception if data unavailable."), false);
                }
                return dRet;
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
                Log.Write("FullWellCapacity get returns " + MaxADU * ElectronsPerADU / (BinX * BinY) + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return MaxADU * ElectronsPerADU / (BinX * BinY);
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
                Log.Write("HasShutter get returns hard coded false\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return false;
            }
        }

        /// <summary>
        /// Returns the current heat sink temperature (called "ambient temperature" by some
        /// manufacturers) in degrees Celsius. Only valid if CanControlTemperature is True.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        public double HeatSinkTemperature
        {
            get
            { 
                Log.Write("HeatSinkTemperature get will throw an exception\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                throw new ASCOM.PropertyNotImplementedException(SetError("HeatSinkTemperature must throw exception if data unavailable"), true);
            }
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
                Log.Write("ImageArray get\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                if (!bImageValid)
                {
                    throw new ASCOM.ValueNotSetException(SetError("The image is not valid."));
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
                Log.Write("ImageArrayVariant get\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                if (!bImageValid)
                {
                    throw new ASCOM.ValueNotSetException(SetError("The image is not valid."));
                }

                try
                {

                    Int32[,] data = (Int32[,])ImageArray;
                    Int32 width = data.GetLength(0);
                    Int32 height = data.GetLength(1);
                    object[,] oReturn = new object[width, height];

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
                    SetError(ex.ToString());
                    throw new ASCOM.ValueNotSetException(MethodBase.GetCurrentMethod().Name, ex);
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
                Log.Write("ImageReady get: bImageValid = " + bImageValid + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return bImageValid;
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
                Log.Write("IsPulseGuiding get: bGuiding = " + bGuiding + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                lock (oGuideStateLock)
                {
                    return bGuiding;
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
                Log.Write("LastError()\n");

                if (!bLastErrorValid)
                {
                    throw new ASCOM.InvalidOperationException(SetError("LastError called when there was no last error"));
                }
                return lastErrorMessage;
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
                Log.Write("LastExposureDuration get: " + actualExposureLength.TotalSeconds + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                if (!bImageValid)
                {
                    throw new ASCOM.ValueNotSetException(SetError("The image is not valid."));
                }

                return actualExposureLength.TotalSeconds;
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
                Log.Write("LastExposureStartTime get" + exposureStart.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff") + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                if (!bImageValid)
                {
                    throw new ASCOM.ValueNotSetException(SetError("The image is not valid."));
                }
 
                return exposureStart.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff");
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
                Log.Write("MaxADU get" + m_MaxADU + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_MaxADU;
            }

            protected set
            {
                m_MaxADU = value;
            }
        }

        /// <summary>
        /// If AsymmetricBinning = False, returns the maximum allowed binning factor. If
        /// AsymmetricBinning = True, returns the maximum allowed binning factor for the X axis.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        public short MaxBinX
        {
            get 
            {
                Log.Write("MaxBinX get" + m_MaxBinX + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_MaxBinX;
            }

            private set
            {
                m_MaxBinX = value;
            }
        }

        /// <summary>
        /// If AsymmetricBinning = False, equals MaxBinX. If AsymmetricBinning = True,
        /// returns the maximum allowed binning factor for the Y axis.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw exception if data unavailable.</exception>
        public short MaxBinY
        {
            get 
            {
                Log.Write("MaxBinY get" + m_MaxBinY + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_MaxBinY;
            }

            private set
            {
                m_MaxBinY = value;
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
                Log.Write("NumX get: " + m_NumX + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_NumX;
            }

            set
            {
                Log.Write("NumX set: " + value + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                m_NumX = value;
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
                Log.Write("NumY get: " + m_NumY + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_NumY;
            }
            set
            {
                Log.Write("NumY set: " + value + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                m_NumY = value;
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
                Log.Write("PixelSizeX get: m_PixelSizeX = " + m_PixelSizeX + "\n"); 

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_PixelSizeX;
            }
            private set
            {
                Log.Write("PixelSizeX set: " + value + "\n"); 
                m_PixelSizeX = value;
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
                Log.Write("PixelSizeX get: m_PixelSizeY = " + m_PixelSizeY + "\n"); 
                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_PixelSizeY;
            }

            private set
            {
                Log.Write("PixelSizeY set: " + value + "\n"); 
                m_PixelSizeY = value;
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
            Log.Write("PulseGuide(" + Direction + "," + Duration + ")\n");
            bLastErrorValid = false;
            
            verifyConnected(MethodBase.GetCurrentMethod().Name);

            if (!CanPulseGuide)
            {
                throw new ASCOM.InvalidOperationException(SetError(String.Format("PulseGuide() cannot be called if CanPuluseGuide == false")));
            }

            try
            {
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
            catch (System.Exception ex)
            {
                SetError(ex.ToString());
                throw new ASCOM.DriverException(SetError("Unable to complete " + MethodBase.GetCurrentMethod().Name + " request"), ex);
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
        public double SetCCDTemperature
        {
            get
            {
                Log.Write("SetCCDTemperature get: will throw an exception\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                throw new ASCOM.PropertyNotImplementedException(String.Format("SetCCDTemperature must throw exception if CanSetCCDTemperature is False."), false);
            }
            set
            {
                Log.Write("SetCCDTemperature set: will throw an exception\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                throw new ASCOM.PropertyNotImplementedException(String.Format("SetCCDTemperature must throw exception if CanSetCCDTemperature is False."), true);
            }
        }

        abstract public void SetupDialog();
 
        internal void hardwareCapture(double Duration, bool Light)
        {
            Log.Write(String.Format("hardwareCapture({0}, {1}): begins\n", Duration, Light));

            try
            {
                exposureStart = DateTime.Now;
                
                lock (oCameraStateLock)
                {
                    if (bAbortRequested)
                    {
                        return;
                    }
                    state = CameraStates.cameraExposing;
                }

                sxCamera.delayMs = (uint)(1000 * Duration);
                sxCamera.recordPixelsDelayed();

                actualExposureLength = DateTime.Now - exposureStart;
                Log.Write(String.Format("hardwareCapture(): exposure + download took {0}, requested {1}\n", actualExposureLength.TotalSeconds, Duration));
                actualExposureLength = new TimeSpan(0, 0, 0, 0, (int)(1000*Duration));

                Log.Write("hardwareCapture(): ends successfully\n");

                bImageValid = true;
            }
            catch (System.Exception ex)
            {
                throw new ASCOM.DriverException(SetError("Unable to complete " + MethodBase.GetCurrentMethod().Name + " request"), ex);
            }
            finally
            {
                lock (oCameraStateLock)
                {
                    state = CameraStates.cameraIdle;
                }
            }
        }

        internal void softwareCapture(double Duration, bool Light)
        {
            Log.Write(String.Format("softwareCapture({0}, {1}): begins\n", Duration, Light));

            try
            {
                sxCamera.clearCcdPixels(); // This clears both the CCD and the recorded pixels.  For
                                           // exposures > 1 second we will clear the recorded pixels again just before
                                           // the exposure ends to clear any accumulated noise.
                bool bRecordedCleared = false;

                if (Duration > ImageCommandTime)
                {
                    Duration -= ImageCommandTime;
                }
                else
                {
                    Duration = 0;
                }
                desiredExposureLength = TimeSpan.FromSeconds(Duration);

                if (desiredExposureLength.TotalSeconds < 1.0)
                {
                    bRecordedCleared = true; // we don't do the clear again inside the loop for short exposures
                    sxCamera.echo("done"); // the clear takes a long time - send something to the camera so we know it is done
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
                    
                    if (remainingExposureTime.TotalSeconds < 1.0 && !bRecordedCleared)
                    {
                        Log.Write("softwareCapture(): doing clearRecordedPixels() inside of loop, remaining exposure=" + remainingExposureTime.TotalSeconds + "\n");
                        sxCamera.clearRecordedPixels();
                        bRecordedCleared = true;
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

                sxCamera.recordPixels(out exposureEnd);

                actualExposureLength = exposureEnd - exposureStart;

                Log.Write(String.Format("softwareCapture(): delay ends, actualExposureLength={0}, requested={1}\n", actualExposureLength.TotalSeconds, Duration));
                
                bImageValid = true;
            }
            catch (System.Exception ex)
            {
                throw new ASCOM.DriverException(SetError(String.Format("Unable to complete {0} request - ex = {1}\n", MethodBase.GetCurrentMethod().Name, ex.ToString())), ex);
            }
            finally
            {
                lock (oCameraStateLock)
                {
                    state = CameraStates.cameraIdle;
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
            Log.Write(String.Format("StartExposure({0}, {1}) begins\n", Duration, Light));

#if False
            Duration /= 1000;
#endif

            // because of timing accuracy, we do all short exposures with the HW timer
            if (Duration <= 1.0)
            {
                StartExposure(Duration, Light, true);
            }
            else
            {
                StartExposure(Duration, Light, false);
            }
        }

        protected void StartExposure(double Duration, bool Light, bool useHardwareTimer)
        {
            Log.Write(String.Format("StartExposure({0}, {1}, {2}) begins\n", Duration, Light, useHardwareTimer));
            bLastErrorValid = false;

            verifyConnected(MethodBase.GetCurrentMethod().Name);

            if (Duration < 0)
            {
                SetError("Exposure duration < 0");
                throw new ASCOM.InvalidValueException(MethodBase.GetCurrentMethod().Name, Duration.ToString(), ">= 0");
            }
            
            lock (oCameraStateLock)
            {
                if (state != CameraStates.cameraIdle)
                {
                    throw new ASCOM.InvalidOperationException(SetError(String.Format("StartExposure called while in state {0}", state)));
                }
                state = CameraStates.cameraExposing;
                bAbortRequested = false;
                bStopRequested = false;
                bImageValid = false;
            }

            try
            {
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

                if (useHardwareTimer)
                {
                    captureDelegate = new CaptureDelegate(hardwareCapture);
                }
                else
                {
                    captureDelegate = new CaptureDelegate(softwareCapture);
                }

                Log.Write("StartExposure() before captureDelegate.BeginInvode()\n");

                captureDelegate.BeginInvoke(Duration, Light, null, null);

                Log.Write("StartExposure() after captureDelegate.BeginInvode()\n");
            }
            catch (System.Exception ex)
            {
                lock (oCameraStateLock)
                {
                    state = CameraStates.cameraIdle;
                }

                throw new ASCOM.DriverException(SetError("Unable to complete " + MethodBase.GetCurrentMethod().Name + " request"), ex);
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
                Log.Write("StartX get: m_StartX = " + m_StartX + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_StartX;
            }

            set
            {
                Log.Write("StartX set: " + value + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                m_StartX = value;
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
                Log.Write("StartY get: m_StartY = " + m_StartY + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_StartY;
            }

            set
            {
                Log.Write("StartY set: " + value + "\n");

                verifyConnected(MethodBase.GetCurrentMethod().Name);

                m_StartY = value;
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
            Log.Write("StopExposure() requested when in state " + state + "\n");
            bLastErrorValid = false;

            verifyConnected(MethodBase.GetCurrentMethod().Name);

            lock (oCameraStateLock)
            {
                switch (state)
                {
                    case CameraStates.cameraExposing:
                    case CameraStates.cameraDownload:
                        bStopRequested = true;
                        break;
                    default:
                        break;
                        if (bStopRequested)
                            break; // they asked when it was legal and are just asking again.
                        throw new ASCOM.InvalidOperationException(String.Format("Stop not possible when camera is in state {0}", state));
                }
            }
        }

        #endregion
    }
}
