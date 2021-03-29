using System.IO;

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

        public static void SignFile(string file, string password)
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

            string cyph = SymmetricCryptography.Encrypt($"{file}.hash", password);

            if (File.Exists($"{file}.hash"))
                File.Delete($"{file}.hash");

            File.Copy(cyph, $"{file}.hash");
        }

        public static bool VerifySignature(string file, string hash, string key)
        {
            hash = Path.GetFileName(hash);

            (string tmpFilename, bool success) = SymmetricCryptography.Decrypt(hash, key: key);
            if (success == false)
                return false;

            if (!File.Exists(file) || !File.Exists(hash))
                return false;

            switch (FileSystem.currentUser.HashType)
            {
                case HashFunction.SHA256:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -prverify \"{Utils.PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -sha256 -signature \"{tmpFilename}\" \"{file}\"").Contains("OK");

                case HashFunction.MD5:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -prverify \"{Utils.PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -md5 -signature \"{tmpFilename}\" \"{file}\"").Contains("OK");

                case HashFunction.SHA1:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -prverify \"{Utils.PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -sha1 -signature \"{tmpFilename}\" \"{file}\"").Contains("OK");

                default:
                    return false;
            }
        }

        public static void SignSharedFile(string file, string name, string key)
        {
            file = Path.GetFullPath(file);

            if (!File.Exists(file))
                return;

            switch (FileSystem.currentUser.HashType)
            {
                case HashFunction.SHA256:
                    Utils.ExecutePowerShellCommand($"openssl dgst -sign \"{Utils.DSA_PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -sha256 -out \"{Utils.SHARED_FOLDER}\\{name}.hash\" \"{file}\"");
                    break;

                case HashFunction.MD5:
                    Utils.ExecutePowerShellCommand($"openssl dgst -sign \"{Utils.DSA_PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -md5 -out \"{Utils.SHARED_FOLDER}\\{name}.hash\" \"{file}\"");
                    break;

                case HashFunction.SHA1:
                    Utils.ExecutePowerShellCommand($"openssl dgst -sign  \"{Utils.DSA_PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -sha1 -out \"{Utils.SHARED_FOLDER}\\{name}.hash\" \"{file}\"");
                    break;

                default:
                    return;
            }

            string current = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Utils.SHARED_FOLDER);

            string cyph = SymmetricCryptography.Encrypt($"{name}.hash", key);

            if (File.Exists($"{name}.hash"))
                File.Delete($"{name}.hash");

            File.Copy(cyph, $"{name}.hash");

            Directory.SetCurrentDirectory(current);
        }

        public static bool VerifySharedSignature(string file, string name, string user, string key)
        {
            file = Path.GetFullPath(file);

            (string tmpFilename, bool success) = SymmetricCryptography.Decrypt($"{Utils.SHARED_FOLDER}\\{name}.hash", key: key);
            if (success == false)
                return false;

            if (!File.Exists(file))
                return false;

            switch (FileSystem.currentUser.HashType)
            {
                case HashFunction.SHA256:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -verify \"{Utils.DSA_PUBLIC_KEYS}\\{user}.key\" -sha256 -signature \"{tmpFilename}\" \"{file}\"").Contains("OK");

                case HashFunction.MD5:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -verify \"{Utils.DSA_PUBLIC_KEYS}\\{user}.key\" -md5 -signature \"{tmpFilename}\" \"{file}\"").Contains("OK");

                case HashFunction.SHA1:
                    return Utils.ExecutePowerShellCommand($"openssl dgst -verify \"{Utils.DSA_PUBLIC_KEYS}\\{user}.key\" -sha1 -signature \"{tmpFilename}\" \"{file}\"").Contains("OK");

                default:
                    return false;
            }
        }
    }

    enum HashFunction
    {
        SHA256,
        MD5,
        SHA1
    }
}
