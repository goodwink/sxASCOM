using System;
using System.Runtime.InteropServices;

namespace ASCOM.SXCamera
{
    [ComVisible(false)]
    public class ReferenceCountedObjectBase
    {
        public ReferenceCountedObjectBase()
        {
            // We increment the global count of objects.
            SXCamera.CountObject();
        }

        ~ReferenceCountedObjectBase()
        {
            // We decrement the global count of objects.
            SXCamera.UncountObject();
            // We then immediately test to see if we the conditions
            // are right to attempt to terminate this server application.
            SXCamera.ExitIf();
        }
    }
}
