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

        public static sx.Controller controller
        {
            get;
            private set;
        }

        private SharedResources() { }							// Prevent creation of instances

        static SharedResources()								// Static initialization
        {
            //m_SharedSerial = new ASCOM.Helper.Serial();

            //Thread.Sleep(15000);

            sx.Log .Write("hello world\n");

            try
            {
                controller = new sx.Controller();
            }
            catch (Exception ex)
            {
                controller = null;
                sx.Log.Write("SharedResources() caught " + ex + "\n");
                throw ex;
            }

            sx.Log.Write("new returned " + controller + "\n");
        }

        //
        // Public access to shared resources
        //
    }
}
