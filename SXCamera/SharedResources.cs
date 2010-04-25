//
// ================
// Shared Resources
// ================
//
// This class is a container for all shared resources that may be needed
// by the drivers served by the Local Server. 
//
// NOTES:
//
//	* ALL DECLARATIONS MUST BE STATIC HERE!! INSTANCES OF THIS CLASS MUST NEVER BE CREATED!
//
// Written by:	Bob Denny	29-May-2007
//
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;

using sx;

namespace ASCOM.SXCamera
{

    public class SharedResources
    {
        //private static sx.Camera camera;
        private const string logPath = @"c:\temp\log.txt";
        private static FileStream logFS;
        static int s_z;

        public static sx.Controller controller
        {
            get;
            private set;
        }

        public static void LogWrite(string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            logFS.Write(info, 0, info.Length);
        }

        private SharedResources() { }							// Prevent creation of instances

        static SharedResources()								// Static initialization
        {
            //m_SharedSerial = new ASCOM.Helper.Serial();
            s_z = 0;

            logFS = File.Create(logPath);

            //Thread.Sleep(15000);

            LogWrite("hello world\n");

            try
            {
                controller = new sx.Controller();
            }
            catch (Exception ex)
            {
                controller = null;
                LogWrite("caught " + ex + "\n");
            }

            LogWrite("new returned " + controller + "\n");
        }

        //
        // Public access to shared resources
        //

        // Shared serial port 
        //public static ASCOM.Helper.Serial SharedSerial { get { return m_SharedSerial; } }


        public static int z { get { return s_z++; } }
    }
}
