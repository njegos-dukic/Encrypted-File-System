using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncFS
{
    class DgstFunctions
    {
        private static string salt = "WjmAoo120Gb92ba7";

        public static string HashPassword(string password)
        {
            return Utils.ExecutePowerShellCommand($"openssl passwd -6 -salt {salt} " + password);
        }

        public static void CreatePrivateKey(string username)
        {
            Utils.ExecutePowerShellCommand($"openssl genrsa -out {Utils.KEYS_FOLDER}{username}.key 4096");
        }

        public static void SignFile(string file)
        {
            if (!File.Exists(file))
                return;

            switch (EncryptedFileSystem.currentUser.DgstType)
            {
                case DgstFunction.SHA256:
                    Utils.ExecutePowerShellCommand($"openssl dgst -sign {Utils.KEYS_FOLDER}{EncryptedFileSystem.currentUser.Username}.key -sha256 -out {file}.hash {file}");
                    break;

                case DgstFunction.MD5:
                    Utils.ExecutePowerShellCommand($"openssl dgst -sign {Utils.KEYS_FOLDER}{EncryptedFileSystem.currentUser.Username}.key -md5 -out {file}.hash {file}");
                    break;

                case DgstFunction.SHA1:
                    Utils.ExecutePowerShellCommand($"openssl dgst -sign {Utils.KEYS_FOLDER}{EncryptedFileSystem.currentUser.Username}.key -sha1 -out {file}.hash {file}");
                    break;

                default:
                    return;
            }

            File.SetAttributes($"{file}.hash", FileAttributes.Hidden);
        }

        public static bool VerifyFile(string file)
        {
            switch (EncryptedFileSystem.currentUser.DgstType)
            {
                case DgstFunction.SHA256:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -prverify {Utils.KEYS_FOLDER}{EncryptedFileSystem.currentUser.Username}.key -sha256 -signature {file}.hash {file}").Contains("OK");

                case DgstFunction.MD5:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -prverify {Utils.KEYS_FOLDER}{EncryptedFileSystem.currentUser.Username}.key -md5 -signature {file}.hash {file}").Contains("OK");

                case DgstFunction.SHA1:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -prverify {Utils.KEYS_FOLDER}{EncryptedFileSystem.currentUser.Username}.key -sha1 -signature {file}.hash {file}").Contains("OK");

                default:
                    return false;
            }
        }
    }

    enum DgstFunction
    {
        SHA256,
        MD5,
        SHA1
    }
}
