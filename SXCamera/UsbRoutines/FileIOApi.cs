using System;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace WinUsbDemo
{
    ///  <summary>
    ///  API declarations relating to file I/O (and used by WinUsb).
    ///  </summary>

    sealed internal class FileIO
    {
        internal const Int32 FILE_ATTRIBUTE_NORMAL = 0X80;
        internal const Int32 FILE_FLAG_OVERLAPPED = 0X40000000;
        internal const Int32 FILE_SHARE_READ = 1;
        internal const Int32 FILE_SHARE_WRITE = 2;
        internal const UInt32 GENERIC_READ = 0X80000000;
        internal const UInt32 GENERIC_WRITE = 0X40000000;
        internal const Int32 INVALID_HANDLE_VALUE = -1;
        internal const Int32 OPEN_EXISTING = 3;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern SafeFileHandle CreateFile(String lpFileName, UInt32 dwDesiredAccess, Int32 dwShareMode, IntPtr lpSecurityAttributes, Int32 dwCreationDisposition, Int32 dwFlagsAndAttributes, Int32 hTemplateFile);

        [DllImport("kernel32", SetLastError = true)]
        internal extern static int ReadFile(SafeFileHandle handle, IntPtr lpBuffer, Int32 numBytesToRead, out Int32 numBytesRead, IntPtr overlapped_MustBeZero);

        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        internal extern static unsafe bool ReadFile
        (
            SafeFileHandle hFile,     // handle to file
            void* pBuffer,            // data buffer
            int NumberOfBytesToRead,  // number of bytes to read
            int *pNumberOfBytesRead,  // number of bytes read
            int Overlapped            // overlapped buffer
        );


        [DllImport("kernel32", SetLastError = true)]
        internal extern static int WriteFile(SafeFileHandle handle, IntPtr lpBuffer, Int32 numBytesToWrite, out Int32 numBytesWritten, IntPtr overlapped_MustBeZero);

        // Free the kernel's file object (close the file).
        [DllImport("kernel32", SetLastError = true)]
        internal extern static bool CloseHandle(SafeFileHandle handle);


        ///  <summary>
        ///  Requests a handle with CreateFile.
        ///  </summary>
        ///  
        ///  <param name="devicePathName"> Returned by SetupDiGetDeviceInterfaceDetail 
        ///  in an SP_DEVICE_INTERFACE_DETAIL_DATA structure. </param>
        ///  
        ///  <returns>
        ///  The handle.
        ///  </returns>

        public static Boolean GetDeviceHandle(String devicePathName, out SafeFileHandle deviceHandle)
        {
            // ***
            // API function

            //  summary
            //  Retrieves a handle to a device.

            //  parameters 
            //  Device path name returned by SetupDiGetDeviceInterfaceDetail
            //  Type of access requested (read/write).
            //  FILE_SHARE attributes to allow other processes to access the device while this handle is open.
            //  Security structure. Using Null for this may cause problems under Windows XP.
            //  Creation disposition value. Use OPEN_EXISTING for devices.
            //  Flags and attributes for files. The winsub driver requires FILE_FLAG_OVERLAPPED.
            //  Handle to a template file. Not used.

            //  Returns
            //  A handle or INVALID_HANDLE_VALUE.
            // ***

            deviceHandle = FileIO.CreateFile
                (devicePathName,
                (FileIO.GENERIC_WRITE | FileIO.GENERIC_READ),
                0,
                IntPtr.Zero,
                FileIO.OPEN_EXISTING,
                FileIO.FILE_ATTRIBUTE_NORMAL,
                0);

            return (!(deviceHandle.IsInvalid));
        }

        public static Boolean CloseDeviceHandle(SafeFileHandle deviceHandle)
        {
            return FileIO.CloseHandle(deviceHandle);
        }
    }
}
