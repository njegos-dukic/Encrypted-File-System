using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncFS
{
    class AccountAccess
    {
        private static Dictionary<string, (string, string, string)> accounts = new Dictionary<string, (string, string, string)>();

        private static void UpdateAccounts()
        {
            accounts.Clear();
            using var reader = new StreamReader(Directory.GetCurrentDirectory() + "\\database\\users.csv");
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                accounts.Add(values[0].Trim(), (values[1], values[2], values[3]));
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
            accounts.TryGetValue(username, out (string pass, string cyph, string dgst) userInfo);

            if (!accounts.ContainsKey(username))
                System.Console.WriteLine("- Account does not exist.\n");

            else if (userInfo.pass == DgstFunctions.HashPassword(password))
            {
                if (!Certification.VerifyCertificate(username))
                {
                    System.Console.WriteLine("- Invalid certificate.\n");
                    return null;
                }

                System.Console.WriteLine("- Successful login.");
                if (!Directory.Exists(username))
                    Directory.CreateDirectory(username);

                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "\\" + username);
                return new User(username, userInfo.pass, userInfo.cyph, userInfo.dgst);
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
                string cypherInput;
                string dgstInput;

                while (true)
                {
                    System.Console.WriteLine("\n   [CYPHER TYPE]\n   -------------");
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
                    System.Console.WriteLine("\n   [DGST TYPE]\n   -----------");
                    System.Console.WriteLine("   [1] SHA256");
                    System.Console.WriteLine("   [2] MD5");
                    System.Console.WriteLine("   [3] SHA1");
                    System.Console.Write("   Please select: ");
                    dgstInput = System.Console.ReadLine();
                    if ("1" == dgstInput || "2" == dgstInput || "3" == dgstInput)
                        break;

                    System.Console.WriteLine("   Please specify [1], [2] or [3].");
                }

                DgstFunctions.CreatePrivateKey(username);
                Certification.CreateCertificate(username);
                writer.WriteLine(username + "," + DgstFunctions.HashPassword(password) + "," + cypherInput.Trim() + "," + dgstInput.Trim(), true);
                writer.Close();
                System.Console.WriteLine("\n- Successful registration.\n");
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