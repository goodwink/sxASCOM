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
        private string m_dumpedModelName;
        private string m_dumpedPath;

        private void setupDump(string dumpedModelName)
        {
            m_dumpedModelName = dumpedModelName;
            m_dumpedPath = dumpedModelName.Substring(0,dumpedModelName.LastIndexOf('\\')+1);
            Log.Write(String.Format("for dumpedModelName={0} dumpedPath={1}", dumpedModelName, m_dumpedPath));
        }

        private object getDumpedObject(BinaryReader binReader, Type objectType)
        {
            IntPtr unmangedBufferPtr = IntPtr.Zero;
            byte [] byteBuffer = new byte[Marshal.SizeOf(objectType)];
            object oReturn = null;

            binReader.Read(byteBuffer, 0, byteBuffer.Length);

            try
            {
                unmangedBufferPtr = Marshal.AllocHGlobal(byteBuffer.Length);

                Marshal.Copy(byteBuffer, 0, unmangedBufferPtr, byteBuffer.Length);
                oReturn = Marshal.PtrToStructure(unmangedBufferPtr, objectType);
            }
            finally
            {
                Marshal.FreeHGlobal(unmangedBufferPtr);
            }

            return oReturn;
        }

        private void getModelDumped()
        {
            try
            {
                using (BinaryReader binReader = new BinaryReader(File.Open(m_dumpedModelName, FileMode.Open)))
                {
                     cameraModel = binReader.ReadUInt16();
                     Log.Write(String.Format("getModelDumped() cameraModel={0}\n", cameraModel));
                }
            }
            catch (System.Exception ex)
            {
                Log.Write(String.Format("Caught an exception trying to restore dumped model: {0}\n", ex.ToString()));
            }
        }

        private void getCCDParamsDumped()
        {
            try
            {
                using (BinaryReader binReader = new BinaryReader(File.Open(m_dumpedPath + String.Format("ascom-sx-{0}.parms", cameraModel) , FileMode.Open)))
                {
                    ccdParms = (SX_CCD_PARAMS)getDumpedObject(binReader, typeof(SX_CCD_PARAMS));
                }
            }
            catch (System.Exception ex)
            {
                Log.Write(String.Format("Caught an exception trying to restore dumped parms: {0}\n", ex.ToString()));
            }
        }

        private void getDumpedExposure()
        {
            try
            {
                using (BinaryReader binReader = new BinaryReader(File.Open(m_dumpedPath + String.Format("ascom-sx-{0}.exposure", cameraModel) , FileMode.Open)))
                {
                    currentExposure = (EXPOSURE_INFO)getDumpedObject(binReader, typeof(EXPOSURE_INFO));
                    dumpReadDelayedBlock(currentExposure.userRequested, "getDumpedExposure() read userRequested");
                    dumpReadDelayedBlock(currentExposure.toCamera, "getDumpedExposure() read toCamera");
                    dumpReadDelayedBlock(currentExposure.toCameraSecond, "getDumpedExposure() read toCameraSecond");

                }
            }
            catch (System.Exception ex)
            {
                Log.Write(String.Format("Caught an exception trying to restore dumped exposure: {0}\n", ex.ToString()));
            }
        }

        private void recordPixelsDumped(bool bDelayed, out DateTimeOffset exposureEnd)
        {
            exposureEnd = DateTimeOffset.Now;

            lock (oImageDataLock)
            {
                imageDataValid = false;

                getDumpedExposure();

                Int32 binnedWidth = currentExposure.toCamera.width / currentExposure.toCamera.x_bin;
                Int32 binnedHeight = currentExposure.toCamera.height / currentExposure.toCamera.y_bin;
                Int32 imagePixels = binnedWidth * binnedHeight;

                rawFrame1 = new byte[binnedWidth*binnedHeight*bytesPerPixel];
                try
                {
                    using (BinaryReader binReader = new BinaryReader(File.Open(m_dumpedPath + String.Format("ascom-sx-{0}.frame1.raw", cameraModel) , FileMode.Open)))
                    {
                        binReader.Read(rawFrame1, 0, rawFrame1.Length);
                        Log.Write(String.Format("undumped {0} bytes into rawFrame1\n", rawFrame1.Length));
                    }

                    if (idx == 0 && interlaced)
                    {
                        binnedWidth = currentExposure.toCameraSecond.width / currentExposure.toCameraSecond.x_bin;
                        binnedHeight = currentExposure.toCameraSecond.height / currentExposure.toCameraSecond.y_bin;
                        imagePixels = binnedWidth * binnedHeight;

                        rawFrame2 = new byte[binnedWidth*binnedHeight*bytesPerPixel];

                        using (BinaryReader binReader = new BinaryReader(File.Open(m_dumpedPath + String.Format("ascom-sx-{0}.frame1.raw", cameraModel) , FileMode.Open)))
                        {
                            binReader.Read(rawFrame2, 0, rawFrame2.Length);
                            Log.Write(String.Format("undumped {0} bytes into rawFrame2\n", rawFrame2.Length));
                        }
                    }
                    convertCameraDataToImageData();
                    imageDataValid = true;
                }
                catch (System.Exception ex)
                {
                    Log.Write(String.Format("Caught an exception trying to restore dumped frames: {0}\n", ex.ToString()));
                }

            }
        }
    }
}
