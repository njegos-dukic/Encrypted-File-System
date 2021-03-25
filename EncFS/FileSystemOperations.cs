using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EncFS
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
                    foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory()))
                    System.Console.WriteLine(Path.GetFileName(file));
                    break;

                case "create":
                    if (args.Length < 3)
                        return true;

                    args = command.Split('\"');
                    CreateTextFile(args[2].Trim(), args[1].Trim());
                    break;

                case "open":
                    if (args.Length < 2)
                        return false;

                    OpenFile(args[1]);
                    break;

                case "upload":
                    if (args.Length < 2)
                        return false;

                    UploadFile(args[1]);
                    break;

                case "download":
                    if (args.Length < 2)
                        return false;
                    
                    DownloadFile(args[1]);
                    break;

                case "edit":
                    if (args.Length < 2)
                        return false;

                    EditTextFile(args[1]);
                    break;

                case "sign":
                    DgstFunctions.SignFile(args[1]);
                    break;

                case "verify":
                    System.Console.WriteLine("Verified: " + DgstFunctions.VerifyFile(args[1]));
                    break;

                case "delete":
                    if (args.Length < 2)
                        return false;

                    DeleteFile(args[1]);
                    break;

                case "logout":
                    break;

                case "exit":
                    Environment.Exit(0);
                    break;

                default:
                    return false;
            }

            System.Console.WriteLine();
            return true;
        }

        private static void CreateTextFile(string fileName, string content)
        {
            if (File.Exists(fileName))
                System.Console.WriteLine("File " + fileName + " already exists. Please specify another name.");

            else
            {
                if (".txt" != Path.GetExtension(fileName))
                {
                    System.Console.WriteLine("File " + fileName + " is not .txt file. Please specify another file.");
                    return;
                }

                File.WriteAllText(fileName, content);
                Cyphers.SymmetricFileEncryption(fileName);
                DgstFunctions.SignFile(Path.GetFileName(fileName));
            }
        }

        private static void OpenFile(string fileName, string program = "explorer", bool waitForClose = false)
        {
            if (!File.Exists(fileName))
                System.Console.WriteLine("File " + fileName + " does not exist. Please specify another file.");

            else
            {
                if (!DgstFunctions.VerifyFile(Path.GetFileName(fileName)))
                {
                    System.Console.WriteLine("File integrity violated. Preventing file from opening.");
                    return;
                }

                (string tmpFilename, bool success) decFile = Cyphers.SymmetricFileDecryption(fileName);
                if (decFile.success == false)
                    return;

                Process fileOpener = new Process();
                fileOpener.StartInfo.FileName = program;
                fileOpener.StartInfo.Arguments = decFile.tmpFilename;
                fileOpener.Start();
                if (waitForClose)
                {
                    while (!fileOpener.HasExited);
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
                    fileName = sourceFile;

                byte[] content = File.ReadAllBytes(sourceFile);
                File.WriteAllBytes(Path.GetFileName(fileName), content);
                File.WriteAllText("errors.txt", "");
                Cyphers.SymmetricFileEncryption(Path.GetFileName(fileName));
                DgstFunctions.SignFile(Path.GetFileName(fileName));
            }
        }

        public static void DownloadFile(string targetFile)
        {
            if (!File.Exists(targetFile))
                System.Console.WriteLine("File " + targetFile + " does not exist. Please specify another file.");

            else
            {
                if (!DgstFunctions.VerifyFile(Path.GetFileName(targetFile)))
                {
                    System.Console.WriteLine("File integrity violated. Preventing file from opening.");
                    return;
                }

                (string tmpFilename, bool success) decFile = Cyphers.SymmetricFileDecryption(targetFile);
                if (decFile.success == false)
                    return;

                File.Copy(decFile.tmpFilename, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + Path.GetFileName(targetFile));
            }
        }
        
        private static void EditTextFile(string fileName)
        {
            if (!File.Exists(fileName))
                System.Console.WriteLine("File " + fileName + " does not exist. Please specify another file.");

            else if (".txt" != Path.GetExtension(fileName))
            {
                System.Console.WriteLine("File " + fileName + " is not .txt file. Please specify another file.");
                return;
            }

            else
                FileSystemOperations.OpenFile(fileName, "notepad", true);
        }

        private static void DeleteFile(string fileName)
        {
            if (!File.Exists(fileName))
                System.Console.WriteLine("File " + fileName + " does not exist. Please specify another file.");

            else
            {
                System.Console.Write("Please enter your password: ");
                var password = AccountAccess.ReadSecretPassword();
                if (DgstFunctions.HashPassword(password) == EncryptedFileSystem.currentUser.PasswordHash)
                    File.Delete(fileName);
                else
                    System.Console.WriteLine("Password is incorrect.");
            }
        }

        // TODO: Srediti.
        private static void ShareFile(string fileName, string userName)
        {
            if (!File.Exists(fileName))
                System.Console.WriteLine("File " + fileName + " does not exist. Please specify another file.");
            
            else
            {
                File.Copy(fileName, "..\\shared-folder\\" + Path.GetFileName(fileName));
            }
        }
    }
}
