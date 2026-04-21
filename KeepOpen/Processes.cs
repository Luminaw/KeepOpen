using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace KeepOpen
{
    internal static class Processes
    {
        internal static bool IsProcessRunning(string processName)
        {
            var processes = System.Diagnostics.Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }

        internal static bool PromptToLaunch(string processName)
        {
            var result = DarkMessageBox.Show(
                $"The process '{processName}' is not running.\n\nWould you like to launch it now?",
                "KeepOpen - Process Monitor");

            return result == System.Windows.Forms.DialogResult.Yes;
        }

        [SupportedOSPlatform("windows")]
        internal static void LaunchDetached(string exePath, string args = "")
        {
            // 1. Setup the WMI connection to the local machine
            var scope = new ManagementScope(@"\\.\root\cimv2");
            scope.Connect();

            // 2. Get the Win32_Process class
            using var managementClass = new ManagementClass(scope, new ManagementPath("Win32_Process"), null);

            // 3. Get the parameters for the 'Create' method
            var methodParams = managementClass.GetMethodParameters("Create");

            // 4. Set the command line (Exe + Arguments)
            // Important: Use quotes if the path has spaces
            methodParams["CommandLine"] = $"\"{exePath}\" {args}";

            // Optional: Set working directory so the app can find its local files
            methodParams["CurrentDirectory"] = System.IO.Path.GetDirectoryName(exePath);

            // 5. Execute
            var outParams = managementClass.InvokeMethod("Create", methodParams, null);

            // 6. Check the ReturnValue (0 = Success)
            uint result = (uint)outParams["ReturnValue"];
            if (result != 0)
            {
                throw new Exception($"WMI Create failed with error code: {result}");
            }
        }
    }
}
