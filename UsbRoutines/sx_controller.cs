using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WinUsbDemo;

namespace sx
{
    public class Controller
        : sxBase
    {
        // Variables
        private USBInterface iface;
        private SX_CCD_PARAMS ccdParms;

        // Properties

        public bool Connected
        {
            get;
            private set;
        }

        public UInt32 firmwareVersion
        {
            get;
            private set;
        }

        public Byte numSerialPorts
        {
            get {return ccdParms.num_serial_ports;}
        }

        public Byte extraCapabilities
        {
            get {return ccdParms.extra_capabilities;}
        }

        public Controller()
        {
            Connected = false;

            try
            {
                iface = new USBInterface();

                reset();
                firmwareVersion = getVersion();
                getParams(ref ccdParms);
                Connected = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void buildCommandBlock(out SX_CMD_BLOCK block, Byte cmd_type, Byte cmd, Int16 cmd_value, Int16 index, Int16 cmd_length)
        {
            block.cmd_type = cmd_type;
            block.cmd = cmd;
            block.cmd_value = cmd_value;
            block.index = index;
            block.cmd_length = cmd_length;
        }

        
        internal void Write(SX_CMD_BLOCK block, Object data, out Int32 numBytesWritten)
        {
            lock (this)
            {
                iface.Write(block, data, out numBytesWritten);
            }
        }
           
        internal void Write(SX_CMD_BLOCK block, out Int32 numBytesWritten)
        {
            Write(block, null, out numBytesWritten);
        }

        internal object Read(Type returnType, Int32 numBytesToRead, out Int32 numBytesRead)
        {
            Log.Write("in controller read\n");
            object oReturn;

            lock (this)
            {
                oReturn = iface.Read(returnType, numBytesToRead, out numBytesRead);
            }
            return oReturn;
        }

        internal void Read(out byte[] bytes, Int32 numBytes, out Int32 numBytesRead)
        {
            bytes = (byte[])Read(typeof(System.Byte[]), numBytes, out numBytesRead);
        }

        internal void Read(out string s, Int32 numBytesToRead, out Int32 numBytesRead)
        {
            s = (string)Read(typeof(System.String), numBytesToRead, out numBytesRead);
        }

        internal object Read(Type returnType, out Int32 numBytesRead)
        {
            return Read(returnType, Marshal.SizeOf(returnType), out numBytesRead);
        }

        public void echo(string s)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;
            string s2;

            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_ECHO, 0, 0, (Int16)s.Length);

            Write(cmdBlock, s, out numBytesWritten);

            Read(out s2, s.Length, out numBytesRead);

            if (s2 != s)
            {
                throw new System.IO.IOException(String.Format("Echo: s2 != s ({0} != {1})", s2, s));
            }
        }

        internal void clear(Byte Flags)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;

            if ((Flags & ~(SX_CCD_FLAGS_NOWIPE_FRAME | SX_CCD_FLAGS_TDI | SX_CCD_FLAGS_NOCLEAR_FRAME)) != 0)
            {
                throw new ArgumentException("Invalid flags passed to ClearPixels");
            }

            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_CLEAR_PIXELS, Flags, 0, 0);
            Write(cmdBlock, out numBytesWritten);
        }

        public void clearCcdPixels()
        {
            clear(0);
        }

        public void clearRecordedPixels()
        {
            clear(SX_CCD_FLAGS_NOWIPE_FRAME);
        }

        public void reset()
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;

            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_RESET, 0, 0, 0);

            Write(cmdBlock, out numBytesWritten);
        }

        public uint getVersion()
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;
            byte[] bytes;
            UInt32 ver = 0;

            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_GET_FIRMWARE_VERSION, 0, 0, 0);

            Write(cmdBlock, out numBytesWritten);

            Read(out bytes, 4, out numBytesRead);

            ver = System.BitConverter.ToUInt32(bytes, 0);

            return ver;
        }

        void getParams(ref SX_CCD_PARAMS parms)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;

            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, SX_CMD_GET_CCD_PARMS, 0, 0, 0);

            Write(cmdBlock, out numBytesWritten);

            parms = (SX_CCD_PARAMS)Read(typeof(SX_CCD_PARAMS), out numBytesRead);
        }

/*
        public uint getTimer()
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;
            byte[] bytes;
            UInt32 ms = 0;

            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_GET_TIMER, 0, 0, 0);

            Write(cmdBlock, out numBytesWritten);

            Read(out bytes, 4, out numBytesRead);

            ms = System.BitConverter.ToUInt32(bytes, 0);

            return ms;
        }

        public void setTimer(UInt32 ms)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;

            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_SET_TIMER, 0, 0, (short)Marshal.SizeOf(ms));

            Write(cmdBlock, ms, out numBytesWritten);
        }
*/
    }
}
