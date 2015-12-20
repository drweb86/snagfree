using System;
using System.Diagnostics;
using System.Threading;
using SnagFree.TrayApp.Core;

namespace SnagFree.TrayApp
{
    class Controller : IDisposable
    {
        private GlobalKeyboardHook _globalKeyboardHook;
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

        public void SetupKeyboardHooks()
        {
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardData.vkCode != GlobalKeyboardHook.VkSnapshot)
                return;

            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                TakeScreenshot();
            }
        }

        private readonly object _syncObject = new object();
        private DateTime _takingScreenshotDate = DateTime.MinValue;
        private void TakeScreenshot()
        {
            //TODO: investigate: Taking screenshots quickly makes application fail for unknown reason with global keys. problem is not reproduced with doing other stuff.
            //TODO: other library?
            lock (_syncObject)
            {
                if (DateTime.Now - _takingScreenshotDate < new TimeSpan(0,0,0,3))
                    return;

                _takingScreenshotDate = DateTime.Now;
            }

            string fileName = ApplicationDirectories.GenerateOutputScreenshotFileName();

            using (var screenshot = ScreenshotTaker.TakeScreenshot())
            {
                screenshot.Save(fileName);
            }

            try
            {
                Process.Start(fileName);
            }
            catch
            {
                Process.Start("explorer.exe", @"/select, " + fileName);
            }
        }

        public void Dispose()
        {
            _globalKeyboardHook?.Dispose();
        }
    }
}
