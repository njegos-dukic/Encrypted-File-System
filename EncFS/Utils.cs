using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EncFS
{
    class Utils
    {
        public static void PrepareEnvironment()
        {
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            if (!Directory.Exists("root"))
                Directory.CreateDirectory("root");

            Directory.SetCurrentDirectory("root");

            if (!Directory.Exists("shared-folder"))
                Directory.CreateDirectory("shared-folder");

            if (!Directory.Exists("certificates"))
                Directory.CreateDirectory("shared-folder");
        }

        public static string ExecutePowerShellCommand(string command)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo("cmd.exe", "/C " + command)
            };

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            while (!process.HasExited);
            return process.StandardOutput.ReadToEnd();
        }
    }
}
