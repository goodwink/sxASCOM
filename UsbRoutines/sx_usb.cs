using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WinUsbDemo;

namespace sx
{
    [StructLayout(LayoutKind.Sequential, Pack=1, CharSet = CharSet.Ansi)]
    internal struct SX_CMD_BLOCK
    {
        internal Byte cmd_type;
        internal Byte cmd;
        internal Int16 cmd_value;
        internal Int16 index;
        internal Int16 cmd_length;
    }

    internal class USBInterface
    {

        // Hardware info

        internal const String GUID_STRING = "{606377C1-2270-11d4-BFD8-00207812F5D5}";
        internal const int MAX_READ_SIZE = 64 * 1024;

        // Variables
        internal SafeFileHandle deviceHandle;
        internal string devicePathName = "";
        internal DeviceManagement myDeviceManagement = new WinUsbDemo.DeviceManagement();

        public USBInterface()
        {
            System.Guid Guid = new System.Guid(GUID_STRING);

            Boolean deviceFound = myDeviceManagement.FindDeviceFromGuid(Guid, ref devicePathName);

            if (!deviceFound)
            {
                throw new System.IO.IOException(String.Format("Unable to locate a USB device with GUID {0}", Guid));
            }

            if (!FileIO.GetDeviceHandle(devicePathName, out deviceHandle))
            {
                throw new System.IO.IOException(String.Format("Unable to get a device handle for GUID {0} using path {1}", Guid, devicePathName));
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
                        Log.Write("Objectsize: invalid type " + elementType.ToString());
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

        internal void Write(SX_CMD_BLOCK block, Object data, out Int32 numBytesWritten)
        {
            IntPtr unManagedBlockBuffer = IntPtr.Zero;
            Int32 numBytesToWrite = Marshal.SizeOf(block) + ObjectSize(data);
            
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
                        byte [] dataAsArray = (byte [])data;
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

        internal void Write(SX_CMD_BLOCK block, out Int32 numBytesWritten)
        {
            Write(block, null, out numBytesWritten);
        }

        internal object Read(Type returnType, Int32 numBytesToRead, out Int32 numBytesRead)
        {
            IntPtr unManagedBuffer = IntPtr.Zero;
            Object obj = null;
            numBytesRead = 0;

            try
            {
                unManagedBuffer = Marshal.AllocHGlobal(numBytesToRead);
                int error = 0;

                while (error == 0 && numBytesToRead > numBytesRead)
                {
                    Int32 thisRead = 0;
                    IntPtr buf = new IntPtr(unManagedBuffer.ToInt64() + numBytesRead);
                    Int32 readSize = numBytesToRead - numBytesRead;

                    //if (readSize > MAX_READ_SIZE)
                    //    readSize = MAX_READ_SIZE;

                    Log.Write(String.Format("usbRead: buf=0x{0:x16} numBytesToRead={1} numBytesRead={2} readSize={3}\n", buf.ToInt64(), numBytesToRead, numBytesRead, readSize));

                    int ret = FileIO.ReadFile(deviceHandle, buf, readSize, out thisRead, IntPtr.Zero);

                    if (ret == 0)
                    {
                        error = Marshal.GetLastWin32Error();
                    }
                    else
                    {
                        numBytesRead += thisRead;
                    }
                }

                if (error != 0)
                {
                    System.ComponentModel.Win32Exception ex = new System.ComponentModel.Win32Exception();
                    string errMsg = ex.Message;

                    Log.Write("ReadFile: error=" + error + " (" + errMsg + ") to read=" + numBytesToRead + " read=" + numBytesRead + "\n");
                    throw new System.IO.IOException("ReadFile: error=" + error + " (" + errMsg + ") to read=" + numBytesToRead + " read=" + numBytesRead);

                }

                if (numBytesRead != numBytesToRead)
                {
                    Log.Write("ReadFile: short read numBytesToRead=" + numBytesToRead + " numBytesRead=" + numBytesRead + "\n");
                    throw new System.IO.IOException("ReadFile: short read numBytesToRead=" + numBytesToRead + " numBytesRead=" + numBytesRead);
                }

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
                    else if (returnType == typeof(Int16[]))
                    {
                        Int32 numPixels = numBytesRead / 2;
                        Int16[] shorts = new Int16[numPixels];
                        Marshal.Copy(unManagedBuffer, shorts, 0, numPixels);
                        obj = shorts;
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

/*
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
*/
    }
}
