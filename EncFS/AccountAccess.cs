using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncFS
{
    class AccountAccess
    {
        private static Dictionary<string, string> accounts = new Dictionary<string, string>();

        private static void UpdateAccounts()
        {
            accounts.Clear();
            using var reader = new StreamReader(Directory.GetCurrentDirectory() + "\\database\\users.csv");
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',', 2);

                accounts.Add(values[0].Trim(), values[1]);
            }

            reader.Close();
        }

        public static User Login()
        {
            UpdateAccounts();
            System.Console.WriteLine("\n[LOGIN]\n-------");
            System.Console.Write("Username: ");
            var username = System.Console.ReadLine().Trim();
            System.Console.Write("Password: ");
            var password = ReadSecretPassword();
            accounts.TryGetValue(username, out string filePassword);

            if (!accounts.ContainsKey(username))
                System.Console.WriteLine("- Account does not exist.\n");

            else if (filePassword == DgstFunctions.HashPassword(password))
            {
                System.Console.WriteLine("- Successful login.");
                return new User(username);
            }

            else
                System.Console.WriteLine("- Invalid password.\n");

            return null;
        }

        public static void Register()
        {
            UpdateAccounts();
            System.Console.WriteLine("\n[REGISTER]\n----------");
            System.Console.Write("Username: ");
            var username = System.Console.ReadLine().Trim();
            System.Console.Write("Password: ");
            var password = ReadSecretPassword();

            if (accounts.ContainsKey(username))
                System.Console.WriteLine("- Account already exists.\n");

            else if (password.Length < 4)
                System.Console.WriteLine("- Password must be at least 4 characters long.\n");

            else
            {
                var writer = new StreamWriter(Directory.GetCurrentDirectory() + "\\database\\users.csv", append: true);
                writer.WriteLine(username + "," + DgstFunctions.HashPassword(password), true);
                writer.Close();
                System.Console.WriteLine("- Successful registration.\n");
            }

            return;
        }

        public static string ReadSecretPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }

                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring(0, password.Length - 1);
                        int pos = Console.CursorLeft;
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            Console.WriteLine();
            return password;
        }
    }
}