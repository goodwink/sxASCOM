using System;
using System.Windows.Forms;

using ASCOM.Utilities;

using Logging;

namespace ASCOM.SXCamera
{
    public class Configuration
    {
        private const string DEVICE_TYPE = "Camera";

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
        Profile profile;
        
        public Configuration()
        {
            driverID = String.Format("ASCOM.SXMain.{0}", DEVICE_TYPE);
            Log.Write("Config class constructor called for driverID " + driverID + "\n");

            profile = new Profile();
            profile.DeviceType = DEVICE_TYPE;
        }

        internal string GetString(string name)
        {
            string ret = null;

            try
            {
                ret = profile.GetValue(driverID, name);
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
            Log.Write(String.Format("config: writing {0}={1} to driver {2}\n", name, value, driverID));

            try
            {
                profile.WriteValue(driverID, name, value);
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
            try
            {
                Log.Write(String.Format("SetupDialog, description = {0}\n", description));

                SetupDialogForm F = new SetupDialogForm();

                F.EnableLoggingCheckBox.Checked = enableLogging;
                F.EnableUntestedCheckBox.Checked = enableUntested;
                F.secondsAreMiliseconds.Checked = secondsAreMilliseconds;
                F.Version.Text = String.Format("Version: {0}", SharedResources.versionNumber);

                F.camera0SelectionAllowAny.Checked = false;
                F.camera0SelectionExactModel.Checked = false;
                F.camera0SelectionExcludeModel.Checked = false;
                F.camera0VID.Text = camera0VID.ToString();
                F.camera0PID.Text = camera0PID.ToString();

                F.vid0Label.Visible = true;
                F.pid0Label.Visible = true;
                F.camera0VID.Visible = true;
                F.camera0PID.Visible = true;

                switch (camera0SelectionMethod)
                {
                    case CAMERA_SELECTION_METHOD.CAMERA_SELECTION_ANY:
                        F.camera0SelectionAllowAny.Checked = true;
                        F.vid0Label.Visible = false;
                        F.pid0Label.Visible = false;
                        F.camera0VID.Visible = false;
                        F.camera0PID.Visible = false;
                        break;
                    case CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL:
                        F.camera0SelectionExactModel.Checked = true;
                        break;
                    case CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL:
                        F.camera0SelectionExcludeModel.Checked = true;
                        break;
                    default:
                        throw new System.Exception(String.Format("Unknown Camera Selection Method {0} in SetupDialog", camera0SelectionMethod));
                }

                F.camera1VID.Text = camera1VID.ToString();
                F.camera1PID.Text = camera1PID.ToString();
                F.vid1Label.Visible = true;
                F.pid1Label.Visible = true;
                F.camera1VID.Visible = true;
                F.camera1PID.Visible = true;

                switch (camera1SelectionMethod)
                {
                    case CAMERA_SELECTION_METHOD.CAMERA_SELECTION_ANY:
                        F.camera1SelectionAllowAny.Checked = true;
                        F.vid1Label.Visible = true;
                        F.pid1Label.Visible = true;
                        F.camera1VID.Visible = true;
                        F.camera1PID.Visible = true;
                        break;
                    case CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL:
                        F.camera1SelectionExactModel.Checked = true;
                        break;
                    case CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL:
                        F.camera1SelectionExcludeModel.Checked = true;
                        break;
                    default:
                        throw new System.Exception(String.Format("Unknown Camera Selection Method {0} in SetupDialog", camera0SelectionMethod));
                }

                F.advancedUSBParmsEnabled.Checked = false;
                F.camera0Group.Enabled = false;
                F.camera1Group.Enabled = false;

                if (F.ShowDialog() == DialogResult.OK)
                {
                    Log.Write("ShowDialog returned OK - saving parameters\n");

                    enableLogging = F.EnableLoggingCheckBox.Checked;
                    enableUntested = F.EnableUntestedCheckBox.Checked;
                    secondsAreMilliseconds = F.secondsAreMiliseconds.Checked;

                    if (F.camera0SelectionAllowAny.Checked)
                    {
                        camera0SelectionMethod = Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_ANY;
                    }
                    else
                    {
                        bool error = false;
                        try
                        {
                            camera0VID = Convert.ToUInt16(F.camera0VID.Text);
                        }
                        catch (System.FormatException ex)
                        {
                            error = true;
                            Log.Write(String.Format("Caught an exception converting VID [{0}] to UInt16: {1}", F.camera0VID.Text, ex.ToString()));
                            MessageBox.Show("An invalid VID was entered.  Value was not changed");
                        }

                        try
                        {
                            camera0PID = Convert.ToUInt16(F.camera0PID.Text);
                        }
                        catch (System.FormatException ex)
                        {
                            error = true;
                            Log.Write(String.Format("Caught an exception converting PID [{0}] to UInt16: {1}", F.camera0PID.Text, ex.ToString()));
                            MessageBox.Show("An invalid PID was entered.  Value was not changed");
                        }

                        if (!error)
                        {
                            if (F.camera0SelectionExactModel.Checked)
                            {
                                camera0SelectionMethod = Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL;
                            }
                            else
                            {
                                camera0SelectionMethod = Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL;
                            }
                        }
                    }

                    if (F.camera1SelectionAllowAny.Checked)
                    {
                        camera1SelectionMethod = Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_ANY;
                    }
                    else
                    {
                        bool error = false;
                        try
                        {
                            camera1VID = Convert.ToUInt16(F.camera0VID.Text);
                        }
                        catch (System.FormatException ex)
                        {
                            error = true;
                            Log.Write(String.Format("Caught an exception converting VID [{0}] to UInt16: {1}", F.camera0VID.Text, ex.ToString()));
                            MessageBox.Show("An invalid VID was entered.  Value was not changed");
                        }

                        try
                        {
                            camera1PID = Convert.ToUInt16(F.camera0PID.Text);
                        }
                        catch (System.FormatException ex)
                        {
                            error = true;
                            Log.Write(String.Format("Caught an exception converting PID [{0}] to UInt16: {1}", F.camera0PID.Text, ex.ToString()));
                            MessageBox.Show("An invalid PID was entered.  Value was not changed");
                        }

                        if (!error)
                        {
                            if (F.camera1SelectionExactModel.Checked)
                            {
                                camera1SelectionMethod = CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL;
                            }
                            else
                            {
                                camera1SelectionMethod = CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL;
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
