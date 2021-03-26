using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncFS
{
    class Certification
    {
        public static void CreateCACertificate()
        {
            System.Console.WriteLine("[Generating CA Certificate]");
            System.Console.WriteLine("---------------------------\n");

            //System.Console.Write("Please enter CA password: ");
            //var password = AccountAccess.ReadSecretPassword().Trim();
            //File.WriteAllText("private\\passin.exe", password);
            // -passout private\\pass

            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\root\\certificates");
            Utils.ExecutePowerShellCommand($"openssl req -x509 -new -out rootca.pem  -config openssl.cnf -days 365", false);
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\root");
        }

        public static void CreateCertificate(string username)
        {
            System.Console.WriteLine();
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\root\\certificates");
            Utils.ExecutePowerShellCommand($"openssl req -new -key ..\\keys\\{username}.key -config openssl.cnf -out requests//{username}.csr", false);
            Utils.ExecutePowerShellCommand($"openssl ca -in requests\\{username}.csr -out certs\\{username}.crt -config openssl.cnf", false);
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\root");
            System.Console.Clear();
        }

        public static bool VerifyCertificate(string username)
        {
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\root\\certificates");
            if (!File.Exists("crl\\crl-list.pem"))
            {
                System.Console.WriteLine("Creating CRL list.");
                Utils.ExecutePowerShellCommand($"openssl ca -gencrl -out crl\\crl-list.pem -config openssl.cnf");
            }

            var valid = (Utils.ExecutePowerShellCommand($"openssl verify -crl_check -CAfile rootca.pem -CRLfile crl\\crl-list.pem certs\\{username}.crt")).Contains("OK");
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\root");
            return valid;
        }

        public static void GenerateCAKey()
        {
            System.Console.WriteLine("[Generating CA Certificate]");
            System.Console.WriteLine("---------------------------\n");

            Utils.ExecutePowerShellCommand($"openssl genrsa -out certificates\\private\\private4096.key 4096");
        }
    }
}
