using System;

namespace EncryptedFileSystem
{
    class FileSystem
    {
        public static User currentUser { get; set; }

        static void Main(string[] args)
        {
            Utils.PrepareEnvironment();
            Interpreter.Start();
        }
    }
}
