using System;
using System.IO;
using System.Text;

namespace chiper
{
    class Program
    {
        static readonly string GuarnteedWritePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static readonly string MainPath = Path.Combine(GuarnteedWritePath, "zsotroav", "chiper");
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to this random chiper by zsotroav! \nWhat do you want to do today?");
            Console.WriteLine("E: Encrypt \nD: Decrypt\nG: Generate key");

            switch (Console.ReadLine())
            {
                case "Encrypt" or "E" or "e":
                    Encrypt();
                    break;
                case "Decrypt" or "D" or "d":
                    Decrypt();
                    break;
                case "Generate" or "G" or "g":
                    Generate();
                    break;
                default:
                    Console.WriteLine("Invalid input provided.");
                    break;
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }

        static void Encrypt()
        {
            if (!CheckKeys())
                return;

            string[] KeyFiles = Directory.GetFiles(MainPath, "*.key.bin");
            int KeyID = int.Parse(Console.ReadLine());

            Console.WriteLine("Do you want to encrypt a file or text?");

            switch(Console.ReadLine())
            {
                case "File" or "file" or "F" or "f":
                    EncFile(KeyFiles[KeyID]);
                    break;
                case "Text" or "text" or "T" or "t":
                    EncText(KeyFiles[KeyID]);
                    break;
            }
        }

        static void EncFile(string KeyLoc)
        {
            Console.Clear();
            Console.WriteLine("Encrypting File. \nPlease input the full path of the file to be encrypted.");
            string FileLoc = Console.ReadLine();
            if (!File.Exists(FileLoc))
            {
                Console.WriteLine("Could not find file. Please check the input and try again.");
                return;
            }

            var KeyData = File.ReadAllBytes(KeyLoc);
            var FileData = File.ReadAllBytes(FileLoc);

            string CompletePath = FileLoc + ".enc.bin";
            SaveBin(CompletePath, EncryptData(FileData, KeyData));
        }

        static void EncText(string KeyLoc)
        {
            Console.Clear();
            Console.WriteLine("Encrypting Text. \nPlease input the text to be encrypted.");
            string TextIn = Console.ReadLine();
            var ByteText = Encoding.UTF8.GetBytes(TextIn);

            var KeyData = File.ReadAllBytes(KeyLoc);
            string Encrypted = Encoding.UTF8.GetString(EncryptData(ByteText, KeyData));
            Console.Clear();
            Console.WriteLine($"Encryption completed: \n\n{Encrypted}");
        }

        static void Decrypt()
        {
            if (!CheckKeys())
                return;

            string[] KeyFiles = Directory.GetFiles(MainPath, "*.key.bin");
            int KeyID = int.Parse(Console.ReadLine());

            Console.WriteLine("Do you want to decrypt a file or a text?");

            switch (Console.ReadLine())
            {
                case "File" or "file" or "F" or "f":
                    DecFile(KeyFiles[KeyID]);
                    break;
                case "Text" or "text" or "T" or "t":
                    DecText(KeyFiles[KeyID]);
                    break;
            }
        }

        static void DecFile(string KeyLoc)
        {
            Console.Clear();
            Console.WriteLine("Decripting Text. \nPlease input the full path of the file to be decripted.");
            string FileLoc = Console.ReadLine();
            if (!File.Exists(FileLoc))
            {
                Console.WriteLine("Could not find file. \nPlease check the input and try again.");
                return;
            }
            if (FileLoc.Substring(FileLoc.Length - 8) != ".enc.bin")
            {
                Console.WriteLine("The entered file's extension doesn't seem to be correct. (should be .enc.bin) \nPlease check the input and try again.");
                return;
            }

            var FileData = File.ReadAllBytes(FileLoc);
            var KeyData = File.ReadAllBytes(KeyLoc);
            var Decrypted = EncryptData(FileData, KeyData);
            Console.Clear();

            SaveBin(FileLoc[0..^8], Decrypted);

            Console.WriteLine($"Decription completed, file saved as {FileLoc[0..^8]}");

        }

        static void DecText(string KeyLoc)
        {
            Console.Clear();
            Console.WriteLine("Decripting Text. \nPlease input the text to be decripted.");
            string TextIn = Console.ReadLine();
            var ByteText = Encoding.UTF8.GetBytes(TextIn);

            var KeyData = File.ReadAllBytes(KeyLoc);
            string Decrypted = Encoding.UTF8.GetString(EncryptData(ByteText, KeyData));
            Console.Clear();
            Console.WriteLine($"Decription completed: \n\n{Decrypted}");
        }

        static void Generate()
        {
            Console.Clear();
            Console.WriteLine("What passphrase should be the base of the key?");
            string KeyPrase = Console.ReadLine();
            Console.Clear();
            string KeyB64 = Convert.ToBase64String(Encoding.Unicode.GetBytes(KeyPrase));
            var KeyP1 = Encoding.UTF8.GetBytes(KeyB64[..(KeyB64.Length / 2)]);
            var KeyP2 = Encoding.UTF8.GetBytes(KeyB64[(KeyB64.Length / 2)..]);

            byte[] Key = new byte[KeyP1.Length];
            
            for (int i = 0; i < KeyP1.Length; i++)
            {
                Key[i] = (byte)(KeyP1[i] ^ KeyP2[i]);
            }

            Console.WriteLine("Enter a name for this key:"); 
            string KeyName = Console.ReadLine() + ".key.bin";
            Directory.CreateDirectory(MainPath);
            string CompletePath = Path.Combine(MainPath, KeyName);

            SaveBin(CompletePath, Key);

            Console.WriteLine($"Key {KeyName} generated successfully.");
        }

        static void SaveBin(string loc, byte[] Data)
        {
            using (FileStream fs = File.Create(loc))
            {
                fs.Write(Data, 0, Data.Length);
                fs.Close();
            }
        }

        private static byte[] EncryptData(byte[] InData, byte[] KeyData)
        {
            var EncData = new byte[InData.Length];
            int x = 0;
            for (int i = 0; i < InData.Length; i++)
            {
                if (KeyData.Length < i - x + 1)
                {
                    x += KeyData.Length;
                }
                EncData[i] = (byte)(KeyData[i - x] ^ InData[i]);
            }
            return EncData;
        }

        private static bool CheckKeys() {

            string[] KeyFiles = Directory.GetFiles(MainPath, "*.key.bin");
            Console.WriteLine("\nPlease select the encryption key you want to use:");
            for (int i = 0; i < KeyFiles.Length; i++)
            {
                Console.WriteLine($"{i}: {Path.GetFileName(KeyFiles[i])}");
            }
            if (KeyFiles.Length == 0)
            {
                Console.WriteLine("No key files found. Do you want to create one?");
                string answ = Console.ReadLine();
                if (answ == "y" || answ == "Y")
                {
                    Generate();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else return true;
        }
    }
}
