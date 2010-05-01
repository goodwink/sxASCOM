using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WinUsbDemo;

namespace sx
{
    public class Camera
        :sxBase
    {

        // Variables
        private Controller controller;
        private SX_CCD_PARAMS ccdParms;
        private SX_READ_DELAYED_BLOCK readDelayedBlock;
        private UInt32[,] imageData;
        private bool imageDataValid;
        private Int16 idx;

        // Properties

        public string description
        {
            get
            {
                string ret = "unknown" + cameraModel;

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

        public Byte extraCapabilities
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
            get { return MAX_BIN;}
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
            get { return MAX_BIN;}
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
                Log.Write("ImageArray entered: imageDataValid=" + imageDataValid + "\n");

                if (!imageDataValid)
                {
                    throw new ArgumentException("ImageArray not valid");
                }

                return imageData; 
            }
        }

        public Camera(Controller controller, Int16 cameraIdx)
        {
            idx = cameraIdx;

            try
            {
                this.controller = controller;

                cameraModel = getModel();
                getParams(ref ccdParms);
                buildReadDelayedBlock(out readDelayedBlock, 0, 0, ccdWidth, ccdHeight, 1, 1, 0);
                imageDataValid = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // we use a READ_DELAYED_BLOCK to store paramters that are accessed as properties.  
        // If the user requests a read without delay, we can just copy all the matching 
        // parameters out of the one we are keeping

        internal void initReadBlock(out SX_READ_BLOCK block, SX_READ_DELAYED_BLOCK inblock)
        {
            block.x_offset = inblock.x_offset;
            block.y_offset = inblock.y_offset;
            block.width = inblock.width;
            block.height = inblock.height;
            block.x_bin = inblock.x_bin;
            block.y_bin = inblock.y_bin;
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
            clear(SX_CCD_FLAGS_NOWIPE_FRAME);
        }

        public int getTimer()
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;
            byte[] bytes;
            Int32 ms = 0;

            controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_GET_TIMER, 0, 0, 0);

            //lock (controller)
            {
                Log.Write("getTimer has locked\n");
                controller.Write(cmdBlock, out numBytesWritten);

                controller.Read(out bytes, 4, out numBytesRead);
            }
            Log.Write("getTimer has unlocked\n");

            ms = System.BitConverter.ToInt32(bytes, 0);

            Log.Write("Timer 0 = " + ms + "\n");

            return ms;
        }

        public void setTimer(UInt32 ms)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;

            controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_SET_TIMER, 0, idx, (short)Marshal.SizeOf(ms));
            controller.Write(cmdBlock, ms, out numBytesWritten);
        }

        public UInt16 getModel()
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;
            byte[] bytes = new byte[2];
            UInt16 model=0;

            controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, SX_CMD_CAMERA_MODEL, 0, idx, (short)Marshal.SizeOf(model));

            
            //lock (controller)
            {
                Log.Write("getModel has locked\n");
                controller.Write(cmdBlock, out numBytesWritten);

                controller.Read(out  bytes, 2, out numBytesRead);
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

            //lock (controller)
            {
                Log.Write("getParams has locked\n");
                controller.Write(cmdBlock, out numBytesWritten);

                parms = (SX_CCD_PARAMS)controller.Read(typeof(SX_CCD_PARAMS), out numBytesRead);
            }
            Log.Write("getParams has unlocked\n");
        }

        internal void getRecordedPixels()
        {
            Int32 numBytesRead;
            Int32 binnedWidth = width/xBin;
            Int32 binnedHeight = height/yBin;
            Int32 imageBytes = binnedWidth * binnedHeight * bitsPerPixel/BITS_PER_BYTE;

            Log.Write("getRecordedPixels() is about to read " + imageBytes + "\n");

            byte [] imageAsBytes = (byte[])controller.Read(typeof(byte[]), imageBytes, out numBytesRead);

            Log.Write("getRecordedPixels() - read completed, numBytesRead=" + numBytesRead + "\n");

            if (bitsPerPixel != 16 && bitsPerPixel != 8)
            {
                throw new ArgumentOutOfRangeException("getRecordedPixels Untested: bitsPerPixel != 16", "bitsPerPixel");
            }
            imageData = new UInt32[binnedWidth, binnedHeight];

            // Copy the bytes read from the camera into a UInt32 array.
            // There must be a better way to do this, but I don't know what it is. 

            Int32 byteoffset = 0;
            UInt32 min = 9999999, max = 0;

            Log.Write("getRecordedPixels() decoding data, bitsPerPixel=" + bitsPerPixel +" binnedWidth = " + binnedWidth + " binnedHeight="+ binnedHeight + "\n");
            
            for (int y = 0; y < binnedHeight; y++)
            {
                for (int x = 0; x < binnedWidth; x++)
                {
                    UInt32 pixelValue;

                    switch(bitsPerPixel)
                    {
                        case 8:
                            pixelValue = imageAsBytes[byteoffset];
                            byteoffset += 1;
                            break;
                        case 16:
                            pixelValue = System.BitConverter.ToUInt16(imageAsBytes, byteoffset);
                            byteoffset += 2;
                            break;
                        case 32:
                            pixelValue = System.BitConverter.ToUInt32(imageAsBytes, byteoffset);
                            byteoffset += 4;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(String.Format("Unexpected bitsPerPixel {0}", bitsPerPixel), "bitsPerPixel");
                    }
                    if (pixelValue < min)
                        min = pixelValue;
                    if (pixelValue > max)
                        max = pixelValue;
                    if (y == 0 || y == binnedHeight-1)
                    {
                        //Log.Write("p[" + x + "][0] = " + pixelValue + "\n");
                    }
                    imageData[x, y] = pixelValue;
                }
            }
            imageDataValid = true;
            Log.Write("getRecordedPixels(): returns, min=" + min +" max=" + max + "\n");
        }

        public void recordPixels(out DateTime exposureEnd)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;
            SX_READ_BLOCK readBlock;

            initReadBlock(out readBlock, readDelayedBlock);

            controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, 
                                         SX_CMD_READ_PIXELS, 
                                         SX_CCD_FLAGS_FIELD_ODD | SX_CCD_FLAGS_FIELD_EVEN,
                                         idx, 
                                         (Int16)Marshal.SizeOf(readBlock));

            //lock (controller)
            {
                Log.Write("recordPixels has locked\n");
                controller.Write(cmdBlock, readBlock, out numBytesWritten);
                exposureEnd = DateTime.Now;
                getRecordedPixels();
            }
            Log.Write("recordPixels has unlocked\n");
        }

        public void recordPixelsDelayed(out DateTime exposureEnd)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;

            controller.buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, 
                                         SX_CMD_READ_PIXELS_DELAYED, 
                                         SX_CCD_FLAGS_FIELD_ODD | SX_CCD_FLAGS_FIELD_EVEN,
                                         idx,
                                         (Int16)Marshal.SizeOf(readDelayedBlock));

            //lock (controller)
            {
                Log.Write("recordPixelsDelayed has locked\n"); 
                // this will be locked for a long time.  It should probably do something
                // different, like write the command, sleep for most of the time, then lock
                // and read, but that would also open the potential for other problems.
                controller.Write(cmdBlock, readDelayedBlock, out numBytesWritten);
                exposureEnd = DateTime.Now;
                getRecordedPixels();
            }
            Log.Write("recordPixelsDelayed has unlocked\n"); 
        }
    }
}
