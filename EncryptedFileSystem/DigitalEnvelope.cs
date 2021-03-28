using System.IO;

namespace EncryptedFileSystem
{
    class DigitalEnvelope
    {
        public static void ShareFile(string filename, string user)
        {
            filename = Path.GetFullPath(filename);
            (string decfile, bool success) decrypt = SymmetricCryptography.Decrypt(filename);
            if (!decrypt.success)
                return;

            PublicKeyCryptography.GeneratePublicKeyFromCertificate(FileSystem.currentUser.Username);
            PublicKeyCryptography.GenerateDSAKeys();

            string password = Utils.GenerateRandomPassword();
            DigitalSignature.SignSharedFile($"{decrypt.decfile}", $"{Utils.SHARED_FOLDER}\\{Path.GetFileName(filename)}");
            SymmetricCryptography.Encrypt(decrypt.decfile, $"{Utils.SHARED_FOLDER}\\{Path.GetFileName(filename)}", key: password);

            Directory.SetCurrentDirectory($"{Utils.SHARED_FOLDER}");
            Directory.SetCurrentDirectory($"{Utils.ROOT_FOLDER}\\{Utils.USERNAME}");
            File.WriteAllText($"{Path.GetFileName(filename)}.envelope", $"{Utils.USERNAME}\n{password}\n{FileSystem.currentUser.CypherType}\n{FileSystem.currentUser.HashType}");
            PublicKeyCryptography.Encrypt($"{Path.GetFileName(filename)}.envelope", $"{Utils.SHARED_FOLDER}\\{Path.GetFileName(filename)}.envelope", user);
            File.Delete($"{filename}.envelope");
        }

        public static void ReceiveFile(string filename)
        {
            Directory.SetCurrentDirectory($"{Utils.SHARED_FOLDER}");

            var info = PublicKeyCryptography.TxtFileDecrypt($"{filename}.envelope");

            if (info == null)
                return;

            string currentCypher = FileSystem.currentUser.CypherType.ToString();
            string currentDgst = FileSystem.currentUser.HashType.ToString();

            string user = info[0];
            string password = info[1];
            string cyph = info[2];
            string dig = info[3];

            FileSystem.currentUser.SetCypherAndHash(cyph, dig);
            Directory.SetCurrentDirectory($"{Utils.SHARED_FOLDER}");
            FileSystemOperations.OpenFile(Path.GetFileName(filename), "explorer", false, key: password, shared: true, user: user);
            
            Directory.SetCurrentDirectory($"{Utils.ROOT_FOLDER}\\{Utils.USERNAME}");
            FileSystem.currentUser.SetCypherAndHash(currentCypher, currentDgst);
        }
    }
}
