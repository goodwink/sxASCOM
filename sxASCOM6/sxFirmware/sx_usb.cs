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
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WinUsbDemo;
using System.Diagnostics;

using Logging;

namespace sx
{
    [StructLayout(LayoutKind.Sequential, Pack=1, CharSet = CharSet.Ansi)]
    internal struct SX_CMD_BLOCK
    {
        internal Byte cmd_type;
        internal Byte cmd;
        internal UInt16 cmd_value;
        internal UInt16 index;
        internal UInt16 cmd_length;
    }


    internal class USBInterface
    {
        // Hardware info
        internal const String GUID_STRING = "{606377C1-2270-11d4-BFD8-00207812F5D5}";
        internal const Int32 MaxIoSize = 64*1024*1024;

        // Variables
        internal SafeFileHandle deviceHandle;
        internal string devicePathName;
        internal DeviceManagement myDeviceManagement = new WinUsbDemo.DeviceManagement();

        // Mutex
        internal Mutex m_mutex;

        public UInt16 vid
        {
            get;
            internal set;
        }

        public UInt16 pid
        {
            get;
            internal set;
        }

        // Look for a device with:
        //  any Vid/Pid if Vid == 0
        //  Vid/Pid as specified if skip = False
        //  any Vid/Pid except the ones specified if skip == True
        public USBInterface()
        {
            Log.Write(String.Format("USBInterface()"));
        }

        public bool connected
        {
            get;
            internal set;
        }

        public void connect(UInt16 vid, UInt16 pid, bool skip)
        {
            if (connected)
            {
                Log.Write(String.Format("USBInterface.connect() for already connected interface - vid{0}, pid={1}, skip={2}) begins\n", vid, pid, skip));
            }
            else
            {
                System.Guid Guid = new System.Guid(GUID_STRING);
                Int32 index = 0;

                Log.Write(String.Format("USBInterface.connect() begins vid{0}, pid={1}, skip={2}) begins\n", vid, pid, skip));

                while (true)
                {
                    bool bUseThisDevice = true;

                    if (!myDeviceManagement.FindDeviceFromGuid(Guid, out devicePathName, ref index))
                    {
                        throw new System.IO.IOException(String.Format("Unable to locate a USB device with GUID {0}, vid={1}, pid={2}, skip={3}", Guid, vid, pid, skip));
                    }

                    Log.Write(String.Format("USBInterface.connect(): Considering USB Device {0}\n", devicePathName));

                    UInt16 foundVid, foundPid;

                    DeviceManagement.parsePath(devicePathName, out foundVid, out foundPid);

                    Log.Write(String.Format("USBInterface.connect(): checking VID/PID - foundVID={0} foundPID={1}\n", foundVid, foundPid));

                    if (vid != 0)
                    {
                        bool bVIDMatch = (foundVid == vid);
                        bool bPIDMatch = (foundPid == pid);

                        Log.Write(String.Format("USBInterface.connect(): initally bVIDMatch={0} bPIDMatch={1}\n", bVIDMatch, bPIDMatch));

                        // a pid of 0xffff matches guide cameras

                        if (pid == 0xffff)
                        {
                           bPIDMatch = (foundPid == 507)  || (foundPid == 509)  || (foundPid == 517);
                        }

                        bUseThisDevice = (bVIDMatch && bPIDMatch);

                        Log.Write(String.Format("USBInterface.connect(): pre skip check bVIDMatch={0} bPIDMatch={1} bUseThisDevice={2}\n", bVIDMatch, bPIDMatch, bUseThisDevice));

                        if (skip)
                        {
                            bUseThisDevice = !bUseThisDevice;
                        }

                        Log.Write(String.Format("USBInterface.connect(): post skip check bVIDMatch={0} bPIDMatch={1} bUseThisDevice={2}\n", bVIDMatch, bPIDMatch, bUseThisDevice));
                    }

                    if (bUseThisDevice)
                    {
                        // For reasons I don't undersand, we can get a device handle for a device thatis already open, 
                        // even though we specify no sharing of the file when we open it.  So, in order to make sure 
                        // that we don't open the same camera twice we create a mutex that has the same name as 
                        // the device (execpt that we have to replace "\" with "/" because // mutex names cannot contain "\").
                        // If we find that the mutex already existed, we assume the device is already in use and keep looking.

                        String mutexName = devicePathName.Replace("\\", "/");
                        bool createdNew;

                        m_mutex = new Mutex(true, mutexName, out createdNew);

                        Log.Write(String.Format("USBInterface.connect(): mutexName={0} createdNew={1}\n", mutexName, createdNew));

                        if (!createdNew)
                        {
                            Log.Write(String.Format("USBInterface.connect(): mutex was already in use - closing handle and continuing search\n"));
                        }
                        else
                        {
                            Log.Write(String.Format("USBInterface: attempting to get a handle for USB Device {0}\n", devicePathName));

                            if (FileIO.GetDeviceHandle(devicePathName, out deviceHandle))
                            {
                                Log.Write(String.Format("USBInterface.connect(): deviceHandle.IsInvalid={0}\n", deviceHandle.IsInvalid));
                                connected = true;
                                vid = foundVid;
                                pid = foundPid;
                                break;
                            }

                            Log.Write(String.Format("USBInterface.connect(): Unable to get a device handle for GUID {0} using path {1} - skipping", Guid, devicePathName));

                        }

                        m_mutex.Close();
                    }
                    else
                    {
                        Log.Write(String.Format("USBInterface.connect(): skipping USB Device {0} because of skip/vid/pid\n", devicePathName));
                    }
                } 
            }
        }

        public void disconnect()
        {
            if (!connected)
            {
                Log.Write(String.Format("USBInterface.disconnect() for unconnected interface\n"));
            }
            else
            {
                Log.Write(String.Format("USBInterface.disconnect()\n"));
                FileIO.CloseDeviceHandle(deviceHandle);

                m_mutex.Close();
                m_mutex = null;
                connected = false;
            }
        }

        internal Int32 ObjectSize(Object o)
        {
            Int32 size = 0;

            if (o != null)
            {
                Type t = o.GetType();

                if (t.IsArray)
                {
                    Array a = (Array)o;
                    Type arrayType = a.GetType();
                    Type elementType = arrayType.GetElementType();

                    if (elementType == typeof(Byte))
                    {
                        size = a.Length;
                    }
                    else
                    {
                        Log.Write("Objectsize: invalid type " + elementType.ToString() + "\n");
                        throw new ArgumentException(String.Format("ObjectSize: invaild type {0}", elementType.ToString()));
                    }
                }
                else if (t == typeof(System.String))
                {
                    String s = (String)o;
                    size = s.Length;
                }
                else
                {
                    size = Marshal.SizeOf(o);
                }
            }
            return size;
        }

        internal void Write(SX_CMD_BLOCK block, Object data)
        {
            IntPtr unManagedBlockBuffer = IntPtr.Zero;
            Int32 numBytesToWrite = Marshal.SizeOf(block) + ObjectSize(data);
            Int32 numBytesWritten;
            
            try
            {
                // allocate storage
                unManagedBlockBuffer = Marshal.AllocHGlobal(numBytesToWrite);

                // Put the fixed size block into the storage
                Marshal.StructureToPtr(block, unManagedBlockBuffer, false);

                if (data != null)
                {
                    Type t = data.GetType();
                    IntPtr unManagedDataPointer = new IntPtr(unManagedBlockBuffer.ToInt64() + Marshal.SizeOf(block));

                    if (t.IsArray)
                    {
                        byte[] dataAsArray = (byte[])data;
                        Marshal.Copy(dataAsArray, 0, unManagedDataPointer, dataAsArray.Length);
                    }
                    else if (t == typeof(System.String))
                    {
                        string dataAsString = (String)data;
                        byte[] dataAsBytes = System.Text.Encoding.ASCII.GetBytes(dataAsString);
                        Marshal.Copy(dataAsBytes, 0, unManagedDataPointer, dataAsBytes.Length);
                    }
                    else
                    {
                        Marshal.StructureToPtr(data, unManagedDataPointer, false);
                    }
                }

                Log.Write(String.Format("usbWrite(): unManagedBlockBuffer=0x{0:x16} numBytesToWrite={1}\n", unManagedBlockBuffer.ToInt64(), numBytesToWrite));
                
                if (true)
                {
                    string hexData = "";
                    for (int i = 0; i < Marshal.SizeOf(block); i++)
                    {
                        hexData += String.Format("{0:x2} ", Marshal.ReadByte(unManagedBlockBuffer, i));
                    }

                    Log.Write(String.Format("Header: {0}\n", hexData));

                    if (data != null)
                    {
                        hexData = "";
                        for (int i = 0; i < ObjectSize(data); i++)
                        {
                            hexData += String.Format("{0:x2} ", Marshal.ReadByte(unManagedBlockBuffer, i + Marshal.SizeOf(block)));
                        }
                        Log.Write(String.Format("Data: {0}\n", hexData));
                    }
                }

                int ret = FileIO.WriteFile(deviceHandle, unManagedBlockBuffer, numBytesToWrite, out numBytesWritten, IntPtr.Zero);

                if (ret == 0)
                {
                    int error = Marshal.GetLastWin32Error();
                    System.ComponentModel.Win32Exception ex = new System.ComponentModel.Win32Exception();
                    string errMsg = ex.Message;

                    Log.Write("WriteFile write error=" + error + " (" + errMsg + ") numBytesToWrite=" + numBytesToWrite + " numBytesWritten=" + numBytesWritten + "\n");
                    throw new System.IO.IOException("WriteFile write error=" + error + " (" + errMsg + ") numBytesToWrite=" + numBytesToWrite + " numBytesWritten=" + numBytesWritten);
                }
                else if (numBytesWritten != numBytesToWrite)
                {
                    Log.Write("WriteFile: short write numBytesToWrite=" + numBytesToWrite + " numBytesWritten=" + numBytesWritten + "\n");
                    throw new System.IO.IOException("WriteFile: short write numBytesToWrite=" + numBytesToWrite + " numBytesWritten=" + numBytesWritten);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(unManagedBlockBuffer);
            }
        }

        internal void Write(SX_CMD_BLOCK block)
        {
            Write(block, null);
        }

        private unsafe void readBytes(byte *buf, Int32 bytesRemaining)
        {
            int error = 0;
            Log.Write(String.Format("readBytes(): begins for {0} bytes\n", bytesRemaining));

            while (bytesRemaining > 0)
            {
                Int32 bytesThisRead = Math.Min(bytesRemaining, MaxIoSize);
                Int32 bytesRead;

                if (FileIO.ReadFile(deviceHandle, buf, bytesThisRead, &bytesRead, 0))
                {
                    buf += bytesRead;
                    bytesRemaining -= bytesRead;
                    Log.Write(String.Format("readBytes(): ReadFile() read {0} bytes - bytesRemaining={1}\n", bytesRead, bytesRemaining));
                }
                else
                {
                    error = Marshal.GetLastWin32Error();
                    System.ComponentModel.Win32Exception ex = new System.ComponentModel.Win32Exception(error);
                    string errMsg = ex.Message;

                    Log.Write(String.Format("ReadFile: error={0} ({1}) bytesThisRead={2} bytesRemaining={3}", error, errMsg, bytesThisRead, bytesRemaining));

                    throw new System.IO.IOException(String.Format("ReadFile: error={0} ({1}) bytesThisRead={2} bytesRemaining={3}", error, errMsg, bytesThisRead, bytesRemaining));
                }
            }

            Log.Write("Read bytes(): ends\n");
        }

        internal unsafe void Read(byte [] buffer, Int32 numElementsToRead)
        {
            fixed (byte *buf = buffer)
            {
                readBytes(buf, numElementsToRead * sizeof(byte));
            }
        }

        internal void Read(byte [] buffer)
        {
            Read(buffer, buffer.Length * sizeof(byte));
        }

        internal unsafe void Read(UInt16 [] buffer, Int32 numElementsToRead)
        {
            fixed (UInt16 *buf = buffer)
            {
                readBytes((byte *)buf, numElementsToRead*sizeof(UInt16));
            }
        }

        internal void Read(UInt16 [] buffer)
        {
            Read(buffer, buffer.Length);
        }

        internal object Read(Type returnType, Int32 numBytesToRead)
        {
            IntPtr unManagedBuffer = IntPtr.Zero;
            Object obj = null;
            Int32 numBytesRead = 0;

            Log.Write(String.Format("interface Read({0}, {1}) begins\n", returnType, numBytesToRead));
            try
            {
                unManagedBuffer = Marshal.AllocHGlobal(numBytesToRead);

                while (numBytesToRead > numBytesRead)
                {
                    Int32 thisRead = 0;
                    IntPtr buf = new IntPtr(unManagedBuffer.ToInt64() + numBytesRead);
                    Int32 readSize = numBytesToRead - numBytesRead;

                    Log.Write(String.Format("usbRead: buf=0x{0:x16} numBytesToRead={1,8} numBytesRead={2,8} readSize={3,8}\n", buf.ToInt64(), numBytesToRead, numBytesRead, readSize));
                    if (FileIO.ReadFile(deviceHandle, buf, readSize, out thisRead, IntPtr.Zero) > 0)
                    {
                        numBytesRead += thisRead;
                    }
                    else
                    {
                        int error = Marshal.GetLastWin32Error();

                        System.ComponentModel.Win32Exception ex = new System.ComponentModel.Win32Exception();
                        string errMsg = ex.Message;

                        Log.Write("ReadFile: error=" + error + " (" + errMsg + ") to read=" + numBytesToRead + " read=" + numBytesRead + "\n");
                        throw new System.IO.IOException("ReadFile: error=" + error + " (" + errMsg + ") to read=" + numBytesToRead + " read=" + numBytesRead);
                    }
               }

                Debug.Assert(numBytesRead == numBytesToRead);

                Log.Write("ReadFile: after loop numBytesRead=" + numBytesRead + "\n");

                if (returnType == typeof(System.String))
                {
                    obj = Marshal.PtrToStringAnsi(unManagedBuffer, numBytesRead);
                }
                else if (returnType.IsArray)
                {
                    if (returnType == typeof(byte[]))
                    {
                        Int32 numPixels = numBytesRead;
                        byte[] bytes = new byte[numPixels];
                        Marshal.Copy(unManagedBuffer, bytes, 0, numPixels);
                        obj = bytes;
                    }
                    else if (returnType == typeof(Int16[]) || returnType == typeof(UInt16[]))
                    {
                        Int32 numPixels = numBytesRead / Marshal.SizeOf(typeof(Int16));
                        Int16[] shorts = new Int16[numPixels];
                        Marshal.Copy(unManagedBuffer, shorts, 0, numPixels);
                        obj = shorts;

                    }
                    else if (returnType == typeof(Int32[]) || returnType == typeof(UInt32[]))
                    {
                        Int32 numPixels = numBytesRead / Marshal.SizeOf(typeof(Int32));
                        Int32[] ints = new Int32[numPixels];
                        Marshal.Copy(unManagedBuffer, ints, 0, numPixels);
                        obj = ints;
                    }
                    else
                    {
                        throw new ArgumentException(String.Format("Read: unahandled array type {0}", returnType.ToString()));
                    }
                }
                else
                {
                    obj = Marshal.PtrToStructure(unManagedBuffer, returnType);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(unManagedBuffer);
            }

            return obj;
        }
    }
}
