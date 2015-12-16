using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Security.Authentication;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SnagFree.TrayApp.Core
{
    static class ProcessHelper
    {
        //https://dukelupus.wordpress.com/2012/08/23/c-kill-process-by-name-for-the-current-user/
        public static void KillProcessByName(string processName, bool currentUserOnly = true)
        {
            var currentPid = Process.GetCurrentProcess().Id;
            try
            {
                string userName = null;
                if (currentUserOnly)
                {
                    WindowsIdentity user = WindowsIdentity.GetCurrent();
                    if (user == null)
                        throw new InvalidCredentialException("Failed to get indentity of current user.");
                    userName = user.Name;
                }

                var processFinder = new ManagementObjectSearcher($"Select * from Win32_Process where Name = '{processName}.exe'");

                var processes = processFinder.Get();
                if (processes.Count == 0)
                {
                    return;
                }

                foreach (ManagementObject managementObject in processes)
                {
                    var pId = Convert.ToInt32(managementObject["ProcessId"]);
                    if (pId == currentPid) //doesn't touch self.
                        continue;

                    var process = Process.GetProcessById(pId);
                    if (currentUserOnly) //current user
                    {
                        var processOwnerInfo = new object[2];
                        managementObject.InvokeMethod("GetOwner", processOwnerInfo);
                        var processOwner = (string)processOwnerInfo[0];
                        var net = (string)processOwnerInfo[1];
                        if (!string.IsNullOrEmpty(net))
                            processOwner = $"{net}\\{processOwner}";
                        if (string.CompareOrdinal(processOwner, userName) == 0)
                        {
                            try
                            {
                                process.Kill();
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                        }
                    }
                    else //any user                    
                        try
                        {
                            process.Kill();
                        }
                        catch
                        {
                        }
                }
            }
            catch
            {
                //There is a good chance for UnauthorizedAccessException here, so
                //log the error or handle otherwise
            }
        }
    }
}
