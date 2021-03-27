using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncryptedFileSystem
{
    class DigitalCertificate
    {
        public static void CreateCACertificate()
        {
            System.Console.WriteLine("[Generating CA Certificate]");
            System.Console.WriteLine("---------------------------\n");

            Directory.SetCurrentDirectory($"{Utils.CERTIFICATES_ROOT}");
            Utils.ExecutePowerShellCommand($"openssl req -x509 -new -out rootca.pem -config openssl.cnf -days 365", false);
            Directory.SetCurrentDirectory($"{Utils.ROOT_FOLDER}");
        }

        public static void IssueCertificate(string username)
        {
            System.Console.WriteLine();
            Directory.SetCurrentDirectory($"{Utils.CERTIFICATES_ROOT}");
            Utils.ExecutePowerShellCommand($"openssl req -new -key {Utils.PRIVATE_KEYS}\\{username}.key -config openssl.cnf -out requests//{username}.csr", false);
            Utils.ExecutePowerShellCommand($"openssl ca -in requests\\{username}.csr -out {Utils.CERTIFICATES}\\{username}.crt -config openssl.cnf", false);
            Directory.SetCurrentDirectory($"{Utils.ROOT_FOLDER}");
            System.Console.Clear();
        }

        public static bool VerifyCertificate(string username)
        {
            Directory.SetCurrentDirectory($"{Utils.CERTIFICATES_ROOT}");
            if (!File.Exists("crl\\crl-list.pem"))
            {
                System.Console.WriteLine("\nCreating CRL list.");
                Utils.ExecutePowerShellCommand($"openssl ca -gencrl -out crl\\crl-list.pem -config openssl.cnf");
            }

            var valid = (Utils.ExecutePowerShellCommand($"openssl verify -crl_check -CAfile rootca.pem -CRLfile crl\\crl-list.pem {Utils.CERTIFICATES}\\{username}.crt")).Contains("OK");
            Directory.SetCurrentDirectory($"{Utils.ROOT_FOLDER}");
            return valid;
        }
    }
}
