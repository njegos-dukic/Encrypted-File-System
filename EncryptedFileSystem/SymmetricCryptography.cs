using System.IO;
using System.Linq;

namespace EncryptedFileSystem
{
    class SymmetricCryptography
    {
        public static void Encrypt(string source, string destination, string key = "", bool create = false, bool upload = false, string message = "Create file password: ")
        {
            source = Path.GetFullPath(source);
            destination = Path.GetFullPath(destination);

            if (create)
            {
                var temp = Path.GetTempFileName() + Path.GetExtension(source);
                File.Copy(source, temp);
                File.Delete(source);
                source = temp;
            }

            if (upload)
            {
                destination = Path.GetFileName(destination);
                var temp = Path.GetTempFileName() + Path.GetExtension(source);
                File.Copy(source, temp);
                source = temp;
            }

            string password = key;

            if (key == "")
            {
                System.Console.Write(message);
                password = AccountAccess.ReadSecretPassword();
            }

            switch (FileSystem.currentUser.CypherType)
            {
                case SymmetricCypher.DES3:
                    Utils.ExecutePowerShellCommand($"openssl des3 -e -pbkdf2 -iter 100000 -k {password} -in \"{source}\" -out \"{destination}\"");
                    break;

                case SymmetricCypher.AES256:
                    Utils.ExecutePowerShellCommand($"openssl aes-256-cbc -e -pbkdf2 -iter 100000 -k {password} -in \"{source}\" -out \"{destination}\"");
                    break;

                case SymmetricCypher.RC4:
                    Utils.ExecutePowerShellCommand($"openssl rc4 -e -pbkdf2 -iter 100000 -k {password} -in \"{source}\" -out \"{destination}\"");
                    break;

                default:
                    return;
            }

            return;
        }

        public static (string, bool) Decrypt(string inputFile, string key = "", string message = "Enter file password: ")
        {
            inputFile = Path.GetFullPath(inputFile);
            var outfile = Path.GetTempFileName() + Path.GetExtension(inputFile);
            
            var password = key;
            if (key == "")
            {
                System.Console.Write(message);
                password = AccountAccess.ReadSecretPassword();
            }

            File.WriteAllText("errors.txt", "");

            switch (FileSystem.currentUser.CypherType)
            {
                case SymmetricCypher.DES3:
                    Utils.ExecutePowerShellCommand($"openssl des3 -d -pbkdf2 -iter 100000 -k {password} -in \"{inputFile}\" -out \"{outfile}\"");
                    break;

                case SymmetricCypher.AES256:
                    Utils.ExecutePowerShellCommand($"openssl aes-256-cbc -d -pbkdf2 -iter 100000 -k {password} -in \"{inputFile}\" -out \"{outfile}\"");
                    break;

                case SymmetricCypher.RC4:
                    Utils.ExecutePowerShellCommand($"openssl rc4 -d -pbkdf2 -iter 100000 -k {password} -in \"{inputFile}\" -out \"{outfile}\"");
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
