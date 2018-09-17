using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SimpleCounter
{
    static class Program
    {

        private const string AppMutexName = "37A12CEA-F6AE-4a4b-915E-DB85D5929426";

        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {

            bool mutexCreated = false;
            Mutex appMutex = new Mutex(false, AppMutexName, out mutexCreated);

            if (!appMutex.WaitOne(0))
            {
                MessageBox.Show("An instance has already in use", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Application());

            appMutex.ReleaseMutex();
        }
    }
}
