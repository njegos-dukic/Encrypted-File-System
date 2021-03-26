using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EncFS
{
    class Cyphers
    {
        public static void SymmetricFileEncryption(string fileName, string message = "Create file password: ")
        {
            File.Copy(fileName, "[TMP] " + fileName);
            File.Delete(fileName);

            System.Console.Write(message);
            var password = AccountAccess.ReadSecretPassword();

            switch (EncryptedFileSystem.currentUser.CypherType)
            {
                case SymmetricCypher.DES3:
                    Utils.ExecutePowerShellCommand($"openssl des3 -e -pbkdf2 -iter 100000 -k {password} -in \"[TMP] {fileName}\" -out {fileName}");
                    break;

                case SymmetricCypher.AES256:
                    Utils.ExecutePowerShellCommand($"openssl aes-256-cbc -e -pbkdf2 -iter 100000 -k {password} -in \"[TMP] {fileName}\" -out {fileName}");
                    break;

                case SymmetricCypher.RC4:
                    Utils.ExecutePowerShellCommand($"openssl rc4 -e -pbkdf2 -iter 100000 -k {password} -in \"[TMP] {fileName}\" -out {fileName}");
                    break;

                default:
                    return;
            }

            File.Delete("[TMP] " + fileName);
            return;
        }

        public static (string, bool) SymmetricFileDecryption(string inputFile, string message = "Enter file password: ")
        {
            var outfile = Path.GetTempFileName() + Path.GetExtension(Path.GetFullPath(inputFile));
            System.Console.Write(message);
            var password = AccountAccess.ReadSecretPassword();
            File.WriteAllText("errors.txt", "");

            switch (EncryptedFileSystem.currentUser.CypherType)
            {
                case SymmetricCypher.DES3:
                    Utils.ExecutePowerShellCommand($"openssl des3 -d -pbkdf2 -iter 100000 -k {password} -in {inputFile} -out {outfile}");
                    break;

                case SymmetricCypher.AES256:
                    Utils.ExecutePowerShellCommand($"openssl aes-256-cbc -d -pbkdf2 -iter 100000 -k {password} -in {inputFile} -out {outfile}");
                    break;

                case SymmetricCypher.RC4:
                    Utils.ExecutePowerShellCommand($"openssl rc4 -d -pbkdf2 -iter 100000 -k {password} -in {inputFile} -out {outfile}");
                    break;

                default:
                    return (null, false);
            }

            var lines = File.ReadLines("errors.txt");
            if (lines.Count() > 0 && lines.Last().Contains("bad decrypt"))
            {
                System.Console.WriteLine("Invalid file password.");
                File.WriteAllText("errors.txt", "");
                return (null, false);
            }

            return (outfile, true);
        }
    }

    enum SymmetricCypher
    {
        DES3,
        AES256,
        RC4
    }
}
