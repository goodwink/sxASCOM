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

using ASCOM;

namespace ASCOM.SXCamera
{
    public class SharedResources
    {
        private SharedResources() { }							// Prevent creation of instances
        private static object m_lock = null;
        private static bool bSerializeControllers = true;
        public static sx.Controller [] controllers;
        private const uint maxControllers = 8;

        static SharedResources()								// Static initialization
        {
            Log.Write("SharedResources()\n");
            Log.Write(String.Format("Driver version = {0}\n", versionNumber));

            if (bSerializeControllers)
            {
                m_lock = new object();
            }

            controllers = new sx.Controller[maxControllers];

            for(int i=0;i<maxControllers;i++)
            {
                controllers[i] = new sx.Controller(m_lock);
            }

            Log.Write(String.Format("SharedResources() returns, m_lock={0}\n", m_lock));
        }

        //
        // Public access to shared resources
        //

        public static string versionNumber
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}
