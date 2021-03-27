using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EncryptedFileSystem
{
    class FileSystemOperations
    {
        public static bool InterpretCommand(string command, User user)
        {
            var args = command.Split(' ');

            for (int i = 0; i < args.Length; i++)
                args[i] = args[i].Trim();

            switch (args[0])
            {
                case "list":
                    if (args.Length != 1)
                        return false;

                    foreach (var file in Directory.GetFiles($"{Utils.ROOT_FOLDER}\\{Utils.USERNAME}"))
                        System.Console.WriteLine(Path.GetFileName(file));

                    break;

                case "list-shared":
                    if (args.Length != 1)
                        return false;

                    foreach (var file in Directory.GetFiles($"{Utils.SHARED_FOLDER}"))
                        System.Console.WriteLine(Path.GetFileName(file));

                    break;

                case "create":
                    if (args.Length != 3)
                        return false;

                    args = command.Split('\"');
                    CreateTextFile(args[2].Trim(), args[1].Trim());
                    break;

                case "open":
                    if (args.Length != 2)
                        return false;

                    OpenFile(args[1]);
                    break;

                case "upload":
                    if (args.Length != 2)
                        return false;

                    UploadFile(args[1]);
                    break;

                case "download":
                    if (args.Length != 2)
                        return false;

                    DownloadFile(args[1]);
                    break;

                case "edit":
                    if (args.Length != 2)
                        return false;

                    EditTextFile(args[1]);
                    break;

                case "sign":
                    if (args.Length != 2)
                        return false;

                    DigitalSignature.SignFile(args[1]);
                    break;

                case "verify":
                    if (args.Length != 2)
                        return false;

                    System.Console.WriteLine("Verified: " + DigitalSignature.VerifySignature(args[1]));
                    break;

                case "delete":
                    if (args.Length != 2)
                        return false;

                    DeleteFile(args[1]);
                    break;

                case "share":
                    if (args.Length != 3)
                        return false;

                    ShareFile(args[1], args[2]);
                    break;

                case "receive":
                    if (args.Length != 2)
                        return false;

                    DigitalEnvelope.ReceiveFile(args[1]);
                    break;

                case "logout":
                    return true;

                case "exit":
                    Environment.Exit(0);
                    break;

                default:
                    return false;
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

                SymmetricCryptography.Encrypt(fileName, fileName, key: password, create: true);
                DigitalSignature.SignFile(fileName);
            }
        }

        public static void OpenFile(string fileName, string program = "explorer", bool waitForClose = false, string key = "", bool shared = false, string user = "")
        {
            fileName = Path.GetFullPath(fileName);

            if (!File.Exists(fileName))
                System.Console.WriteLine("File " + Path.GetFileName(fileName) + " does not exist. Please specify another file.");

            else
            {
                if (shared)
                {
                    if (!DigitalSignature.VerifySharedSignature(fileName, user))
                    {
                        System.Console.WriteLine("File integrity violated. Preventing file from opening.");
                        return;
                    }
                }

                else if (!DigitalSignature.VerifySignature(fileName))
                {
                    System.Console.WriteLine("File integrity violated. Preventing file from opening.");
                    return;
                }

                (string tmpFilename, bool success) decFile = SymmetricCryptography.Decrypt(fileName, key: key);
                if (decFile.success == false)
                    return;

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

                SymmetricCryptography.Encrypt(fileName, Path.GetFileName(fileName), key: password, upload: true);
                DigitalSignature.SignFile(fileName);
            }
        }

        public static void DownloadFile(string targetFile)
        {
            targetFile = Path.GetFullPath(targetFile);

            if (!File.Exists(targetFile))
                System.Console.WriteLine("File " + targetFile + " does not exist. Please specify another file.");

            else
            {
                if (!DigitalSignature.VerifySignature(targetFile))
                {
                    System.Console.WriteLine("File integrity violated. Preventing file from opening.");
                    return;
                }

                (string tmpFilename, bool success) decFile = SymmetricCryptography.Decrypt(targetFile);
                if (decFile.success == false)
                    return;

                File.Copy(decFile.tmpFilename, $"{Utils.DESKTOP}\\{Path.GetFileName(targetFile)}");
            }
        }

        private static void EditTextFile(string fileName)
        {
            fileName = Path.GetFullPath(fileName);

            if (!File.Exists(fileName))
                System.Console.WriteLine("File " + fileName + " does not exist. Please specify another file.");

            else if (".txt" != Path.GetExtension(fileName))
            {
                System.Console.WriteLine("File " + fileName + " is not .txt file. Please specify another file.");
                return;
            }

            else
                OpenFile(fileName, "notepad", true);
        }

        private static void DeleteFile(string fileName)
        {
            fileName = Path.GetFullPath(fileName);

            if (!File.Exists(fileName))
                System.Console.WriteLine("File " + fileName + " does not exist. Please specify another file.");

            else
            {
                System.Console.Write("Please enter your password: ");
                var password = AccountAccess.ReadSecretPassword();
                
                if (DigitalSignature.HashPassword(password) == FileSystem.currentUser.PasswordHash)
                    File.Delete(fileName);
                
                else
                    System.Console.WriteLine("Password is incorrect.");
            }
        }

        private static void ShareFile(string fileName, string userName)
        {
            fileName = Path.GetFullPath(fileName);

            if (!File.Exists(fileName))
                System.Console.WriteLine("File " + fileName + " does not exist. Please specify another file.");

            else
                DigitalEnvelope.ShareFile(fileName, userName);
        }
    }
}
