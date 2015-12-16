using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SnagFree.TrayApp.Core;

namespace SnagFree.TrayApp
{
    class Controller
    {
        public Model Model { get; private set; }
        private ApplicationSingleInstancePerUser _singleInstancePerUser;

        public Controller()
        {
            Model = new Model();
        }

        public void EnsureTrayAppIsSinglePerUser()
        {
            _singleInstancePerUser = new ApplicationSingleInstancePerUser();
            if (_singleInstancePerUser.FirstInstance)
                return;

            int attempts = 3;
            int timeoutMsec = 1000;

            while (attempts > 0)
            {
                _singleInstancePerUser.Dispose();

                ProcessHelper.KillProcessByName(Process.GetCurrentProcess().ProcessName, true);

                _singleInstancePerUser = new ApplicationSingleInstancePerUser();
                if (_singleInstancePerUser.FirstInstance)
                    return;

                attempts--;
                Thread.Sleep(timeoutMsec);
            }
            throw new InvalidOperationException("Other instance of SnagFree is running. Attempts to terminate it failed. Logoff/login is required.");
        }
    }
}
