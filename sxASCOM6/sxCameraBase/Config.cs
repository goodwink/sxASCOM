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

namespace ASCOM.sxCameraBase
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

        private const string KEY_LOG_FILE_NAME = "LogFileName";
        private string DEFAULT_LOG_FILE_NAME = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "ascom-sx-camera.log";

        private const string KEY_SECONDS_ARE_MILLISECONDS = "SecondsAreMilliseconds";
        private const bool DEFAULT_SECONDS_ARE_MILLISECONDS = false;

        private const string KEY_SELECTION_METHOD = "Selection";
        private const string KEY_VID = "VID";
        private const string KEY_PID = "PID";

        private const string KEY_SYMETRIC_BINNING = "SymetricBinning";
        private const bool DEFAULT_SYMETRIC_BINNING = true;

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

        private const String KEY_DUMP_DATA_ENABLED = "DumpDataEnabled";
        private const bool DEFAULT_DUMP_DATA_ENABLED = false;

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
            public bool secondsAreMilliseconds;
            public bool dumpDataEnabled;
            public string selectionMethod;
            public UInt16 VID;
            public UInt16 PID;
            public bool symetricBinning;
            public byte maxXBin;
            public byte maxYBin;

            internal CAMERA_VALUES(bool enableUntested, bool enableLogging, bool secondsAreMilliseconds, string selectionMethod, UInt16 VID, UInt16 PID, bool symetricBinning, byte maxXBin, byte maxYBin, bool dumpDataEnabled)
            {
                this.enableUntested = enableUntested;
                this.enableLogging = enableLogging;
                this.secondsAreMilliseconds = secondsAreMilliseconds;
                this.selectionMethod = selectionMethod;
                this.VID = VID;
                this.PID = PID;
                this.symetricBinning = symetricBinning;
                this.maxXBin = maxXBin;
                this.maxYBin = maxYBin;
                this.dumpDataEnabled = dumpDataEnabled;
            }
        };

        internal CAMERA_VALUES[] DEFAULT_VALUES = 
        {
            new CAMERA_VALUES(DEFAULT_ENABLE_UNTESTED, DEFAULT_ENABLE_LOGGING, DEFAULT_SECONDS_ARE_MILLISECONDS, Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL), 1278, 0xffff, DEFAULT_SYMETRIC_BINNING, DEFAULT_MAX_X_BIN, DEFAULT_MAX_Y_BIN, DEFAULT_DUMP_DATA_ENABLED),
            new CAMERA_VALUES(DEFAULT_ENABLE_UNTESTED, DEFAULT_ENABLE_LOGGING, DEFAULT_SECONDS_ARE_MILLISECONDS, Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL), 1278, 0xffff, DEFAULT_SYMETRIC_BINNING, DEFAULT_MAX_X_BIN, DEFAULT_MAX_Y_BIN, DEFAULT_DUMP_DATA_ENABLED),
            new CAMERA_VALUES(DEFAULT_ENABLE_UNTESTED, DEFAULT_ENABLE_LOGGING, DEFAULT_SECONDS_ARE_MILLISECONDS, Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL), 1278, 507, DEFAULT_SYMETRIC_BINNING, DEFAULT_MAX_X_BIN, DEFAULT_MAX_Y_BIN, DEFAULT_DUMP_DATA_ENABLED),
            new CAMERA_VALUES(DEFAULT_ENABLE_UNTESTED, DEFAULT_ENABLE_LOGGING, DEFAULT_SECONDS_ARE_MILLISECONDS, Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL), 1278, 507, DEFAULT_SYMETRIC_BINNING, DEFAULT_MAX_X_BIN, DEFAULT_MAX_Y_BIN, DEFAULT_DUMP_DATA_ENABLED),
            new CAMERA_VALUES(DEFAULT_ENABLE_UNTESTED, DEFAULT_ENABLE_LOGGING, DEFAULT_SECONDS_ARE_MILLISECONDS, Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL), 1278, 517, DEFAULT_SYMETRIC_BINNING, 1, 1, DEFAULT_DUMP_DATA_ENABLED),
            new CAMERA_VALUES(DEFAULT_ENABLE_UNTESTED, DEFAULT_ENABLE_LOGGING, DEFAULT_SECONDS_ARE_MILLISECONDS, Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL), 1278, 517, DEFAULT_SYMETRIC_BINNING, 1, 1, DEFAULT_DUMP_DATA_ENABLED),
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
            
            m_driverId = "ASCOM.SXMain" + whichController.ToString() + "." + DEVICE_TYPE;

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
                catch (OverflowException ex)
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
                catch (OverflowException ex)
                {
                    Log.Write(String.Format("GetUInt16 was unable to convert {0} -- caught exception {1}\n", str, ex));
                }
            }

            return iRet;
        }

        public bool enableUntested
        {
            
            get { return GetBool(KEY_ENABLE_UNTESTED, DEFAULT_VALUES[m_whichController].enableUntested);}
            set { SetString(KEY_ENABLE_UNTESTED, value.ToString());}
        }

        public bool enableLogging
        {
            get { return GetBool(KEY_ENABLE_LOGGING, DEFAULT_VALUES[m_whichController].enableLogging); }
            set { SetString(KEY_ENABLE_LOGGING, value.ToString()); }
        }

        public bool secondsAreMilliseconds
        {
            get { return GetBool(KEY_SECONDS_ARE_MILLISECONDS, DEFAULT_VALUES[m_whichController].secondsAreMilliseconds); }
            set { SetString(KEY_SECONDS_ARE_MILLISECONDS, value.ToString()); }
        }

        public bool dumpDataEnabled
        {
            get { return GetBool(KEY_DUMP_DATA_ENABLED, DEFAULT_VALUES[m_whichController].dumpDataEnabled); }
            set { SetString(KEY_DUMP_DATA_ENABLED, value.ToString()); }
        }

        public string logFileName
        {
            get { return GetString(KEY_LOG_FILE_NAME, DEFAULT_LOG_FILE_NAME); }
            set { SetString(KEY_LOG_FILE_NAME, value); }
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
            set { SetString(KEY_VID, value.ToString()); }
        }

        public UInt16 PID
        {
            get { return GetUInt16(KEY_PID, DEFAULT_VALUES[m_whichController].PID); }
            set { SetString(KEY_PID, value.ToString()); }
        }   

        public String description
        {
            get { return GetString("", "default"); }
        }

        public bool symetricBinning
        {
            get { return GetBool(KEY_SYMETRIC_BINNING, DEFAULT_VALUES[m_whichController].symetricBinning); }
            set { SetString(KEY_SYMETRIC_BINNING, value.ToString()); }
        }

        public byte maxXBin
        {
            get { return GetByte(KEY_MAX_X_BIN, DEFAULT_VALUES[m_whichController].maxXBin); }
            set { SetString(KEY_MAX_X_BIN, value.ToString()); }
        }

        public byte maxYBin
        {
            get { return GetByte(KEY_MAX_Y_BIN, DEFAULT_VALUES[m_whichController].maxYBin); }
            set { SetString(KEY_MAX_Y_BIN, value.ToString()); }
        }

#if false
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


#endif
    }
}
