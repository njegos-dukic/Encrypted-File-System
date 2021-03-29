using System;
using System.Diagnostics;
using System.IO;

namespace EncryptedFileSystem
{
    class FileSystemOperations
    {
        public static bool InterpretCommand(string command, User user)
        {
            command = command.Trim();
            var args = command.Split(' ');

            for (int i = 0; i < args.Length; i++)
                args[i] = args[i].Trim();

            switch (args[0])
            {
                case "list":
                    if (args.Length != 1)
                        break;

                    foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory()))
                    {
                        if (!Path.GetFileName(file).Contains(".hash") && !Path.GetFileName(file).Contains("errors.txt"))
                            System.Console.WriteLine(Path.GetFileName(file));
                    }

                    break;

                case "cd":
                    if (args.Length != 2)
                        break;

                    if (!Directory.Exists(args[1]))
                        Directory.CreateDirectory(args[1]);

                    Directory.SetCurrentDirectory(args[1]);
                    break;

                case "list-shared":
                    if (args.Length != 1)
                        break;

                    foreach (var file in Directory.GetFiles($"{Utils.SHARED_FOLDER}"))
                    {
                        if (!Path.GetFileName(file).Contains(".hash") && !Path.GetFileName(file).Contains(".envelope") && !Path.GetFileName(file).Contains("errors.txt"))

                            System.Console.WriteLine(Path.GetFileName(file));
                    }

                    break;

                case "list-users":
                    if (args.Length != 1)
                        break;

                    foreach (var file in AccountAccess.GetAccounts())
                        System.Console.WriteLine(file.Key);

                    break;

                case "create":
                    args = command.Split('\"');

                    for (int i = 0; i < args.Length; i++)
                        args[i] = args[i].Trim();

                    if (args.Length == 3 && args[2] != "")
                        CreateTextFile(args[2].Trim(), args[1].Trim());

                    break;

                case "open":
                    if (args.Length == 2)
                        OpenFile(args[1]);

                    break;

                case "upload":
                    if (args.Length == 2)
                        UploadFile(args[1]);

                    break;

                case "download":
                    if (args.Length == 2)
                        DownloadFile(args[1]);

                    break;

                case "edit":
                    if (args.Length == 2)
                        EditTextFile(args[1]);

                    break;

                case "delete":
                    if (args.Length == 2)
                        DeleteFile(args[1]);

                    break;

                case "share":
                    if (args.Length == 3)
                        ShareFile(args[1], args[2]);

                    break;

                case "receive":
                    if (args.Length == 2)
                        DigitalEnvelope.ReceiveFile(args[1]);

                    break;

                case "logout":
                    if (args.Length == 1)
                        return true;

                    break;

                case "clear":
                    if (args.Length == 1)
                        System.Console.Clear();

                    return false;

                case "exit":
                    if (args.Length == 1)
                        Environment.Exit(0);

                    break;

                default:
                    break;
            }

            System.Console.WriteLine();
            return false;
        }

        private static void CreateTextFile(string fileName, string content)
        {
            fileName = Path.GetFullPath(fileName);

            if (File.Exists(fileName))
                System.Console.WriteLine("File " + Path.GetFileName(fileName) + " already exists. Please specify another name.");

            else
            {
                if (".txt" != Path.GetExtension(fileName))
                {
                    System.Console.WriteLine("File " + Path.GetFileName(fileName) + " is not .txt file. Please specify another file.");
                    return;
                }

                File.WriteAllText(fileName, content);
                System.Console.Write("Create file password: ");
                var password = AccountAccess.ReadSecretPassword();

                DigitalSignature.SignFile(fileName, password);

                string cyph = SymmetricCryptography.Encrypt(fileName, password);

                if (File.Exists(fileName))
                    File.Delete(fileName);

                File.Copy(cyph, fileName);
            }
        }

        public static void OpenFile(string fileName, string program = "explorer", bool waitForClose = false, string key = "", bool shared = false, string user = "")
        {
            fileName = Path.GetFullPath(fileName);

            if (!File.Exists(fileName))
                System.Console.WriteLine("File " + Path.GetFileName(fileName) + " does not exist. Please specify another file.");

            else
            {
                if (key == "")
                {
                    System.Console.Write("Enter file password: ");
                    key = AccountAccess.ReadSecretPassword();
                }

                (string tmpFilename, bool success) decFile = SymmetricCryptography.Decrypt(fileName, key: key);
                if (decFile.success == false)
                    return;

                if (shared)
                {
                    if (!DigitalSignature.VerifySharedSignature(decFile.tmpFilename, $"{Path.GetFileName(fileName)}", user, key))
                    {
                        System.Console.WriteLine("File integrity violated. Preventing file from opening.");
                        return;
                    }
                }

                else if (!DigitalSignature.VerifySignature(decFile.tmpFilename, $"{Path.GetFileName(fileName)}.hash", key))
                {
                    System.Console.WriteLine("File integrity violated. Preventing file from opening.");
                    return;
                }

                Process fileOpener = new Process();
                fileOpener.StartInfo.FileName = program;
                fileOpener.StartInfo.Arguments = decFile.tmpFilename;
                fileOpener.Start();
                if (waitForClose)
                {
                    while (!fileOpener.HasExited) ;
                    UploadFile(decFile.tmpFilename, fileName);
                }
            }
        }

        private static void UploadFile(string sourceFile, string fileName = "")
        {
            sourceFile = Path.GetFullPath(sourceFile);

            if (!File.Exists(sourceFile))
                System.Console.WriteLine("File " + Path.GetFileName(sourceFile) + " does not exist. Please specify another file.");

            else
            {
                if ("" == fileName)
                    fileName = Path.GetFileName(sourceFile);

                byte[] content = File.ReadAllBytes(sourceFile);
                File.WriteAllBytes(fileName, content);
                File.WriteAllText("errors.txt", "");

                System.Console.Write("Create file password: ");
                var password = AccountAccess.ReadSecretPassword();

                DigitalSignature.SignFile(fileName, password);


                string cyph = SymmetricCryptography.Encrypt(fileName, password);

                if (File.Exists(fileName))
                    File.Delete(fileName);

                File.Copy(cyph, fileName);
            }
        }

        public static void DownloadFile(string targetFile)
        {
            targetFile = Path.GetFullPath(targetFile);

            if (!File.Exists(targetFile))
                System.Console.WriteLine("File " + Path.GetFileName(targetFile) + " does not exist. Please specify another file.");

            else
            {
                System.Console.Write("Enter file password: ");
                string key = AccountAccess.ReadSecretPassword();

                (string tmpFilename, bool success) decFile = SymmetricCryptography.Decrypt(targetFile, key: key);
                if (decFile.success == false)
                    return;

                if (!DigitalSignature.VerifySignature(decFile.tmpFilename, $"{targetFile}.hash", key))
                {
                    System.Console.WriteLine("File integrity violated. Preventing file from opening.");
                    return;
                }

                File.Copy(decFile.tmpFilename, $"{Utils.DESKTOP}\\{Path.GetFileName(targetFile)}");
            }
        }

        private static void EditTextFile(string fileName)
        {
            fileName = Path.GetFullPath(fileName);

            if (!File.Exists(fileName))
                System.Console.WriteLine("File " + Path.GetFileName(fileName) + " does not exist. Please specify another file.");

            else if (".txt" != Path.GetExtension(fileName))
            {
                System.Console.WriteLine("File " + Path.GetFileName(fileName) + " is not .txt file. Please specify another file.");
                return;
            }

            else
                OpenFile(fileName, "notepad", true);
        }

        private static void DeleteFile(string fileName)
        {
            fileName = Path.GetFullPath(fileName);

            if (!File.Exists(fileName))
                System.Console.WriteLine("File " + Path.GetFileName(fileName) + " does not exist. Please specify another file.");

            else
            {
                System.Console.Write("Please enter your password: ");
                var password = AccountAccess.ReadSecretPassword();

                if (DigitalSignature.HashPassword(password) == FileSystem.currentUser.PasswordHash)
                {
                    File.Delete(fileName);
                    File.Delete($"{fileName}.hash");
                }

                else
                    System.Console.WriteLine("Password is incorrect.");
            }
        }

        private static void ShareFile(string fileName, string userName)
        {
            if (!AccountAccess.GetAccounts().ContainsKey(userName))
            {
                System.Console.WriteLine($"User {userName} does not exist. Please specify another user.");
                return;
            }

            if (userName == FileSystem.currentUser.Username)
            {
                System.Console.WriteLine($"Why are you sharing your file with yourself? Please specify another user.");
                return;
            }

            fileName = Path.GetFullPath(fileName);

            if (!File.Exists(fileName))
                System.Console.WriteLine("File \"" + Path.GetFileName(fileName) + "\" does not exist. Please specify another file.");

            else
                DigitalEnvelope.ShareFile(fileName, userName);
        }
    }
}
