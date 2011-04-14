using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Logging
{
    public class Log
    {
        private static string m_logPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "ascom-sx-camera-log.txt";
        private static FileStream m_logFS = null;
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
                            m_logFS = new FileStream(m_logPath, FileMode.Create, FileAccess.Write, FileShare.Read, 1);
                            m_lastWriteTime = DateTime.Now;
                            m_startTime = m_lastWriteTime;
                            m_enabled = true;

                            Log.Write(String.Format("Logging begins at {0}\n", m_startTime));
                        }
                        else
                        {
                            m_enabled = false;
                            if (m_logFS != null)
                            {
                                m_logFS.Close();
                                m_logFS = null;
                            }
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
                lock (m_logPath)
                {
                    DateTime currentTime = DateTime.Now;
                    TimeSpan elapsed = currentTime - m_startTime;
                    TimeSpan delta = currentTime - m_lastWriteTime;

                    string message = String.Format("{0,9:####0.000} {1,7:##0.000} {2}", elapsed.TotalSeconds, delta.TotalSeconds, logMsg);
                    byte[] info = new UTF8Encoding(true).GetBytes(message);
                    m_logFS.Write(info, 0, info.Length);
                    m_lastWriteTime = currentTime;
                }
            }
        }
    }
}
