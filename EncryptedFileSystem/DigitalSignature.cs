using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncryptedFileSystem
{
    class DigitalSignature
    {
        // TODO: Save salt securely.
        private static string salt = "WjmAoo120Gb92ba7";

        public static string HashPassword(string password)
        {
            return Utils.ExecutePowerShellCommand($"openssl passwd -6 -salt {salt} {password}");
        }

        public static void SignFile(string file)
        {
            file = Path.GetFileName(file);

            if (!File.Exists(file))
                return;

            switch (FileSystem.currentUser.HashType)
            {
                case HashFunction.SHA256:
                    Utils.ExecutePowerShellCommand($"openssl dgst -sign \"{Utils.PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -sha256 -out \"{file}.hash\" \"{file}\"");
                    break;

                case HashFunction.MD5:
                    Utils.ExecutePowerShellCommand($"openssl dgst -sign \"{Utils.PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -md5 -out \"{file}.hash\" \"{file}\"");
                    break;

                case HashFunction.SHA1:
                    Utils.ExecutePowerShellCommand($"openssl dgst -sign  \"{Utils.PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -sha1 -out \"{file}.hash\" \"{file}\"");
                    break;

                default:
                    return;
            }

            // TODO: Kriptovati hash.
            // SymmetricCryptography.SymmetricFileEncryption($"\"{file}.hash\"", $"\"{file}.hash\"", key: FileSystem.currentUser.PasswordHash);
            // File.SetAttributes($"{file}.hash", FileAttributes.Hidden);
        }

        public static bool VerifySignature(string file, string hash)
        {
            hash = Path.GetFileName(hash);

            if (!File.Exists(file) || !File.Exists(hash))
                return false;

            switch (FileSystem.currentUser.HashType)
            {
                case HashFunction.SHA256:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -prverify \"{Utils.PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -sha256 -signature \"{hash}\" \"{file}\"").Contains("OK");

                case HashFunction.MD5:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -prverify \"{Utils.PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -md5 -signature \"{hash}\" \"{file}\"").Contains("OK");

                case HashFunction.SHA1:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -prverify \"{Utils.PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -sha1 -signature \"{hash}\" \"{file}\"").Contains("OK");

                default:
                    return false;
            }
        }

        public static void SignSharedFile(string file, string destination)
        {
            file = Path.GetFullPath(file);

            if (!File.Exists(file))
                return;

            switch (FileSystem.currentUser.HashType)
            {
                case HashFunction.SHA256:
                    Utils.ExecutePowerShellCommand($"openssl dgst -sign \"{Utils.DSA_PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -sha256 -out \"{destination}.hash\" \"{file}\"");
                    break;

                case HashFunction.MD5:
                    Utils.ExecutePowerShellCommand($"openssl dgst -sign \"{Utils.DSA_PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -md5 -out \"{destination}.hash\" \"{file}\"");
                    break;

                case HashFunction.SHA1:
                    Utils.ExecutePowerShellCommand($"openssl dgst -sign  \"{Utils.DSA_PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -sha1 -out \"{destination}.hash\" \"{file}\"");
                    break;

                default:
                    return;
            }
        }

        public static bool VerifySharedSignature(string file, string hash, string user)
        {
            file = Path.GetFullPath(file);

            if (!File.Exists(file))
                return false;

            switch (FileSystem.currentUser.HashType)
            {
                case HashFunction.SHA256:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -verify \"{Utils.DSA_PUBLIC_KEYS}\\{user}.key\" -sha256 -signature \"{Utils.SHARED_FOLDER}\\{hash}.hash\" \"{file}\"").Contains("OK");

                case HashFunction.MD5:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -verify \"{Utils.DSA_PUBLIC_KEYS}\\{user}.key\" -md5 -signature \"{Utils.SHARED_FOLDER}\\{hash}.hash\" \"{file}\"").Contains("OK");

                case HashFunction.SHA1:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -verify \"{Utils.DSA_PUBLIC_KEYS}\\{user}.key\" -sha1 -signature \"{Utils.SHARED_FOLDER}\\{hash}.hash\" \"{file}\"").Contains("OK");

                default:
                    return false;
            }
        }

        public static void CreatePrivateKey(string username)
        {
            Utils.ExecutePowerShellCommand($"openssl genrsa -out {Utils.PRIVATE_KEYS}\\{username}.key 4096");
        }
    }

    enum HashFunction
    {
        SHA256,
        MD5,
        SHA1
    }
}
