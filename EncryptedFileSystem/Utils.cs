using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EncryptedFileSystem
{
    class Utils
    {
        public static readonly string DESKTOP = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static readonly string ROOT_FOLDER = DESKTOP + "\\root";
        public static readonly string PRIVATE_KEYS = ROOT_FOLDER + "\\keys\\private";
        public static readonly string PUBLIC_KEYS = ROOT_FOLDER + "\\keys\\public";
        public static readonly string DSA_KEYS = ROOT_FOLDER + "\\keys\\dsa";
        public static readonly string DSA_PRIVATE_KEYS = DSA_KEYS + "\\private";
        public static readonly string DSA_PUBLIC_KEYS = DSA_KEYS + "\\public";
        public static readonly string CERTIFICATES_ROOT = ROOT_FOLDER + "\\certificates";
        public static readonly string CERTIFICATES = CERTIFICATES_ROOT + "\\certs";
        public static readonly string USERS_DATABASE = ROOT_FOLDER + "\\database\\users.csv";
        public static readonly string SHARED_FOLDER = ROOT_FOLDER + "\\shared-folder";
        public static string USERNAME = "";

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

            if (!File.Exists("certificates\\openssl.cnf"))
                File.Copy($"{DESKTOP}\\openssl.cnf", "certificates\\openssl.cnf");

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

            if (!File.Exists("certificates\\rootca.pem"))
            {
                DigitalCertificate.CreateCACertificate();
                System.Console.Clear();
            }

            if (!Directory.Exists("keys\\private"))
                Directory.CreateDirectory("keys\\private");

            if (!Directory.Exists("keys\\dsa"))
                Directory.CreateDirectory("keys\\dsa");

            if (!Directory.Exists("keys\\dsa\\private"))
                Directory.CreateDirectory("keys\\dsa\\private");

            if (!Directory.Exists("keys\\dsa\\public"))
                Directory.CreateDirectory("keys\\dsa\\public");

            if (!Directory.Exists("keys\\public"))
                Directory.CreateDirectory("keys\\public");

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
            while (!process.HasExited) ;
            return process.StandardOutput.ReadToEnd().Trim();
        }

        public static string GenerateRandomPassword()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 20)
                                         .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
