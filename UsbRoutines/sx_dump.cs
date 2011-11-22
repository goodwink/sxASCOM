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
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WinUsbDemo;
using Logging;

namespace sx
{
    public partial class Camera
        : sxBase
    {
        private string dumpPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\";

        private void dumpObject(BinaryWriter binWriter, object o)
        {
            IntPtr unmanagedBufferPtr = IntPtr.Zero;
            byte [] byteBuffer = new byte[Marshal.SizeOf(o)];

            try
            {
                unmanagedBufferPtr = Marshal.AllocHGlobal(unmanagedBufferPtr);

                Marshal.StructureToPtr(o, unmanagedBufferPtr, false);
                Marshal.Copy(unmanagedBufferPtr, byteBuffer, 0, byteBuffer.Length);
            }
            finally
            {
                Marshal.FreeHGlobal(unmanagedBufferPtr);
            }

            binWriter.Write(byteBuffer);
        }


        private void dumpModel()
        {
            try
            {
                using (BinaryWriter binWriter = new BinaryWriter(File.Open(dumpPath + String.Format("ascom-sx-{0}.model", cameraModel) , FileMode.Create)))
                {
                    binWriter.Write(cameraModel);
                }
            }
            catch (System.Exception ex)
            {
                Log.Write(String.Format("Caught an exception while trying to dump model: {0}\n", ex.ToString()));
            }
        }

        private void dumpCCDParams()
        {
            try
            {
                using (BinaryWriter binWriter = new BinaryWriter(File.Open(dumpPath + String.Format("ascom-sx-{0}.parms", cameraModel) , FileMode.Create)))
                {

                    dumpObject(binWriter, ccdParms);
                }
            }
            catch (System.Exception ex)
            {
                Log.Write(String.Format("Caught an exception while trying to dump parms: {0}\n", ex.ToString()));
            }
        }

        private void dumpCurrentExposure()
        {
            try
            {
                using (BinaryWriter binWriter = new BinaryWriter(File.Open(dumpPath + String.Format("ascom-sx-{0}.exposure", cameraModel) , FileMode.Create)))
                {
                    dumpObject(binWriter, currentExposure);
                }
            }
            catch (System.Exception ex)
            {
                Log.Write(String.Format("Caught an exception while trying to dump parms: {0}\n", ex.ToString()));
            }
        }

        private void dumpFrame()
        {
            try
            {
                using (BinaryWriter binWriter = new BinaryWriter(File.Open(dumpPath + String.Format("ascom-sx-{0}.frame1.raw", cameraModel) , FileMode.Create)))
                {
                    binWriter.Write(rawFrame1);
                }

                if (rawFrame2 != null)
                {
                    using (BinaryWriter binWriter = new BinaryWriter(File.Open(dumpPath + String.Format("ascom-sx-{0}.frame2.raw", cameraModel) , FileMode.Create)))
                    {
                        binWriter.Write(rawFrame2);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Write(String.Format("Caught an exception while trying to dump frames: {0}\n", ex.ToString()));
            }
        }
    }
}
