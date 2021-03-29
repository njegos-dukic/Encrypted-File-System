using System.IO;
using System.Linq;

namespace EncryptedFileSystem
{
    class PublicKeyCryptography
    {
        public static void Encrypt(string source, string destination, string publicKeyOwner)
        {
            source = Path.GetFullPath(source);
            destination = Path.GetFullPath(destination);

            if (source == destination)
            {
                var temp = Path.GetTempFileName() + Path.GetExtension(source);
                File.Copy(source, temp);
                File.Delete(source);
                source = temp;
            }

            GeneratePublicKeyFromCertificate(publicKeyOwner);
            Directory.SetCurrentDirectory($"{Utils.ROOT_FOLDER}\\{Utils.USERNAME}");
            Utils.ExecutePowerShellCommand($"openssl rsautl -encrypt -in \"{source}\" -out \"{destination}\" -inkey {Utils.PUBLIC_KEYS}\\{publicKeyOwner}.key -pubin", false);
        }

        public static void GeneratePublicKeyFromCertificate(string user)
        {
            Directory.SetCurrentDirectory($"{Utils.CERTIFICATES}");
            Utils.ExecutePowerShellCommand($"openssl x509 -pubkey -noout -out {Utils.PUBLIC_KEYS}\\{user}.key -in {Utils.CERTIFICATES}\\{user}.crt", false);
        }

        public static string[] TxtFileDecrypt(string filename)
        {
            filename = Path.GetFullPath(filename);
            var file = Path.GetTempFileName() + Path.GetExtension(filename);

            Utils.ExecutePowerShellCommand($"openssl rsautl -decrypt -in {Path.GetFileName(filename)} -out {file} -inkey {Utils.PRIVATE_KEYS}\\{Utils.USERNAME}.key");

            var lines = File.ReadLines("errors.txt");
            if (lines.Count() > 0 && lines.Last().Contains("failed"))
            {
                System.Console.WriteLine("File is not shared with you.");
                File.WriteAllText("errors.txt", "");
                return null;
            }

            return File.ReadAllLines(file);
        }

        public static void GenerateDSAKeys()
        {
            if (File.Exists($"{Utils.DSA_KEYS}\\{Utils.USERNAME}.key"))
                return;

            Utils.ExecutePowerShellCommand($"openssl dsaparam -out \"{Utils.DSA_KEYS}\\{Utils.USERNAME}.key\" 4096");
            Utils.ExecutePowerShellCommand($"openssl gendsa -out \"{Utils.DSA_PRIVATE_KEYS}\\{Utils.USERNAME}.key\" \"{Utils.DSA_KEYS}\\{Utils.USERNAME}.key\"");
            Utils.ExecutePowerShellCommand($"openssl dsa -in \"{Utils.DSA_PRIVATE_KEYS}\\{Utils.USERNAME}.key\" -pubout -out \"{Utils.DSA_PUBLIC_KEYS}\\{Utils.USERNAME}.key\"");
        }

        public static void CreatePrivateKey(string username)
        {
            Utils.ExecutePowerShellCommand($"openssl genrsa -out {Utils.PRIVATE_KEYS}\\{username}.key 4096");
        }
    }
}
