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
using System.Collections;
using System.Runtime.InteropServices;
using ASCOM.DeviceInterface;
using ASCOM.Utilities;
using System.Globalization;

using Logging;

namespace ASCOM.sxUsbCamera6
{
    //
    // Your driver's ID is ASCOM.UsbCamera6.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.UsbCamera6.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("2509b68e-8fc4-4d8a-8263-bb94716c4577")]
    [ServedClassName("UsbCamera6 Camera")]
    [ProgId("ASCOM.sxUsbCamera6.Camera")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class Camera : ASCOM.sxUsbCameraBase.Camera
    {
        #region Camera Constants
        //
        // Driver ID and descriptive string that shows in the Chooser
        //
        private static string driverId = null;
        private const UInt16 DEFAULT_CONTROLLER_NUMBER = 5;
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
            Log.Write("Camera(): sxUsbCamera6");
        }
    }
}
