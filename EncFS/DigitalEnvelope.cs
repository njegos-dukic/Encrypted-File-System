using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EncFS
{
    class DigitalEnvelope
    {
        public static string GenerateRandomPassword()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string (Enumerable.Repeat(chars, 20)
                                         .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void ShareFile(string filename, string user)
        {
            string password = GenerateRandomPassword();
            Cyphers.SymmetricFileEncryption(Cyphers.SymmetricFileDecryption(filename).Item1, destination: $"{Utils.ROOT_FOLDER}\\shared-folder\\{filename}", key: password);
            Directory.SetCurrentDirectory($"{Utils.ROOT_FOLDER}\\shared-folder");
            DgstFunctions.SignFile(Path.GetFileName($"{Utils.ROOT_FOLDER}\\shared-folder\\{filename}"));
            Directory.SetCurrentDirectory($"{Utils.ROOT_FOLDER}\\{EncryptedFileSystem.currentUser.Username}");
            // Cyphers.SymmetricFileEncryption(filename, destination: $"..\\shared-folder\\{filename}.hash", key: password);
            File.WriteAllText($"{filename}.envelope", $"{password}\n{EncryptedFileSystem.currentUser.CypherType}\n{EncryptedFileSystem.currentUser.DgstType}");
            AssymetricEncryption($"{filename}.envelope", user);
            File.Delete($"{filename}.envelope");
        }

        public static void AssymetricEncryption(string filename, string user)
        {
            Directory.SetCurrentDirectory($"{Utils.ROOT_FOLDER}\\certificates");
            Utils.ExecutePowerShellCommand($"openssl x509 -pubkey -noout -out .\\..\\keys\\public-{user}.key -in certs\\{user}.crt", false);
            Directory.SetCurrentDirectory($"{Utils.ROOT_FOLDER}\\{EncryptedFileSystem.currentUser.Username}");
            Utils.ExecutePowerShellCommand($"openssl rsautl -encrypt -in {filename} -out {Utils.ROOT_FOLDER}\\shared-folder\\{filename} -inkey {Utils.ROOT_FOLDER}\\keys\\public-{user}.key -pubin", false);
        }

        public static void ReceiveFile(string filename)
        {
            var info = AssymetricDecryption(filename);

            if (info == null)
                return;

            string currentCypher = EncryptedFileSystem.currentUser.CypherType.ToString();
            string currentDgst = EncryptedFileSystem.currentUser.DgstType.ToString();

            string password = info[0];
            string cyph = info[1];
            string dig = info[2];

            SetCypherAndDigest(cyph, dig);

            FileSystemOperations.OpenFile(filename, "explorer", false, key: password);
            Directory.SetCurrentDirectory($"{Utils.ROOT_FOLDER}\\{EncryptedFileSystem.currentUser.Username}");

            SetCypherAndDigest(currentCypher, currentDgst);

        }

        public static string[] AssymetricDecryption(string filename)
        {
            var file = Path.GetTempFileName() + Path.GetExtension(Path.GetFullPath(filename));

            Directory.SetCurrentDirectory($"{Utils.ROOT_FOLDER}\\shared-folder");
            Utils.ExecutePowerShellCommand($"openssl rsautl -decrypt -in {filename}.envelope -out {file} -inkey {Utils.ROOT_FOLDER}\\keys\\{EncryptedFileSystem.currentUser.Username}.key");

            var lines = File.ReadLines("errors.txt");
            if (lines.Count() > 0 && lines.Last().Contains("failed"))
            {
                System.Console.WriteLine("File is not shared with you.");
                File.WriteAllText("errors.txt", "");
                return null;
            }

            return File.ReadAllLines(file);
        }

        private static void SetCypherAndDigest(string cypher, string dgst)
        {
            switch (cypher.Trim())
            {
                case "DES3":
                    EncryptedFileSystem.currentUser.CypherType = SymmetricCypher.DES3;
                    break;

                case "AES256":
                    EncryptedFileSystem.currentUser.CypherType = SymmetricCypher.AES256;
                    break;

                case "RC4":
                    EncryptedFileSystem.currentUser.CypherType = SymmetricCypher.RC4;
                    break;

                default:
                    break;
            }

            switch (dgst.Trim())
            {
                case "SHA256":
                    EncryptedFileSystem.currentUser.DgstType = DgstFunction.SHA256;
                    break;

                case "MD5":
                    EncryptedFileSystem.currentUser.DgstType = DgstFunction.MD5;
                    break;

                case "SHA1":
                    EncryptedFileSystem.currentUser.DgstType = DgstFunction.SHA1;
                    break;

                default:
                    return;
            }
        }
    }
}
