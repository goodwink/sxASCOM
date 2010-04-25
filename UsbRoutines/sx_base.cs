using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using Microsoft.Win32.SafeHandles;
using WinUsbDemo;

namespace sx
{
    internal static class Log
    {
        private const string logPath = @"c:\temp\sx_log.txt";
        private static FileStream logFS;

        static Log()
        {
            logFS = File.Create(logPath);
        }
        
        internal static void Write(string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            logFS.Write(info, 0, info.Length);
        }
    }

    public class sxBase
    {
        // cmd_type
        internal const Byte SX_CMD_TYPE_PARMS = 0x40;
        internal const Byte SX_CMD_TYPE_READ = 0xc0;

        // commands
        internal const Byte SX_CMD_GET_FIRMWARE_VERSION = 255;
        internal const Byte SX_CMD_ECHO = 0;
        internal const Byte SX_CMD_CLEAR_PIXELS = 1;
        internal const Byte SX_CMD_READ_PIXELS_DELAYED = 2;
        internal const Byte SX_CMD_READ_PIXELS = 3;
        internal const Byte SX_CMD_SET_TIMER = 4;
        internal const Byte SX_CMD_GET_TIMER = 5;
        internal const Byte SX_CMD_RESET = 6;
        internal const Byte SX_CMD_SET_CCD_PARMS = 7;
        internal const Byte SX_CMD_GET_CCD_PARMS = 8;
        internal const Byte SX_CMD_SET_STAR2K = 9;
        internal const Byte SX_CMD_WRITE_SERIAL_PORT = 10;
        internal const Byte SX_CMD_READ_SERIAL_PORT = 11;
        internal const Byte SX_CMD_SET_SERIAL = 12;
        internal const Byte SX_CMD_GET_SERIAL = 13;
        internal const Byte SX_CMD_CAMERA_MODEL = 14;
        internal const Byte SX_CMD_LOAD_EEPROM = 15;

        // flags
        internal const Byte SX_CCD_FLAGS_FIELD_ODD = 1;       // Specify odd field for MX cameras
        internal const Byte SX_CCD_FLAGS_FIELD_EVEN = 2;	   // Specify even field for MX cameras
        internal const Byte SX_CCD_FLAGS_NOBIN_ACCUM = 4;	   // Don't accumulate charge if binning
        internal const Byte SX_CCD_FLAGS_NOWIPE_FRAME = 8;	   // Don't apply WIPE when clearing frame
        internal const Byte SX_CCD_FLAGS_TDI = 32;	           // Implement TDI (drift scan) operation
        internal const Byte SX_CCD_FLAGS_NOCLEAR_FRAME = 64;  // Don't clear frame, even when asked

        // constants
        internal const Byte BITS_PER_BYTE=8; 
        // limits
        internal Byte MAX_BIN=4; // I made this up

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

        const Byte STAR2000_PORT = 0x1;
        const Byte DEPRICATED_COMPRESSED_PIXEL_FORMAT = 0x2;
        const Byte EEPROM = 0x4;
        const Byte INTEGRATED_GUIDER_CCD = 0x8;

        const UInt16 COLOR_MATRIX_PACKED_RGB         = 0x8000;
        const UInt16 COLOR_MATRIX_PACKED_BGR         = 0x4000;
        const UInt16 COLOR_MATRIX_PACKED_RED_SIZE    = 0x0F00;
        const UInt16 COLOR_MATRIX_PACKED_GREEN_SIZE  = 0x00F0;
        const UInt16 COLOR_MATRIX_PACKED_BLUE_SIZE   = 0x000F;
        const UInt16 COLOR_MATRIX_MATRIX_ALT_EVEN    = 0x2000;
        const UInt16 COLOR_MATRIX_MATRIX_ALT_ODD     = 0x1000;
        const UInt16 COLOR_MATRIX_MATRIX_2X2         = 0x0000;
        const UInt16 COLOR_MATRIX_MATRIX_RED_MASK    = 0x0F00;
        const UInt16 COLOR_MATRIX_MATRIX_GREEN_MASK  = 0x00F0;
        const UInt16 COLOR_MATRIX_MATRIX_BLUE_MASK   = 0x000F;
        const UInt16 COLOR_MATRIX_MONOCHROME         = 0x0FFF;
    }
}
