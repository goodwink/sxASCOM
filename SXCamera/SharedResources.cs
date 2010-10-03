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
using sx;
using ASCOM;

namespace ASCOM.SXCamera
{

    public class SharedResources
    {
        private static bool m_bConnected;
        private static sx.Controller m_controller;
        private SharedResources() { }							// Prevent creation of instances

        static SharedResources()								// Static initialization
        {
            Log.Write("SharedResources()\n");
        }

        //
        // Public access to shared resources
        //


        public static sx.Controller controller
        {
            get
            {
                if (!m_bConnected)
                {
                    Log.Write("SharedResources():controller creating object\n");
                    try
                    {
                        m_controller = new sx.Controller();
                    }
                    catch (Exception ex)
                    {
                        m_controller = null;
                        string msg = String.Format("SharedResources()::controller get caught an exception {0}\n", ex.ToString());
                        Log.Write(msg);
                        throw new ASCOM.DriverException(msg, ex);
                    }
                    m_bConnected = true;
                    Log.Write("SharedResources():controller object create succeeded\n");
                }

                return m_controller;
            }
        }

        public static uint versionMajor
        {
            get { return 1; }
        }
        public static uint versionMinor
        {
            get { return 2; }
        }

        public static uint versionMaintenance
        {
            get { return 2; }
        }
    }
}
