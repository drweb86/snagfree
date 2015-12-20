using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
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

            // seems, not needed in the life.
            //if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown &&
            //    e.KeyboardData.flags == GlobalKeyboardHook.LlkhfAltdown)
            //{
            //    MessageBox.Show("Alt + Print Screen");
            //    e.Handled = true;
            //}
            //else
            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                MessageBox.Show("Print Screen");
                e.Handled = true;
            }
        }

        public void Dispose()
        {
            _globalKeyboardHook?.Dispose();
        }
    }
}
