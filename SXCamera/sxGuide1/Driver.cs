//tabs=4
using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

using ASCOM;
using ASCOM.Helper;
using ASCOM.Helper2;
using ASCOM.Interface;

using Logging;

namespace ASCOM.SXGuide1
{
    //
    // Your driver's ID is ASCOM.SXCamera.Camera
    //
    // The Guid attribute sets the CLSID for ASCOM.SXCamera.Camera
    // The ClassInterface/None addribute prevents an empty interface called
    // _Camera from being created and used as the [default] interface
    //
    [Guid("c150cbaa-429d-4bad-84ff-27077b4156a5")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Camera : ASCOM.SXGuide0.Camera
    {
        private const UInt16 DEFAULT_CONTROLLER_NUMBER = 1;
        private const UInt16 DEFAULT_CAMERAID = 1;

        public Camera() :
            base(DEFAULT_CONTROLLER_NUMBER, DEFAULT_CAMERAID)
        {
            Log.Write(String.Format("Guide1::Camera() executing\n"));
        }
    }
}
