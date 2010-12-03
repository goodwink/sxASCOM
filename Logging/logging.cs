using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;


namespace Logging
{
    public class Log
    {
        private static string logPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "ascom-sx-camera-log.txt";
        private static FileStream logFS = null;
        private static DateTime lastWriteTime;

        static Log()
        {
            try
            {
                logFS = new FileStream(logPath, FileMode.Create, FileAccess.Write, FileShare.Read, 1);
                lastWriteTime = DateTime.Now;
            }
            catch
            {
            }

            //logFS = File.Create(logPath);
        }

        public static void Write(string value)
        {
            if (logFS != null)
            {
                lock (logPath)
                {
                    DateTime currentTime = DateTime.Now;
                    TimeSpan delta = currentTime - lastWriteTime;

                    byte[] info = new UTF8Encoding(true).GetBytes(String.Format("{0,6:##0.000} {1}", delta.TotalSeconds, value));
                    logFS.Write(info, 0, info.Length);
                    lastWriteTime = currentTime;
                }
            }
        }
    }

}
