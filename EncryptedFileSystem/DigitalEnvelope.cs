using System.IO;

namespace EncryptedFileSystem
{
    class DigitalEnvelope
    {
        public static void ShareFile(string filename, string user)
        {
            filename = Path.GetFullPath(filename);

            System.Console.Write("Enter file password: ");
            string key = AccountAccess.ReadSecretPassword();

            (string decfile, bool success) decrypt = SymmetricCryptography.Decrypt(filename, key: key);
            if (!decrypt.success)
                return;

            PublicKeyCryptography.GeneratePublicKeyFromCertificate(FileSystem.currentUser.Username);
            PublicKeyCryptography.GenerateDSAKeys();

            string password = Utils.GenerateRandomPassword();

            DigitalSignature.SignSharedFile($"{decrypt.decfile}", Path.GetFileName(filename), password);

            string cyph = SymmetricCryptography.Encrypt($"{decrypt.decfile}", key: password);

            if (File.Exists($"{Utils.SHARED_FOLDER}\\{Path.GetFileName(filename)}"))
                File.Delete($"{Utils.SHARED_FOLDER}\\{Path.GetFileName(filename)}");

            File.Copy(cyph, $"{Utils.SHARED_FOLDER}\\{Path.GetFileName(filename)}");

            File.WriteAllText($"{Path.GetFileName(filename)}.envelope", $"{Utils.USERNAME}\n{password}\n{FileSystem.currentUser.CypherType}\n{FileSystem.currentUser.HashType}");
            PublicKeyCryptography.Encrypt($"{Path.GetFileName(filename)}.envelope", $"{Utils.SHARED_FOLDER}\\{Path.GetFileName(filename)}.envelope", user);
            File.Delete($"{Utils.CERTIFICATES}\\{Path.GetFileName(filename)}.envelope");
        }

        public static void ReceiveFile(string filename)
        {
            string current = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory($"{Utils.SHARED_FOLDER}");

            if (!File.Exists(Path.GetFileName(filename)))
            {
                System.Console.WriteLine("File " + Path.GetFileName(filename) + " is not shared. Please specify another file.");
                Directory.SetCurrentDirectory(current);
                return;
            }

            var info = PublicKeyCryptography.TxtFileDecrypt($"{filename}.envelope");

            if (info == null)
            {
                Directory.SetCurrentDirectory(current);
                return;
            }

            string currentCypher = FileSystem.currentUser.CypherType.ToString();
            string currentDgst = FileSystem.currentUser.HashType.ToString();
             
            string user = info[0];
            string password = info[1];
            string cyph = info[2];
            string dig = info[3];

            FileSystem.currentUser.SetCypherAndHash(cyph, dig);
            Directory.SetCurrentDirectory($"{Utils.SHARED_FOLDER}");
            FileSystemOperations.OpenFile(Path.GetFileName(filename), "explorer", key: password, shared: true, user: user);

            Directory.SetCurrentDirectory($"{Utils.ROOT_FOLDER}\\{Utils.USERNAME}");
            FileSystem.currentUser.SetCypherAndHash(currentCypher, currentDgst);

            Directory.SetCurrentDirectory(current);
        }
    }
}
