using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;

using Microsoft.Win32.SafeHandles;
using WinUsbDemo;
using Logging;

namespace sx
{
    /// <summary>
    /// This class defines the the functions to access the resources which exist on the 
    /// USB controller board.  If there are two cameras conneted (a main camera and a guide
    /// camera), there is still only one of these devices present.
    ///
    /// Locking:
    ///    The controller object is used as the lock.  It is necessary to lock the interface when
    /// a transaction is occuring.  There are three types of transactions:
    /// - simple command writes that return no data
    /// - operations that require a write to begin, and which return data that is collected with read
    /// - operations that require a write to begin and another write to end (STAR2K guiding is the only one)
    /// 
    /// Much of the functionality is defined by the hardware.  The controller on the SX cameras is fairly simple,
    /// and the following limitations were observed:
    /// - It is not possible to perform full duplex operations - you cannot write to the device if a read is in progress. I
    ///   assume the converse is also true, but writes are so fast that I never hit it and it wasn't worth the effort to
    ///   verify it.
    /// - If you attempt to do simultanious reads and writes, one operation (generally the write) will timeout after 10 seconds.  
    ///   The operation that times our returns an I/O error.
    /// - These factors together caused the guide routine to be rewritten to be synchronous. It would be possible to make it 
    ///   async, but it would have to hold the lock the entire time, so there is no real benefit to doing so.  
    ///   If it did not hold the lock, it would be possible for a read to occur between the write which
    ///   starts guiding and the write which stops it, making a guide operation that was requrest for a few MS take (at best) several seconds or
    ///   (at worst ) never stop because the write operation failed
    /// - Despite there being two cameras, there is only one hardware timer, which can be used to control either camera but
    ///   not both.  
    /// - Use of the hardware camera requires keeping the controller locked for the entire exposure.
    ///   My preferred usage is to use the hardware timer for the guide camera when present.  The exposures are usually short, so 
    ///   having the controller locked for the entire time isn't too bad. Also, the autoguiding programs I have looked at tend to take
    ///   an exposure, then guide, then take an exposure.  This usage pattern prevents the "lock during HW exposure" and the "lock while guiding"
    ///   from actually colliding.
    /// </summary>

    public class Controller
        : sxBase
    {
        // Variables
        private USBInterface m_iface;
        private SX_CCD_PARAMS m_ccdParms;
        UInt32 m_version;

        // Properties

        public bool Connected
        {
            get;
            private set;
        }

        protected void verifyConnected(string caller)
        {
            if (!Connected)
            {
                throw new System.Exception(String.Format("{0}: Camera not connected", caller));
            }
        }

        public UInt32 firmwareVersion
        {
            get
            {
                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_version;
            }

            private set
            {
                m_version = value;
            }
        }

        public Byte numSerialPorts
        {
            get 
            {
                verifyConnected(MethodBase.GetCurrentMethod().Name);

                return m_ccdParms.num_serial_ports;
            }
        }

        public Controller()
        {
            Log.Write(String.Format("Controller() entered\n"));

            Connected = false;

            Log.Write("Controller(): returns\n");
        }

        public void connect(UInt16 vid, UInt16 pid, bool skip)
        {
            Log.Write(String.Format("controller.connect({0}, {1}, {2})\n", vid, pid, skip));

            m_iface = new USBInterface(vid, pid, skip);

            Connected = true;

            reset();
            firmwareVersion = getVersion();
            getParams(ref m_ccdParms);

            Log.Write("controller.connect(): returns\n");
        }

        internal void buildCommandBlock(out SX_CMD_BLOCK block, Byte cmd_type, Byte cmd, UInt16 cmd_value, UInt16 index, UInt16 cmd_length)
        {
            verifyConnected(MethodBase.GetCurrentMethod().Name);

            block.cmd_type = cmd_type;
            block.cmd = cmd;
            block.cmd_value = cmd_value;
            block.index = index;
            block.cmd_length = cmd_length;
            Log.Write(String.Format("buildCommandBlock(): type=0x{0:x2} cmd=0x{1:x2} cmd_value=0x{2:x4} index=0x{3:x4} cmd_length=0x{4:x4}\n", cmd_type, cmd, cmd_value, index, cmd_length));
        }

        internal void Write(SX_CMD_BLOCK block, Object data, out Int32 numBytesWritten)
        {
            verifyConnected(MethodBase.GetCurrentMethod().Name);

            // I lock here to prevent the data from two writes from getting interleaved. I doubt windows would actually do that, but 
            // it is easy to prevent it here and then I know I don't have to worry about it.
            lock (this)
            {
                Log.Write("Write has locked\n");
                m_iface.Write(block, data, out numBytesWritten);
            }
            Log.Write("Write has unlocked\n");
        }
           
        internal void Write(SX_CMD_BLOCK block, out Int32 numBytesWritten)
        {
            Write(block, null, out numBytesWritten);
        }

        internal object Read(Type returnType, Int32 numBytesToRead, out Int32 numBytesRead)
        {
            object oReturn;
            
            verifyConnected(MethodBase.GetCurrentMethod().Name);

            // See the comment above the lock in Write() for more information on this lock. 
            lock (this)
            {
                Log.Write("Read has locked\n");
                oReturn = m_iface.Read(returnType, numBytesToRead, out numBytesRead);
            }
            Log.Write("Read has unlocked\n"); 
            return oReturn;
        }

        internal Array ReadArray(Type elementType, Int32 numElements, out Int32 numBytesRead)
        {
            Log.Write(String.Format("ReadArry begins, reading {0} elements of type {1}\n", numElements, elementType));
            return (Array)Read(System.Array.CreateInstance(elementType, 0).GetType(), numElements * Marshal.SizeOf(elementType), out numBytesRead);
        }

        internal byte[] ReadBytes(Int32 numBytes, out Int32 numBytesRead)
        {
            return (byte[])Read(typeof(byte[]), numBytes, out numBytesRead);
        }

        internal string ReadString(Int32 numCharsToRead, out Int32 numBytesRead)
        {
            return (string)Read(typeof(System.String), numCharsToRead, out numBytesRead);
        }

        internal object ReadObject(Type returnType, out Int32 numBytesRead)
        {
            return Read(returnType, Marshal.SizeOf(returnType), out numBytesRead);
        }

        public void echo(string s)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;
            string s2;

            verifyConnected(MethodBase.GetCurrentMethod().Name);

            Log.Write(String.Format("echo({0}) begins\n", s));
            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_ECHO, 0, 0, (UInt16)s.Length);

            lock (this)
            {

                Write(cmdBlock, s, out numBytesWritten);

                s2 = ReadString(s.Length, out numBytesRead);
            }

            if (s2 != s)
            {
                throw new System.IO.IOException(String.Format("Echo: s2 != s ({0} != {1})", s2, s));
            }
            Log.Write("echo() completed successfully");
        }

        public void reset()
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;

            verifyConnected(MethodBase.GetCurrentMethod().Name);

            Log.Write("resetting()\n");

            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_RESET, 0, 0, 0);
            Write(cmdBlock, out numBytesWritten);
        }

        public uint getVersion()
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;
            byte[] bytes;
            UInt32 ver = 0;

            verifyConnected(MethodBase.GetCurrentMethod().Name);

            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_GET_FIRMWARE_VERSION, 0, 0, 0);

            lock (this)
            {
                Log.Write("getVersion has locked\n");
                Write(cmdBlock, out numBytesWritten);

                bytes = ReadBytes(Marshal.SizeOf(ver), out numBytesRead);
            }
            Log.Write("getVersion has unlocked\n");

            ver = System.BitConverter.ToUInt32(bytes, 0);

            Log.Write(String.Format("getVersion() returns {0}\n", ver));

            return ver;
        }

        void getParams(ref SX_CCD_PARAMS parms)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;

            verifyConnected(MethodBase.GetCurrentMethod().Name);

            Log.Write("gettings params\n");

            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_READ, SX_CMD_GET_CCD_PARMS, 0, 0, 0);

            Write(cmdBlock, out numBytesWritten);

            parms = (SX_CCD_PARAMS)ReadObject(typeof(SX_CCD_PARAMS), out numBytesRead);
        }

        public int getTimer()
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten, numBytesRead;
            byte[] bytes;
            Int32 ms = 0;

            verifyConnected(MethodBase.GetCurrentMethod().Name);

            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_GET_TIMER, 0, 0, 0);

            lock (this)
            {
                Log.Write("getTimer has locked\n");
                Write(cmdBlock, out numBytesWritten);

                bytes = ReadBytes(Marshal.SizeOf(ms), out numBytesRead);
            }
            Log.Write("getTimer has unlocked\n");

            ms = System.BitConverter.ToInt32(bytes, 0);

            Log.Write("Timer = " + ms + "\n");

            return ms;
        }

        public void setTimer(UInt32 ms)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;

            verifyConnected(MethodBase.GetCurrentMethod().Name);

            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_SET_TIMER, 0, 0, (UInt16)Marshal.SizeOf(ms));
            Write(cmdBlock, ms, out numBytesWritten);
        }

        public Boolean hasGuideCamera
        {
            get
            { 
                verifyConnected(MethodBase.GetCurrentMethod().Name);
                return (m_ccdParms.extra_capabilities & SXUSB_CAPS_GUIDER) == SXUSB_CAPS_GUIDER;
            }
        }

        public Boolean hasGuidePort
        {
            get
            {
                verifyConnected(MethodBase.GetCurrentMethod().Name);
                return (m_ccdParms.extra_capabilities & SXUSB_CAPS_STAR2K) == SXUSB_CAPS_STAR2K; 
            }
        }

        public void guide(UInt16 direction, int durationMS)
        {
            SX_CMD_BLOCK cmdBlock;
            Int32 numBytesWritten;
            DateTime guideStart = DateTime.Now;
            
            verifyConnected(MethodBase.GetCurrentMethod().Name);

            if (!hasGuidePort)
            {
                throw new System.Exception("Guide request but no guide port");
            }

            buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_SET_STAR2K, direction, 0, 0);

            Log.Write(String.Format("guide({0}, {1}) begins\n", direction, durationMS));

            lock (this)
            {
                try
                {
                    TimeSpan desiredGuideDuration = TimeSpan.FromMilliseconds(durationMS);
                    DateTime guideEnd = guideStart + desiredGuideDuration;

                    Write(cmdBlock, out numBytesWritten);

                    // We sleep for most of the guide time, then spin for the last little bit
                    // because this helps us end closer to the right time

                    Log.Write("guide(): about to begin loop\n");

                    for (TimeSpan remainingGuideTime = desiredGuideDuration;
                        remainingGuideTime.TotalMilliseconds > 0;
                        remainingGuideTime = guideEnd - DateTime.Now)
                    {
                        if (remainingGuideTime.TotalMilliseconds > 75)
                        {
                            // sleep in small chunks so that we are responsive to abort and stop requests
                            //Log.Write("Before sleep, remaining exposure=" + remainingGuideTime.TotalSeconds + "\n");
                            Thread.Sleep(50);
                        }
                    }
                }
                finally
                {
                    buildCommandBlock(out cmdBlock, SX_CMD_TYPE_PARMS, SX_CMD_SET_STAR2K, SX_STAR2K_STOP, 0, 0);
                    Write(cmdBlock, out numBytesWritten);
                }
            }

            Log.Write(String.Format("guide(): delay ends, actualGuideLength={0:F4}\n", (DateTime.Now - guideStart).TotalMilliseconds));
        }
    }
}
