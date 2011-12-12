//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM Camera driver for UsbCamera1
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

namespace ASCOM.UsbCamera1
{
    //
    // Your driver's ID is ASCOM.UsbCamera1.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.UsbCamera1.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("923968f2-8ba4-4cf7-b0a9-8cf4b74bf45d")]
    [ServedClassName("UsbCamera1 Camera")]
    [ProgId("ASCOM.sxUsbCamera1.Camera")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class Camera : ASCOM.sxUsbCameraBase.Camera
    {
        #region Camera Constants
        private static string driverId;
        private const UInt16 DEFAULT_CONTROLLER_NUMBER = 0;
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
            Log.Write("Camera(): sxUsbCamera1");
        }
    }
}
