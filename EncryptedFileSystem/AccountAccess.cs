using System;
using System.Collections.Generic;
using System.IO;

namespace EncryptedFileSystem
{
    class AccountAccess
    {
        private static Dictionary<string, (string, string, string)> accounts = new Dictionary<string, (string, string, string)>();

        public static Dictionary<string, (string, string, string)> GetAccounts()
        {
            accounts.Clear();
            using var reader = new StreamReader(Utils.USERS_DATABASE);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                accounts.Add(values[0].Trim(), (values[1], values[2], values[3]));
            }

            reader.Close();
            return accounts;
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

            if (password == "")
                password = ReadSecretPassword();

            else
                System.Console.WriteLine();

            return password;
        }

        public static void Register()
        {
            GetAccounts();
            Console.ForegroundColor = ConsoleColor.Magenta;
            System.Console.WriteLine("\n[REGISTER]");
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("----------");

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
                var writer = new StreamWriter(Utils.USERS_DATABASE, append: true);
                string cypherInput;
                string dgstInput;

                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    System.Console.WriteLine("\n   [CYPHER TYPE]");
                    Console.ForegroundColor = ConsoleColor.White;
                    System.Console.WriteLine("   -------------");
                    System.Console.WriteLine("   [1] DES3");
                    System.Console.WriteLine("   [2] AES256");
                    System.Console.WriteLine("   [3] RC4");
                    System.Console.Write("   Please select: ");
                    cypherInput = System.Console.ReadLine();
                    if ("1" == cypherInput || "2" == cypherInput || "3" == cypherInput)
                        break;

                    System.Console.WriteLine("   Please specify [1], [2] or [3].");
                }

                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    System.Console.WriteLine("\n   [HASH TYPE]");
                    Console.ForegroundColor = ConsoleColor.White;
                    System.Console.WriteLine("   -----------");
                    System.Console.WriteLine("   [1] SHA256");
                    System.Console.WriteLine("   [2] MD5");
                    System.Console.WriteLine("   [3] SHA1");
                    System.Console.Write("   Please select: ");
                    dgstInput = System.Console.ReadLine();
                    if ("1" == dgstInput || "2" == dgstInput || "3" == dgstInput)
                        break;

                    System.Console.WriteLine("   Please specify [1], [2] or [3].");
                }

                DigitalSignature.CreatePrivateKey(username);
                DigitalCertificate.IssueCertificate(username);
                writer.WriteLine(username + "," + DigitalSignature.HashPassword(password) + "," + cypherInput.Trim() + "," + dgstInput.Trim(), true);
                writer.Close();

                System.Console.Write($"- Successful registration ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                System.Console.Write(username);
                Console.ForegroundColor = ConsoleColor.White;
                System.Console.WriteLine(".\n");
            }

            return;
        }

        public static User Login()
        {
            GetAccounts();
            Console.ForegroundColor = ConsoleColor.Magenta;
            System.Console.WriteLine("\n[LOGIN]");
            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("-------");
            System.Console.Write("Username: ");
            var username = System.Console.ReadLine().Trim();
            System.Console.Write("Password: ");
            var password = ReadSecretPassword();
            accounts.TryGetValue(username, out (string pass, string cyph, string dgst) userInfo);

            if (!accounts.ContainsKey(username))
                System.Console.WriteLine("\n- Account does not exist.\n");

            else if (userInfo.pass == DigitalSignature.HashPassword(password))
            {
                if (!DigitalCertificate.VerifyCertificate(username))
                {
                    System.Console.WriteLine("\n- Invalid certificate.\n");
                    return null;
                }

                Utils.USERNAME = username;
                System.Console.WriteLine("\n- Successful login.");
                if (!Directory.Exists(username))
                    Directory.CreateDirectory(username);

                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "\\" + username);
                return new User(username, userInfo.pass, userInfo.cyph, userInfo.dgst);
            }

            else
                System.Console.WriteLine("\n- Invalid password.\n");

            return null;
        }
    }
}
