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

        private const string KEY_LOG_FILE_NAME = "LogFileName";
        private string DEFAULT_LOG_FILE_NAME = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "ascom-sx-camera.log";

        private const string KEY_SECONDS_ARE_MILLISECONDS = "SecondsAreMilliseconds";
        private const bool DEFAULT_SECONDS_ARE_MILLISECONDS = false;

        private const string KEY_SELECTION_METHOD = "Selection";
        private const string KEY_VID = "VID";
        private const string KEY_PID = "PID";


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
            public string selectionMethod;
            public UInt16 VID;
            public UInt16 PID;

            internal CAMERA_VALUES(bool enableUntested, bool enableLogging, bool secondsAreMilliseconds, string selectionMethod, UInt16 VID, UInt16 PID)
            {
                this.enableUntested = enableUntested;
                this.enableLogging = enableLogging;
                this.secondsAreMilliseconds = secondsAreMilliseconds;
                this.selectionMethod = selectionMethod;
                this.VID = VID;
                this.PID = PID;
            }
        };

        internal CAMERA_VALUES[] DEFAULT_VALUES = {
            new CAMERA_VALUES(DEFAULT_ENABLE_UNTESTED, DEFAULT_ENABLE_LOGGING, DEFAULT_SECONDS_ARE_MILLISECONDS, Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL), 1278, 507),
            new CAMERA_VALUES(DEFAULT_ENABLE_UNTESTED, DEFAULT_ENABLE_LOGGING, DEFAULT_SECONDS_ARE_MILLISECONDS, Enum.GetName(typeof(CAMERA_SELECTION_METHOD), CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL), 1278, 507)
        };

        private Profile m_profile;
        private string m_driverId;
        private UInt16 m_whichController,
                       m_whichCamera;
        
        public Configuration(UInt16 whichController, UInt16 whichCamera)
        {
            Log.Write(String.Format("Configuration({0}, {1} starts\n", whichController, whichCamera));

            m_profile = new Profile();
            m_profile.DeviceType = DEVICE_TYPE;

            m_driverId = "ASCOM.";

            if (whichCamera == 0)
            {
                m_driverId  += "SXMain" + whichController.ToString();
            }
            else
            {
                m_driverId  += "SXGuide";
            }

            m_driverId += "." + DEVICE_TYPE;

            m_whichController = whichController;
            m_whichCamera = whichCamera;

            Log.Write(String.Format("Configuration() computes driverId={0}\n", m_driverId));
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

                F.EnableLoggingCheckBox.Checked = enableLogging;
                F.EnableUntestedCheckBox.Checked = enableUntested;
                F.secondsAreMiliseconds.Checked = secondsAreMilliseconds;
                F.Version.Text = String.Format("Version: {0}", SharedResources.versionNumber);

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
                    secondsAreMilliseconds = F.secondsAreMiliseconds.Checked;

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
