using System;

namespace EncFS
{
    class EncryptedFileSystem
    {
        private static User currentUser = new User("njegos", "dukic");

        static void Main(string[] args)
        {
            Utils.PrepareEnvironment();
            StartInterpreter();
        }

        public static void StartInterpreter()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                System.Console.Write("\n" + currentUser.Username + " >> ");
                Console.ForegroundColor = ConsoleColor.White;
                var input = System.Console.ReadLine();
                FileSystemOperations.InterpretCommand(input, currentUser);
            }
        }

        public static void Login()
        {
            System.Console.WriteLine("1 - Login\n2 - Create account");
            System.Console.Write("Please specify: ");
            var input = System.Console.ReadLine();
            if ("1" == input.Trim())
                EncryptedFileSystem.Login();

            else if ("2" == input.Trim())
                EncryptedFileSystem.Register();

            else
                System.Console.WriteLine("Please select 1 or 2.");
        }

        public static void Register()
        {
        }
    }   
}
