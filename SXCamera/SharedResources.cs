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

            controller0 = new sx.Controller();
            controller1 = new sx.Controller();

            Log.Write("SharedResources() returns\n");
        }

        //
        // Public access to shared resources
        //

        public static sx.Controller controller0
        {
            get;
            private set;
        }

        public static sx.Controller controller1
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
