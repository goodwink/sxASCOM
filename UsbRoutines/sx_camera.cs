using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WinUsbDemo;
using Logging;

namespace sx
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct SX_COOLER_BLOCK
    {
        internal UInt16 coolerTemp;
        internal byte coolerEnabled;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct SX_READ_BLOCK
    {
        internal UInt16 x_offset;
        internal UInt16 y_offset;
        internal UInt16 width;
        internal UInt16 height;
        internal Byte x_bin;
        internal Byte y_bin;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct SX_READ_DELAYED_BLOCK
    {
        internal UInt16 x_offset;
        internal UInt16 y_offset;
        internal UInt16 width;
        internal UInt16 height;
        internal Byte x_bin;
        internal Byte y_bin;
        internal UInt32 delay;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct SX_CCD_PARAMS
    {
        internal Byte hfront_porch;
        internal Byte hback_porch;
        internal UInt16 width;
        internal Byte vfront_porch;
        internal Byte vback_porch;
        internal UInt16 height;
        internal UInt16 pixel_uwidth;
        internal UInt16 pixel_uheight;
        internal UInt16 color_matrix;
        internal Byte bits_per_pixel;
        internal Byte num_serial_ports;
        internal Byte extra_capabilities;
    }

    internal struct EXPOSURE_INFO
    {
        internal SX_READ_DELAYED_BLOCK userRequested;
        internal SX_READ_DELAYED_BLOCK toCamera;
        internal SX_READ_DELAYED_BLOCK toCameraSecond;
    }

    internal enum CameraModels
    {
        // from http://tech.groups.yahoo.com/group/starlightxpress/message/85249
        // ------------------ H5 --------------------
        MODEL_H5 = 0x05,
        MODEL_H5C = 0x85,
        // ------------------ H9 --------------------
        MODEL_H9 = 0x09,
        MODEL_H9C = 0x89,
        // ------------------ H16 --------------------
        MODEL_H16 = 0x10,
        MODEL_H16C = 0x90,
        // ------------------ H35 --------------------
        MODEL_H35 = 0x23,
        MODEL_H35C = 0xB5,
        // ------------------ H36 --------------------
        MODEL_H36 = 0x24,
        MODEL_H36C = 0xB6,
        // ------------------ MX5 --------------------
        MODEL_MX5 = 0x45,
        MODEL_MX5C = 0x84,
        // ------------------ MX7 --------------------
        MODEL_MX7 = 0x47,
        MODEL_MX7C = 0xC7,
        // ------------------ MX8 --------------------
        MODEL_MX8C = 0xC8,
        // ------------------ MX9 --------------------
        MODEL_MX9 = 0x49,
        // ------------------ M25C --------------------
        MODEL_M25C = 0x59,
        // ------------------ Lodestar --------------------
        MODEL_LX1 = 0x46,
    }

    public class Camera
        : sxBase
    {
        // Variables
        private Controller m_controller;
        private SX_CCD_PARAMS ccdParms;
        private SX_READ_DELAYED_BLOCK nextExposure;
        private EXPOSURE_INFO currentExposure;
        Array imageRawData;
        Type pixelType;
        private Int32[,] imageData;
        private bool imageDataValid;
        private object oImageDataLock;
        private UInt16 idx;
        private SX_COOLER_BLOCK m_coolerBlock;

        // Properties

        public UInt16 cameraModel
        {
            get;
            private set;
        }

        public string description
        {
            get;
            internal set;
        }

        public double fullWellCapacity
        {
            get;
            internal set;
        }

        public double electronsPerADU
        {
            get;
            internal set;
        }

        private bool progressive
        {
            get;
            set;
        }

        private bool interlaced
        {
            get
            {
                return !progressive;
            }
        }

        public Byte hFrontPorch
        {
            get { return ccdParms.hfront_porch; }
        }

        public Byte hBackPorch
        {
            get { return ccdParms.hback_porch; }
        }

        public Byte vFrontPorch
        {
            get { return ccdParms.hfront_porch; }
        }

        public Byte vBackPorch
        {
            get { return ccdParms.hback_porch; }
        }

        public UInt16 frameHeight
        {
            get { return ccdParms.height; }
        }

        public UInt16 ccdWidth
        {
            get { return ccdParms.width; }
        }

        public UInt16 ccdHeight
        {
            get 
            { 
                UInt16 ret=ccdParms.height; 

                if (interlaced)
                {
                    ret *= 2;
                }

                return ret;
            }
        }

        public double pixelWidth
        {
            get { return ccdParms.pixel_uwidth / (double)256; }
        }

        public double pixelHeight
        {
            get { return ccdParms.pixel_uheight / (double)256; }
        }

        public Byte bitsPerPixel
        {
            get { return ccdParms.bits_per_pixel; }
        }

        public Boolean hasGuideCamera
        {
            get { return m_controller.hasGuideCamera; }
        }

        public Boolean hasGuidePort
        {
            get { return m_controller.hasGuidePort; }
        }

        private Byte extraCapabilities
        {
            get { return ccdParms.extra_capabilities; }
        }

        public UInt16 colorMatrix
        {
            get { return ccdParms.color_matrix; }
        }

        public UInt16 xOffset
        {
            get { return nextExposure.x_offset; }
            set
            {
                if (value > ccdWidth)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid xOffset {0} 0<=xOffset<={1}", value, ccdWidth), "xOffset");
                }

                if (value + width > ccdWidth)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid xOffset + width: 0 < xOffset {0} + width {1} <= {2}", value, width, ccdWidth), "xOffset");
                }

                nextExposure.x_offset = value;
            }
        }

        public UInt16 yOffset
        {
            get { return nextExposure.y_offset; }
            set
            {
                if (value >= ccdHeight)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid yOffset {0} 0<=yOffset<={1}", value, ccdHeight), "yOffset");
                }
                if (value + height > ccdHeight)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid yOffset + height: 0 < yOffset {0} + height {1} <= {2}", value, height, ccdHeight), "yOffset");
                }
                nextExposure.y_offset = value;
            }
        }

        public UInt16 width
        {
            get { return nextExposure.width; }
            set
            {
                if (value == 0 || value > ccdWidth)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid width {0} 1<=width<={1}", value, ccdWidth), "width");
                }
                nextExposure.width = value;
            }
        }

        public UInt16 height
        {
            get { return nextExposure.height; }
            set
            {
                if (value == 0 || value > ccdHeight)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid height {0} 1<=height<={1}", value, ccdHeight), "height");
                }
                nextExposure.height = value;
            }
        }

        public Byte xBin
        {
            get { return nextExposure.x_bin; }
            set
            {
                if (value <= 0 || value > MAX_X_BIN)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid xBin {0} 1<=height<={1}", value, MAX_BIN), "xBin");
                }

                // note that this disallows non power of 2 binning values.  The camera hardware can deal with them, but 
                // there are interactions with bayer matrices that I don't want to deal with now
                //if ((value & (value - 1)) != 0)
                //{
                //    throw new ArgumentOutOfRangeException(String.Format("non-power of 2 binning value set: {0}", value), "yBin");
                //}

                nextExposure.x_bin = value;
            }
        }

        public Byte xBinMax
        {
            get { return MAX_X_BIN; }
        }

        public Byte yBin
        {
            get { return nextExposure.y_bin; }
            set
            {
                if (value <= 0 || value > MAX_Y_BIN)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid yBin {0} 1<=height<={1}", value, MAX_BIN), "yBin");
                }

                // note that this disallows non power of 2 binning values.  The camera hardware can deal with them, but 
                // there are interactions with bayer matrices that I don't want to deal with now
                //if ((value & (value - 1)) != 0)
                //{
                //    throw new ArgumentOutOfRangeException(String.Format("non-power of 2 binning value set: {0}", value), "yBin");
                //}

                nextExposure.y_bin = value;
            }
        }

        public Byte yBinMax
        {
            get { return MAX_Y_BIN; }
        }

        public UInt32 delayMs
        {
            get { return nextExposure.delay; }
            set { nextExposure.delay = value; }
        }

        public object ImageArray
        {
            get
            {
                lock (oImageDataLock)
                {
                    Log.Write("ImageArray entered: imageDataValid=" + imageDataValid + "\n");

                    if (!imageDataValid)
                    {
                        throw new ArgumentException("ImageArray not valid");
                    }

                    if (imageData == null)
                    {
                        convertCameraDataToImageData();
                    }
#if false
                    using (BinaryWriter binWriter = new BinaryWriter(File.Open("c:\\temp\\sx-ascom\\image.cooked", FileMode.Create)))
                    {
                        Int32 binnedWidth = currentExposure.userRequested.width / currentExposure.userRequested.x_bin;
                        Int32 binnedHeight = currentExposure.userRequested.height / currentExposure.userRequested.y_bin;

                        if (idx == 0 && cameraModel == CameraModels.MODEL_M25C)
                        {
                            binnedWidth /= 2;
                            binnedHeight *= 2;
                        }

                        for (int xx = 0; xx < binnedWidth; xx++)
                            for (int yy = 0; yy < binnedHeight; yy++)
                            {
                                binWriter.Write(imageData[xx, yy]);
                            }
                    }
#endif
                    return imageData;
                }
            }
        }


        // setINFO sets information for the camera.  It must set:
        // - description
        // - fullWellCapacity
        // - electronsPerADU
        // - progressive
        private void setInfo(bool bAllowUntested)
        {
            bool bUntested = false;

            switch ((CameraModels)cameraModel)
            {
                case CameraModels.MODEL_H5:
                    bUntested = true;
                    description = "H5";
                    fullWellCapacity = 30000;
                    electronsPerADU = 0.40;
                    progressive = true;
                    break;
#if false
// I don't know if this is progressive or interlaced
                case CameraModels.MODEL_H5C:
                    bUntested = true;
                    description = "H5C";
                    fullWellCapacity = 30000;
                    electronsPerADU = 0.40;
                    progressive = true;
#endif
                case CameraModels.MODEL_H9:
                    description = "H9";
                    fullWellCapacity = 27000;
                    electronsPerADU = 0.45;
                    progressive = true;
                    break;
                case CameraModels.MODEL_H9C:
                    description = "H9C";
                    fullWellCapacity = 27000;
                    electronsPerADU = 0.45;
#if true
                    progressive = true;
#else
                    Log.Write("For testing, H9_C set to interleaved\n");
#endif
                    break;
                case CameraModels.MODEL_H16:
                    description = "H16";
                    fullWellCapacity = 40000;
                    electronsPerADU = 0.6;
                    progressive = true;
                    break;
                case CameraModels.MODEL_H35:
                    bUntested = true;
                    description = "H35";
                    fullWellCapacity = 50000;
                    electronsPerADU = 0.9;
                    progressive = true;
                    break;
                case CameraModels.MODEL_H36:
                    bUntested = true;
                    description = "H36";
                    fullWellCapacity = 30000;
                    electronsPerADU = 0.4;
                    progressive = true;
                    break;
                case CameraModels.MODEL_LX1:
                    bUntested = true;
                    description = "Lodestar";
                    fullWellCapacity = 50000;
                    electronsPerADU = 0.9;
                    progressive = false;
                    break;
                case CameraModels.MODEL_M25C:
                    description = "M25C";
                    fullWellCapacity = 25000;
                    electronsPerADU = 0.40;
                    progressive = true;
                    break;
                case CameraModels.MODEL_MX5:
                    bUntested = true;
                    description = "MX5";
                    fullWellCapacity = 60000;
                    electronsPerADU = 1.0;
                    progressive = false;
                    break;
                case CameraModels.MODEL_MX5C:
                    bUntested = true;
                    description = "MX5C";
                    fullWellCapacity = 60000;
                    electronsPerADU = 1.0;
                    progressive = false;
                    break;
                case CameraModels.MODEL_MX7:
                    bUntested = true;
                    description = "MX7";
                    fullWellCapacity = 70000;
                    electronsPerADU = 1.3;
                    progressive = false;
                    break;
                case CameraModels.MODEL_MX7C:
                    bUntested = true;
                    description = "MX7C";
                    fullWellCapacity = 70000;
                    electronsPerADU = 1.3;
                    progressive = false;
                    break;
                case CameraModels.MODEL_MX8C:
                    bUntested = true;
                    description = "MX8C";
                    fullWellCapacity = 10000;
                    electronsPerADU = 1.0;
                    progressive = false;
                    break;
                case CameraModels.MODEL_MX9:
                    bUntested = true;
                    description = "MX9";
                    fullWellCapacity = 100000;
                    electronsPerADU = 2.0;
                    progressive = false;
                    break;
                default:
                    bUntested = true;
                    description = String.Format("unknown 0x{0:x}", cameraModel);
                    fullWellCapacity = 0.0;
                    electronsPerADU = 0.0;
                    // This is a guess, but I think I have all the old cameras in the 
                    // switch statement, and I expect most new models will be 
                    // progressive
                    progressive = true;
                    break;
            }

            Log.Write(String.Format("setInfo(): description={0} fullWellCapacity={1} electronsPerADU={2} progressive={3} bUntested={4}\n",
                        description, fullWellCapacity, electronsPerADU, progressive, bUntested));

            if (bUntested)
            {
                if (!bAllowUntested)
                {
                    throw new System.Exception(String.Format("camera model {0} is untested and \"Enable Untested\" is not set", description));
                }

                description = "(untested) " + description;
            }
        }

        public Camera(Controller controller, UInt16 cameraIdx, bool bAllowUntested)
        {
            Log.Write(String.Format("sx.Camera() constructor: controller={0} cameraIdx={1}\n", controller, cameraIdx));

            idx = cameraIdx;

            m_controller = controller;

            if (cameraIdx > 0)
            {
                if (cameraIdx != 1)
                {
                    throw new ArgumentException("Error: cameraIdx > 1");
                }

                if (!hasGuideCamera)
                {
                    Log.Write(String.Format("sx.Camera() constructor: Guide Camera is not connected\n"));
                    throw new ArgumentException("Error: cameraIDX == 1 and INTEGRATED_GUIDER_CCD == 0");
                }
            }

            cameraModel = getModel();
            setInfo(bAllowUntested);
            getParams(ref ccdParms);
            setPixelType();
            buildReadDelayedBlock(out nextExposure, 0, 0, ccdWidth, ccdHeight, 1, 1, 0);
            imageDataValid = false;
            oImageDataLock = new object();
            if (hasCoolerControl)
            {
                SX_COOLER_BLOCK tempBlock;
                // get the initial state of the cooler.  For lack of a better plan, assume that the
                // cooler is at the desired temperature.  Read the cooler value, and then set the cooler 
                // back to the state that was read.  
                tempBlock.coolerTemp = 0;
                tempBlock.coolerEnabled = 0;
                setCoolerInfo(ref tempBlock, out m_coolerBlock);
                setCoolerInfo(ref m_coolerBlock, out tempBlock);
                Log.Write(String.Format("sx.Camera() constructor cooler init.  temp={0} enabled={1}\n", m_coolerBlock.coolerTemp, m_coolerBlock.coolerEnabled));
            }
            Log.Write(String.Format("sx.Camera() constructor returns\n"));
        }

        internal void checkParms(bool useFrameHeight, UInt16 width, UInt16 height, UInt16 xOffset, UInt16 yOffset, Byte xBin, Byte yBin)
        {
            UInt16 effectiveHeight;

            if (useFrameHeight)
            {
                effectiveHeight = frameHeight;
            }
            else
            {
                effectiveHeight = ccdHeight;
            }

            if (width > ccdWidth)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid width {0}: 0<=width<={1}", width, ccdWidth), "width");
            }
            if (height > effectiveHeight)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid height {0}: 0<=height<={1}", height, effectiveHeight), "height");
            }
            if (xOffset > ccdWidth)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid xOffset {0}: 0<=width<={1}", xOffset, ccdWidth), "xOffset");
            }
            if (yOffset > effectiveHeight)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid height {0}: 0<=height<={1}", yOffset, effectiveHeight), "yOffset");
            }
            if (xOffset + width > ccdWidth)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid xOffset + width: 0 < xOffset {0} + width {1} <= {2}", xOffset, width, ccdWidth), "width+xOffset");
            }
            if (yOffset + height > effectiveHeight)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid yOffset + height: 0 < yOffset {0} + height {1} <= {2}", yOffset, height, effectiveHeight), "height+yOffset");
            }
            // The following if disallows asymmetric binning. The SX cameras will do it, but it there are difficulties that arise from
            // asymmetric binning and bayer matrices that I don't want to deal with at this point...
            if (xBin != yBin)
            {
                throw new ArgumentOutOfRangeException("xBin != yBin");
            }

            if (width % xBin != 0)
            {
                throw new ArgumentOutOfRangeException("width % xBin != 0");
            }

            if (height % yBin != 0)
            {
                throw new ArgumentOutOfRangeException(String.Format("height ({0}) % yBin ({1}) != 0 ({2}", height, yBin, height % yBin));
            }

        }

        // we use a READ_DELAYED_BLOCK to store paramters that are accessed as properties.  
        // If the user requests a read without delay, we can just copy all the matching 
        // parameters out of the one we are keeping

        internal void initReadBlock(SX_READ_DELAYED_BLOCK inBlock, out SX_READ_BLOCK outBlock)
        {
            outBlock.width = (UInt16)inBlock.width;
            outBlock.height = (UInt16)inBlock.height;

            outBlock.x_offset = inBlock.x_offset;
            outBlock.y_offset = inBlock.y_offset;

            outBlock.x_bin = inBlock.x_bin;
            outBlock.y_bin = inBlock.y_bin;

            Log.Write(String.Format("initReadBlock() x_off={0} y_off={1} width={2} height={3} x_bin={4} y_bin={5}\n", outBlock.x_offset, outBlock.y_offset, outBlock.width, outBlock.height, outBlock.x_bin, outBlock.y_bin));
        }

        public void freezeParameters()
        {
            dumpReadDelayedBlock(nextExposure, "freezing parameters");
            currentExposure.userRequested = nextExposure;
        }

        internal UInt16 adjustReadDelayedBlock()
        {
            UInt16 fieldFlags = SX_CCD_FLAGS_FIELD_EVEN | SX_CCD_FLAGS_FIELD_ODD;

            dumpReadDelayedBlock(currentExposure.userRequested, "as requested - before adjustments");

            checkParms(false, currentExposure.userRequested.width, currentExposure.userRequested.height, currentExposure.userRequested.x_offset, currentExposure.userRequested.y_offset, currentExposure.userRequested.x_bin, currentExposure.userRequested.y_bin);

            currentExposure.toCamera = currentExposure.userRequested;

            // interlaced cameras require some special care.  Some of it is handled
            // later, but we have to divide the offset and the height by 2
            // we need to make sure that the height requested is a multiple of 
            // the y binning factor

            if (interlaced)
            {
                currentExposure.toCamera.y_offset /= 2;

                UInt16 binnedRows = (UInt16)(currentExposure.toCamera.height/currentExposure.toCamera.y_bin);
                currentExposure.toCamera.height = (UInt16)(binnedRows/2 * currentExposure.toCamera.y_bin);

                // See if our modified parameters are still legal
                try
                {
                    checkParms(true, currentExposure.toCamera.width, currentExposure.toCamera.height, currentExposure.toCamera.x_offset, currentExposure.toCamera.y_offset, currentExposure.toCamera.x_bin, currentExposure.toCamera.y_bin);
                }
                catch (Exception ex)
                {
                    Log.Write(String.Format("checkParms for toCamera generated exception {0}\n", ex));
                    throw;
                }

                dumpReadDelayedBlock(currentExposure.userRequested, "after initial progressive adjustments");
            }

            // cameras with a Bayer matrix need the offsets to be even so that the subframe returned 
            // has the same color representation as a full frame.  Since this (at most) offsets 
            // the image by 1 pixel on each axis, we just do it for all cameras

            // If it isn't divisible by 2, we move it one back.  

            if (currentExposure.toCamera.x_bin == 1 && (currentExposure.toCamera.x_offset % 2) != 0)
            {
                currentExposure.toCamera.x_offset--;
                Log.Write(String.Format("after x bincheck x_offset = {0}\n", currentExposure.toCamera.x_offset));
            }

            if (currentExposure.toCamera.y_bin == 1 && (currentExposure.toCamera.y_offset % 2) != 0)
            {
                currentExposure.toCamera.y_offset--;
                Log.Write(String.Format("after y bincheck y_offset = {0}\n", currentExposure.toCamera.y_offset));
            }

            Debug.Assert(currentExposure.toCamera.x_bin != 1 || currentExposure.toCamera.x_offset % 2 == 0);
            Debug.Assert(currentExposure.toCamera.y_bin != 1 || currentExposure.toCamera.y_offset % 2 == 0);
            Debug.Assert(currentExposure.toCamera.width % currentExposure.toCamera.x_bin == 0);
            Debug.Assert(currentExposure.toCamera.height % currentExposure.toCamera.y_bin == 0);

            dumpReadDelayedBlock(currentExposure.toCamera, "after bayer adjust, before M25C adjustment");

            // See if our modified parameters are still legal
            try
            {
                checkParms(true, currentExposure.toCamera.width, currentExposure.toCamera.height, currentExposure.toCamera.x_offset, currentExposure.toCamera.y_offset, currentExposure.toCamera.x_bin, currentExposure.toCamera.y_bin);
            }
            catch (Exception ex)
            {
                Log.Write(String.Format("checkParms for toCamera generated exception {0}\n", ex));
                throw;
            }

            // interlaced cameras require some special care
            // we have to divide the offset and the height by 2, but that can lead to
            // issues
            // +--------+--------+-------+----------------
            // | Height | Offset | First | Adjustments
            // +--------+--------+-------+----------------
            // |  Even  +  Even  | Even  | None
            // +--------+--------+-------+----------------
            // |  Even  |  Odd   | Odd   | None
            // +--------+--------+-------+----------------
            // |  Odd   |  Even  | Even  | second height - 1
            // +--------+--------+-------+----------------
            // |  Odd   |  Odd   | Odd   | second height - 1
            // +--------+--------+------------------------
            // 

            if (interlaced)
            {
                bool offsetIsOdd = (currentExposure.userRequested.y_offset % 2) != 0;
                bool heightIsOdd = (currentExposure.userRequested.height % 2) != 0;

                currentExposure.toCameraSecond = currentExposure.toCamera;

                if (heightIsOdd)
                {
                    currentExposure.toCamera.height += 1;
                }

                if (offsetIsOdd)
                {
                    fieldFlags = SX_CCD_FLAGS_FIELD_ODD;
                }
                else
                {
                    fieldFlags = SX_CCD_FLAGS_FIELD_EVEN;
                }

                // See if our modified parameters are still legal
                try
                {
                    checkParms(true, currentExposure.toCamera.width, currentExposure.toCamera.height, currentExposure.toCamera.x_offset, currentExposure.toCamera.y_offset, currentExposure.toCamera.x_bin, currentExposure.toCamera.y_bin);
                }
                catch (Exception ex)
                {
                    Log.Write(String.Format("checkParms for toCamera generated exception {0}\n", ex));
                    throw;
                }

                // Check the second frame's parameters
                try
                {
                    checkParms(true, currentExposure.toCameraSecond.width, currentExposure.toCameraSecond.height, currentExposure.toCameraSecond.x_offset, currentExposure.toCameraSecond.y_offset, currentExposure.toCameraSecond.x_bin, currentExposure.toCameraSecond.y_bin);
                }
                catch (Exception ex)
                {
                    Log.Write(String.Format("checkParms for toCameraSecond generated exception {0}\n", ex));
                    throw;
                }


                dumpReadDelayedBlock(currentExposure.toCamera, "after progressive adjustments");
                dumpReadDelayedBlock(currentExposure.toCameraSecond, "second frame after progressive adjustments");
            }

            // Make any adjustments required for specific cameras

            // I have no idea why the next bit is required, but it is.  If it isn't there,
            // the read of the image data fails. I found this in the sample basic application from SX.

            if (idx == 0 && (CameraModels)cameraModel == CameraModels.MODEL_M25C)
            {
                currentExposure.toCamera.width *= 2;

                // in order for this to work, the height must be even

                if (currentExposure.toCamera.height % 2 == 1)
                {
                    if (currentExposure.toCamera.height + currentExposure.toCamera.y_bin <= ccdHeight)
                    {
                        currentExposure.toCamera.height += currentExposure.toCamera.y_bin;
                    }
                    else
                    {
                        currentExposure.toCamera.height -= currentExposure.toCamera.y_bin;
                    }
                }

                Debug.Assert(currentExposure.toCamera.height % 2 == 0);
                currentExposure.toCamera.height /= 2;

                currentExposure.toCamera.x_offset *= 2;

                // The y_offset must also be even.

                if (currentExposure.toCamera.y_offset % 2 == 1)
                {
                    Debug.Assert(currentExposure.toCamera.y_offset >= currentExposure.toCamera.y_bin);
                    currentExposure.toCamera.y_offset -= currentExposure.toCamera.y_bin;
                    Debug.Assert(currentExposure.toCamera.y_offset % 2 == 0);
                }

                currentExposure.toCamera.y_offset /= 2;

                dumpReadDelayedBlock(currentExposure.toCamera, "after M25C adjustment");
            }

            return fieldFlags;
        }

        internal void dumpReadBlock(SX_READ_BLOCK block)
        {
            Log.Write(String.Format("\tx_offset={0:d}, y_offset={1:d}\n", block.x_offset, block.y_offset));
            Log.Write(String.Format("\twidth={0:d}, height={1:d}\n", block.width, block.height));
            Log.Write(String.Format("\tx_bin={0:d}, y_bin={1:d}\n", block.x_bin, block.y_bin));
        }

        internal void dumpReadBlock(SX_READ_BLOCK block, string title)
        {
            Log.Write("Read Block:" + title + "\n");
            dumpReadBlock(block);
        }

        internal void dumpReadDelayedBlock(SX_READ_DELAYED_BLOCK block, string title)
        {
            SX_READ_BLOCK readBlock;

            initReadBlock(block, out readBlock);

            Log.Write("Read Delayed Block:" + title + "\n");
            dumpReadBlock(readBlock);
            Log.Write(String.Format("\tdelay={0:d}\n", block.delay));
        }

        internal void buildReadDelayedBlock(out SX_READ_DELAYED_BLOCK block, UInt16 x_offset, UInt16 y_offset, UInt16 width, UInt16 height, Byte x_bin, Byte y_bin, UInt32 delay)
        {
            block.width = width;
            block.height = height;

            block.x_offset = x_offset;
            block.y_offset = y_offset;

            block.x_bin = x_bin;
            block.y_bin = y_bin;

            block.delay = delay;

            dumpReadDelayedBlock(block, "buildReadDelayedBlock");
        }

        internal void clear(Byte Flags)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;

            if ((Flags & ~(SX_CCD_FLAGS_NOWIPE_FRAME | SX_CCD_FLAGS_TDI | SX_CCD_FLAGS_NOCLEAR_FRAME)) != 0)
            {
                throw new ArgumentException("Invalid flags passed to ClearPixels");
            }

            m_controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_CLEAR_PIXELS, Flags, idx, 0);

            Log.Write("clear about to Write\n");
            m_controller.Write(cmdBlock, out numBytesWritten);
            Log.Write("clear about to return\n");
        }

        public void echo(string s)
        {
            m_controller.echo(s);
        }

        public void clearCcdPixels()
        {
            Log.Write("clearCcdPixels entered\n");
            clear(0);
            Log.Write("clearCcdPixels returns\n");
        }

        public void clearRecordedPixels()
        {
            Log.Write("clearRecordedPixels entered\n");
            clear(SX_CCD_FLAGS_NOWIPE_FRAME);
            Log.Write("clearRecordedPixels returns\n");
        }

        public UInt16 getModel()
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;
            byte[] bytes = new byte[2];
            UInt16 model = 0;

            m_controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, SX_CMD_CAMERA_MODEL, 0, idx, (UInt16)Marshal.SizeOf(model));

            lock (m_controller)
            {
                Log.Write("getModel has locked\n");
                m_controller.Write(cmdBlock, out numBytesWritten);

                bytes = m_controller.ReadBytes(Marshal.SizeOf(model), out numBytesRead);
            }
            Log.Write("getModel has unlocked\n");

            model = System.BitConverter.ToUInt16(bytes, 0);

            return model;
        }

        internal void setCoolerInfo(ref SX_COOLER_BLOCK inBlock, out SX_COOLER_BLOCK outBlock)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;
            byte[] bytes = new byte[Marshal.SizeOf(inBlock)];

            Log.Write(String.Format("setCoolerInfo inBlock temp={0} enabled={1}\n", inBlock.coolerTemp, inBlock.coolerEnabled));
            m_controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, SX_CMD_COOLER_CONTROL, inBlock.coolerTemp, (UInt16)inBlock.coolerEnabled, 0);

            lock (m_controller)
            {
                Log.Write("setCooler has locked\n");
                m_controller.Write(cmdBlock, out numBytesWritten);

                bytes = m_controller.ReadBytes(Marshal.SizeOf(inBlock), out numBytesRead);
            }
            Log.Write("setCooler has unlocked\n");

            outBlock.coolerTemp = System.BitConverter.ToUInt16(bytes, 0);
            outBlock.coolerEnabled = bytes[Marshal.SizeOf(outBlock.coolerTemp)];

            Log.Write(String.Format("setCoolerInfo outBlock temp={0} enabled={1}\n", outBlock.coolerTemp, outBlock.coolerEnabled));
        }

        public UInt16 coolerSetPoint
        {
            get
            {
                if (!hasCoolerControl)
                {
                    Log.Write("coolerTemp get(): called when not avaialable - throwing exception\n");
                    throw new System.Exception("coolerTemp get(): Temperature not available");
                }

                Log.Write(String.Format("coolerSetPoint returning {0}\n", m_coolerBlock.coolerTemp));
                return m_coolerBlock.coolerTemp;
            }
            set
            {
                if (!hasCoolerControl)
                {
                    Log.Write("coolerTemp set(): called when not available - throwing exception\n");
                    throw new System.Exception("coolerTemp set(): Invalid attempt to control cooler");
                }

                SX_COOLER_BLOCK tempBlock;
                m_coolerBlock.coolerTemp = value;
                Log.Write(String.Format("setting cooler temperature to {0}\n", m_coolerBlock.coolerTemp));
                setCoolerInfo(ref m_coolerBlock, out tempBlock);
            }
        }

        public UInt16 coolerTemp
        {
            get
            {
                if (!hasCoolerControl)
                {
                    Log.Write("coolerTemp get(): called when not avaialable - throwing exception\n");
                    throw new System.Exception("coolerTemp get(): Temperature not available");
                }

                SX_COOLER_BLOCK outBlock;
                setCoolerInfo(ref m_coolerBlock, out outBlock);

                Log.Write(String.Format("coolerTemp returning {0}\n", outBlock.coolerTemp));

                return outBlock.coolerTemp;
            }
        }

        public bool coolerEnabled
        {
            get
            {
                if (!hasCoolerControl)
                {
                    Log.Write("coolerEnabled get(): called when not available - throwing exception\n");
                    throw new System.Exception("coolerEnabled get(): Invalid attempt to control cooler");
                }

                Log.Write(String.Format("coolerEnabled get(): returning {0}\n", m_coolerBlock.coolerEnabled));
                return m_coolerBlock.coolerEnabled != 0;
            }
            set
            {
                if (!hasCoolerControl)
                {
                    Log.Write("coolerEnabled set(): called when not available - throwing exception\n");
                    throw new System.Exception("coolerEnabled() set: Invalid attempt to control cooler");
                }

                if (value)
                {
                    m_coolerBlock.coolerEnabled = 1;
                }
                else
                {
                    m_coolerBlock.coolerEnabled = 0;
                }
                SX_COOLER_BLOCK tempBlock;
                Log.Write(String.Format("setting coolerEnabled={0}\n", m_coolerBlock.coolerEnabled));
                setCoolerInfo(ref m_coolerBlock, out tempBlock);
            }
        }

        public bool hasCoolerControl
        {
            get { return (ccdParms.extra_capabilities & SXUSB_CAPS_COOLER) == SXUSB_CAPS_COOLER; }
        }

        void dumpParams(SX_CCD_PARAMS parms)
        {
            Log.Write(String.Format("params:\n"));
            Log.Write(String.Format("\thfront_porch={0:d}, hback_porch={1:d}\n", parms.hfront_porch, parms.hback_porch));
            Log.Write(String.Format("\tvfront_porch={0:d}, vback_porch={1:d}\n", parms.vfront_porch, parms.vback_porch));
            Log.Write(String.Format("\twidth={0:d}, height={1:d}\n", parms.width, parms.height));
            Log.Write(String.Format("\tpixel_uwidth={0:d}, pixel_uheight={1:d}\n", parms.pixel_uwidth, parms.pixel_uheight));
            Log.Write(String.Format("\tbits_per_pixel={0:d}, num_serial_ports={1:d}\n", parms.bits_per_pixel, parms.num_serial_ports));
            Log.Write(String.Format("\tcolor_matrix=0x{0:x}, extra_capabilitites=0x{1:x}\n", parms.color_matrix, parms.extra_capabilities));
        }

        void getParams(ref SX_CCD_PARAMS parms)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;

            m_controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, SX_CMD_GET_CCD_PARMS, 0, idx, 0);

            lock (m_controller)
            {
                Log.Write("getParams has locked\n");
                m_controller.Write(cmdBlock, out numBytesWritten);

                parms = (SX_CCD_PARAMS)m_controller.ReadObject(typeof(SX_CCD_PARAMS), out numBytesRead);
            }

            Log.Write("getParams has unlocked\n");

            dumpParams(parms);
        }

        internal void convertCameraDataToImageData()
        {
            Int32 binnedWidth  = currentExposure.userRequested.width / currentExposure.userRequested.x_bin;
            Int32 binnedHeight = currentExposure.userRequested.height / currentExposure.userRequested.y_bin;

            if (bitsPerPixel != 16 && bitsPerPixel != 8)
            {
                throw new ArgumentOutOfRangeException("downloadPixels(): bitsPerPixel != 16 && bitPerPixel !=8", "bitsPerPixel");
            }

            imageData = new Int32[binnedWidth, binnedHeight];

            // Copy the bytes read from the camera into a UInt32 array.
            // There must be a better way to do this, but I don't know what it is. 

            Log.Write(String.Format("convertCameraDataToImageData(): x_bin = {0} binnedWidth={1} binnedHeight={2}\n", currentExposure.toCamera.x_bin, binnedWidth, binnedHeight));

            if (idx == 0 && cameraModel == 0x59)
            {

                Log.Write("convertCameraDataToImageData(): decoding M25C data\n");

                // to go along with the odd requirement that we must double the width and halve the height 
                // to read the data from MX25C, we have to unscramble the data here
                //
                // Before we took the picture, we adjusted the parameters we sent to the camera so that:
                // - The subimage starts on an even pixel if it is unbinned
                // - The width and height are even multiples of the respective binning factors
                // 
                // A single row of the returned data is unpacked into 2 image rows like this:
                //
                // Input row:   ABCDEFGHIJKL...
                // Output Row1: ADEHIL...
                // Output Row2: BCFGJK...
                //

                Int32 cameraBinnedWidth = currentExposure.toCamera.width / currentExposure.toCamera.x_bin;
                Int32 cameraBinnedHeight = currentExposure.toCamera.height / currentExposure.toCamera.y_bin;
                int srcIdx = 0;
                int x = -1, y = -1;

                // get the binned height and width from the camera. We may not have quite enough data
                // from the camera to fill the output array, so there could be zeros on the right or on the
                // bottom
                // Consider
                //    sensor: 16x16, binning: 5
                //    height x width = 3x3 (9 pixels), but the swizzle will get us 6x1 (6 pixels)
                // The adjustment below would then get us 3x2, and we would put those 6 pixels into the
                // "upper left" of the output array, leaving zeros in the "lower right"
                //
                // In the adustment code, we try to bump the number height if we can, so the only time this 
                // will actually happen is if we are full height and binning in a way that gets us an odd
                // number of rows (2016 binned 5, gives 403 rows. The swizzle will leave us with 403/2 
                // 201 rows that becomes 402 rows when we unswizzle it here, so the last row will be 0

                cameraBinnedWidth /= 2;
                cameraBinnedHeight *= 2;

                Log.Write(String.Format("convertCameraDataToImageData(): cameraBinnedWidth = {0} cameraBinnedHeight={1}\n", cameraBinnedWidth, cameraBinnedHeight));
                Log.Write(String.Format("convertCameraDataToImageData(): userPixels = {0}, cameraPixes={1}, len={2}\n", binnedWidth * binnedHeight, cameraBinnedWidth * cameraBinnedHeight, imageRawData.Length));

                int saveCase = -2;

                try
                {
                    for (y = 0; y < cameraBinnedHeight; y += 2)
                    {

                        // This loop is based on cameraBinnedWidth, not binnedWidth because we have to 
                        // get all the pixels accoss each row every time, even if we don't want them all.
                        // Otherwise we will give the wrong data to next ro
                        for (x = 0; x < cameraBinnedWidth; x += 2)
                        {
                            UInt16 pixel;

                            // This inner loop handles 4 pixels.  
                            //
                            // It is possible that we requested more pixels from the camera than the user
                            // requested from us, so we have to check to make sure before we assign them
                            //

                            saveCase = 0;
                            pixel = (UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                            if (x < binnedWidth && y < binnedHeight)
                            {
                                imageData[x, y] = pixel;
                            }

                            saveCase++;
                            pixel = (UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                            if (x < binnedWidth && y + 1 < binnedHeight)
                            {
                                imageData[x, y + 1] = pixel;
                            }


                            if (x + 1 < cameraBinnedWidth)
                            {
                                saveCase++;
                                pixel = (UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                                if (x + 1 < binnedWidth && y + 1 < binnedHeight)
                                {
                                    imageData[x + 1, y + 1] = pixel;
                                }

                                saveCase++;
                                pixel = (UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                                if (x + 1 < binnedWidth && y < binnedHeight)
                                {
                                    imageData[x + 1, y] = pixel;
                                }
                            }
                        }
                    }

                    if (srcIdx != imageRawData.Length)
                    {
                        Log.Write(String.Format("surprise: srcIdx ({0}) != imageData.Length ({1})\n", srcIdx, imageRawData.Length));
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Write(String.Format("convertCameraDataToImageData(): Caught an exception processing M25C data\n"));
                    Log.Write(String.Format("x = {0} y={1} srcIdx={2} case={3}\n", x, y, srcIdx, saveCase));
                    Log.Write("Exception data was: \n" + ex.ToString() + "\n");
                    throw ex;
                }
            }
            else
            {
                int srcIdx = 0;
                int x, y;

                try
                {
                    for (y = 0; y < binnedHeight; y++)
                    {
                        for (x = 0; x < binnedWidth; x++)
                        {
                            imageData[x, y] = (UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Write(String.Format("convertCameraDataToImageData(): Caught an exception processing non-M25C data - {0}\n", ex.ToString()));
                    throw ex;
                }
            }

            Log.Write("convertCameraDataToImageData(): ends\n");
        }

        internal void setPixelType()
        {
            switch (bitsPerPixel)
            {
                case 8:
                    pixelType = typeof(System.Byte);
                    break;
                case 16:
                    pixelType = typeof(System.Int16);
                    break;
                case 32:
                    pixelType = typeof(System.Int32);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(String.Format("Unexpected bitsPerPixel {0}", bitsPerPixel), "bitsPerPixel");
            }
            Log.Write(String.Format("setPixelType set the type to {0}\n", pixelType));
        }

        internal void downloadPixels(bool setValid, SX_READ_DELAYED_BLOCK exposure)
        {
            Int32 numBytesRead;
            Int32 binnedWidth = exposure.width / exposure.x_bin;
            Int32 binnedHeight = exposure.height / exposure.y_bin;
            Int32 imagePixels = binnedWidth * binnedHeight;

            Log.Write(String.Format("downloadPixels(): requesting {0} pixels, {1} bytes each ({2} bytes)\n", imagePixels, Marshal.SizeOf(pixelType), imagePixels * Marshal.SizeOf(pixelType)));

            imageRawData = (Array)m_controller.ReadArray(pixelType, imagePixels, out numBytesRead);

            Log.Write(String.Format("typeof(pixelType)= {0}\n", pixelType.GetType()));
            Log.Write("typeof(imageRawDate)=" + imageRawData.GetType().ToString() + "\n");

            lock (oImageDataLock)
            {
                if (setValid)
                {
                    imageDataValid = true;
                    imageData = null;
                }
                else
                {
                    Debug.Assert(imageDataValid == false);
                }
            }

            Log.Write("downloadPixels(): read completed, numBytesRead=" + numBytesRead + "\n");

#if false
            using (BinaryWriter binWriter = new BinaryWriter(File.Open("c:\\temp\\sx-ascom\\image.raw", FileMode.Create)))
            {
                int srcIdx = 0;
                for (int xx = 0; xx < binnedWidth; xx++)
                {
                    for (int yy = 0; yy < binnedHeight; yy++)
                    {
                        binWriter.Write((UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++)));
                    }
                }
            }
#endif
        }

        public void guideNorth(int durationMS)
        {
            m_controller.guide(SX_STAR2K_NORTH, durationMS);
        }

        public void guideSouth(int durationMS)
        {
            m_controller.guide(SX_STAR2K_SOUTH, durationMS);
        }

        public void guideEast(int durationMS)
        {
            m_controller.guide(SX_STAR2K_EAST, durationMS);
        }

        public void guideWest(int durationMS)
        {
            m_controller.guide(SX_STAR2K_WEST, durationMS);
        }

        public void recordPixels(bool bDelayed, out DateTimeOffset exposureEnd)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;

            Log.Write(String.Format("recordPixels entered - bDelayed = {0}\n", bDelayed));

            UInt16 firstExposureFlags = adjustReadDelayedBlock();
            UInt16 secondExposureFlags = (UInt16)((SX_CCD_FLAGS_FIELD_EVEN | SX_CCD_FLAGS_FIELD_ODD) & ~firstExposureFlags);

            // We invalidate the data here, so that there is no chance we have to wait for someone
            // to download an image later in the routine when the data is ready for download
            lock (oImageDataLock)
            {
                imageDataValid = false;
            }

            lock (m_controller)
            {
                Log.Write("recordPixels() has locked\n");

                if (bDelayed)
                {
                    m_controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS,
                                                    SX_CMD_READ_PIXELS_DELAYED,
                                                    firstExposureFlags,
                                                    idx,
                                                    (UInt16)Marshal.SizeOf(currentExposure.toCamera));
                    m_controller.Write(cmdBlock, currentExposure.toCamera, out numBytesWritten);
                }
                else
                {
                    SX_READ_BLOCK readBlock;

                    initReadBlock(currentExposure.toCamera, out readBlock);

                    m_controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS,
                                                    SX_CMD_READ_PIXELS,
                                                    firstExposureFlags,
                                                    idx,
                                                    (UInt16)Marshal.SizeOf(readBlock));
                    m_controller.Write(cmdBlock, readBlock, out numBytesWritten);
                }

                Log.Write(String.Format("recordPixels() requested read, flags = {0}\n", firstExposureFlags)); 
                exposureEnd = DateTimeOffset.Now;
                Log.Write("recordPixelsDelayed requesting download\n");
                downloadPixels(progressive, currentExposure.toCamera);
                Log.Write("recordPixels() download completed\n");

                if (interlaced)
                {
                    SX_READ_BLOCK readBlock;
                    Array firstFrame = imageRawData;
                    Log.Write("recordPixels() preparing for second frame download\n");

                    Debug.Assert(secondExposureFlags != 0);

                    initReadBlock(currentExposure.toCamera, out readBlock);

                    m_controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS,
                                                    SX_CMD_READ_PIXELS,
                                                    secondExposureFlags,
                                                    idx,
                                                    (UInt16)Marshal.SizeOf(readBlock));

                    Log.Write(String.Format("recordPixels() second frame requesting read, flags = {0}\n", secondExposureFlags));
                    m_controller.Write(cmdBlock, readBlock, out numBytesWritten);
                    exposureEnd = DateTimeOffset.Now;
                    Log.Write("recordPixelsDelayed second frame requesting download\n");
                    downloadPixels(false, currentExposure.toCameraSecond);
                    Log.Write("recordPixels() second frame download completed\n");
                    try
                    {
                        stitchImages(firstFrame, imageRawData);
                    }
                    catch (System.Exception ex)
                    {
                        Log.Write(String.Format("stitch caught an exception: {0}\n", ex.ToString()));
                    }
                }
            }
            Log.Write("recordPixels() has unlocked\n");
        }

        internal void stitchImages(Array firstFrame, Array secondFrame)
        {

            int totalLines = currentExposure.toCamera.height + currentExposure.toCameraSecond.height;
            int width = currentExposure.toCamera.width;

            Log.Write(String.Format("stitching interlaced images. totalLines={0} width={1}\n", totalLines, width));

            imageRawData = System.Array.CreateInstance(pixelType, totalLines * width);

            Log.Write(String.Format("firstFrame:   typeof()={0} rank={1} length={2}\n", firstFrame.GetType().ToString(), firstFrame.Rank, firstFrame.LongLength));
            Log.Write(String.Format("secondFrame:  typeof()={0} rank={1} length={2}\n", secondFrame.GetType().ToString(), secondFrame.Rank, secondFrame.LongLength));
            Log.Write(String.Format("imageRawData: typeof()={0} rank={1} length={2}\n", imageRawData.GetType().ToString(), secondFrame.Rank, imageRawData.LongLength));

            for (int line=0;line < totalLines; line += 2)
            {
                // copy line from the first image
                try
                {
                    Array.Copy(firstFrame,    line/2 * width,
                               imageRawData,  line   * width, 
                               width);
                }
                catch
                {
                    Log.Write(String.Format("stitch exception in line={0} first Copy(firstFrame, {1}, imageRawData, {2}, {3})\n", line, line/2*width, line*width, width));
                    throw;
                }

                // if we have an odd number of lines in the requested exposure, 
                // the first frame can have 1 more line that the second, so we have
                // to check to make sure that there is a line in the second image
                if (line < currentExposure.toCameraSecond.height)
                {
                    // copy second line
                    try
                    {
                        Array.Copy(secondFrame,   line/2   * width,
                                   imageRawData,  (line+1) * width, 
                                   width);
                    }
                    catch
                    {
                        Log.Write(String.Format("stitch exception in line={0} second Copy(firstFrame, {1}, imageRawData, {2}, {3})\n", line, line/2*width, (line+1)*width, width));
                        throw;
                    }
                }
            }

            // now that we have built the requested image, mark it as valid
            lock (oImageDataLock)
            {
                imageDataValid = true;
                imageData = null;
            }
            Log.Write("stitched interlaced images, set imageDataValid = true\n");
        }
    }
}
