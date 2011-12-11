//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM Camera driver for sxUsbCameraBase
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
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ASCOM.DeviceInterface;
using ASCOM.Utilities;

using Logging;

namespace ASCOM.sxUsbCameraBase
{
    //
    // Your driver's ID is ASCOM.sxUsbCameraBase.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.sxUsbCameraBase.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("d1e3449b-d8c2-4d06-8065-fb54ff1ce22c")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class Camera : ASCOM.sxCameraBase.Camera
    {
        protected Camera(UInt16 controllerNumber, UInt16 cameraId) :
            base(controllerNumber, cameraId)
        {
            // TODO Add your constructor code
        }

        override public void SetupDialog()
        {
            try
            {
                Log.Write(String.Format("SetupDialog, description = {0}\n", m_config.description));

                SetupDialogForm F = new SetupDialogForm();

                F.EnableLoggingCheckBox.Checked = m_config.enableLogging;
                F.EnableUntestedCheckBox.Checked = m_config.enableUntested;
                F.secondsAreMiliseconds.Checked = m_config.secondsAreMilliseconds;
                F.dumpDataEnabled.Checked = m_config.dumpDataEnabled;
                F.Version.Text = String.Format("Version: {0}", ASCOM.StarlightXpress.SharedResources.versionNumber);

                F.selectionAllowAny.Checked = false;
                F.selectionExactModel.Checked = false;
                F.selectionExcludeModel.Checked = false;
                F.VID.Text = m_config.VID.ToString();
                F.PID.Text = m_config.PID.ToString();

                F.vidLabel.Visible = true;
                F.pidLabel.Visible = true;
                F.VID.Visible = true;
                F.PID.Visible = true;

                switch (m_config.selectionMethod)
                {
                    case ASCOM.sxCameraBase.Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_ANY:
                        F.selectionAllowAny.Checked = true;
                        F.vidLabel.Visible = false;
                        F.pidLabel.Visible = false;
                        F.VID.Visible = false;
                        F.PID.Visible = false;
                        break;
                    case ASCOM.sxCameraBase.Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL:
                        F.selectionExactModel.Checked = true;
                        break;
                    case ASCOM.sxCameraBase.Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL:
                        F.selectionExcludeModel.Checked = true;
                        break;
                    default:
                        throw new System.Exception(String.Format("Unknown Camera Selection Method {0} in SetupDialog", m_config.selectionMethod));
                }

                F.advancedUSBParmsEnabled.Checked = false;
                F.usbGroup.Enabled = false;

                // some cameras cannot bin.  
                // If this camera can't bin, disble the binGroup so binning cannot be modified.
                if (m_config.maxXBin > 1)
                {
                    F.binGroup.Enabled = true;
                }
                else
                {
                    F.binGroup.Enabled = false;
                }

                if (m_config.symetricBinning)
                {
                    F.symetricBinning.Checked = true;
                    F.binLabel.Text = "Max Bin";
                    F.xBinLabel.Visible = false;
                    F.maxXBin.Visible = false;
                }
                else
                {
                    F.symetricBinning.Checked = false;
                    F.binLabel.Text = "Max Y Bin";
                    F.xBinLabel.Visible = true;
                    F.maxXBin.Visible = true;
                }

                F.maxXBin.Value  = m_config.maxXBin;
                F.maxYBin.Value  = m_config.maxYBin;

                if (F.ShowDialog() == DialogResult.OK)
                {
                    Log.Write("ShowDialog returned OK - saving parameters\n");

                    m_config.enableLogging = F.EnableLoggingCheckBox.Checked;
                    m_config.enableUntested = F.EnableUntestedCheckBox.Checked;
                    m_config.secondsAreMilliseconds = F.secondsAreMiliseconds.Checked;
                    m_config.dumpDataEnabled = F.dumpDataEnabled.Checked;

                    if (F.selectionAllowAny.Checked)
                    {
                        m_config.selectionMethod = ASCOM.sxCameraBase.Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_ANY;
                    }
                    else
                    {
                        bool error = false;
                        try
                        {
                            m_config.VID = Convert.ToUInt16(F.VID.Text);
                        }
                        catch (System.FormatException ex)
                        {
                            error = true;
                            Log.Write(String.Format("Caught an exception converting VID [{0}] to UInt16: {1}\n", F.VID.Text, ex.ToString()));
                            MessageBox.Show("An invalid VID was entered.  Value was not changed");
                        }
                        catch (OverflowException ex)
                        {
                            error = true;
                            Log.Write(String.Format("Caught an exception converting VID [{0}] to UInt16: {1}\n", F.VID.Text, ex.ToString()));
                            MessageBox.Show("An invalid VID was entered.  Value was not changed");
                        }

                        try
                        {
                            m_config.PID = Convert.ToUInt16(F.PID.Text);
                        }
                        catch (System.FormatException ex)
                        {
                            error = true;
                            Log.Write(String.Format("Caught an exception converting PID [{0}] to UInt16: {1}\n", F.PID.Text, ex.ToString()));
                            MessageBox.Show("An invalid PID was entered.  Value was not changed");
                        }
                        catch (OverflowException ex)
                        {
                            error = true;
                            Log.Write(String.Format("Caught an exception converting VID [{0}] to UInt16: {1}\n", F.VID.Text, ex.ToString()));
                            MessageBox.Show("An invalid VID was entered.  Value was not changed");
                        }

                        if (!error)
                        {
                            if (F.selectionExactModel.Checked)
                            {
                                m_config.selectionMethod = ASCOM.sxCameraBase.Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXACT_MODEL;
                            }
                            else
                            {
                                m_config.selectionMethod = ASCOM.sxCameraBase.Configuration.CAMERA_SELECTION_METHOD.CAMERA_SELECTION_EXCLUDE_MODEL;
                            }
                        }
                    }

                    m_config.symetricBinning = F.symetricBinning.Checked;
                    m_config.maxYBin = (byte)F.maxYBin.Value;

                    if (m_config.symetricBinning)
                    {
                        m_config.maxXBin = m_config.maxYBin;
                    }
                    else
                    {
                        m_config.maxXBin = (byte)F.maxXBin.Value;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Write(String.Format("Unable to complete SetupDialog request - ex = {0}\n", ex.ToString()));
                throw ex;
            }
        }

        override public double CCDTemperature
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!CanSetCCDTemperature)
                    {
                        throw new ASCOM.PropertyNotImplementedException(SetError("CCDTemperature get: must throw exception if data unavailable"), false);
                    }

                    double dReturn =  ((double)(sxCamera.coolerTemp - 2732)) / 10;

                    Log.Write(String.Format("Main::CCDTemperature get returns {0}\n", dReturn));

                    return dReturn;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        override public bool CanGetCoolerPower
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("Main::CanGetCoolerPower get returns false\n");

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

        override public bool CanPulseGuide
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    bool bReturn = false;

                    if (!bHasGuideCamera)
                    {
                        bReturn = sxCamera.hasGuidePort;
                    }

                    Log.Write(String.Format("Main::CanPulseGuide get returns {0}\n", bReturn));

                    return bReturn;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        override public bool CanSetCCDTemperature
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    bool bReturn = sxCamera.hasCoolerControl;

                    Log.Write(String.Format("Main::CanSetCCDTemperature get returns {0}\n", bReturn));

                    return bReturn;
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        override public bool CoolerOn
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!CanSetCCDTemperature)
                    {
                        throw new ASCOM.PropertyNotImplementedException(SetError("CoolerOn get is not supported"), true);
                    }

                    bool bReturn = sxCamera.coolerEnabled;

                    Log.Write(String.Format("Main::CoolerOn get returns {0}\n", bReturn));

                    return bReturn;
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

                    if (!CanSetCCDTemperature)
                    {
                        throw new ASCOM.PropertyNotImplementedException(SetError("CoolerOn set is not supported"), true);
                    }

                    sxCamera.coolerEnabled = value;
                    Log.Write(String.Format("CoolerOn set to {0}\n", value));
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        override public double CoolerPower
        {
            get 
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("Main::CoolerPower will throw exception\n");

                    throw new ASCOM.PropertyNotImplementedException(SetError(String.Format("Cooler Power Must throw exception if not supported.")), false);
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        override public double HeatSinkTemperature
        {
            get
            { 
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("Main::HeatSinkTemperature will throw exception\n");

                    throw new ASCOM.PropertyNotImplementedException(SetError("HeatSinkTemperature must throw exception if data unavailable"), true);
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        override public double SetCCDTemperature
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    if (!CanSetCCDTemperature)
                    {
                        throw new ASCOM.PropertyNotImplementedException(String.Format("SetCCDTemperature get: must throw exception if CanSetCCDTemperature is False."), false);
                    }

                    double dReturn =  ((double)(sxCamera.coolerSetPoint - 2732)) / 10;

                    Log.Write(String.Format("Main::SetCCDTemperature get returns {0}\n", dReturn));

                    return dReturn;
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

                    if (!CanSetCCDTemperature)
                    {
                        throw new ASCOM.PropertyNotImplementedException(String.Format("SetCCDTemperature set: must throw exception if CanSetCCDTemperature is False."), false);
                    }

                    Log.Write(String.Format("Main::SetCCDTemperature set to {0}\n", value));

                    // These two range checks are required by conform, but not by the spec.
                    if (value >= 100 || value <= -273)
                    {
                        throw new ASCOM.InvalidValueException(SetError(String.Format("SetCCDTemperature request for {0} is not reasonable", value)), value.ToString(), "<= 100");
                    }

                    sxCamera.coolerSetPoint = (UInt16) ((value * 10) + 2732);
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }
    }
}
