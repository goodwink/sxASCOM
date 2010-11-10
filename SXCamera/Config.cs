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

            if (ret == null)
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

            if (str != null)
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
    }
}
