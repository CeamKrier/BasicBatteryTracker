using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCounter
{
    class BatteryLogEvent
    {
        private String elapsedTime;
        private float drainedBatteryPercent;

        public BatteryLogEvent(String time, float drainedPercent)
        {
            this.elapsedTime = time;
            this.drainedBatteryPercent = drainedPercent;
        }

        public String getElapsedTime() { return this.elapsedTime; }
        public float getDrainedBatteryPercent() { return this.drainedBatteryPercent; }
    }
}
