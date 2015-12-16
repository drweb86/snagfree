using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using SnagFree.TrayApp.Core;

namespace SnagFree.TrayApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _notifyIcon;
        private readonly Controller _controller = new Controller();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            try
            {
                _controller.EnsureTrayAppIsSinglePerUser();
            }
            catch (InvalidOperationException exception)
            {
                MessageBox.Show(exception.Message, ApplicationInformation.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }

        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }
    }
}
