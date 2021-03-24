using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncFS
{
    class Cyphers
    {
        public static void SymmetricFileEncryption(string inputFile)
        {
            var outputFile = inputFile;
            File.Copy(inputFile, "tmp" + inputFile);
            File.Delete(inputFile);
            inputFile = "tmp" + outputFile;

            switch (EncryptedFileSystem.currentUser.CypherType)
            {
                case SymmetricCypher.DES3:
                    Utils.ExecutePowerShellCommand($"openssl des3 -e -in {inputFile} -out {outputFile}");
                    break;

                case SymmetricCypher.AES256:
                    Utils.ExecutePowerShellCommand($"openssl aes-256-cbc -e -in {inputFile} -out {outputFile}");
                    break;

                case SymmetricCypher.BF:
                    Utils.ExecutePowerShellCommand($"openssl bf-cbc -e -in {inputFile} -out {outputFile}");
                    break;

                default:
                    return;
            }

            File.Delete(inputFile);
            return;
        }

        public static string SymmetricFileDecryption(string inputFile)
        {
            var outfile = Path.GetTempFileName() + Path.GetExtension(Path.GetFullPath(inputFile));

            switch (EncryptedFileSystem.currentUser.CypherType)
            {
                case SymmetricCypher.DES3:
                    Utils.ExecutePowerShellCommand($"openssl des3 -d -in {inputFile} -out {outfile}");
                    break;

                case SymmetricCypher.AES256:
                    Utils.ExecutePowerShellCommand($"openssl aes-256-cbc -d -in {inputFile} -out {outfile}");
                    break;

                case SymmetricCypher.BF:
                    Utils.ExecutePowerShellCommand($"openssl bf-cbc -d -in {inputFile} -out {outfile}");
                    break;

                default:
                    return null;
            }

            return outfile;
        }
    }

    enum SymmetricCypher
    {
        DES3,
        AES256,
        BF
    }
}
