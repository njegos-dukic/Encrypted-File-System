using System;
using System.Collections.Generic;
using System.Text;

namespace EncFS
{
    class Cyphers
    {
        public static void SymmetricFileEncryption(string file)
        {
            switch (EncryptedFileSystem.currentUser.CypherType)
            {
                case SymmetricCypher.DES3:
                    Utils.ExecutePowerShellCommand($"openssl des3 -e -in {file} -out {file}");
                    break;

                case SymmetricCypher.AES256:
                    Utils.ExecutePowerShellCommand($"openssl aes-256-cbc -in {file} -e -salt ST -out {file}");
                    break;

                case SymmetricCypher.BF:
                    Utils.ExecutePowerShellCommand($"openssl bf-cbc -in {file} -e -salt ST -out {file}");
                    break;

                default:
                    return;
            }

            return;
        }

        public static void SymmetricFileDecryption(string file)
        {
            switch (EncryptedFileSystem.currentUser.CypherType)
            {
                case SymmetricCypher.DES3:
                    Utils.ExecutePowerShellCommand($"openssl des3 -e -in {file} -out {file}");
                    break;

                case SymmetricCypher.AES256:
                    Utils.ExecutePowerShellCommand($"openssl aes-256-cbc -in {file} -e -salt ST -out {file}");
                    break;

                case SymmetricCypher.BF:
                    Utils.ExecutePowerShellCommand($"openssl bf-cbc -in {file} -e -salt ST -out {file}");
                    break;

                default:
                    return;
            }

            return;
        }
    }

    enum SymmetricCypher
    {
        DES3,
        AES256,
        BF
    }
}
