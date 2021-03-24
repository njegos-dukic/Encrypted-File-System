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
                System.Console.Write("\n" + currentUser.Username + " > ");
                var input = System.Console.ReadLine();
                FileSystemOperations.InterpretCommand(input);
            }
        }
    }   
}
