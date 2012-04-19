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
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Logging
{
    public class Log
    {
        private static ASCOM.Utilities.TraceLogger m_logger;
        private static DateTime m_lastWriteTime;
        private static DateTime m_startTime;
        private static bool m_enabled;

        static Log()
        {
            try
            {
#if DEBUG
                enabled = true;
#else
                enabled = false;
#endif
            }
            catch
            {
            }
        }

        public static bool enabled
        {
            get { return m_enabled; }
            set
            {
                try
                {
                    if (value != m_enabled)
                    {
                        if (value)
                        {
                            m_logger = new ASCOM.Utilities.TraceLogger(null, "SXCamera");
                            m_logger.Enabled = true;
                            m_lastWriteTime = DateTime.Now;
                            m_startTime = m_lastWriteTime;
                            m_enabled = true;

                            Log.Write(String.Format("Logging begins at {0}", m_startTime));
                        }
                        else
                        {
                            m_enabled = false;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    m_enabled = false;
                    MessageBox.Show(String.Format("logging: enabled set caught an exception: {0}", ex));
                }
            }
        }

        public static void Write(string logMsg)
        {
            if (enabled)
            {
                // the original logger required a newline, but the ASCOM logging 
                // doesn't need newlines.  Remove trailing newlines
                
                if (logMsg[logMsg.Length-1] == '\n')
                {
                    logMsg = logMsg.Remove(logMsg.Length-1);
                }

                lock (m_logger)
                {
                    DateTime currentTime = DateTime.Now;
                    TimeSpan elapsed = currentTime - m_startTime;
                    TimeSpan delta = currentTime - m_lastWriteTime;

                    m_logger.LogMessage(String.Format("{0,12:####0.000000} {1,10:##0.000000}", elapsed.TotalSeconds, delta.TotalSeconds), logMsg);
                    m_lastWriteTime = currentTime;
                }
            }
        }
    }
}
