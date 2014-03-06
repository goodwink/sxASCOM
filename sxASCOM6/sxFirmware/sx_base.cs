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
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WinUsbDemo;

namespace sx
{
    public class sxBase
    {
        // cmd_type
        internal const Byte SX_CMD_TYPE_PARMS = 0x40;
        internal const Byte SX_CMD_TYPE_READ  = 0xc0;

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
        internal const Byte SX_CMD_COOLER_CONTROL = 30;
        internal const Byte SX_CMD_SHUTTER_CONTROL = 32;

        // flags
        internal const Byte SX_CCD_FLAGS_FIELD_ODD = 1;        // Specify odd field for MX cameras
        internal const Byte SX_CCD_FLAGS_FIELD_EVEN = 2;	   // Specify even field for MX cameras
        internal const Byte SX_CCD_FLAGS_NOBIN_ACCUM = 4;	   // Don't accumulate charge if binning
        internal const Byte SX_CCD_FLAGS_NOWIPE_FRAME = 8;	   // Don't apply WIPE when clearing frame
        internal const Byte SX_CCD_FLAGS_TDI = 32;	           // Implement TDI (drift scan) operation
        internal const Byte SX_CCD_FLAGS_NOCLEAR_FRAME = 65;   // Don't clear frame, even when asked
        internal const Byte SX_CCD_FLAGS_CLEAR_VERT = 128;     // Clear the vertical registers

        // STAR2K values
        internal const UInt16 SX_STAR2K_STOP  = 0;
        internal const UInt16 SX_STAR2K_WEST  = 1;
        internal const UInt16 SX_STAR2K_SOUTH = 2; 
        internal const UInt16 SX_STAR2K_NORTH = 4;
        internal const UInt16 SX_STAR2K_EAST  = 8;
        
        // limits
        internal const Byte MAX_BIN=8; // The hardware will bin 1-6 & 8, skipping 7
        internal const Byte MAX_X_BIN = MAX_BIN;
        internal const Byte MAX_Y_BIN = MAX_BIN;

        internal const Byte SXUSB_CAPS_STAR2K       = 0x01;
        internal const Byte SXUSB_CAPS_COMPRESS     = 0x02;
        internal const Byte SXUSB_CAPS_EEPROM       = 0x04;
        internal const Byte SXUSB_CAPS_GUIDER       = 0x08;
        internal const Byte SXUSB_CAPS_COOLER       = 0x10;
        internal const Byte SXUSB_CAPS_SHUTTER      = 0x20;

        internal const UInt16 COLOR_MATRIX_PACKED_RGB         = 0x8000;
        internal const UInt16 COLOR_MATRIX_PACKED_BGR         = 0x4000;
        internal const UInt16 COLOR_MATRIX_PACKED_RED_SIZE    = 0x0F00;
        internal const UInt16 COLOR_MATRIX_PACKED_GREEN_SIZE  = 0x00F0;
        internal const UInt16 COLOR_MATRIX_PACKED_BLUE_SIZE   = 0x000F;
        internal const UInt16 COLOR_MATRIX_MATRIX_ALT_EVEN    = 0x2000;
        internal const UInt16 COLOR_MATRIX_MATRIX_ALT_ODD     = 0x1000;
        internal const UInt16 COLOR_MATRIX_MATRIX_2X2         = 0x0000;
        internal const UInt16 COLOR_MATRIX_MATRIX_RED_MASK    = 0x0F00;
        internal const UInt16 COLOR_MATRIX_MATRIX_GREEN_MASK  = 0x00F0;
        internal const UInt16 COLOR_MATRIX_MATRIX_BLUE_MASK   = 0x000F;
        internal const UInt16 COLOR_MATRIX_MONOCHROME         = 0x0FFF;

        internal const UInt16 SHUTTER_CONTROL_OPEN_SHUTTER    = 0x4000;
        internal const UInt16 SHUTTER_CONTROL_CLOSE_SHUTTER   = 0x8000;

    }
}
