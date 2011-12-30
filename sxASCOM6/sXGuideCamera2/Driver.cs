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

namespace ASCOM.sxGuideCamera2
{
    //
    // Your driver's ID is ASCOM.GuideCamera2.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.GuideCamera2.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("674fb492-5179-4226-bdc8-ae73da014209")]
    [ServedClassName("GuideCamera2 Camera")]
    [ProgId("ASCOM.sxGuideCamera2.Camera")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class Camera : ASCOM.sxGuideCameraBase.Camera
    {
        #region Camera Constants
        //
        // Driver ID and descriptive string that shows in the Chooser
        //
        private static string driverId = null;
        private const UInt16 DEFAULT_CONTROLLER_NUMBER = 1;
        private const UInt16 DEFAULT_CAMERAID = 1;
        #endregion

        //
        // Constructor - Must be public for COM registration!
        //
        protected Camera(UInt16 controllerNumber, UInt16 cameraId) :
            base(controllerNumber, cameraId)
        {
            driverId = Marshal.GenerateProgIdForType(this.GetType());
            Log.Write(String.Format("Guide::Camera({0}, {1}) executing\n", controllerNumber, cameraId));
        }

        public Camera() :
            this(DEFAULT_CONTROLLER_NUMBER, DEFAULT_CAMERAID)
        {
            Log.Write("Camera(): sxGuideCamera2");
        }
    }
}
