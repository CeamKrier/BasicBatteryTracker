using MongoDB.Driver;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCounter
{   
    class FileIO
    {
        private static readonly String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\BatteryLog-" + DateTime.Now.ToString("HH-mm-ss.fff") + ".txt";

        public static bool writeBatteryLog(List<BatteryLogEvent> logColl)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path);

                foreach (var log in logColl)
                {
                    String line = "Drained: " + log.getDrainedBatteryPercent().ToString() + "% Elapsed Time: " + log.getElapsedTime();
                    sw.WriteLine(line);
                }
                sw.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    
}
