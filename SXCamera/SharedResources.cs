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
using Logging;
using System.Reflection;

using sx;
using ASCOM;

namespace ASCOM.SXCamera
{

    public class SharedResources
    {
        private SharedResources() { }							// Prevent creation of instances

        static SharedResources()								// Static initialization
        {
            Log.Write("SharedResources()\n");
        }

        //
        // Public access to shared resources
        //

        public static bool controllerConnected
        {
            get;
            private set;
        }

        public static void controllerConnect(UInt16 vid, UInt16 pid, bool skip)
        {
            Log.Write(String.Format("SharedResources.controllerConnect({0}, {1}, {2})", vid, pid, skip));

            try
            {
                controller = new sx.Controller(vid, pid, skip);
            }
            catch (Exception ex)
            {
                controller = null;
                string msg = String.Format("SharedResources().controllerConnect(): caught an exception {0}\n", ex.ToString());
                Log.Write(msg);
                throw new ASCOM.DriverException(msg, ex);
            }
            controllerConnected = true;
            Log.Write("SharedResources().controllerConnect(): object create succeeded\n");
        }        

        public static sx.Controller controller
        {
            get;
            private set;
        }

        public static string versionNumber
        {
            get
            {
                string version =  Assembly.GetExecutingAssembly().GetName().Version.ToString();
                // I only use a 3 digit version number - strip off the .0"
                return version.Substring(0, version.LastIndexOf("."));
            }
        }
    }
}
