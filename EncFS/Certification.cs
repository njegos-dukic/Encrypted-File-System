using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncFS
{
    class Certification
    {
        public static void GenerateCAKey()
        {
            System.Console.WriteLine("[Generating CA Certificate]");
            System.Console.WriteLine("---------------------------\n");

            Utils.ExecutePowerShellCommand($"openssl genrsa -out certificates\\private\\private4096.key 4096");
        }

        public static void CreateCACertificate()
        {
            System.Console.WriteLine("[Generating CA Certificate]");
            System.Console.WriteLine("---------------------------\n");

            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\root\\certificates");
            Utils.ExecutePowerShellCommand($"openssl req -x509 -new -out rootca.pem -config openssl.cnf -days 365", false);
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\root");
        }
    }
}
