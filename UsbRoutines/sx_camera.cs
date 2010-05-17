using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WinUsbDemo;
using Logging;

namespace sx
{
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

    public class Camera
        :sxBase
    {
        // Variables
        private Controller controller;
        private SX_CCD_PARAMS ccdParms;
        private SX_READ_DELAYED_BLOCK readDelayedBlock;
        private SX_READ_DELAYED_BLOCK lastExposureReadDelayedBlock;
        //byte[] imageAsBytes;
        Array imageRawData;
        Type pixelType;
        private UInt32[,] imageData;
        private bool imageDataValid;
        private object oImageDataLock;
        private UInt16 idx;

        byte [] imageAsBytes;
        private UInt32[,] imageDataFromBytes;

        // Properties

        public string description
        {
            get
            {
                string ret = "unknown " + cameraModel;

                switch (cameraModel)
                {
                    case 0x09:
                        ret = "HX9";
                        break;
                    case 0x45:
                        ret = "MX5";
                        break;
                    case 0xc5:
                        ret = "MX5C";
                        break;
                    case 0x47:
                        ret = "MX7";
                        break;
                    case 0xc7:
                        ret = "MX7C";
                        break;
                    case 0x49:
                        ret = "MX9";
                        break;
                    case 0x59:
                        ret = "MX25C";
                        break;
                }

                Log.Write("getDescription() returns " + ret + "\n");
                return ret;
            }
        }
        public UInt16 cameraModel
        {
            get;
            private set;
        }

        public double electronsPerADU
        {
            get
            {
                double dReturn;

                switch (cameraModel)
                {
                    case 0x59:
                        dReturn = 0.40;
                        break;
                    default:
                        throw new System.Exception("ElectronsPerADU unknown for this camera model");
                }

                return dReturn;
            }
        }

        public Byte hFrontPorch
        {
            get {return ccdParms.hfront_porch;}
        }

        public Byte hBackPorch
        {
            get {return ccdParms.hback_porch;}
        }

        public Byte vFrontPorch
        {
            get {return ccdParms.hfront_porch;}
        }

        public Byte vBackPorch
        {
            get {return ccdParms.hback_porch;}
        }

        public UInt16 ccdWidth
        {
            get { return ccdParms.width; }
        }

        public UInt16 ccdHeight
        {
            get { return ccdParms.height; }
        }

        public double pixelWidth
        {
            get {return ccdParms.pixel_uwidth/(double)256;}
        }

        public double pixelHeight
        {
            get {return ccdParms.pixel_uheight/(double)256;}
        }

        public Byte bitsPerPixel
        {
            get {return ccdParms.bits_per_pixel;}
        }

        public Boolean hasGuideCamera
        {
            get { return controller.hasGuideCamera; }
        }

        public Boolean hasGuidePort
        {
            get { return controller.hasGuidePort; }
        }

        private Byte extraCapabilities
        {
            get {return ccdParms.extra_capabilities;}
        }

        public UInt16 colorMatrix
        {
            get { return ccdParms.color_matrix; }
        }

        public UInt16 xOffset
        {
            get {return readDelayedBlock.x_offset;}
            set {
                    if (value >= ccdParms.width)
                    {
                        throw new ArgumentOutOfRangeException(String.Format("Invalid xOffset {0} 0<=xOffset<={1}", value, ccdParms.width), "xOffset");
                    }
                    readDelayedBlock.x_offset = value;
                }
        }

        public UInt16 yOffset
        {
            get {return readDelayedBlock.y_offset;}
            set {
                    if (value >= ccdParms.height)
                    {
                        throw new ArgumentOutOfRangeException(String.Format("Invalid yOffset {0} 0<=yOffset<={1}", value, ccdParms.height), "yOffset");
                    }
                    readDelayedBlock.x_offset = value;
                }
        }

        public UInt16 width
        {
            get {return readDelayedBlock.width;}
            set {
                    if (value > ccdParms.width)
                    {
                        throw new ArgumentOutOfRangeException(String.Format("Invalid width {0} 0<=width<={1}", value, ccdParms.width), "width");
                    }
                    readDelayedBlock.width = value;
                }
        }

        public UInt16 height
        {
            get {return readDelayedBlock.height;}
            set {
                    if (value > ccdParms.height)
                    {
                        throw new ArgumentOutOfRangeException(String.Format("Invalid height {0} 0<=height<={1}", value, ccdParms.height), "height");
                    }
                    readDelayedBlock.height = value;
                }
        }

        public Byte xBin
        {
            get {return readDelayedBlock.x_bin;}
            set {
                    if (value > MAX_BIN)
                    {
                        throw new ArgumentOutOfRangeException(String.Format("Invalid xBin {0} 0<=height<={1}", value, MAX_BIN), "xBin");
                    }
                    readDelayedBlock.x_bin = value;
                }
        }

        public Byte xBinMax
        {
            get { return MAX_X_BIN;}
        }

        public Byte yBin
        {
            get {return readDelayedBlock.y_bin;}
            set {
                    if (value > MAX_BIN)
                    {
                        throw new ArgumentOutOfRangeException(String.Format("Invalid lBin {0} 0<=height<={1}", value, MAX_BIN), "xBin");
                    }
                    readDelayedBlock.y_bin = value;
                }
        }
        
        public Byte yBinMax
        {
            get { return MAX_Y_BIN;}
        }

        public UInt32 delayMs 
        {
            get { return readDelayedBlock.delay; }
            set { readDelayedBlock.delay = value; }
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

                    return imageData;
                }
            }
        }

        public Camera(Controller controller, UInt16 cameraIdx)
        {
            Log.Write(String.Format("sx.Camera() constructor: controller={0} cameraIdx={1}\n", controller, cameraIdx));

            idx = cameraIdx;

            this.controller = controller;

            if (cameraIdx > 0)
            {
                if (cameraIdx != 1)
                {
                    throw new ArgumentException("Error: Untested with cameraIdx > 1");
                }

                if (!hasGuideCamera)
                {
                    Log.Write(String.Format("sx.Camera() constructor: Guide Camera is not connected\n"));
                    throw new ArgumentException("Error: cameraIDX == 1 and INTEGRATED_GUIDER_CCD == 0");
                }
            }

            cameraModel = getModel();
            getParams(ref ccdParms);
            setPixelType();
            buildReadDelayedBlock(out readDelayedBlock, 0, 0, ccdWidth, ccdHeight, 1, 1, 0);
            imageDataValid = false;
            oImageDataLock = new object();
            Log.Write(String.Format("sx.Camera() constructor returns\n"));
         }

        // we use a READ_DELAYED_BLOCK to store paramters that are accessed as properties.  
        // If the user requests a read without delay, we can just copy all the matching 
        // parameters out of the one we are keeping

        internal void initReadBlock(out SX_READ_BLOCK block, SX_READ_DELAYED_BLOCK inblock)
        {
            block.x_offset = inblock.x_offset;
            block.y_offset = inblock.y_offset;
            block.width = (UInt16)inblock.width;
            block.height = (UInt16)inblock.height;
            // I have no idea why the next bit is required, but it is.  If it isn't there, 
            // the read of the image data fails with a semaphore timeout. I found this in the
            // sample application from SX.
            if (idx == 0 && cameraModel == 0x59)
            {
                block.width *= 2;
                block.height /= 2;
            }
            block.x_bin = inblock.x_bin;
            block.y_bin = inblock.y_bin;

            Log.Write(String.Format("initReadBlock() x_off={0} y_off={1} width={2} height={3} x_bin={4} y_bin={5}\n", block.x_offset, block.y_offset, block.width, block.height, block.x_bin, block.y_bin));
        }

        internal void buildReadDelayedBlock(out SX_READ_DELAYED_BLOCK block, UInt16 x_offset, UInt16 y_offset, UInt16 width, UInt16 height, Byte x_bin, Byte y_bin, UInt32 delay)
        {
            block.x_offset = x_offset;
            block.y_offset = y_offset;
            block.width = width;
            block.height = height;
            block.x_bin = x_bin;
            block.y_bin = y_bin;
            block.delay = delay;

            Log.Write(String.Format("initReadBlock() x_off={0} y_off={1} width={2} height={3} x_bin={4} y_bin={5} delay={6}\n", block.x_offset, block.y_offset, block.width, block.height, block.x_bin, block.y_bin, delay));
        }

        internal void clear(Byte Flags)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;

            if ((Flags & ~(SX_CCD_FLAGS_NOWIPE_FRAME | SX_CCD_FLAGS_TDI | SX_CCD_FLAGS_NOCLEAR_FRAME)) != 0)
            {
                throw new ArgumentException("Invalid flags passed to ClearPixels");
            }

            controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_CLEAR_PIXELS, Flags, idx, 0);
            
            Log.Write("clear about to Write\n");
            controller.Write(cmdBlock, out numBytesWritten);
            Log.Write("clear about to return\n");
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
            Log.Write("clearRecordedPixels entered\n");
        }

        public UInt16 getModel()
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;
            byte[] bytes = new byte[2];
            UInt16 model=0;

            controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, SX_CMD_CAMERA_MODEL, 0, idx, (UInt16)Marshal.SizeOf(model));

            lock (controller)
            {
                Log.Write("getModel has locked\n");
                controller.Write(cmdBlock, out numBytesWritten);

                bytes = controller.ReadBytes(Marshal.SizeOf(model), out numBytesRead);
            }
            Log.Write("getModel has unlocked\n");
            model = System.BitConverter.ToUInt16(bytes, 0);

            return model;
        }

        void getParams(ref SX_CCD_PARAMS parms)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;

            controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, SX_CMD_GET_CCD_PARMS, 0, idx, 0);

            lock (controller)
            {
                Log.Write("getParams has locked\n");
                controller.Write(cmdBlock, out numBytesWritten);

                parms = (SX_CCD_PARAMS)controller.ReadObject(typeof(SX_CCD_PARAMS), out numBytesRead);
            }
            Log.Write("getParams has unlocked\n");
        }

        internal void convertCameraDataToImageData()
        {
            Int32 binnedWidth = lastExposureReadDelayedBlock.width;
            Int32 binnedHeight = lastExposureReadDelayedBlock.height;

            if (bitsPerPixel != 16 && bitsPerPixel != 8)
            {
                throw new ArgumentOutOfRangeException("downloadPixels(): Untested: bitsPerPixel != 16", "bitsPerPixel");
            }

            imageData = new UInt32[binnedWidth, binnedHeight];

            // Copy the bytes read from the camera into a UInt32 array.
            // There must be a better way to do this, but I don't know what it is. 

            Log.Write("convertCameraDataToImageData(): decoding data, bitsPerPixel=" + bitsPerPixel + " binnedWidth = " + binnedWidth + " binnedHeight=" + binnedHeight + "\n");

            if (idx == 0 && cameraModel == 0x59)
            {

                Log.Write("convertCameraDataToImageData(): decoding M25C data\n");

                // to go along with the odd requirement that we must double the width and halve the height 
                // to read the data from MX25C, we have to unscramble the data here

                int srcIdx = 0;
                int x, y;

                try
                {
                    for (y = 0; y < binnedHeight; y += 2)
                    {
                        for (x = 0; x < binnedWidth; x += 2)
                        {
                            imageData[x, y] = (UInt32)(UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                            imageData[x, y + 1] = (UInt32)(UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                            imageData[x + 1, y + 1] = (UInt32)(UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                            imageData[x, y + 1] = (UInt32)(UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Write(String.Format("convertCameraDataToImageData(): Caught an exception processing M25C data - {0}\n", ex.ToString()));
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
                            imageData[x,y ] = (UInt32)(UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                            if (imageData[x, y] != imageDataFromBytes[x, y])
                            {
                                Log.Write(String.Format("difference for {0},{1}: {2} != {3}\n", y, x, imageData[x, y], imageDataFromBytes[x, y]));
                            }
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
                    pixelType = typeof(System.UInt16);
                    break;
                case 32:
                    pixelType = typeof(System.UInt32);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(String.Format("Unexpected bitsPerPixel {0}", bitsPerPixel), "bitsPerPixel");
            }
        }

        internal void downloadPixels()
        {
            Int32 numBytesRead;
            Int32 binnedWidth = lastExposureReadDelayedBlock.width;
            Int32 binnedHeight = lastExposureReadDelayedBlock.height;
            Int32 imagePixels = binnedWidth * binnedHeight;

            Log.Write(String.Format("downloadPixels(): requesting {0} pixels, {1} bytes each ({2} bytes)\n", imagePixels, Marshal.SizeOf(pixelType), imagePixels * Marshal.SizeOf(pixelType)));

            imageRawData = (Array)controller.ReadArray(pixelType, imagePixels, out numBytesRead);
           
            lock (oImageDataLock)
            {
                imageDataValid = true;
                imageData = null;
            }

            Log.Write("downloadPixels(): read completed, numBytesRead=" + numBytesRead + "\n");
        }

        public void guideNorth(int durationMS)
        {
            controller.guide(SX_STAR2K_NORTH, durationMS);
        }

        public void guideSouth(int durationMS)
        {
            controller.guide(SX_STAR2K_SOUTH, durationMS);
        }

        public void guideEast(int durationMS)
        {
            controller.guide(SX_STAR2K_EAST, durationMS);
        }

        public void guideWest(int durationMS)
        {
            controller.guide(SX_STAR2K_WEST, durationMS);
        }

         public void recordPixels(out DateTime exposureEnd)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;
            SX_READ_BLOCK readBlock;

            Log.Write("recordPixels() entered\n");

            lastExposureReadDelayedBlock = readDelayedBlock;

            initReadBlock(out readBlock, readDelayedBlock);

            controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS,
                               SX_CMD_READ_PIXELS,
                               SX_CCD_FLAGS_FIELD_ODD | SX_CCD_FLAGS_FIELD_EVEN,
                               idx,
                               (UInt16)Marshal.SizeOf(readBlock));

            lock (controller)
            {
                Log.Write("recordPixels() has locked\n");
                lock (oImageDataLock)
                {
                    imageDataValid = false;
                }
                Log.Write("recordPixels() requesting read\n");
                controller.Write(cmdBlock, readBlock, out numBytesWritten);
                exposureEnd = DateTime.Now;
                Log.Write("recordPixels() beginning downloading\n");
                downloadPixels();
                Log.Write("recordPixels() download completed\n");
                //controller.echo("hello");     
            }
            Log.Write("recordPixels() has unlocked\n");
        }

        public void recordPixelsDelayed()
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;

            lastExposureReadDelayedBlock = readDelayedBlock;

            controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, 
                                         SX_CMD_READ_PIXELS_DELAYED, 
                                         SX_CCD_FLAGS_FIELD_ODD | SX_CCD_FLAGS_FIELD_EVEN,
                                         idx,
                                         (UInt16)Marshal.SizeOf(readDelayedBlock));

            // this will be locked for a long time.  It should probably do something
            // different, like write the command, sleep for most of the time, then lock
            // and read, but that would also open the potential for other problems.


            lock (controller)
            {
                Log.Write("recordPixelsDelayed has locked\n");
                lock (oImageDataLock)
                {
                    imageDataValid = false;
                }
                Log.Write("recordPixelsDelayed requesting read\n");
                controller.Write(cmdBlock, readDelayedBlock, out numBytesWritten);
                Log.Write("recordPixelsDelayed requesting download\n");
                downloadPixels();
            }

            Log.Write("recordPixelsDelayed has unlocked\n"); 
        }
    }
}
