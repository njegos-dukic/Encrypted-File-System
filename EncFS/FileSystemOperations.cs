using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EncFS
{
    class FileSystemOperations
    {
        public static void InterpretCommand(string command)
        {
            var args = command.Split(' ');
            for (int i = 0; i < args.Length; i++)
                args[i] = args[i].Trim();

            switch (args[0])
            {
                // create "Content" fileName.txt
                case "create":
                    if (args.Length < 3)
                        return;

                    args = command.Split('\"');
                    CreateTextFile(args[2].Trim(), args[1].Trim());
                    break;

                // open fileName
                case "open":
                    if (args.Length < 2)
                        return;

                    OpenFile(args[1]);
                    break;


                // upload path
                case "upload":
                    if (args.Length < 2)
                        return;

                    UploadFile(args[1]);
                    break;

                // download fileName
                case "download":
                    if (args.Length < 2)
                        return;
                    
                    DownloadFile(args[1]);
                    break;

                // edit fileName.txt 
                case "edit":
                    if (args.Length < 2)
                        return;

                    EditTextFile(args[1]);
                    break;

                // delete fileName
                case "delete":
                    if (args.Length < 2)
                        return;

                    DeleteFile(args[1]);
                    break;

                case "exit":
                    Environment.Exit(0);
                    break;

                default:
                    return;
            }
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

                // TODO: content = Cyphers.Encrypt(content, algorithm, password);
                File.WriteAllText(fileName, content);
            }
        }

        private static void OpenFile(string fileName, string program = "explorer", bool waitForClose = false)
        {
            if (!File.Exists(fileName))
                System.Console.WriteLine("File " + fileName + " does not exist. Please specify another file.");

            else
            {
                byte[] content = File.ReadAllBytes(fileName);
                // TODO: content = Cyphers.Decrypt(content, algorithm, password);
                string tmpFilename = Path.GetTempFileName() + Path.GetExtension(Path.GetFullPath(fileName));
                File.WriteAllBytes(tmpFilename, content);
                Process fileOpener = new Process();
                fileOpener.StartInfo.FileName = program;
                fileOpener.StartInfo.Arguments = tmpFilename;
                fileOpener.Start();
                if (waitForClose)
                {
                    while (!fileOpener.HasExited);
                    DeleteFile(Path.GetFileName(fileName));
                    UploadFile(tmpFilename, fileName);
                }
            }
        }

        private static void UploadFile(string sourceFile, string fileName = "")
        {
            if (!File.Exists(sourceFile))
                System.Console.WriteLine("File " + sourceFile + " does not exist. Please specify another file.");

            else
            {
                if ("" == fileName)
                    fileName = sourceFile;

                if (File.Exists(fileName))
                    System.Console.WriteLine("File " + sourceFile + " already exists. Please specify another file.");

                byte[] content = File.ReadAllBytes(sourceFile);
                // TODO: content = Cyphers.Encrypt(content, algorithm, password);
                File.WriteAllBytes(Path.GetFileName(fileName), content);
            }
        }

        public static void DownloadFile(string targetFile)
        {
            if (!File.Exists(targetFile))
                System.Console.WriteLine("File " + targetFile + " does not exist. Please specify another file.");

            else
            {
                byte[] content = File.ReadAllBytes(targetFile);
                // TODO: content = Cyphers.Decrypt(content, algorithm, password);
                File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + Path.GetFileName(targetFile), content);
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
                File.Delete(fileName);
        }
    }
}
