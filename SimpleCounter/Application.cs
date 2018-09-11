using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleCounter
{
    public partial class Application : Form
    {
        private float startingBatteryPercentage;
        private int cycleOfTens;
        private List<BatteryLogEvent> logCollector;
        public int batteryLogInterval;
        public Application()
        {
            InitializeComponent();
            this.startingBatteryPercentage = getBatteryPercentage();
            this.logCollector = new List<BatteryLogEvent>();
            this.cycleOfTens = 1;
            this.batteryLogInterval = 10;
            this.updateIntervalLabel();
        }

        int elapsedSeconds = 0, elapsedMinutes = 0, elapsedHours = 0;

        private float getBatteryPercentage()
        {
            float percentage;
            PowerStatus ps = SystemInformation.PowerStatus;
            percentage = (SystemInformation.PowerStatus.BatteryLifePercent * 100);
            return percentage;
        }

        public void updateIntervalLabel()
        {
            lblLogInterval.Text = "Log Interval: " + this.batteryLogInterval.ToString() + "%";
        }

        private void hideMe()
        {
            notifyIcon.BalloonTipTitle = "Working";
            notifyIcon.BalloonTipText = "Counter is minimized";

            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon.Visible = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
                hideMe();
                e.Cancel = true;
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            bool res = FileIO.writeBatteryLog(this.logCollector);

            if (res)
            {
                MessageBox.Show("You can find your battery usage log on the desktop", "Log Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            System.Windows.Forms.Application.Exit();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (btnStop.Text == "Start")
            {
                counter.Start();
                btnStop.Text = "Stop";
            } else if (btnStop.Text == "Stop")
            {
                counter.Stop();
                btnStop.Text = "Start";
            }
        }

        private String getElapsedTime()
        {
            return (elapsedHours + ":" + elapsedMinutes + ":" + elapsedSeconds);
        }

        private void lblLogInterval_DoubleClick(object sender, EventArgs e)
        {
            Edit ef = new Edit(this);
            ef.StartPosition = FormStartPosition.CenterParent;
            ef.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            counter.Start();
            btnStop.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            elapsedSeconds++;
            if (elapsedSeconds > 59)
            {
                elapsedMinutes++;
                elapsedSeconds = 0;

                float currentBattery = getBatteryPercentage();

                if(currentBattery + (this.batteryLogInterval * this.cycleOfTens) <= startingBatteryPercentage)
                {
                    BatteryLogEvent blg = new BatteryLogEvent(this.getElapsedTime(), this.batteryLogInterval);
                    cycleOfTens++;
                    this.logCollector.Add(blg);
                }
            }
            if (elapsedMinutes > 59)
            {
                elapsedHours++;
                elapsedMinutes = 0;
            }
            lblTime.Text = "Elapsed time: " + elapsedHours + ":" + elapsedMinutes + ":" + elapsedSeconds;
        }
    }
}
