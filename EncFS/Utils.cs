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
        public static readonly string CERTIFICATES_ROOT = ROOT_FOLDER + "\\certificates\\";
        public static readonly string CERTIFICATES_DATABASE = CERTIFICATES_ROOT + "\\certs\\";

        public static readonly string SHARED_FOLDER = ROOT_FOLDER + "\\shared-folder\\";

        public static void PrepareEnvironment()
        {
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            //if (!File.Exists("openssl.conf"))
            //{
            //    System.Console.WriteLine("Please place OpenSSL configuration file on Desktop.\n");
            //    Environment.Exit(1);
            //}

            if (!Directory.Exists("root"))
                Directory.CreateDirectory("root");

            Directory.SetCurrentDirectory("root");

            if (!Directory.Exists("shared-folder"))
                Directory.CreateDirectory("shared-folder");

            if (!Directory.Exists("certificates"))
                Directory.CreateDirectory("certificates");

            if (!Directory.Exists("certificates\\certs"))
                Directory.CreateDirectory("certificates\\certs");

            if (!Directory.Exists("certificates\\crl"))
                Directory.CreateDirectory("certificates\\crl");

            if (!Directory.Exists("certificates\\newcerts"))
                Directory.CreateDirectory("certificates\\newcerts");

            if (!Directory.Exists("certificates\\private"))
                Directory.CreateDirectory("certificates\\private");

            if (!Directory.Exists("certificates\\requests"))
                Directory.CreateDirectory("certificates\\requests");

            if (!File.Exists("certificates\\index.txt"))
                File.Create("certificates\\index.txt").Close();

            if (!File.Exists("certificates\\serial"))
                File.WriteAllText("certificates\\serial", "01");

            if (!File.Exists("certificates\\crlnumber"))
                File.WriteAllText("certificates\\crlnumber", "01");

            if (!File.Exists("certificates\\openssl.cnf"))
                File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\openssl.cnf", ".\\certificates\\openssl.cnf");

            if (!File.Exists("certificates\\rootca.pem"))
            {
                Certification.CreateCACertificate();
                System.Console.Clear();
            }

            if (!Directory.Exists("keys"))
                Directory.CreateDirectory("keys");

            if (!Directory.Exists("database"))
                Directory.CreateDirectory("database");

            if (!File.Exists("database\\users.csv"))
                File.Create("database\\users.csv").Close();
        }

        public static string ExecutePowerShellCommand(string command, bool redirect = true)
        {
            var process = new Process();

            if (redirect)
                process.StartInfo = new ProcessStartInfo("cmd.exe", "/C " + command + " 2> errors.txt");

            else
                process.StartInfo = new ProcessStartInfo("cmd.exe", "/C " + command);

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            while (!process.HasExited);
            return process.StandardOutput.ReadToEnd().Trim();
        }
    }
}
