using Microsoft.Win32;
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
        private int elapsedSeconds = 0, elapsedMinutes = 0, elapsedHours = 0;

        public Application()
        {
            if (checkIfCanProgramRun())
            {
                InitializeComponent();
                this.startingBatteryPercentage = getBatteryPercentage();
                this.logCollector = new List<BatteryLogEvent>();
                this.cycleOfTens = 1;
                this.batteryLogInterval = 10;
                this.updateIntervalLabel();
                lblTime.Text = "Elapsed Time: 0:0:0";
                SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);
            }
            else
            {
                Environment.Exit(0);
            }
        }
        /*
         -If computer shutsdown, user logs off or reboots system
         -then print the current log to the desktop
             */
        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            FileIO.writeBatteryLog(this.logCollector);
            e.Cancel = false;
        }

        /*
         -Program will be able to run if:
             -battery does present in system
             -is it on AC power or not
             */
        private bool checkIfCanProgramRun()
        {
            PowerStatus ps = SystemInformation.PowerStatus;
            if(ps.BatteryChargeStatus != BatteryChargeStatus.NoSystemBattery || 
                ps.BatteryChargeStatus != BatteryChargeStatus.Unknown)
            {
                if (ps.PowerLineStatus != PowerLineStatus.Online)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Your laptop is working on AC power. Please run the BatteryTracker after you have switched off from the AC power.", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
                
            }
            else
            {
                MessageBox.Show("It seems that your system does not have a battery. BatteryTracker can not run.", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
        }

        /*
         -Returns current battery percentage in 2 floating points
             */
        private float getBatteryPercentage()
        {
            float percentage;
            PowerStatus ps = SystemInformation.PowerStatus;
            percentage = (SystemInformation.PowerStatus.BatteryLifePercent * 100);
            return percentage;
        }

        /*
         -Forms the string to be shown on label about the battery percentage log interval
             */
        public void updateIntervalLabel()
        {
            lblLogInterval.Text = "Log Interval: " + this.batteryLogInterval.ToString() + "%";
        }

        /*
         -Toggles the visibility of the form according to caller state of the form
             */
        private void hideMe()
        {
            notifyIcon.BalloonTipTitle = "Working at Background";
            notifyIcon.BalloonTipText = "BatteryTracker has minimized";

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

        /*
         -Overrides the closing event of the main form
         -Instead, pushes the application to the system tray as minimized formation
             */
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

        /*
         -Shows back the application and removes its instance from the minimized tray
             */
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        /*
         -Creates the battery usage log and terminates the application
             */
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*
         -Opens the edit form to get new interval value from user
             */
        private void btnEdit_Click(object sender, EventArgs e)
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

        /*
         -Counts elapsed usage time
         -Performs various tasks according to spent time
             */
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
            lblTime.Text = "Elapsed Time: " + elapsedHours + ":" + elapsedMinutes + ":" + elapsedSeconds;
        }
    }
}
