//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM Camera driver for UsbCamera2
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
using System.Runtime.InteropServices;
using ASCOM.DeviceInterface;
using ASCOM.Utilities;
using System.Globalization;

using Logging;

namespace ASCOM.UsbCamera2
{
    //
    // Your driver's ID is ASCOM.UsbCamera2.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.UsbCamera2.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("f93602ec-c025-442b-ba94-9441cb59a535")]
    [ServedClassName("UsbCamera2 Camera")]
    [ProgId("ASCOM.sxUsbCamera2.Camera")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class Camera : ASCOM.sxUsbCameraBase.Camera
    {
        #region Camera Constants
        //
        // Driver ID and descriptive string that shows in the Chooser
        //
        private static string driverId = null;
        private const UInt16 DEFAULT_CONTROLLER_NUMBER = 1;
        private const UInt16 DEFAULT_CAMERA_ID = 0;
        #endregion

        //
        // Constructor - Must be public for COM registration!
        //
        protected Camera(UInt16 controllerNumber, UInt16 cameraId) :
            base(controllerNumber, cameraId)
        {
            driverId = Marshal.GenerateProgIdForType(this.GetType());
            Log.Write(String.Format("sxUsbCamera::Camera({0}, {1}) executing\n", controllerNumber, cameraId));
        }

        public Camera() :
            this(DEFAULT_CONTROLLER_NUMBER, DEFAULT_CAMERA_ID)
        {
            Log.Write("Camera(): sxUsbCamera2");
        }
    }
}
