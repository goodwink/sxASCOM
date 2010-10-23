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
    }

    public class Camera
        :sxBase
    {
        // Variables
        private Controller controller;
        private SX_CCD_PARAMS ccdParms;
        private SX_READ_DELAYED_BLOCK nextExposure;
        private EXPOSURE_INFO currentExposure;
        Array imageRawData;
        Type pixelType;
        private Int32 [,] imageData;
        private bool imageDataValid;
        private object oImageDataLock;
        private UInt16 idx;

        // Properties

        public string description
        {
            get
            {
                string ret = "unknown (and untested) " + cameraModel;

                switch (cameraModel)
                {
                    //case 0x09:
                    //    ret = "HX9";
                    //    break;
                    //case 0x45:
                    //    ret = "MX5";
                    //    break;
                    //case 0xc5:
                    //    ret = "MX5C";
                    //    break;
                    //case 0x47:
                    //    ret = "MX7";
                    //    break;
                    //case 0xc7:
                    //    ret = "MX7C";
                    //    break;
                    //case 0x49:
                    //    ret = "MX9";
                    //    break;
                    case 137:
                        ret = "H9C";
                        break;
                    case 0x59:
                        ret = "M25C";
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
            get {return nextExposure.x_offset;}
            set 
            {
                if (value > ccdWidth)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid xOffset {0} 0<=xOffset<={1}", value, ccdParms.width), "xOffset");
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
            get {return nextExposure.y_offset;}
            set 
            {
                if (value >= ccdHeight)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid yOffset {0} 0<=yOffset<={1}", value, ccdParms.height), "yOffset");
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
            get {return nextExposure.width;}
            set
            {
                if (value == 0 || value > ccdParms.width)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid width {0} 1<=width<={1}", value, ccdParms.width), "width");
                }
                nextExposure.width = value;
            }
        }

        public UInt16 height
        {
            get {return nextExposure.height;}
            set 
            {
                if (value == 0 || value > ccdParms.height)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Invalid height {0} 1<=height<={1}", value, ccdParms.height), "height");
                }
                nextExposure.height = value;
            }
        }

        public Byte xBin
        {
            get {return nextExposure.x_bin;}
            set 
            {
                if (value <=0 || value > MAX_X_BIN)
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
            get { return MAX_X_BIN;}
        }

        public Byte yBin
        {
            get {return nextExposure.y_bin;}
            set {
                    if (value <=0 || value > MAX_Y_BIN)
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
            get { return MAX_Y_BIN;}
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

                        if (idx == 0 && cameraModel == 0x59)
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
            buildReadDelayedBlock(out nextExposure, 0, 0, ccdWidth, ccdHeight, 1, 1, 0);
            imageDataValid = false;
            oImageDataLock = new object();
            Log.Write(String.Format("sx.Camera() constructor returns\n"));
         }

        internal void checkParms(UInt16 width, UInt16 height, UInt16 xOffset, UInt16 yOffset, Byte xBin, Byte yBin)
        {
            if (width > ccdWidth)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid width {0}: 0<=width<={1}", width, ccdWidth), "width");
            }
            if (height > ccdHeight)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid height {0}: 0<=height<={1}", height, ccdHeight), "height");
            }
            if (xOffset > ccdWidth)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid xOffset {0}: 0<=width<={1}", xOffset, ccdWidth), "xOffset");
            }
            if (yOffset > ccdHeight)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid height {0}: 0<=height<={1}", yOffset, ccdHeight), "yOffset");
            }
            if (xOffset + width > ccdWidth)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid xOffset + width: 0 < xOffset {0} + width {1} <= {2}", xOffset, width, ccdWidth), "width+xOffset");
            }
            if (yOffset + height > ccdHeight)
            {
                throw new ArgumentOutOfRangeException(String.Format("Invalid yOffset + height: 0 < yOffset {0} + height {1} <= {2}", yOffset, height, ccdHeight), "height+yOffset");
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
                throw new ArgumentOutOfRangeException("height % yBin != 0");
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

        internal void adjustReadDelayedBlock(SX_READ_DELAYED_BLOCK inBlock, ref EXPOSURE_INFO exposure)
        {
            exposure.userRequested = inBlock;

            checkParms(exposure.userRequested.width, exposure.userRequested.height, exposure.userRequested.x_offset, exposure.userRequested.y_offset, exposure.userRequested.x_bin, exposure.userRequested.y_bin);

            exposure.toCamera = exposure.userRequested;

            dumpReadDelayedBlock(exposure.toCamera, "as requested - before adjustments");

            // cameras with a Bayer matrix need the offsets to be even so that the subframe returned 
            // has the same color representation as a full frame.  Since this (at most) offsets 
            // the image by 1 pixel on each axis, just do it for all cameras
            // 
            // If it isn't divisible by 2, we move it one back.  

            if (exposure.toCamera.x_bin == 1 && (exposure.toCamera.x_offset % 2) != 0)
            {
                exposure.toCamera.x_offset--;
                Log.Write(String.Format("after x bincheck x_offset = {0}\n", exposure.toCamera.x_offset));
            }

            if (exposure.toCamera.y_bin == 1 && (exposure.toCamera.y_offset % 2) != 0)
            {
                exposure.toCamera.y_offset--;
                Log.Write(String.Format("after y bincheck y_offset = {0}\n", exposure.toCamera.y_offset));
            }

            Debug.Assert(exposure.toCamera.x_bin != 1 || exposure.toCamera.x_offset % 2 == 0);
            Debug.Assert(exposure.toCamera.y_bin != 1 || exposure.toCamera.y_offset % 2 == 0);
            Debug.Assert(exposure.toCamera.width % exposure.toCamera.x_bin == 0);
            Debug.Assert(exposure.toCamera.height % exposure.toCamera.y_bin == 0);

            dumpReadDelayedBlock(exposure.toCamera, "after bayer adjust, before M25C adjustment");

            // See if our modified parameters are still legal
            try
            {
                checkParms(exposure.toCamera.width, exposure.toCamera.height, exposure.toCamera.x_offset, exposure.toCamera.y_offset, exposure.toCamera.x_bin, exposure.toCamera.y_bin);
            }
            catch (Exception ex)
            {
                Log.Write(String.Format("checkParms for toCamera generated exception {0}\n", ex));
                throw;
            }

            // I have no idea why the next bit is required, but it is.  If it isn't there,
            // the read of the image data fails. I found this in the sample basic application from SX.

            if (idx == 0 && cameraModel == 0x59)
            {
                exposure.toCamera.width *= 2;

                // in order for this to work, the height must be even

                if (exposure.toCamera.height % 2 == 1)
                {
                    if (exposure.toCamera.height + exposure.toCamera.y_bin <= ccdHeight)
                    {
                        exposure.toCamera.height += exposure.toCamera.y_bin;
                    }
                    else
                    {
                        exposure.toCamera.height -= exposure.toCamera.y_bin;
                    }
                }

                Debug.Assert(exposure.toCamera.height % 2 == 0);
                exposure.toCamera.height /= 2;

                exposure.toCamera.x_offset *= 2;

                // The y_offset must also be even.

                if (exposure.toCamera.y_offset % 2 == 1)
                {
                    Debug.Assert(exposure.toCamera.y_offset >= exposure.toCamera.y_bin);
                    exposure.toCamera.y_offset -= exposure.toCamera.y_bin;
                    Debug.Assert(exposure.toCamera.y_offset % 2 == 0);
                }

                exposure.toCamera.y_offset /= 2;  

            }

            dumpReadDelayedBlock(exposure.toCamera, "after adjust and M25C adjustment");
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

            controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_CLEAR_PIXELS, Flags, idx, 0);
            
            Log.Write("clear about to Write\n");
            controller.Write(cmdBlock, out numBytesWritten);
            Log.Write("clear about to return\n");
        }

        public void echo(string s)
        {
            controller.echo(s);
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

        void dumpParams(SX_CCD_PARAMS parms)
        {
            Log.Write(String.Format("params:"));
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

            controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, SX_CMD_GET_CCD_PARMS, 0, idx, 0);

            lock (controller)
            {
                Log.Write("getParams has locked\n");
                controller.Write(cmdBlock, out numBytesWritten);

                parms = (SX_CCD_PARAMS)controller.ReadObject(typeof(SX_CCD_PARAMS), out numBytesRead);
            }

            Log.Write("getParams has unlocked\n");

            dumpParams(parms);
        }

        internal void convertCameraDataToImageData()
        {
            Int32 binnedWidth = currentExposure.userRequested.width / currentExposure.userRequested.x_bin;
            Int32 binnedHeight = currentExposure.userRequested.height / currentExposure.userRequested.y_bin;

            if (bitsPerPixel != 16 && bitsPerPixel != 8)
            {
                throw new ArgumentOutOfRangeException("downloadPixels(): Untested: bitsPerPixel != 16 && bitPerPixel !=8", "bitsPerPixel");
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
                int x=-1, y=-1;

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

                cameraBinnedWidth  /= 2;
                cameraBinnedHeight *= 2;

                Log.Write(String.Format("convertCameraDataToImageData(): cameraBinnedWidth = {0} cameraBinnedHeight={1}\n", cameraBinnedWidth, cameraBinnedHeight));
                Log.Write(String.Format("convertCameraDataToImageData(): userPixels = {0}, cameraPixes={1}, len={2}\n", binnedWidth*binnedHeight, cameraBinnedWidth * cameraBinnedHeight, imageRawData.Length));

                int saveCase=-2;

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
                            
                            saveCase=0;
                            pixel = (UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                            if (x < binnedWidth && y < binnedHeight)
                            {
                                imageData[x, y] = pixel;
                            }

                            saveCase++;
                            pixel = (UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
                            if (x < binnedWidth && y + 1 < binnedHeight)
                            {
                                imageData[x, y + 1]     = pixel;
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
                    Log.Write("Exception data was: \n" +  ex.ToString() + "\n");
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
                            imageData[x,y ] = (UInt16)Convert.ToInt32(imageRawData.GetValue(srcIdx++));
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
            Int32 binnedWidth = currentExposure.toCamera.width / currentExposure.toCamera.x_bin;
            Int32 binnedHeight = currentExposure.toCamera.height / currentExposure.toCamera.y_bin;
            Int32 imagePixels = binnedWidth * binnedHeight;

            Log.Write(String.Format("downloadPixels(): requesting {0} pixels, {1} bytes each ({2} bytes)\n", imagePixels, Marshal.SizeOf(pixelType), imagePixels * Marshal.SizeOf(pixelType)));

            imageRawData = (Array)controller.ReadArray(pixelType, imagePixels, out numBytesRead);

            lock (oImageDataLock)
            {
                imageDataValid = true;
                imageData = null;
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

        public void recordPixels(out DateTimeOffset exposureEnd)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;
            SX_READ_BLOCK readBlock;

            Log.Write("recordPixels() entered\n");

            adjustReadDelayedBlock(nextExposure, ref currentExposure);
            initReadBlock(currentExposure.toCamera, out readBlock);

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
                exposureEnd = DateTimeOffset.Now;
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

            adjustReadDelayedBlock(nextExposure, ref currentExposure);

            controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, 
                                         SX_CMD_READ_PIXELS_DELAYED, 
                                         SX_CCD_FLAGS_FIELD_ODD | SX_CCD_FLAGS_FIELD_EVEN,
                                         idx,
                                         (UInt16)Marshal.SizeOf(currentExposure.toCamera));

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
                controller.Write(cmdBlock, currentExposure.toCamera, out numBytesWritten);
                Log.Write("recordPixelsDelayed requesting download\n");
                downloadPixels();
            }

            Log.Write("recordPixelsDelayed has unlocked\n"); 
        }
    }
}
