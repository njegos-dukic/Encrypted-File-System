using System;
using System.Collections.Generic;
using System.IO;

namespace EncFS
{
    class EncryptedFileSystem
    {
        public static User currentUser { get; private set; }

        static void Main(string[] args)
        {
            Utils.PrepareEnvironment();
            StartInterpreter();
        }

        public static void StartInterpreter()
        {
            Initialize();
            System.Console.Clear();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                System.Console.Write(currentUser.Username + " >> ");
                Console.ForegroundColor = ConsoleColor.White;
                var input = System.Console.ReadLine();
                FileSystemOperations.InterpretCommand(input, currentUser);
            }
        }
    
        public static void Initialize()
        {
            while (currentUser == null)
            {
                System.Console.WriteLine("[ACCOUNT SETUP]\n---------------");
                System.Console.WriteLine("[1] Login\n[2] Create new account");
                System.Console.Write("Please select: ");
                var input = System.Console.ReadLine();

                if ("1" == input.Trim())
                    currentUser = AccountAccess.Login();

                else if ("2" == input.Trim())
                    AccountAccess.Register();

                else
                    System.Console.WriteLine("- Please select 1 or 2.\n");
            }

            return;
        }
    }
}   

