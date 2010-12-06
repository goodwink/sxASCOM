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

        private const string CAMERA_SELECTION = "CameraSelection";
        private string DEFAULT_CAMERA_SELECTION = Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_ANY);

        private const string CAMERA_VID = "CameraVID";
        private const UInt16 DEFAULT_CAMERA_VID = 1278;

        private const string CAMERA_PID = "CameraPID";
        private const UInt16 DEFAULT_CAMERA_PID = 0325;
        
        private string driverID;
        ProfileClass profile;
        
        public Configuration(string cameraType)
        {
            driverID = String.Format("ASCOM.SX{0}.{1}", cameraType, DEVICE_TYPE);
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

        public CAMERA_SELECTION_METHOD cameraSelectionMethod
        {
            get
            {
                Log.Write(String.Format("enum madness =[{0}]\n", Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_ANY)));
                Log.Write(String.Format("default={0}\n", DEFAULT_CAMERA_SELECTION));
                String selection = GetString(CAMERA_SELECTION, DEFAULT_CAMERA_SELECTION);
                Log.Write(String.Format("cameraSelectionMethod get converting {0}\n", selection));
                return (CAMERA_SELECTION_METHOD)Enum.Parse(typeof(CAMERA_SELECTION_METHOD), selection, true);
            }
            set
            {
                String selection = Enum.GetName(typeof(CAMERA_SELECTION_METHOD), value);
                SetString(CAMERA_SELECTION, selection);
            }
        }

        public UInt16 cameraVID
        {
            get { return GetUInt16(CAMERA_VID, DEFAULT_CAMERA_VID); }
            set { SetString(CAMERA_VID, value.ToString()); }
        }

        public UInt16 cameraPID
        {
            get { return GetUInt16(CAMERA_PID, DEFAULT_CAMERA_PID); }
            set { SetString(CAMERA_PID, value.ToString()); }
        }
    }
}
