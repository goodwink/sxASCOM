//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM Camera driver for GuideCamera2
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
