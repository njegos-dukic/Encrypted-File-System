using System;

namespace EncFS
{
    class EncryptedFileSystem
    {
        private static User currentUser = new User("njegos", "dukic");

        static void Main(string[] args)
        {
            Utils.PrepareEnvironment();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                System.Console.Write("\n" + currentUser.Username + " >> ");
                Console.ForegroundColor = ConsoleColor.White;
                var input = System.Console.ReadLine();
                FileSystemOperations.InterpretCommand(input);
            }
        }
    }   
}
