using System;

using ASCOM.Helper;

using Logging;

namespace ASCOM.SXCamera
{
    public class Configuration
    {
        private const string DEVICE_TYPE = "Camera";
        private const string CONFIGURATION_SUBKEY = "Configuration";

        private const String ENABLE_UNTESTED = "EnableUntested";
#if DEBUG
        private const bool DEFAULT_ENABLE_UNTESTED = true;
#else
        private const bool   DEFAULT_ENABLE_UNTESTED = false;
#endif

        private const String ENABLE_LOGGING = "EnableLogging";
#if DEBUG
        private const bool   DEFAULT_ENABLE_LOGGING = true;
#else
        private const bool   DEFAULT_ENABLE_LOGGING = false;
#endif

        private const string LOG_FILE_NAME = "LogFileName";
        private string DEFAULT_LOG_FILE_NAME = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "ascom-sx-camera.log";

        private const string SECONDS_ARE_MILLISECONDS = "SecondsAreMilliseconds";
        private const bool DEFAULT_SECONDS_ARE_MILLISECONDS = false;

        public enum CAMERA_SELECTION_METHOD
        {
            CAMERA_SELECTION_ANY,
            CAMERA_SELECTION_EXACT_MODEL,
            CAMERA_SELECTION_EXCLUDE_MODEL
        };

        private const string CAMERA0_SELECTION = "camera0Selection";
        private string DEFAULT_CAMERA0_SELECTION = Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL);

        private const string CAMERA0_VID = "camera0VID";
        private const UInt16 DEFAULT_CAMERA0_VID = 1278;

        private const string CAMERA0_PID = "camera0PID";
        private const UInt16 DEFAULT_CAMERA0_PID = 507;

        private const string CAMERA1_SELECTION = "camera1Selection";
        private string DEFAULT_CAMERA1_SELECTION = Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL);

        private const string CAMERA1_VID = "camera1VID";
        private const UInt16 DEFAULT_CAMERA1_VID = 1278;

        private const string CAMERA1_PID = "camera1PID";
        private const UInt16 DEFAULT_CAMERA1_PID = 507;  
      
        private string driverID;
        ProfileClass profile;
        
        public Configuration()
        {
            driverID = String.Format("ASCOM.SXMain.{0}", DEVICE_TYPE);
            Log.Write("Config class constructor called for driverID " + driverID + "\n");

            profile = new ProfileClass();
            profile.DeviceTypeV = DEVICE_TYPE;
        }

        internal string GetString(string name)
        {
            string ret = null;

            try
            {
                ret = profile.GetValue(driverID, name, CONFIGURATION_SUBKEY);
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
            try
            {
                profile.SubKeys(driverID, CONFIGURATION_SUBKEY);
            }
            catch
            {
                Log.Write(String.Format("config: creating configuration subkey {0} of driver {1}\n", CONFIGURATION_SUBKEY, driverID));
                try
                {
                    profile.CreateSubKey(driverID, CONFIGURATION_SUBKEY);
                }
                catch (Exception ex)
                {
                    Log.Write(String.Format("config: CreateSubKey caught an exception: {0}\n", ex.ToString()));
                }
            }

            Log.Write(String.Format("config: writing {0}={1} to configuration subkey {2} of driver {3}\n", name, value, CONFIGURATION_SUBKEY, driverID));

            try
            {
                profile.WriteValue(driverID, name, value, CONFIGURATION_SUBKEY);
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
                catch
                {
                    Log.Write(String.Format("GetBool was unable to convert {0}\n", str));
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
                catch
                {
                    Log.Write(String.Format("GetUInt16 was unable to convert {0}\n", str));
                }
            }

            return iRet;
        }

        public bool enableUntested
        {
            
            get { return GetBool(ENABLE_UNTESTED, DEFAULT_ENABLE_UNTESTED);}
            set { SetString(ENABLE_UNTESTED, value.ToString());}
        }

        public bool enableLogging
        {
            get { return GetBool(ENABLE_LOGGING, DEFAULT_ENABLE_LOGGING); }
            set { SetString(ENABLE_LOGGING, value.ToString()); }
        }

        public bool secondsAreMilliseconds
        {
            get { return GetBool(SECONDS_ARE_MILLISECONDS, DEFAULT_SECONDS_ARE_MILLISECONDS); }
            set { SetString(SECONDS_ARE_MILLISECONDS, value.ToString()); }
        }

        public string logFileName
        {
            get { return GetString(LOG_FILE_NAME, DEFAULT_LOG_FILE_NAME); }
            set { SetString(LOG_FILE_NAME, value); }
        }

        public CAMERA_SELECTION_METHOD camera0SelectionMethod
        {
            get
            {
                Log.Write(String.Format("default={0}\n", DEFAULT_CAMERA0_SELECTION));
                String selection = GetString(CAMERA0_SELECTION, DEFAULT_CAMERA0_SELECTION);
                Log.Write(String.Format("CAMERA0SelectionMethod get converting {0}\n", selection));
                return (CAMERA_SELECTION_METHOD)Enum.Parse(typeof(CAMERA_SELECTION_METHOD), selection, true);
            }
            set
            {
                String selection = Enum.GetName(typeof(CAMERA_SELECTION_METHOD), value);
                SetString(CAMERA0_SELECTION, selection);
            }
        }

        public UInt16 camera0VID
        {
            get { return GetUInt16(CAMERA0_VID, DEFAULT_CAMERA0_VID); }
            set { SetString(CAMERA0_VID, value.ToString()); }
        }

        public UInt16 camera0PID
        {
            get { return GetUInt16(CAMERA0_PID, DEFAULT_CAMERA0_PID); }
            set { SetString(CAMERA0_PID, value.ToString()); }
        }
        public CAMERA_SELECTION_METHOD camera1SelectionMethod
        {
            get
            {
                Log.Write(String.Format("default={0}\n", DEFAULT_CAMERA1_SELECTION));
                String selection = GetString(CAMERA1_SELECTION, DEFAULT_CAMERA1_SELECTION);
                Log.Write(String.Format("CAMERA1SelectionMethod get converting {0}\n", selection));
                return (CAMERA_SELECTION_METHOD)Enum.Parse(typeof(CAMERA_SELECTION_METHOD), selection, true);
            }
            set
            {
                String selection = Enum.GetName(typeof(CAMERA_SELECTION_METHOD), value);
                SetString(CAMERA1_SELECTION, selection);
            }
        }

        public UInt16 camera1VID
        {
            get { return GetUInt16(CAMERA1_VID, DEFAULT_CAMERA1_VID); }
            set { SetString(CAMERA1_VID, value.ToString()); }
        }

        public UInt16 camera1PID
        {
            get { return GetUInt16(CAMERA1_PID, DEFAULT_CAMERA1_PID); }
            set { SetString(CAMERA1_PID, value.ToString()); }
        }
    }
}
