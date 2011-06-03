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
        private static object m_mutex;

        static SharedResources()								// Static initialization
        {
            Log.Write("SharedResources()\n");

            m_mutex = new object();
            controller0 = new sx.Controller(m_mutex);
            controller1 = new sx.Controller(m_mutex);
            controller2 = new sx.Controller(m_mutex);
            controller3 = new sx.Controller(m_mutex);
            controller4 = new sx.Controller(m_mutex);
            controller5 = new sx.Controller(m_mutex);

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

        public static sx.Controller controller2
        {
            get;
            private set;
        }

        public static sx.Controller controller3
        {
            get;
            private set;
        }

        public static sx.Controller controller4
        {
            get;
            private set;
        }

        public static sx.Controller controller5
        {
            get;
            private set;
        }

        public static string versionNumber
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}
