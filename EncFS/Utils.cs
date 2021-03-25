using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EncFS
{
    class Utils
    {
        public static readonly string ROOT_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\root";
        public static readonly string KEYS_FOLDER = ROOT_FOLDER + "\\keys\\";
        public static readonly string CERTIFICATES_FOLDER = ROOT_FOLDER + "\\certificates\\";
        public static readonly string SHARED_FOLDER = ROOT_FOLDER + "\\shared-folder\\";

        public static void PrepareEnvironment()
        {
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            if (!Directory.Exists("root"))
                Directory.CreateDirectory("root");

            Directory.SetCurrentDirectory("root");

            if (!Directory.Exists("shared-folder"))
                Directory.CreateDirectory("shared-folder");

            if (!Directory.Exists("certificates"))
                Directory.CreateDirectory("certificates");

            if (!Directory.Exists("keys"))
                Directory.CreateDirectory("keys");

            if (!Directory.Exists("database"))
                Directory.CreateDirectory("database");

            if (!File.Exists("database\\users.csv"))
                File.Create("database\\users.csv").Close();
        }

        public static string ExecutePowerShellCommand(string command)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo("cmd.exe", "/C " + command + " 2> errors.txt")
            };

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            while (!process.HasExited);
            return process.StandardOutput.ReadToEnd().Trim();
        }
    }
}
