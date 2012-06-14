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

using System;
using System.Windows.Forms;

using ASCOM.Utilities;

using Logging;

namespace ASCOM.SXCamera
{
    public class Configuration
    {
        private const String DEVICE_TYPE = "Camera";

        private const String KEY_ENABLE_UNTESTED = "EnableUntested";
#if DEBUG
        private const bool DEFAULT_ENABLE_UNTESTED = true;
#else
        private const bool   DEFAULT_ENABLE_UNTESTED = false;
#endif

        private const String KEY_ENABLE_LOGGING = "EnableLogging";
#if DEBUG
        private const bool   DEFAULT_ENABLE_LOGGING = true;
#else
        private const bool   DEFAULT_ENABLE_LOGGING = false;
#endif

        private const String KEY_USE_DUMPED_DATA = "UseDumpedData";

        private const String KEY_DUMP_DATA_ENABLED = "DumpDataEnabled";
        private const bool DEFAULT_DUMP_DATA_ENABLED = false;

        private const string KEY_SELECTION_METHOD = "Selection";
        private const string KEY_VID = "VID";
        private const string KEY_PID = "PID";

        private const string KEY_ASYMETRIC_BINNING = "AsymetricBinning";
        private const bool DEFAULT_ASYMETRIC_BINNING = false;

        private const string KEY_MAX_Y_BIN = "MaxYBin";
#if DEBUG
        private const byte DEFAULT_MAX_Y_BIN = 8;
#else
        private const byte DEFAULT_MAX_Y_BIN = 4;
#endif

        private const string KEY_MAX_X_BIN = "MaxXBin";
#if DEBUG
        private const byte DEFAULT_MAX_X_BIN = 8;
#else
        private const byte DEFAULT_MAX_X_BIN = 4;
#endif

        private const string KEY_FIXED_BINNING = "FixedBinning";
        private const bool DEFAULT_FIXED_BINNING = false;

        private const string KEY_FIXED_BIN = "FixedBin";
        private const byte DEFAULT_FIXED_BIN = 1;

        private const string KEY_INTERLACED_EQUALIZE_FRAMES = "InterlacedEqualizedFrames";
        private const bool DEFAULT_INTERLACED_EQUALIZE_FRAMES = true;

        private const string   KEY_SQUARE_LODESTAR_PIXELS = "SquareLodestarPixels";
        private const bool DEFAULT_SQUARE_LODESTAR_PIXELS = false;

        private const string KEY_INTERLACED_DOUBLE_EXPOSE_SHORT_EXPOSURES = "InterlacedDoubleExposeShortExposures";
        private const bool DEFAULT_INTERLACED_DOUBLE_EXPOSE_SHORT_EXPOSURES = true;

        private const string KEY_INTERLACED_DOUBLE_EXPOSURE_THRESHOLD = "InterlacedDoubleExposeThreshold";
        private const UInt16 DEFAULT_INTERLACED_DOUBLE_EXPOSURE_THRESHOLD = 334;

        private const string KEY_INTERLACED_GAUSSIAN_BLUR = "InterlacedGaussianBlur";
        private const bool DEFAULT_INTERLACED_GAUSSIAN_BLUR = false;

        private const string KEY_INTERLACED_GAUSSIAN_BLUR_RADIUS = "InterlacedGaussianBlurRadius";
        private const double DEFAULT_INTERLACED_GAUSSIAN_BLUR_RADIUS = 1.0;

        public enum CAMERA_SELECTION_METHOD
        {
            CAMERA_SELECTION_ANY,
            CAMERA_SELECTION_EXACT_MODEL,
            CAMERA_SELECTION_EXCLUDE_MODEL
        };

        internal struct CAMERA_VALUES
        {
            public bool enableUntested;
            public bool enableLogging;
            public bool dumpDataEnabled;
            public bool useDumpedData;

            public string selectionMethod;
            public UInt16 VID;
            public UInt16 PID;

            public bool asymetricBinning;
            public byte maxXBin;
            public byte maxYBin;

            public bool fixedBinning;
            public byte fixedBin;

            public bool interlacedEqualizeFrames;
            public bool squareLodestarPixels;

            public bool interlacedDoubleExposeShortExposures;
            public UInt16 interlacedDoubleExposureThreshold;

            public bool interlacedGaussianBlur;
            public double interlacedGaussianBlurRadius;

            internal CAMERA_VALUES(
                            bool enableUntested,
                            bool enableLogging,
                            bool dumpDataEnabled,
                            string selectionMethod, UInt16 VID, UInt16 PID,
                            bool asymetricBinning, byte maxXBin, byte maxYBin,
                            bool fixedBinning, byte fixedBin,
                            bool interlacedEqualizeFrames,
                            bool squareLodestarPixels,
                            bool interlacedDoubleExposeShortExposures, UInt16 interlacedDoubleExposureThreshold,
                            bool interlacedGaussianBlur, double interlacedGaussianBlurRadius
                            )
            {
                this.enableUntested = enableUntested;
                this.enableLogging = enableLogging;
                this.dumpDataEnabled = dumpDataEnabled;
                this.useDumpedData = false; // defaults to false for all cameras

                this.selectionMethod = selectionMethod;
                this.VID = VID;
                this.PID = PID;

                this.asymetricBinning = asymetricBinning;
                this.maxXBin = maxXBin;
                this.maxYBin = maxYBin;

                this.fixedBinning = fixedBinning;
                this.fixedBin = fixedBin;

                this.interlacedEqualizeFrames = interlacedEqualizeFrames;
                this.squareLodestarPixels = squareLodestarPixels;

                this.interlacedDoubleExposeShortExposures = interlacedDoubleExposeShortExposures;
                this.interlacedDoubleExposureThreshold = interlacedDoubleExposureThreshold;

                this.interlacedGaussianBlur = interlacedGaussianBlur;
                this.interlacedGaussianBlurRadius = interlacedGaussianBlurRadius;
            }
        };

        internal CAMERA_VALUES[] DEFAULT_VALUES =
        {
            // main cameras
            new CAMERA_VALUES(
                        DEFAULT_ENABLE_UNTESTED, 
                        DEFAULT_ENABLE_LOGGING, 
                        DEFAULT_DUMP_DATA_ENABLED,
                        Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL), 1278, 0xffff,
                        DEFAULT_ASYMETRIC_BINNING, DEFAULT_MAX_X_BIN, DEFAULT_MAX_Y_BIN,
                        DEFAULT_FIXED_BINNING, DEFAULT_FIXED_BIN,
                        DEFAULT_INTERLACED_EQUALIZE_FRAMES,
                        DEFAULT_SQUARE_LODESTAR_PIXELS,
                        DEFAULT_INTERLACED_DOUBLE_EXPOSE_SHORT_EXPOSURES, DEFAULT_INTERLACED_DOUBLE_EXPOSURE_THRESHOLD,
                        DEFAULT_INTERLACED_GAUSSIAN_BLUR, DEFAULT_INTERLACED_GAUSSIAN_BLUR_RADIUS
                    ),
            new CAMERA_VALUES(
                        DEFAULT_ENABLE_UNTESTED, 
                        DEFAULT_ENABLE_LOGGING, 
                        DEFAULT_DUMP_DATA_ENABLED,
                        Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL), 1278, 0xffff,
                        DEFAULT_ASYMETRIC_BINNING, DEFAULT_MAX_X_BIN, DEFAULT_MAX_Y_BIN,
                        DEFAULT_FIXED_BINNING, DEFAULT_FIXED_BIN,
                        DEFAULT_INTERLACED_EQUALIZE_FRAMES,
                        DEFAULT_SQUARE_LODESTAR_PIXELS,
                        DEFAULT_INTERLACED_DOUBLE_EXPOSE_SHORT_EXPOSURES, DEFAULT_INTERLACED_DOUBLE_EXPOSURE_THRESHOLD,
                        DEFAULT_INTERLACED_GAUSSIAN_BLUR, DEFAULT_INTERLACED_GAUSSIAN_BLUR_RADIUS
                    ),
            // lodestars
            new CAMERA_VALUES(
                        DEFAULT_ENABLE_UNTESTED, 
                        DEFAULT_ENABLE_LOGGING, 
                        DEFAULT_DUMP_DATA_ENABLED,
                        Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL), 1278, 507,
                        DEFAULT_ASYMETRIC_BINNING, DEFAULT_MAX_X_BIN, DEFAULT_MAX_Y_BIN,
                        DEFAULT_FIXED_BINNING, DEFAULT_FIXED_BIN,
                        DEFAULT_INTERLACED_EQUALIZE_FRAMES,
                        DEFAULT_SQUARE_LODESTAR_PIXELS,
                        true, DEFAULT_INTERLACED_DOUBLE_EXPOSURE_THRESHOLD,
                        true, 1.0
                    ),
            new CAMERA_VALUES(
                        DEFAULT_ENABLE_UNTESTED, 
                        DEFAULT_ENABLE_LOGGING, 
                        DEFAULT_DUMP_DATA_ENABLED,
                        Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL), 1278, 507,
                        DEFAULT_ASYMETRIC_BINNING, DEFAULT_MAX_X_BIN, DEFAULT_MAX_Y_BIN,
                        DEFAULT_FIXED_BINNING, DEFAULT_FIXED_BIN,
                        DEFAULT_INTERLACED_EQUALIZE_FRAMES,
                        DEFAULT_SQUARE_LODESTAR_PIXELS,
                        true, DEFAULT_INTERLACED_DOUBLE_EXPOSURE_THRESHOLD,
                        true, 1.0
                    ),
            // costars
            new CAMERA_VALUES(
                        DEFAULT_ENABLE_UNTESTED, 
                        DEFAULT_ENABLE_LOGGING, 
                        DEFAULT_DUMP_DATA_ENABLED,
                        Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL), 1278, 517,
                        DEFAULT_ASYMETRIC_BINNING, 1, 1,
                        DEFAULT_FIXED_BINNING, DEFAULT_FIXED_BIN,
                        DEFAULT_INTERLACED_EQUALIZE_FRAMES,
                        DEFAULT_SQUARE_LODESTAR_PIXELS,
                        DEFAULT_INTERLACED_DOUBLE_EXPOSE_SHORT_EXPOSURES, DEFAULT_INTERLACED_DOUBLE_EXPOSURE_THRESHOLD,
                        DEFAULT_INTERLACED_GAUSSIAN_BLUR, DEFAULT_INTERLACED_GAUSSIAN_BLUR_RADIUS
                    ),
            new CAMERA_VALUES(
                        DEFAULT_ENABLE_UNTESTED, 
                        DEFAULT_ENABLE_LOGGING, 
                        DEFAULT_DUMP_DATA_ENABLED,
                        Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL), 1278, 517,
                        DEFAULT_ASYMETRIC_BINNING, 1, 1,
                        DEFAULT_FIXED_BINNING, DEFAULT_FIXED_BIN,
                        DEFAULT_INTERLACED_EQUALIZE_FRAMES,
                        DEFAULT_SQUARE_LODESTAR_PIXELS,
                        DEFAULT_INTERLACED_DOUBLE_EXPOSE_SHORT_EXPOSURES, DEFAULT_INTERLACED_DOUBLE_EXPOSURE_THRESHOLD,
                        DEFAULT_INTERLACED_GAUSSIAN_BLUR, DEFAULT_INTERLACED_GAUSSIAN_BLUR_RADIUS
                    ),
        };

        private ASCOM.Utilities.Profile m_profile;
        private string m_driverId;
        private UInt16 m_whichController,
                       m_whichCamera;
       
        public Configuration(UInt16 whichController, UInt16 whichCamera)
        {
            Log.Write(String.Format("Configuration({0}, {1}) starts\n", whichController, whichCamera));

            m_profile = new ASCOM.Utilities.Profile();
            m_profile.DeviceType = DEVICE_TYPE;

            // Note that this picks main camera configuraitons for guide cameras too -
            // there are currently no configuration values for guide cameras
           
            m_driverId = String.Format("ASCOM.SXMain{0}.{1}", whichController, DEVICE_TYPE);

            Log.Write(String.Format("Configuration() computes driverId={0}\n", m_driverId));

            m_whichController = whichController;
            m_whichCamera = whichCamera;

            if (enableLogging && !Log.enabled)
            {
                Log.enabled = true;
                Log.Write("Configuration() has enabled logging\n");
            }

        }

        internal string GetString(string name)
        {
            string ret = null;

            try
            {
                ret = m_profile.GetValue(m_driverId, name);
            }
            catch (Exception ex)
            {
                Log.Write(String.Format("config: GetValue raised an exception: {0}\n", ex.ToString()));
            }

            return ret;
        }

        internal string GetString(string name, string defaultValue)
        {
            string ret = GetString(name);

            Log.Write(String.Format("GetString({0}, {1}) ret = [{2}]\n", name, defaultValue, ret));

            if (ret == null || ret=="")
            {
                ret = defaultValue;
            }

            return ret;
        }

        internal void SetString(string name, string value)
        {
            Log.Write(String.Format("config: writing {0}={1} to driver {2}\n", name, value, m_driverId));

            try
            {
               m_profile.WriteValue(m_driverId, name, value);
            }
            catch (Exception ex)
            {
                Log.Write(String.Format("config: WriteValue raised an exception: {0}\n", ex.ToString()));
            }
        }

        internal bool GetBool(string name, bool defaultValue)
        {
            bool bRet = defaultValue;
            string str = GetString(name);

            if (str != null && str != "")
            {
                try
                {
                    bRet = Convert.ToBoolean(str);
                }
                catch (FormatException ex)
                {
                    Log.Write(String.Format("GetBool was unable to convert {0} - caught exception {1}\n", str, ex));
                }
            }
            return bRet;
        }

        internal UInt16 GetUInt16(string name, UInt16 defaultValue)
        {
            UInt16 iRet = defaultValue;
            string str = GetString(name);

            if (str != null && str != "")
            {
                try
                {
                    iRet = Convert.ToUInt16(str);
                }
                catch (FormatException ex)
                {
                    Log.Write(String.Format("GetUInt16 was unable to convert {0} -- caught exception {1}\n", str, ex));
                }
            }

            return iRet;
        }

        internal byte GetByte(string name, byte defaultValue)
        {
            byte iRet = defaultValue;
            string str = GetString(name);

            if (str != null && str != "")
            {
                try
                {
                    iRet = Convert.ToByte(str);
                }
                catch (FormatException ex)
                {
                    Log.Write(String.Format("GetByte was unable to convert {0} -- caught exceptin {1}\n", str, ex));
                }
            }

            return iRet;
        }

        internal double GetDouble(string name, double defaultValue)
        {
            double dRet = defaultValue;
            string str = GetString(name);

            if (str != null && str != "")
            {
                try
                {
                    dRet = Convert.ToDouble(str);
                }
                catch (FormatException ex)
                {
                    Log.Write(String.Format("GetDouble was unable to convert {0} -- caught exceptin {1}\n", str, ex));
                }
            }

            return dRet;
        }

        public bool enableUntested
        {
           
            get { return GetBool(KEY_ENABLE_UNTESTED, DEFAULT_VALUES[m_whichController].enableUntested);}
            set { SetString(     KEY_ENABLE_UNTESTED, value.ToString());}
        }

        public bool enableLogging
        {
            get { return GetBool(KEY_ENABLE_LOGGING, DEFAULT_VALUES[m_whichController].enableLogging); }
            set { SetString(     KEY_ENABLE_LOGGING, value.ToString()); }
        }

        public bool bUseDumpedData
        {
            get { return GetBool(KEY_USE_DUMPED_DATA, DEFAULT_VALUES[m_whichController].useDumpedData); }
            set { SetString(     KEY_USE_DUMPED_DATA, value.ToString()); }
        }

        public bool bDumpData
        {
            get { return GetBool(KEY_DUMP_DATA_ENABLED, DEFAULT_VALUES[m_whichController].dumpDataEnabled); }
            set { SetString(     KEY_DUMP_DATA_ENABLED, value.ToString()); }
        }

        public CAMERA_SELECTION_METHOD selectionMethod
        {
            get
            {
                String selection = GetString(KEY_SELECTION_METHOD, DEFAULT_VALUES[m_whichController].selectionMethod);
                return (CAMERA_SELECTION_METHOD)Enum.Parse(typeof(CAMERA_SELECTION_METHOD), selection, true);
            }
            set
            {
                String selection = Enum.GetName(typeof(CAMERA_SELECTION_METHOD), value);
                SetString(KEY_SELECTION_METHOD, selection);
            }
        }

        public UInt16 VID
        {
            get { return GetUInt16(KEY_VID, DEFAULT_VALUES[m_whichController].VID); }
            set { SetString(       KEY_VID, value.ToString()); }
        }

        public UInt16 PID
        {
            get { return GetUInt16(KEY_PID, DEFAULT_VALUES[m_whichController].PID); }
            set { SetString(       KEY_PID, value.ToString()); }
        }  

        public String description
        {
            get { return GetString("", "default"); }
        }

        public bool asymetricBinning
        {
            get { return GetBool(KEY_ASYMETRIC_BINNING, DEFAULT_VALUES[m_whichController].asymetricBinning); }
            set { SetString(     KEY_ASYMETRIC_BINNING, value.ToString()); }
        }

        public byte maxXBin
        {
            get { return GetByte(KEY_MAX_X_BIN, DEFAULT_VALUES[m_whichController].maxXBin); }
            set { SetString(     KEY_MAX_X_BIN, value.ToString()); }
        }

        public byte maxYBin
        {
            get { return GetByte(KEY_MAX_Y_BIN, DEFAULT_VALUES[m_whichController].maxYBin); }
            set { SetString(     KEY_MAX_Y_BIN, value.ToString()); }
        }

        public bool fixedBinning
        {
            get { return GetBool(KEY_FIXED_BINNING, DEFAULT_VALUES[m_whichController].fixedBinning); }
            set { SetString(     KEY_FIXED_BINNING, value.ToString()); }
        }

        public byte fixedBin
        {
            get { return GetByte(KEY_FIXED_BIN, DEFAULT_VALUES[m_whichController].fixedBin); }
            set { SetString(     KEY_FIXED_BIN, value.ToString()); }
        }

        public bool interlacedEqualizeFrames
        {
            get { return GetBool(KEY_INTERLACED_EQUALIZE_FRAMES, DEFAULT_VALUES[m_whichController].interlacedEqualizeFrames); }
            set { SetString(     KEY_INTERLACED_EQUALIZE_FRAMES, value.ToString()); }
        }

        public bool squareLodestarPixels
        {
            get { return GetBool(KEY_SQUARE_LODESTAR_PIXELS, DEFAULT_VALUES[m_whichController].squareLodestarPixels); }
            set { SetString(     KEY_SQUARE_LODESTAR_PIXELS, value.ToString()); }
        }

        public bool interlacedDoubleExposeShortExposures
        {
            get { return GetBool(KEY_INTERLACED_DOUBLE_EXPOSE_SHORT_EXPOSURES, DEFAULT_VALUES[m_whichController].interlacedDoubleExposeShortExposures); }
            set { SetString(     KEY_INTERLACED_DOUBLE_EXPOSE_SHORT_EXPOSURES, value.ToString()); }
        }

        public UInt16 interlacedDoubleExposureThreshold
        {
            get { return GetUInt16(KEY_INTERLACED_DOUBLE_EXPOSURE_THRESHOLD, DEFAULT_VALUES[m_whichController].interlacedDoubleExposureThreshold); }
            set { SetString(       KEY_INTERLACED_DOUBLE_EXPOSURE_THRESHOLD, value.ToString()); }
        }  

        public bool interlacedGaussianBlur
        {
            get { return GetBool(KEY_INTERLACED_GAUSSIAN_BLUR, DEFAULT_VALUES[m_whichController].interlacedGaussianBlur); }
            set { SetString(     KEY_INTERLACED_GAUSSIAN_BLUR, value.ToString()); }
        }

        public double interlacedGaussianBlurRadius
        {
            get { return GetDouble(KEY_INTERLACED_GAUSSIAN_BLUR_RADIUS, DEFAULT_VALUES[m_whichController].interlacedGaussianBlurRadius); }
            set { SetString(       KEY_INTERLACED_GAUSSIAN_BLUR_RADIUS, value.ToString()); }
        }

        /// <summary>
        /// Launches a configuration dialog box for the driver.  The call will not return
        /// until the user clicks OK or cancel manually.
        /// </summary>
        /// <exception cref=" System.Exception">Must throw an exception if Setup dialog is unavailable.</exception>
        public void SetupDialog()
        {
            switch (m_whichCamera)
            {
                case 0:
                    mainCameraSetupDialog();
                    break;
                case 1:
                    guideCameraSetupDialog();
                    break;
                default:
                    throw new System.Exception(String.Format("Unknown cameraID {0} in SetupDialog", m_whichCamera));
            }
        }

        private void guideCameraSetupDialog()
        {
            MessageBox.Show(String.Format("There are no configurable settings for guide cameras.  All configuration is done from the main camera's configuration screen."));
        }

        private void mainCameraSetupDialog()
        {
            try
            {
                Log.Write(String.Format("SetupDialog, description = {0}\n", description));

                SetupDialogForm F = new SetupDialogForm();

                F.Version.Text = String.Format("Version: {0}", SharedResources.versionNumber);

                F.EnableLoggingCheckBox.Checked = enableLogging;

                F.EnableUntestedCheckBox.Checked = enableUntested;

                F.dumpDataEnabled.Checked = bDumpData;

                F.useDumpedData.Checked = bUseDumpedData;

                // some cameras cannot bin.  
                // If this camera can't bin, disble the binGroup so binning cannot be modified.
                if (maxXBin > 1)
                {
                    F.binGroup.Enabled = true;
                }
                else
                {
                    F.binGroup.Enabled = false;
                }

                if (asymetricBinning)
                {
                    F.asymetricBinning.Checked = true;
                    F.binLabel.Text = "Max Y Bin";
                    F.xBinLabel.Visible = true;
                    F.maxXBin.Visible = true;
                }
                else
                {
                    F.asymetricBinning.Checked = false;
                    F.binLabel.Text = "Max Bin";
                    F.xBinLabel.Visible = false;
                    F.maxXBin.Visible = false;
                }

                F.maxXBin.Value  = maxXBin;
                F.maxYBin.Value  = maxYBin;

                F.fixedBinning.Checked = fixedBinning;
                F.fixedBin.Enabled = F.fixedBinning.Checked;
                F.fixedBin.Value = fixedBin;

                // interlaced box
                F.equalizeFrames.Checked = interlacedEqualizeFrames;
                F.squareLodestarPixels.Checked = squareLodestarPixels;

                F.doubleExposeShort.Checked = interlacedDoubleExposeShortExposures;
                F.doubleExposureThreshold.Enabled = F.doubleExposeShort.Checked;
                F.doubleExposureThreshold.Value = interlacedDoubleExposureThreshold;

                F.gaussianBlur.Checked = interlacedGaussianBlur;
                F.gaussianBlurRadius.Enabled = F.gaussianBlur.Checked;
                F.gaussianBlurRadius.Value = (decimal)interlacedGaussianBlurRadius;

                // advanced USB box
                F.selectionAllowAny.Checked = false;
                F.selectionExactModel.Checked = false;
                F.selectionExcludeModel.Checked = false;
                F.VID.Text = VID.ToString();
                F.PID.Text = PID.ToString();

                F.vidLabel.Visible = true;
                F.pidLabel.Visible = true;
                F.VID.Visible = true;
                F.PID.Visible = true;

                switch (selectionMethod)
                {
                    case CAMERA_SELECTION_METHOD.CAMERA_SELECTION_ANY:
                        F.selectionAllowAny.Checked = true;
                        F.vidLabel.Visible = false;
                        F.pidLabel.Visible = false;
                        F.VID.Visible = false;
                        F.PID.Visible = false;
                        break;
                    case CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL:
                        F.selectionExactModel.Checked = true;
                        break;
                    case CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL:
                        F.selectionExcludeModel.Checked = true;
                        break;
                    default:
                        throw new System.Exception(String.Format("Unknown Camera Selection Method {0} in SetupDialog", selectionMethod));
                }

                F.advancedUSBParmsEnabled.Checked = false;
                F.usbGroup.Enabled = false;

                if (F.ShowDialog() == DialogResult.OK)
                {
                    Log.Write("ShowDialog returned OK - saving parameters\n");

                    enableLogging = F.EnableLoggingCheckBox.Checked;
                    enableUntested = F.EnableUntestedCheckBox.Checked;
                    bDumpData = F.dumpDataEnabled.Checked;
                    bUseDumpedData = F.useDumpedData.Checked;

                    // interlaced box
                    interlacedEqualizeFrames = F.equalizeFrames.Checked;
                    squareLodestarPixels = F.squareLodestarPixels.Checked;

                    interlacedDoubleExposeShortExposures = F.doubleExposeShort.Checked;
                    if (interlacedDoubleExposeShortExposures)
                    {
                        interlacedDoubleExposureThreshold = (UInt16)F.doubleExposureThreshold.Value;
                    }

                    interlacedGaussianBlur = F.gaussianBlur.Checked;
                    if (interlacedGaussianBlur)
                    {
                        interlacedGaussianBlurRadius = (double)F.gaussianBlurRadius.Value;
                    }

                    // binning box

                    fixedBinning = F.fixedBinning.Checked;

                    if (fixedBinning)
                    {
                        fixedBin = (byte)F.fixedBin.Value;
                    }
                    
                    asymetricBinning = F.asymetricBinning.Checked;
                    maxYBin = (byte)F.maxYBin.Value;

                    if (asymetricBinning)
                    {
                        maxXBin = maxYBin;
                    }
                    else
                    {
                        maxXBin = (byte)F.maxXBin.Value;
                    }

                    // advanced usp box
                    if (F.selectionAllowAny.Checked)
                    {
                        selectionMethod = Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_ANY;
                    }
                    else
                    {
                        bool error = false;
                        try
                        {
                            VID = Convert.ToUInt16(F.VID.Text);
                        }
                        catch (System.FormatException ex)
                        {
                            error = true;
                            Log.Write(String.Format("Caught an exception converting VID [{0}] to UInt16: {1}", F.VID.Text, ex.ToString()));
                            MessageBox.Show("An invalid VID was entered.  Value was not changed");
                        }

                        try
                        {
                            PID = Convert.ToUInt16(F.PID.Text);
                        }
                        catch (System.FormatException ex)
                        {
                            error = true;
                            Log.Write(String.Format("Caught an exception converting PID [{0}] to UInt16: {1}", F.PID.Text, ex.ToString()));
                            MessageBox.Show("An invalid PID was entered.  Value was not changed");
                        }

                        if (!error)
                        {
                            if (F.selectionExactModel.Checked)
                            {
                                selectionMethod = Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL;
                            }
                            else
                            {
                                selectionMethod = Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL;
                            }
                        }
                    }
                }
            }
            catch (ASCOM.DriverException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                Log.Write(String.Format("Unable to complete SetupDialog request - ex = {0}\n", ex.ToString()));
                throw ex;
            }
        }
    }
}
