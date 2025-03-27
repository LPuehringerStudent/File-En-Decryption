using System;
using System.IO;
using System.Linq;
using System.Text;

class Program
{
    static void Main()
    {
        Console.WriteLine("|====================|");
        Console.WriteLine("| FILE EN/DECRYPTION |");
        Console.WriteLine("|====================|");
        Console.WriteLine();

        bool encryptMode = GetEncryptionMode();
        string filePath = GetValidFilePath();
        string password = GetPassword(encryptMode);
        

        if (encryptMode)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("WARNING: THIS WILL OVERWRITE THE FILE!");
            Console.ResetColor();
            if (!ConfirmAction("Proceed? (y/N): ")) 
                Environment.Exit(0);

            ObfuscateFile(filePath, password, encrypt: true);
            Console.WriteLine("File obfuscated (not securely encrypted!).");
        }
        else
        {
            ObfuscateFile(filePath, password, encrypt: false);
            Console.WriteLine("File restored.");
        }
    }

    // --- Core Logic ---
    static void ObfuscateFile(string path, string pwd, bool encrypt)
    {
        string content = File.ReadAllText(path);
        string transformedContent = TransformContent(content, pwd, encrypt);
        File.WriteAllText(path, transformedContent);
    }

    static string TransformContent(string input, string pwd, bool encrypt)
    {
        StringBuilder output = new StringBuilder();
        int pwdIndex = 0;

        foreach (char c in input)
        {
            int pwdChar = pwd[pwdIndex % pwd.Length];
            int transformedChar = encrypt ? c + pwdChar : c - pwdChar;

            // Ensure the character stays within valid UTF-16 range (0x0000-0xFFFF)
            transformedChar = (transformedChar + 0x10000) % 0x10000;
            output.Append((char)transformedChar);

            pwdIndex++;
        }
        return output.ToString();
    }

    // --- Helper Methods ---
    static bool GetEncryptionMode()
    {
        while (true)
        {
            Console.Write("Encrypt (en) or Decrypt (de)? ");
            string input = Console.ReadLine()?.Trim().ToLower();
            if (input == "en") return true;
            if (input == "de") return false;
            Console.WriteLine("Invalid input. Use 'en' or 'de'.");
        }
    }

    static string GetValidFilePath()
    {
        while (true)
        {
            Console.Write("File path: ");
            string path = Console.ReadLine()?.Trim();
            if (File.Exists(path)) return path;
            Console.WriteLine("File not found.");
        }
    }

    static string GetPassword(bool isEncrypt)
    {
        while (true)
        {
            Console.Write(isEncrypt ? "Password (or 'gen-pwd'): " : "Password: ");
            string pwd = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(pwd))
            {
                if (pwd == "gen-pwd" && isEncrypt) 
                    return GeneratePassword();
                return pwd;
            }
            Console.WriteLine("Password cannot be empty.");
        }
    }

    static string GeneratePassword()
    {
        Random rand = new Random();
        const string chars = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        string pwd = new string(Enumerable.Repeat(chars, 12).Select(s => s[rand.Next(s.Length)]).ToArray());
        Console.WriteLine($"Generated password: {pwd}");
        return pwd;
    }

    static bool ConfirmAction(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine()?.Trim().ToLower() == "y";
    }
}