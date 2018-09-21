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
                sw.WriteLine(""); //Empty line

                int totalDrainage = logColl.Count;
                BatteryLogEvent lastLog = logColl.ElementAt(logColl.Count - 1);

                sw.WriteLine("Drained " + totalDrainage.ToString() + "% in " + lastLog.getElapsedTime());
                sw.WriteLine("On average, you have used your computer " + AverageDrainage(totalDrainage, lastLog) + " minutes for 1% of your battery.");
                sw.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static double AverageDrainage(int drainage, BatteryLogEvent lastLog)
        {
            String[] time = lastLog.getElapsedTime().Split(':');
            int elapsedMinutes = Int32.Parse(time[0]) * 60 + Int32.Parse(time[1]);
            return Convert.ToDouble(elapsedMinutes) / drainage;
        }
    }
    
}
