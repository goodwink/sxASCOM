using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;

using ASCOM;
using ASCOM.Interface;

using Logging;

namespace ASCOM.SXMain2
{
    //
    // Your driver's ID is ASCOM.SXCamera.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.SXCamera.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("c150cbaa-429d-4bad-84ff-27077b4156a2")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Camera : ASCOM.SXMain0.Camera
    {
        private const UInt16 DEFAULT_CONTROLLER_NUMBER = 2;
        private const UInt16 DEFAULT_CAMERA_ID = 0;

        public Camera() :
            base(DEFAULT_CONTROLLER_NUMBER, DEFAULT_CAMERA_ID)
        {
            Log.Write(String.Format("Main2::Camera() executing\n"));
        }
    }
}
