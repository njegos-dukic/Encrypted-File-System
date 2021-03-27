﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncryptedFileSystem
{
    class Interpreter
    {
        public static void AccessAccount()
        {
            while (FileSystem.currentUser == null)
            {
                System.Console.WriteLine("[ACCOUNT SETUP]\n---------------");
                System.Console.WriteLine("[1] Login\n[2] Create new account");
                System.Console.Write("Please select: ");
                var input = System.Console.ReadLine();

                if ("1" == input.Trim())
                    FileSystem.currentUser = AccountAccess.Login();

                else if ("2" == input.Trim())
                    AccountAccess.Register();

                else
                    System.Console.WriteLine("- Please select 1 or 2.\n");
            }

            return;
        }

        public static void Start()
        {
            System.Console.Clear();
            AccessAccount();
            System.Console.Clear();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                System.Console.Write(FileSystem.currentUser.Username + " >> ");
                Console.ForegroundColor = ConsoleColor.White;
                var input = System.Console.ReadLine();
                if (FileSystemOperations.InterpretCommand(input, FileSystem.currentUser))
                    break;
            }

            Directory.SetCurrentDirectory(Utils.ROOT_FOLDER);
            FileSystem.currentUser = null;
            System.Console.Clear();
            Interpreter.Start();
        }
    }
}