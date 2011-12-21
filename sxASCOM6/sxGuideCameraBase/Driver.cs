//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM Camera driver for sxGuideCameraBase
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

namespace ASCOM.sxGuideCameraBase
{
    //
    // Your driver's ID is ASCOM.sxGuideCameraBase.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.sxGuideCameraBase.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("cac98010-ea49-465e-892c-194c6518a4c7")]
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
            MessageBox.Show(String.Format("There are no configurable settings for guide cameras.  All configuration is done from the main camera's configuration screen."));
        }

        override public double CCDTemperature
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("Guide Camera CCDTemperature get will throw an exception\n");

                    throw new ASCOM.PropertyNotImplementedException(SetError("CCDTemperature: must throw exception if data unavailable"), false);
                }
                catch (System.Exception ex)
                {
                    String msg = SetError(String.Format("{0} caught and is rethrowing exception {1}", MethodBase.GetCurrentMethod().Name, ex));
                    Log.Write(msg);
                    throw ex;
                }
            }
        }

        override public string Description
        {
            get
            {
                try
                {
                    string sReturn = String.Format("SX-Guider ({0})", base.Description);

                    Log.Write(String.Format("Guide::Descripton get returns {0}\n", sReturn));

                    return sReturn;
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

                    Log.Write("Guide::CanGetCoolerPower get returns: false\n");

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

                    bool bReturn = bHasGuideCamera;

                    Log.Write(String.Format("Guide::CanPulseGuide returns {0}\n",bReturn));

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

                    Log.Write("Guide::CanSetCCDTemperature get returns false\n");

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

        override public bool CoolerOn
        {
            get
            {
                try
                {
                    verifyConnected(MethodBase.GetCurrentMethod().Name);

                    Log.Write("Guide::CoolerOn get will throw an exception\n");

                    throw new ASCOM.PropertyNotImplementedException(SetError("CoolerOn get is not supported"), true);
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

                    Log.Write("Guide Camera CoolerOn set will throw an exception\n");

                    throw new ASCOM.PropertyNotImplementedException(SetError("CoolerOn set is not supported"), true);
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

                    Log.Write("Guide::CoolerPower will throw an exception\n");

                    throw new ASCOM.PropertyNotImplementedException(SetError("CoolerPower get is not supported"), true);
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

                    Log.Write("Guide::HeatSinkTemperature get will throw an exception\n");

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

                    Log.Write("Giide::SetCCDTemperature get will throw an exception\n");

                    throw new ASCOM.PropertyNotImplementedException(String.Format("SetCCDTemperature must throw exception if CanSetCCDTemperature is False."), false);
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

                    Log.Write("Giide Camera SetCCDTemperature set will throw an exception\n");

                    throw new ASCOM.PropertyNotImplementedException(String.Format("SetCCDTemperature must throw exception if CanSetCCDTemperature is False."), true);
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
