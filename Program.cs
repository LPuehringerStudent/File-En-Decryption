using System;
using System.IO;
using System.Linq;
using System.Text;

class Program
{
    static void Main()
    {
        Console.WriteLine("|====================|");
        Console.WriteLine("| MULTI-LAYER FILE OBFUSCATOR |");
        Console.WriteLine("|====================|");
        Console.WriteLine();

        bool encryptMode = GetEncryptionMode();
        string filePath = GetValidFilePath();

        if (encryptMode)
        {
            int layers = GetLayerCount();
            string[] passwords = new string[layers];
            
            passwords[0] = GetFirstPassword();
            
            for (int i = 1; i < layers; i++)
                passwords[i] = GeneratePassword();

            string masterPassword = $"{layers}|{string.Join("|", passwords)}";
            
            ObfuscateFile(filePath, masterPassword, encrypt: true);
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n🔑 MASTER PASSWORD (SAVE THIS!): {masterPassword}");
            Console.ResetColor();
        }
        else
        {
            string masterPassword = GetPassword("Enter MASTER password (format 'layers|pwd1|pwd2|...'): ");
            ObfuscationResult result = ObfuscateFile(filePath, masterPassword, encrypt: false);
            
            if (result.Success)
            {
                Console.WriteLine("✅ File restored.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ {result.ErrorMessage}");
                Console.ResetColor();
            }
        }
    }

    // ---- Core Logic ----
    static ObfuscationResult ObfuscateFile(string path, string masterPassword, bool encrypt)
    {
        try
        {
            string content = File.ReadAllText(path);
            string[] parts = masterPassword.Split('|');
            
            if (!int.TryParse(parts[0], out int layers) || parts.Length != layers + 1)
            {
                return new ObfuscationResult(false, "Invalid master password format!");
            }

            string[] passwords = parts.Skip(1).ToArray();

            if (encrypt)
            {
                // Apply each password in order
                for (int i = 0; i < layers; i++)
                    content = TransformContent(content, passwords[i], encrypt: true);
            }
            else
            {
                // Apply passwords in REVERSE order for decryption
                for (int i = layers - 1; i >= 0; i--)
                    content = TransformContent(content, passwords[i], encrypt: false);
            }

            File.WriteAllText(path, content);
            return new ObfuscationResult(true);
        }
        catch (Exception ex)
        {
            return new ObfuscationResult(false, $"Error: {ex.Message}");
        }
    }

    static string TransformContent(string input, string pwd, bool encrypt)
    {
        StringBuilder output = new StringBuilder();
        int pwdIndex = 0;

        foreach (char c in input)
        {
            int pwdChar = pwd[pwdIndex % pwd.Length];
            int transformedChar = encrypt ? c + pwdChar : c - pwdChar;
            
            // Handle wrap-around for UTF-16
            if (transformedChar < 0)
                transformedChar += 0x10000;
            else if (transformedChar >= 0x10000)
                transformedChar -= 0x10000;
                
            output.Append((char)transformedChar);
            pwdIndex++;
        }
        return output.ToString();
    }

    // ---- Helpers ----
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

    static int GetLayerCount()
    {
        while (true)
        {
            Console.Write("Layers (1-10): ");
            if (int.TryParse(Console.ReadLine(), out int layers) && layers >= 1 && layers <= 10)
                return layers;
            Console.WriteLine("Invalid input. Use 1-10.");
        }
    }

    static string GetFirstPassword()
    {
        while (true)
        {
            Console.Write("First password (or 'gen-pwd'): ");
            string input = Console.ReadLine()?.Trim();
            if (input == "gen-pwd") return GeneratePassword();
            if (!string.IsNullOrEmpty(input)) return input;
            Console.WriteLine("Password cannot be empty.");
        }
    }

    static string GetPassword(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(input)) return input;
            Console.WriteLine("Password cannot be empty.");
        }
    }

    static string GeneratePassword()
    {
        Random rand = new Random();
        const string chars = "!@#$%^&*()_+-=ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] pwd = new char[12];
        for (int i = 0; i < pwd.Length; i++)
            pwd[i] = chars[rand.Next(chars.Length)];
        return new string(pwd);
    }
}

class ObfuscationResult
{
    public bool Success { get; }
    public string ErrorMessage { get; }

    public ObfuscationResult(bool success, string errorMessage = "")
    {
        Success = success;
        ErrorMessage = errorMessage;
    }
}