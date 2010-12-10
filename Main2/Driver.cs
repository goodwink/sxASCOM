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
    [Guid("23B414C0-6CB0-4f87-B0F7-00B2F8793D9C")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Camera : ASCOM.SXMain.Camera
    {
        private const UInt16 DEFAULT_CAMERA_ID = 0;
        private const UInt16 DEFAULT_CONTROLLER_NUMBER = 1;

        public Camera() :
            base(DEFAULT_CAMERA_ID, DEFAULT_CONTROLLER_NUMBER)
        {
            Log.Write(String.Format("Camera() called for main2\n"));
        }
    }
}
