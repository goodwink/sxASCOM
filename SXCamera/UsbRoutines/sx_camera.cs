//#define INTERLACED_DEBUG
//#define USE_DUMPED_DATA

using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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
        // ------------------ CoStar --------------------
        MODEL_COSTAR = 0x27,
    }

    public partial class Camera
        : sxBase
    {
        // Variables
        private Controller m_controller;
        private SX_CCD_PARAMS ccdParms;
        private SX_READ_DELAYED_BLOCK nextExposure;
        private EXPOSURE_INFO currentExposure;
        private byte [] rawFrame1;
        private byte [] rawFrame2;
        private Int32[,] imageData;
        private bool imageDataValid;
        private object oImageDataLock;
        private UInt16 idx;
        private SX_COOLER_BLOCK m_coolerBlock;
        private bool m_dump = true;
        private bool m_useDumped = false;

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

        public int fullWellCapacity
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

        // ccdWidth and ccdHeight are the sizes than can be read from the ccd
        internal UInt16 ccdWidth
        {
            get { return ccdParms.width; }
        }

        internal UInt16 ccdHeight
        {
            get { return ccdParms.height; }
        }

        // frameWidth and frameHeight are the sizes that the caller request
        public UInt16 frameWidth
        {
            get 
            { 
                UInt16 ret = ccdWidth;
                
                if ((CameraModels)cameraModel == CameraModels.MODEL_COSTAR)
                {
                    ret -= 16;
                }

                return ret;
            }
        }

        public UInt16 frameHeight
        {
            get 
            { 
                UInt16 ret; ret=ccdHeight;

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
            get 
            { 
                double dRet =  ccdParms.pixel_uheight / (double)256; 

                // interlaced cameras report their pixel height as 2X what
                // it actually is
                if (idx == 0 && interlaced)
                {
                    dRet /= 2.0;
                }

                return dRet;
            }
        }

        public Byte bytesPerPixel
        {
            get;
            private set;
        }

        public Byte bitsPerPixel
        {
            get { return ccdParms.bits_per_pixel; }
        }

        public Boolean hasGuideCamera
        {
            get { return !m_useDumped && m_controller.hasGuideCamera; }
        }

        public Boolean hasGuidePort
        {
            get { return !m_useDumped && m_controller.hasGuidePort; }
        }

        private Byte extraCapabilities
        {
            get { return ccdParms.extra_capabilities; }
        }

        public UInt16 colorMatrix
        {
            get { return ccdParms.color_matrix; }
        }

        private Boolean hasBiasData
        {
            get;
            set;
        }

        private UInt32 numBiasPixels
        {
            get
            {
                UInt32 ret = 0;

                if (!hasBiasData)
                {
                    throw new System.Exception("numBiasPixels() called when hasBiasData == false");
                }

                if ((CameraModels)cameraModel == CameraModels.MODEL_COSTAR)
                {
                    ret = 16;
                }

                return ret;
            }
        }

        public UInt16 xOffset
        {
            get { return nextExposure.x_offset; }
            set
            {
                if (value > frameWidth)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid xOffset {0} 0<=xOffset<={1}", value, frameWidth), "xOffset");
                }

                if (value + width > frameWidth)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid xOffset + width: 0 < xOffset {0} + width {1} <= {2}", value, width, frameWidth), "xOffset");
                }

                nextExposure.x_offset = value;
            }
        }

        public UInt16 yOffset
        {
            get { return nextExposure.y_offset; }
            set
            {
                if (value >= frameHeight)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid yOffset {0} 0<=yOffset<={1}", value, frameHeight), "yOffset");
                }
                if (value + height > frameHeight)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid yOffset + height: 0 < yOffset {0} + height {1} <= {2}", value, height, frameHeight), "yOffset");
                }
                nextExposure.y_offset = value;
            }
        }

        public UInt16 width
        {
            get { return nextExposure.width; }
            set
            {
                if (value == 0 || value > frameWidth)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid width {0} 1<=width<={1}", value, frameWidth), "width");
                }
                nextExposure.width = value;
            }
        }

        public UInt16 height
        {
            get { return nextExposure.height; }
            set
            {
                if (value == 0 || value > frameHeight)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid height {0} 1<=height<={1}", value, frameHeight), "height");
                }
                nextExposure.height = value;
            }
        }

        public Byte xBin
        {
            get { return nextExposure.x_bin; }
            set
            {
                if (value <= 0 || value > maxXBin || value == 7)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid xBin {0} 1<=height<={1}", value, maxXBin), "xBin");
                }
                nextExposure.x_bin = value;
            }
        }

        public Byte yBin
        {
            get { return nextExposure.y_bin; }
            set
            {
                if (value <= 0 || value > maxYBin || value == 7)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid yBin {0} 1<=height<={1}", value, maxYBin), "yBin");
                }

                nextExposure.y_bin = value;
            }
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

                    Debug.Assert(imageData != null);
                    return imageData;
                }
            }
        }

        public bool mustUseHardwareTimer
        {
            get;
            private set;
        }


        // setINFO sets information for the camera.  It must set:
        // - description
        // - fullWellCapacity
        // - electronsPerADU
        // - progressive
        private void setInfo(bool bAllowUntested)
        {
            bool bUntested = false;

            // compute the pixel size
            setPixelBytes();

            // Most cameras don't have any bias restrictions
            maxXBin = MAX_X_BIN;
            maxYBin = MAX_Y_BIN;

            // Most cameras don't have bias data, so default is false
            hasBiasData = false;

            // Most cameras can use software timing, so default is false
            mustUseHardwareTimer = false;

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
                    progressive = true;
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
                case CameraModels.MODEL_COSTAR:
                    description = "CoStar";
                    fullWellCapacity = 20000;
                    electronsPerADU = 0.3;
                    progressive = true;
                    maxXBin = 1;
                    maxYBin = 1;
                    mustUseHardwareTimer = true;
                    hasBiasData = true;
                    break;
                case CameraModels.MODEL_LX1:
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
                    fullWellCapacity = 1;
                    electronsPerADU = 1.0;
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
             }
        }

        public Camera(Controller controller, UInt16 cameraIdx, bool bAllowUntested)
        {
            Log.Write(String.Format("sx.Camera() constructor: controller={0} cameraIdx={1}\n", controller, cameraIdx));

            idx = cameraIdx;

            m_controller = controller;

            if (m_useDumped)
            {
                m_controller = null;
            }

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

            getModel();
            getCCDParams();

            setInfo(bAllowUntested);

            buildReadDelayedBlock(out nextExposure, 0, 0, frameWidth, frameHeight, 1, 1, 0);

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

        internal Byte maxXBin
        {
            get;
            set;
        }

        internal Byte maxYBin
        {
            get;
            set;
        }

        internal void checkParms(bool useCCDSizes, SX_READ_DELAYED_BLOCK exposure)
        {
            UInt16 effectiveHeight = frameHeight;
            UInt16 effectiveWidth = frameWidth;

            if (useCCDSizes)
            {
                effectiveHeight = ccdHeight;
                effectiveWidth = ccdWidth;
            }

            if (exposure.width > effectiveWidth)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid width {0}: 0<=width<={1}", exposure.width, effectiveWidth), "width");
            }

            if (exposure.height > effectiveHeight)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid height {0}: 0<=height<={1}", exposure.height, effectiveHeight), "height");
            }

            if (exposure.x_offset > effectiveWidth)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid xOffset {0}: 0<=width<={1}", exposure.x_offset, effectiveWidth), "xOffset");
            }

            if (exposure.y_offset > effectiveHeight)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid yOffset {0}: 0<=yOffset<={1}", exposure.y_offset, effectiveHeight), "yOffset");
            }

            if (exposure.x_offset + exposure.width > effectiveWidth)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid xOffset + width: 0 < xOffset {0} + width {1} <= {2}", exposure.x_offset, exposure.width, effectiveWidth), "width+xOffset");
            }

            if (exposure.y_offset + exposure.height > effectiveHeight)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid yOffset + height: 0 < yOffset {0} + height {1} <= {2}", exposure.y_offset, exposure.height, effectiveHeight), "height+yOffset");
            }

            // The following if disallows asymmetric binning. The SX cameras will do it, but it there are difficulties that arise from
            // asymmetric binning and bayer matrices that I don't want to deal with at this point...
            if (xBin != yBin)
            {
                throw new ArgumentOutOfRangeException("xBin != yBin");
            }

            if (exposure.width % exposure.x_bin != 0)
            {
                throw new ArgumentOutOfRangeException(String.Format("width ({0}) % x_bin ({1}) != 0 ({2})", exposure.width, exposure.x_bin, exposure.width % exposure.x_bin));
            }

            if (exposure.height % exposure.y_bin != 0)
            {
                throw new ArgumentOutOfRangeException(String.Format("height ({0}) % y_bin ({1}) != 0 ({2}", exposure.height, exposure.y_bin, exposure.height % exposure.y_bin));
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

            dumpReadDelayedBlock(currentExposure.userRequested, "exposure as requested - before any adjustments");

            checkParms(false, currentExposure.userRequested);

            currentExposure.toCamera = currentExposure.userRequested;

            // the following adjustments only apply to main cameras, since the SXV guide camera
            // is progressive and monochrome
            if (idx == 0)
            {
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

                dumpReadDelayedBlock(currentExposure.toCamera, "after bayer adjustments");

                // See if our modified parameters are still legal
                try
                {
                    checkParms(false, currentExposure.toCamera);
                }
                catch (Exception ex)
                {
                    Log.Write(String.Format("checkParms after bayer adjustment generated exception {0}\n", ex));
                    throw;
                }

                // interlaced cameras require some special care.  
                // We have to divide the y_offset and the height by 2

                if (interlaced)
                {
                    // if we are binning, it is possible that dividing the height by 2 will elminiate a
                    // a binned row that would exist in a progressive camera
                    //
                    // suppose a sensor has 10 lines and we are binning by 3.  If the sensor was progressive,
                    // there would be 3 binned lines.  But with a progressive sensor, we can only get 2 binned lines, 
                    // 1 from each "half" of the sensor

                    UInt16 yBin = currentExposure.userRequested.y_bin;
                    UInt16 binnedOffset = (UInt16)(currentExposure.toCamera.y_offset/yBin);
                    currentExposure.toCamera.y_offset = (UInt16)(binnedOffset/2 * yBin);

                    UInt16 binnedRows = (UInt16)(currentExposure.toCamera.height/yBin);
                    currentExposure.toCamera.height = (UInt16)(binnedRows/2 * yBin);

                    // See if our modified parameters are still legal
                    try
                    {
                        checkParms(true, currentExposure.toCamera);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(String.Format("checkParms after intial interlaced adjustments generated an exception {0}\n", ex));
                        throw;
                    }

                    dumpReadDelayedBlock(currentExposure.toCamera, "after initial progressive adjustments");

                    bool binnedOffsetIsOdd = ((currentExposure.userRequested.y_offset/yBin) % 2) != 0;
                    bool binnedHeightIsOdd = ((currentExposure.userRequested.height/yBin) % 2) != 0;

                    Log.Write(String.Format("making final interlaced adjustments, binnedOffsetIsOdd={0}, binnedHeightIsOdd={1}\n", binnedOffsetIsOdd, binnedHeightIsOdd));

                    currentExposure.toCameraSecond = currentExposure.toCamera;

                    Log.Write(String.Format("binnedHeightIsOdd = {0}, height={1}, ccdHeight={2}\n", binnedHeightIsOdd, currentExposure.userRequested.height, ccdHeight));

                    if (binnedOffsetIsOdd)
                    {
                        fieldFlags = SX_CCD_FLAGS_FIELD_ODD;
                    }
                    else
                    {
                        fieldFlags = SX_CCD_FLAGS_FIELD_EVEN;
                    }

                    // if the height is odd, we need to adjust the height of the first frame because after the division by
                    // two for interlaced, we have lost a row.
                    //
                    // For example, if the original height was 3, the height of both frames after division is 1, which 
                    // leaves us capturing only 2 lines.  We add one back if we can without falling off the bottom
                    //

                    if (binnedHeightIsOdd && currentExposure.toCamera.y_offset + currentExposure.toCamera.height + yBin < ccdHeight)
                    {
                        Log.Write(String.Format("adding {0} to first camera height ({1}) for odd height\n", yBin, currentExposure.toCamera.height));
                        currentExposure.toCamera.height += yBin;
                    }

                    int expectedRows = currentExposure.userRequested.height/yBin;
                    int actualRows = currentExposure.toCamera.height/yBin + currentExposure.toCameraSecond.height/yBin;
                    int rowDelta = expectedRows - actualRows;

                    Debug.Assert(rowDelta == 0 || rowDelta == 1);

                    if (rowDelta == 1)
                    {
                        // Note we are losing a line that we might be able to recover by trying to move everthing up.
                        // This is hard so we just let the user end up with a black line a the bottom.  
                        //
                        // This is where the code to do that would go if we were doing it
                        
                        Log.Write(String.Format("Interlaced camera is losing a row in for odd height\n"));
                    }

#if DEBUG
                    int expectedStartRow = currentExposure.userRequested.y_offset / yBin;
                    int actualStartRow = 2 * (currentExposure.toCamera.y_offset / yBin);

                    if (fieldFlags == SX_CCD_FLAGS_FIELD_ODD)
                    {
                        actualStartRow += 1;
                    }

                    int startDelta = expectedStartRow - actualStartRow;
                    Debug.Assert(startDelta == 0 || startDelta == 2);

                    Log.Write(String.Format("rowDelta={0} startDelta={1}\n", rowDelta, startDelta));
#endif

                    dumpReadDelayedBlock(currentExposure.toCamera, "first frame after final interlaced adjustments");
                    // See if our modified parameters are still legal
                    try
                    {
                        checkParms(true, currentExposure.toCamera);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(String.Format("checkParms of first frame after final interlaced adjusments generated exception {0}\n", ex));
                        throw;
                    }

                    dumpReadDelayedBlock(currentExposure.toCameraSecond, "second frame after final interlaced adjustments");
                    try
                    {
                        checkParms(true, currentExposure.toCameraSecond);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(String.Format("checkParms of second frame after final interlaced adjustments generated exception {0}\n", ex));
                        throw;
                    }
                }

                // Make any final adjustments required for specific cameras

                if ((CameraModels)cameraModel == CameraModels.MODEL_M25C)
                {
                    // I have no idea why the next bit is required, but it is.  If it isn't there,
                    // the read of the image data fails. I found this in the sample basic application from SX.

                    currentExposure.toCamera.width *= 2;

                    // in order for this to work, the height must be even

                    if (currentExposure.toCamera.height % 2 == 1)
                    {
                        if (currentExposure.toCamera.height + currentExposure.toCamera.y_bin <= frameHeight)
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

                    dumpReadDelayedBlock(currentExposure.toCamera, "after M25C adjustments");
                }
                else if ((CameraModels)cameraModel == CameraModels.MODEL_COSTAR)
                {
                    Debug.Assert(currentExposure.toCamera.x_bin == 1);
                    Debug.Assert(currentExposure.toCamera.y_bin == 1);

                    // costar cannot do subimage widths - it always reads out whole lines
                    currentExposure.toCamera.x_offset = 0;
                    currentExposure.toCamera.width = ccdWidth;

                    dumpReadDelayedBlock(currentExposure.toCamera, "after CoStar adjustments");

                    // See if our modified parameters are still legal
                    try
                    {
                        checkParms(true, currentExposure.toCamera);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(String.Format("checkParms after CoStar adjustment generated exception {0}\n", ex));
                        throw;
                    }
                }
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

        public void echo(string s)
        {
            m_controller.echo(s);
        }

        internal void clear(Byte Flags)
        {
            SX_CMD_BLOCK cmdBlock;

            if ((Flags & ~(SX_CCD_FLAGS_NOWIPE_FRAME | SX_CCD_FLAGS_TDI | SX_CCD_FLAGS_NOCLEAR_FRAME | SX_CCD_FLAGS_CLEAR_VERT)) != 0)
            {
                throw new ArgumentException("Invalid flags passed to ClearPixels");
            }

            m_controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_CLEAR_PIXELS, Flags, idx, 0);

            Log.Write("clear about to Write\n");
            m_controller.Write(cmdBlock);
            echo("done"); // the clear takes a long time - when the echo returns we know it is done
            Log.Write("clear about to return\n");
        }

        public void clearCCDAndRegisters()
        {
            Log.Write("clearCCDAndRegisters() entered\n");
            clear(0);
            Log.Write("clearCCDAndRegisters() returns\n");
        }

        public void clearAllRegisters()
        {
            Log.Write("clearAllRegisters() entered\n");
            clear(SX_CCD_FLAGS_NOWIPE_FRAME);
            Log.Write("clearAllRegisters() returns\n");
        }

        public void clearVerticalRegisters()
        {
#if false // this might be M26C specific - don't call until better understood
            Log.Write("clearVertialRegisters entered\n");
            clear(SX_CCD_FLAGS_CLEAR_VERT);
            Log.Write("clearVertialRegisters returns\n");
#endif
        }

        public void getModel()
        {
            if (m_useDumped)
            {
                getModelDumped();
            }
            else
            {
                getModelUSB();
            }

            Log.Write(String.Format("cameraModel={0}\n", cameraModel));
        }

        public void getModelUSB()
        {
            SX_CMD_BLOCK cmdBlock;
            byte[] bytes = new byte[Marshal.SizeOf(cameraModel)];

            m_controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, SX_CMD_CAMERA_MODEL, 0, idx, (UInt16)bytes.Length);

            lock (m_controller.Lock)
            {
                Log.Write("getModel has locked\n");
                m_controller.Write(cmdBlock);

                bytes = m_controller.ReadBytes(bytes.Length);
            }
            Log.Write("getModel has unlocked\n");

            cameraModel = System.BitConverter.ToUInt16(bytes, 0);

            if (m_dump)
            {
                dumpModel();
            }
        }

        internal void setCoolerInfo(ref SX_COOLER_BLOCK inBlock, out SX_COOLER_BLOCK outBlock)
        {
            SX_CMD_BLOCK cmdBlock;
            byte[] bytes = new byte[Marshal.SizeOf(inBlock)];

            Log.Write(String.Format("setCoolerInfo inBlock temp={0} enabled={1}\n", inBlock.coolerTemp, inBlock.coolerEnabled));
            m_controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, SX_CMD_COOLER_CONTROL, inBlock.coolerTemp, (UInt16)inBlock.coolerEnabled, 0);

            lock (m_controller.Lock)
            {
                Log.Write("setCooler has locked\n");
                m_controller.Write(cmdBlock);

                bytes = m_controller.ReadBytes(Marshal.SizeOf(inBlock));
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
            get { return !m_useDumped && ((ccdParms.extra_capabilities & SXUSB_CAPS_COOLER) == SXUSB_CAPS_COOLER); }
        }

        void printCCDParams()
        {
            Log.Write(String.Format("params:\n"));
            Log.Write(String.Format("\thfront_porch={0:d}, hback_porch={1:d}\n", ccdParms.hfront_porch, ccdParms.hback_porch));
            Log.Write(String.Format("\tvfront_porch={0:d}, vback_porch={1:d}\n", ccdParms.vfront_porch, ccdParms.vback_porch));
            Log.Write(String.Format("\twidth={0:d}, height={1:d}\n", ccdParms.width, ccdParms.height));
            Log.Write(String.Format("\tpixel_uwidth={0:d}, pixel_uheight={1:d}\n", ccdParms.pixel_uwidth, ccdParms.pixel_uheight));
            Log.Write(String.Format("\tbits_per_pixel={0:d}, num_serial_ports={1:d}\n", ccdParms.bits_per_pixel, ccdParms.num_serial_ports));
            Log.Write(String.Format("\tcolor_matrix=0x{0:x}, extra_capabilitites=0x{1:x}\n", ccdParms.color_matrix, ccdParms.extra_capabilities));
        }

        void getCCDParams()
        {
            if (m_useDumped)
            {
                getCCDParamsDumped();
            }
            else
            {
                getCCDParamsUSB();
            }

            printCCDParams();

        }

        void getCCDParamsUSB()
        {
            SX_CMD_BLOCK cmdBlock;

            m_controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, SX_CMD_GET_CCD_PARMS, 0, idx, 0);

            lock (m_controller.Lock)
            {
                Log.Write("getCCDParams has locked\n");
                m_controller.Write(cmdBlock);

                ccdParms = (SX_CCD_PARAMS)m_controller.ReadObject(typeof(SX_CCD_PARAMS));
            }

            Log.Write("getCCDParams has unlocked\n");

            if (m_dump)
            {
                dumpCCDParams();
            }
        }

        internal void convertCameraDataToImageData()
        {
            UInt32 cameraBinnedWidth  = (UInt32)(currentExposure.toCamera.width / currentExposure.toCamera.x_bin);
            UInt32 cameraBinnedHeight = (UInt32)(currentExposure.toCamera.height / currentExposure.toCamera.y_bin);
            UInt32 binnedWidth        = (UInt32)(currentExposure.userRequested.width / currentExposure.userRequested.x_bin);
            UInt32 binnedHeight       = (UInt32)(currentExposure.userRequested.height / currentExposure.userRequested.y_bin);

            Log.Write(String.Format("convertCameraDataToImageData(): x_bin = {0} binnedWidth={1} binnedHeight={2}\n", currentExposure.toCamera.x_bin, binnedWidth, binnedHeight));
            if (imageData == null || imageData.GetUpperBound(0) + 1 != binnedWidth || imageData.GetUpperBound(1) + 1 != binnedHeight)
            {
                Log.Write(String.Format("allocating imageData\n"));
                imageData = new Int32[binnedWidth, binnedHeight];
            }
            else
            {
                Log.Write(String.Format("reusing imageData\n"));
            }

            // Decode the bytes from the camera and store them in the Int32 imageData

            if (idx == 0 && (CameraModels)cameraModel == CameraModels.MODEL_M25C)
            {
                Log.Write("convertCameraDataToImageData(): decoding M25C data\n");

                // We use the binned height and width from the camera. We may not have quite enough data
                // from the camera to fill the output array, so there could be zeros on the right or on the
                // bottom.
                // Consider:
                //    sensor: 16x16, binning: 5
                //    height x width = 3x3 (9 pixels), but the swizzle will get us 6x1 (6 pixels)
                // The adjustment below would then get us 3x2, and we would put those 6 pixels into the
                // "upper left" of the output array, leaving zeros in the "lower right"
                //
                // In the adustment code, we try to bump the height (if we can) to avoid this,
                // so the only time this will actually happen is if we are within the binning of 
                // full height and binning in a way that gets us an odd number of rows. 
                // For example, 2016 binned 5, gives 403 rows. The swizzle will leave us with 403/2 
                // 201 rows that becomes 402 rows when we unswizzle it here, so the last row will be 0

                Log.Write(String.Format("convertCameraDataToImageData(): cameraBinnedWidth = {0} cameraBinnedHeight={1}\n", cameraBinnedWidth, cameraBinnedHeight));
                Log.Write(String.Format("convertCameraDataToImageData(): userPixels = {0}, cameraPixes={1}, len={2}\n", binnedWidth * binnedHeight, cameraBinnedWidth * cameraBinnedHeight, rawFrame1.Length));

                Debug.Assert((cameraBinnedHeight * 2 == binnedHeight) || (cameraBinnedHeight * 2 + 1 == binnedHeight));

                try
                {
                    for (UInt32 y = 0; y < cameraBinnedHeight; y++)
                    {
                        Debug.Assert(y < 2*binnedHeight);

                        for(Byte z=0;z<2;z++)
                        {
                            UInt32 yIdx = 2U*y + z;

                            if (yIdx < binnedHeight)
                            {
                                copyM25CPixels(rawFrame1, y*cameraBinnedWidth, 
                                                imageData, yIdx,
                                                z == 0,
                                                binnedHeight, binnedWidth);
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Write(String.Format("convertCameraDataToImageData(): Caught an exception processing M25C data\n"));
                    Log.Write("Exception data was: \n" + ex.ToString() + "\n");
                    throw ex;
                }
            }
            else if (bytesPerPixel == 1 || bytesPerPixel == 2)
            {
                Log.Write(String.Format("convertCameraDataToImageData() processing {0} bit {1} camera data\n", bitsPerPixel, interlaced?"interlaced":"progressive"));

#if INTERLACED_DEBUG
                if (rawFrame2 != null)
                {
                    Int64 frame1=0;
                    Int64 frame2=0;

                    for(Int32 i = 0; i <rawFrame1.Length/2; i++)
                    {
                        frame1 += BitConverter.ToUInt16(rawFrame1, 2*i);
                    }

                    for(Int32 i = 0; i <rawFrame2.Length/2; i++)
                    {
                        frame2 += BitConverter.ToUInt16(rawFrame2, 2 * i);
                    }

                    Log.Write(String.Format("stats: frame1 + frame2 = {0:N0}\n", frame1 + frame2));
                    Log.Write(String.Format("stats: frame1 = {0:N0}, ave={1}\n", frame1, frame1/(rawFrame1.LongLength/2)));
                    Log.Write(String.Format("stats: frame2 = {0:N0}, ave={1}\n", frame2, frame2/(rawFrame2.LongLength/2)));
                }
#endif

                UInt32 y = 0xffffffff;

                try
                {
                    UInt32 actualHeight = (UInt32)(rawFrame1.Length / bytesPerPixel / cameraBinnedWidth);

                    UInt32 startingXIndex = 0;
                    Int32 [] bias = null;
                    Int32 firstBias = 0;
                    Int32 secondBias = 0;

                    // It is possible for interlaced cameras to not supply as much data 
                    // would be expected from just the height and binning mode.
                    // For  example, you would expect a 10x10 sensor binned 3X to give 3 rows.  But
                    // with an interlaced camera it is actally 2x10x5, and the 5 only gives 1 row for 3X
                    // binning. 
                    if (interlaced)
                    {
                        actualHeight += (UInt32)(rawFrame2.Length / bytesPerPixel / cameraBinnedWidth);
                    }

                    Log.Write(String.Format("actualHeight={0}\n", actualHeight));

                    Debug.Assert(actualHeight == binnedHeight || (interlaced && (actualHeight == binnedHeight - 1)));

                    if ((CameraModels)cameraModel == CameraModels.MODEL_COSTAR)
                    {
                        Debug.Assert(currentExposure.toCamera.x_bin == 1);
                        Debug.Assert(currentExposure.toCamera.y_bin == 1);
                        Debug.Assert(currentExposure.toCamera.width == ccdWidth);
                        Debug.Assert(idx == 0);
                        Debug.Assert(hasBiasData);

                        Log.Write(String.Format("CoStar decode begins: userRequest.x_offset={0}, userReqested.width={1}, userRequested.Height={2}\n",
                                currentExposure.userRequested.x_offset, currentExposure.userRequested.width, currentExposure.userRequested.height));
                    }

                    if (hasBiasData)
                    {
                        bias = new Int32[2];
                        Debug.Assert(progressive); // no current interlaced cameras have bias, and the loop code below cannot be tested
                    }

                    for (y = 0; y < actualHeight; y++)
                    {
                        UInt32 yOffset =  y;
                        byte [] rawFrame = rawFrame1;

                        if (interlaced)
                        {
                            if (y % 2 == 1)
                            {
                                rawFrame = rawFrame2;
                            }

                            yOffset /= 2;
                        }
                        UInt32 destOffset = y;
#if false && INTERLACED_DEBUG
                        if (interlaced)
                        {
                                destOffset = yOffset + (y % 2) * binnedHeight/2;
                        }
#endif

                        if (hasBiasData)
                        {
                            computeBias(rawFrame, yOffset*cameraBinnedWidth, bias);

                            startingXIndex = numBiasPixels;

                            if  ((CameraModels)cameraModel == CameraModels.MODEL_COSTAR)
                            {
                                Int32 biasOffset = 1000;

                                if (currentExposure.userRequested.x_offset % 2 == 0)
                                {
                                    firstBias  = biasOffset - bias[0];
                                    secondBias = biasOffset - bias[1];
                                }
                                else
                                {
                                    firstBias  = biasOffset - bias[1];
                                    secondBias = biasOffset - bias[0];
                                }

                                startingXIndex += currentExposure.userRequested.x_offset;
                            }
                        }

                        //Log.Write(String.Format("calling CopyPixels(rawFrame={0}, yOffset={1}, destOffset={2}\n", rawFrame, yOffset, destOffset));
                        copyPixels(rawFrame, yOffset*cameraBinnedWidth+startingXIndex, imageData, destOffset, firstBias, secondBias, binnedHeight, binnedWidth);
                    }
                    Log.Write(String.Format("Processed {0} lines, {1} pixels/line\n", actualHeight, binnedWidth));
                }
                catch (System.Exception ex)
                {
                    Log.Write(String.Format("convertCameraDataToImageData(): Caught an exception processing image data at row {0} - {1}\n", y, ex.ToString()));
                    throw ex;
                }
            }
            else
            {
                throw new System.Exception(String.Format("Unable to decode image - unknown camera or pixel type"));
            }
            if (interlaced)
            {
                //adjustInterlaced();
            }

            Log.Write("convertCameraDataToImageData(): ends\n");
        }

        internal void setPixelBytes()
        {

            // there are better ways to do this (like bitsPerPixle divided by 8), but I wanted to throw the exception
            // in the default case and looked as good as any

            switch (bitsPerPixel)
            {
                case 8:
                        bytesPerPixel = 1;
                        break;
                case 16:
                        bytesPerPixel = 2;
                        break;
                default:
                       throw new System.Exception(String.Format("setPixelSize: unhandled pixel size {0}\n", bytesPerPixel));
            }

            Log.Write(String.Format("setPixelSize: bytesPerPixel=\n", bytesPerPixel));
        }

        internal byte [] downloadPixels(bool setValid, SX_READ_DELAYED_BLOCK exposure)
        {
            Int32 binnedWidth = exposure.width / exposure.x_bin;
            Int32 binnedHeight = exposure.height / exposure.y_bin;
            Int32 imagePixels = binnedWidth * binnedHeight;
            byte [] ret;

            Log.Write(String.Format("downloadPixels(): requesting {0} pixels, {1} bytes each ({2} bytes)\n", imagePixels, bytesPerPixel, imagePixels * bytesPerPixel));

            ret = m_controller.ReadBytes(imagePixels * bytesPerPixel);

            Log.Write("downloadPixels(): read completed\n");

            return ret;
        }

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
        // Notice that execpt for the beginning and ending, we copy 2 pixels from input to 
        // output and them skip 2 pixels. For the first output row we have to copy 1 pixel
        // first, then move ahead 2 before we get into that state, and for the second we 
        // simply skip the first pixel.

        unsafe internal void copyM25CPixels(byte [] src, UInt32 srcOffset, 
                                        Int32 [,] dest, UInt32 destOffset, 
                                        bool isEvenRow,
                                        UInt32 height, UInt32 width)
        {

            UInt32 stride = height;
            UInt32 count = width;

            Debug.Assert(count > 0);
            Debug.Assert(src.Length > srcOffset + count);
            Debug.Assert(dest.Length >= destOffset + stride*(count-1));

            //Log.Write(String.Format("copyM25CPixels: srcOffset={0} destOffset={1} isEvenRow={2} height={3} width={4}\n", srcOffset, destOffset, isEvenRow, height, width));

            fixed(byte *pSrcBase = src)
            {
                fixed(Int32 *pDestBase = dest)
                {
                    UInt16 *pSrc = ((UInt16 *)pSrcBase) + srcOffset;
                    Int32 *pDest = pDestBase + destOffset;

                    try
                    {

                        // Deal with the first pixel...
                        if (isEvenRow)
                        {
                            // by storing it and skipping two for the first row
                            *pDest = *pSrc++;
                            pDest += stride;
                            count--;
                            pSrc += 2;
                        }
                        else
                        {
                            // by skipping it for the second row
                            pSrc++;
                        }

                        // we are now aligned for using 2 then skipping two
                        while(count > 1)
                        {
                            *pDest = *pSrc++;
                            pDest += stride;
                            *pDest = *pSrc++;
                            pDest += stride;

                            count -= 2;
                            pSrc += 2;
                        }

                        // there is at most 1 pixel still needed.  Copy it if needed
                        if (count > 0)
                        {
                            *pDest = *pSrc++;
                            pDest += stride;
                            count--;
                            pSrc += 2;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Log.Write(String.Format("copyM25CPixels() caught an exception {0}\n", ex.ToString()));
                        throw ex;
                    }
                }
            }
        }

        unsafe internal void computeBias(byte [] src, UInt32 srcOffset, Int32 [] bias)
        {

            if ((CameraModels)cameraModel != CameraModels.MODEL_COSTAR)
            {
                throw new System.Exception("combuteBias called for Non-CoStar camera");
            }

            fixed(byte *pSrcBase = src)
            {
                UInt16 *pSrc = ((UInt16 *)pSrcBase) + srcOffset;
                Int32 divisor = (Int32)numBiasPixels / 2;

                Debug.Assert(bias.Length == 2);
                Debug.Assert(numBiasPixels % 2 == 0);

                bias[0] = *pSrc++;
                bias[1] = *pSrc++;

                for(UInt32 i=2;i<numBiasPixels;i++)
                {
                    bias[i % 2] += *pSrc++;
                }

                bias[0] /= divisor;
                bias[1] /= divisor;
            }
        }

        /* 
         * It seems like it would be better to have one routine for cameras that
         * provide bias data and different one for those that don't, since only 
         * one camera has bias data, but my performance measurements show no
         * measureable difference between the version with and without 
         * (i.e. over several runs neither was consistently faster)
         * Since peformance is a wash, I just have a single routine, especially
         * since it is a complex routine and two copies would mean twice
         * the potential for bugs.
         */

        unsafe internal void copyPixels(byte [] src, UInt32 srcOffset, 
                                        Int32 [,] dest, UInt32 destOffset, 
                                        Int32 evenBias, Int32 oddBias, 
                                        UInt32 height, UInt32 width)
        {
            UInt32 stride = height;
            UInt32 count = width;
            Int64 oddPixelTotal = 0;
            Int64 evenPixelTotal = 0;

            Debug.Assert(dest.Length/count == stride);
            Debug.Assert(count > 0);
            Debug.Assert(src.Length >= srcOffset + count);
            Debug.Assert(dest.Length >= destOffset + stride*(count-1));

            //Log.Write(String.Format("copyPixels() - srcOffset={0} destOffset={1} evenBias={2} oddBias={3} height={4} width={5} count={6} stride={6}\n", srcOffset, destOffset, evenBias, oddBias, height, width, count, stride));

            fixed(byte *pSrcBase = src)
            {
                fixed(Int32 *pDestBase = dest)
                {
                    byte *pSrcBytes = pSrcBase + srcOffset*bytesPerPixel;
                    Int32 *pDest = pDestBase + destOffset;

                    try
                    {
                        switch(bytesPerPixel)
                        {
                            case 1:
                                    {
                                        byte *pSrc = pSrcBytes;

                                        if (srcOffset % 2 == 1)
                                        {
                                            Int32 value;

                                            oddPixelTotal += *pSrc;
                                            value = *pSrc++ + oddBias;
                                            if (value > Byte.MaxValue)
                                            {
                                                value = Byte.MaxValue;
                                            }
                                            *pDest = value;
                                            pDest += stride;

                                            count--;
                                        }

                                        while (count > 1)
                                        {
                                            Int32 value;
                                            
                                            evenPixelTotal += *pSrc;
                                            value = *pSrc++ + evenBias;
                                            if (value > Byte.MaxValue)
                                            {
                                                value = Byte.MaxValue;
                                            }
                                            *pDest = value;
                                            pDest += stride;

                                            oddPixelTotal += *pSrc;
                                            value = *pSrc++ + oddBias;
                                            if (value > Byte.MaxValue)
                                            {
                                                value = Byte.MaxValue;
                                            }
                                            *pDest = value;
                                            pDest += stride;

                                            count -= 2;
                                        }

                                        if (count != 0)
                                        {
                                            Int32 value;

                                            evenPixelTotal += *pSrc;
                                            value = *pSrc++ + evenBias;
                                            if (value > Byte.MaxValue)
                                            {
                                                value = Byte.MaxValue;
                                            }
                                            *pDest = value;
                                            pDest += stride;

                                            count--;
                                        }
                                        Debug.Assert(pSrc - pSrcBytes - 1 < src.Length);
                                    }
                                    break;
                            case 2:
                                    {
                                        UInt16 *pSrc = (UInt16 *)pSrcBytes;

                                        if (srcOffset % 2 == 1)
                                        {
                                            Int32 value;

                                            oddPixelTotal += *pSrc;
                                            value = *pSrc++ + oddBias;
                                            if (value > UInt16.MaxValue)
                                            {
                                                value = UInt16.MaxValue;
                                            }
                                            *pDest = value;
                                            pDest += stride;

                                            count--;
                                        }

                                        while(count > 1)
                                        {
                                            Int32 value;

                                            evenPixelTotal += *pSrc;
                                            value = *pSrc++ + evenBias;
                                            if (value > UInt16.MaxValue)
                                            {
                                                value = UInt16.MaxValue;
                                            }
                                            *pDest = value;
                                            pDest += stride;

                                            oddPixelTotal += *pSrc;
                                            value = *pSrc++ + oddBias;
                                            if (value > UInt16.MaxValue)
                                            {
                                                value = UInt16.MaxValue;
                                            }
                                            *pDest = value;
                                            pDest += stride;

                                            count -= 2;
                                        }

                                        if (count != 0)
                                        {
                                            Int32 value;

                                            evenPixelTotal += *pSrc;
                                            value = *pSrc++ + evenBias;
                                            if (value > UInt16.MaxValue)
                                            {
                                                value = UInt16.MaxValue;
                                            }
                                            *pDest = value;
                                            pDest += stride;

                                            count--;
                                        }

                                        Debug.Assert((Byte *)pSrc - pSrcBytes - bytesPerPixel < src.Length);
                                    }
                                    break;
                            default:
                                    throw new System.Exception(String.Format("copyPixels: unhandled pixel size {0}\n", bytesPerPixel));
                        }

                        Debug.Assert(pDest - (pDestBase + destOffset) - stride < dest.Length);
                    }
                    catch (System.Exception ex)
                    {
                        Log.Write(String.Format("copyPixels() caught an exception {0}\n", ex.ToString()));
                        throw ex;
                    }
                }
            }
            //Log.Write(String.Format("copyPixels(): evenPixelTotal={0} (ave={1}) oddPixelTotal={2} (ave={3})\n", evenPixelTotal, evenPixelTotal / (width / 2), oddPixelTotal, oddPixelTotal / (width / 2)));
            Debug.Assert(count == 0);
        }

        unsafe internal void adjustInterlaced()
        {
            Int64 evenTotal=0;
            Int64 oddTotal=0;

            Log.Write("adjustProgressive() begins\n");
#if INTERLACED_DEBUG
            {
                Int64 frameFinal=0;
                Int64 frameFinalEven=0;
                Int64 frameFinalOdd=0;

                for(UInt32 y=0;y<imageData.GetUpperBound(1) + 1;y++)
                {
                    for(UInt32 x=0;x<imageData.GetUpperBound(0) + 1;x++)
                    {
                        frameFinal += imageData[x,y];
                        if (y % 2 == 0)
                        {
                            frameFinalEven += imageData[x,y];
                        }
                        else
                        {
                            frameFinalOdd += imageData[x,y];
                        }
                    }
                }
                Log.Write(String.Format("before stats: frameFinal = {0:N0}, even+odd={1:N0}\n", frameFinal, frameFinalEven+frameFinalOdd));
                Log.Write(String.Format("stats: frameFinal = {0:N0}, ave={1}\n", frameFinal, frameFinal/imageData.LongLength));
                Log.Write(String.Format("stats: frameFinalEven = {0:N0}, ave={1}\n", frameFinalEven, frameFinalEven/(imageData.LongLength/2)));
                Log.Write(String.Format("stats: frameFinalOdd = {0:N0}, ave={1}\n", frameFinalOdd, frameFinalOdd/(imageData.LongLength/2)));
            }
#endif


            // compute the values
            for(UInt32 x=0;x<imageData.GetUpperBound(0)+1;x++)
            {
                fixed(Int32 *pImageData = &imageData[x,0])
                {
                    Int32 count=imageData.GetUpperBound(1)+1;
                    Int32 *p = pImageData;

                    while(count > 1)
                    {
                        evenTotal += *p++;
                        oddTotal  += *p++;
                        count-=2;
                    }

                    if (count != 0)
                    {
                        evenTotal += *p++;
                    }
                }
            }

            Log.Write(String.Format("evenTotal={0:N0} oddTotal={1:N0} evenTotal+oddTotal={2:N0}\n", evenTotal, oddTotal, evenTotal+oddTotal));

            if (evenTotal < oddTotal)
            {
                double ratio = evenTotal/(double)oddTotal;
                Log.Write(String.Format("adjusting even pixels, ratio={0}\n", ratio));

                // adjust the even rows
                for(UInt32 x=0;x<imageData.GetUpperBound(0)+1;x++)
                {
                    fixed(Int32 *pImageData = &imageData[x,0])
                    {
                        Int32 count=imageData.GetUpperBound(1)+1;
                        Int32 *p = pImageData;

                        while(count > 1)
                        {
                            Int32 value = (Int32)(*p * ratio);

                            if (value > UInt16.MaxValue)
                            {
                                value = UInt16.MaxValue;
                            }
                            *p = value;
                            p += 2;
                            count-=2;
                        }

                        if (count != 0)
                        {
                            Int32 value = (Int32)(*p * ratio);

                            if (value > UInt16.MaxValue)
                            {
                                value = UInt16.MaxValue;
                            }
                            *p = value;
                            p += 2;
                        }
                    }
                }
            }
            else
            {
                double ratio = oddTotal/(double)evenTotal;

                Log.Write(String.Format("adjusting even pixels, ratio={0}\n", ratio));

                // adjust the odd rows
                for(UInt32 x=0;x<imageData.GetUpperBound(0)+1;x++)
                {
                    fixed(Int32 *pImageData = &imageData[x,1])
                    {
                        Int32 count=imageData.GetUpperBound(1);
                        Int32 *p = pImageData;

                        while(count > 1)
                        {
                            Int32 value = (Int32)(*p * ratio);

                            if (value > UInt16.MaxValue)
                            {
                                value = UInt16.MaxValue;
                            }
                            *p = value;
                            p += 2;
                            count-=2;
                        }

                        if (count != 0)
                        {
                            Int32 value = (Int32)(*p * ratio);

                            if (value > UInt16.MaxValue)
                            {
                                value = UInt16.MaxValue;
                            }
                            *p = value;
                            p += 2;
                        }
                    }
                }
            }
#if INTERLACED_DEBUG
            {
                Int64 frameFinal=0;
                Int64 frameFinalEven=0;
                Int64 frameFinalOdd=0;

                for(UInt32 y=0;y<imageData.GetUpperBound(1) + 1;y++)
                {
                    for(UInt32 x=0;x<imageData.GetUpperBound(0) + 1;x++)
                    {
                        frameFinal += imageData[x,y];
                        if (y % 2 == 0)
                        {
                            frameFinalEven += imageData[x,y];
                        }
                        else
                        {
                            frameFinalOdd += imageData[x,y];
                        }
                    }
                }
                Log.Write(String.Format("after stats: frameFinal = {0:N0}, even+odd={1:N0}\n", frameFinal, frameFinalEven+frameFinalOdd));
                Log.Write(String.Format("stats: frameFinal = {0:N0}, ave={1}\n", frameFinal, frameFinal/imageData.LongLength));
                Log.Write(String.Format("stats: frameFinalEven = {0:N0}, ave={1}\n", frameFinalEven, frameFinalEven/(imageData.LongLength/2)));
                Log.Write(String.Format("stats: frameFinalOdd = {0:N0}, ave={1}\n", frameFinalOdd, frameFinalOdd/(imageData.LongLength/2)));
            }
#endif
            Log.Write("adjustProgressive() ends\n");
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
            if (m_useDumped)
            {
                recordPixelsDumped(bDelayed, out exposureEnd);
            }
            else
            {
                recordPixelsUSB(bDelayed, out exposureEnd);
            }
        }

        public void recordPixelsUSB(bool bDelayed, out DateTimeOffset exposureEnd)
        {
            SX_CMD_BLOCK cmdBlock;

            Log.Write(String.Format("recordPixels entered - bDelayed = {0}\n", bDelayed));
            UInt16 firstExposureFlags = adjustReadDelayedBlock();
            UInt16 secondExposureFlags = (UInt16)((SX_CCD_FLAGS_FIELD_EVEN | SX_CCD_FLAGS_FIELD_ODD) & ~firstExposureFlags);

            Debug.Assert(currentExposure.toCamera.x_bin != 7 && currentExposure.toCamera.y_bin != 7);

            if (m_dump)
            {
                dumpCurrentExposure();
            }

            // We invalidate the data here, so that there is no chance we have to wait for someone
            // to download an image later in the routine when the data is ready for download
            lock (oImageDataLock)
            {
                imageDataValid = false;
            }

            lock (m_controller.Lock)
            {
                Log.Write("recordPixels() has locked\n");
                if (bDelayed)
                {
                    m_controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS,
                                                    SX_CMD_READ_PIXELS_DELAYED,
                                                    firstExposureFlags,
                                                    idx,
                                                    (UInt16)Marshal.SizeOf(currentExposure.toCamera));
                    m_controller.Write(cmdBlock, currentExposure.toCamera);
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
                    m_controller.Write(cmdBlock, readBlock);
                }

                Log.Write(String.Format("recordPixels() requested read, flags = {0}\n", firstExposureFlags)); 
                exposureEnd = DateTimeOffset.Now;
            
                
                Log.Write("recordPixelsDelayed requesting download\n");
                rawFrame1 = downloadPixels(progressive, currentExposure.toCamera);
                Log.Write("recordPixels() download completed\n");

                if (idx == 0 && interlaced)
                {
                    SX_READ_BLOCK readBlock;
                    Log.Write("recordPixels() preparing for second frame download\n");

                    Debug.Assert(secondExposureFlags != 0);
                    Debug.Assert(secondExposureFlags != firstExposureFlags);

                    clearAllRegisters();
                    initReadBlock(currentExposure.toCamera, out readBlock);
                    m_controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS,
                                                    SX_CMD_READ_PIXELS,
                                                    secondExposureFlags,
                                                    idx,
                                                    (UInt16)Marshal.SizeOf(readBlock));

                    Log.Write(String.Format("recordPixels() second frame requesting read, flags = {0}\n", secondExposureFlags));
                    m_controller.Write(cmdBlock, readBlock);

                    exposureEnd = DateTimeOffset.Now;
                    Log.Write("recordPixels() second frame requesting download\n");
                    rawFrame2 = downloadPixels(false, currentExposure.toCameraSecond);
                    Log.Write("recordPixels() second frame download completed\n");
                }
            }

            if (m_dump)
            {
                dumpFrame();
            }

            convertCameraDataToImageData();

            lock (oImageDataLock)
            {
                imageDataValid = true;
            }

            Log.Write("recordPixels() returns\n");
        }
    }
}
