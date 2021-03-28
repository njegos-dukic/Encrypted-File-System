using System;
using System.Diagnostics;
using System.IO;

namespace CertificateAuthority
{
    class CertificateAuthority
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

        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(CERTIFICATES_ROOT);


        }

        public static void CreateCACertificate()
        {
            System.Console.ForegroundColor = ConsoleColor.Magenta;
            System.Console.WriteLine("[Generating CA Certificate]");
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("---------------------------\n");

            Directory.SetCurrentDirectory($"{CERTIFICATES_ROOT}");
            ExecutePowerShellCommand($"openssl req -x509 -new -out rootca.pem -config openssl.cnf -days 365", false);
            Directory.SetCurrentDirectory($"{ROOT_FOLDER}");
        }

        public static void IssueCertificate(string username)
        {
            System.Console.WriteLine();
            Directory.SetCurrentDirectory($"{CERTIFICATES_ROOT}");
            ExecutePowerShellCommand($"openssl req -new -key {PRIVATE_KEYS}\\{username}.key -config openssl.cnf -out requests//{username}.csr", false);
            ExecutePowerShellCommand($"openssl ca -in requests\\{username}.csr -out {CERTIFICATES}\\{username}.crt -config openssl.cnf", false);
            Directory.SetCurrentDirectory($"{ROOT_FOLDER}");
            System.Console.Clear();
        }

        public static bool VerifyCertificate(string username)
        {
            Directory.SetCurrentDirectory($"{CERTIFICATES_ROOT}");
            if (!File.Exists("crl\\crl-list.pem"))
            {
                System.Console.WriteLine("\nCreating CRL list.");
                ExecutePowerShellCommand($"openssl ca -gencrl -out crl\\crl-list.pem -config openssl.cnf");
            }

            var valid = (ExecutePowerShellCommand($"openssl verify -crl_check -CAfile rootca.pem -CRLfile crl\\crl-list.pem {CERTIFICATES}\\{username}.crt")).Contains("OK");
            Directory.SetCurrentDirectory($"{ROOT_FOLDER}");
            return valid;
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
    }
}
