using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Logging
{
    public class Log
    {
        private static string m_logPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "ascom-sx-camera-log.txt";
        private static FileStream m_logFS = null;
        private static DateTime m_lastWriteTime;

        static Log()
        {
            try
            {
                m_logFS = new FileStream(m_logPath, FileMode.Create, FileAccess.Write, FileShare.Read, 1);
                m_lastWriteTime = DateTime.Now;
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
            get;
            set;
        }

        public static void Write(string value)
        {
            if (m_logFS != null)
            {
                lock (m_logPath)
                {
                    DateTime currentTime = DateTime.Now;
                    TimeSpan delta = currentTime - m_lastWriteTime;

                    byte[] info = new UTF8Encoding(true).GetBytes(String.Format("{0,6:##0.000} {1}", delta.TotalSeconds, value));
                    m_logFS.Write(info, 0, info.Length);
                    m_lastWriteTime = currentTime;
                }
            }
        }
    }
}
