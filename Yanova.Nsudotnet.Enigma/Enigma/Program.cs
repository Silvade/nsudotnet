using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace Enigma
{
    class Program
    {
        private static string mode, inFile, outFile, algorithmName, keyFile;

        private static void CheckFile(string fileName, string type, string extension)
        {
            if (!File.Exists(fileName))
            {
                throw new Exception("Файл " + fileName + " не найден.");
            }

            if (!Path.GetExtension(fileName).Equals(extension))
            {
                throw new Exception(type + " файл должен иметь расширение " + extension + ".");
            }
        }

        private static void ParseArgs(string[] args)
        {
            int len = args.Length;

            if (len != 4 && len != 5)
            {
                throw new Exception("Некорректные аргументы.");
            }
            else
            {
                mode = args[0];
                algorithmName = args[2];
            }

            if (mode.Equals("encrypt"))
            {
                if (len != 4)
                {
                    throw new Exception("Некорректные аргументы.");
                }

                inFile = args[1];

                CheckFile(inFile, "Входной", ".txt");

                outFile = args[3];

                if (!Path.GetExtension(outFile).Equals(".bin"))
                {
                    throw new Exception("Выходной файл должен иметь расширение bin.");
                }

                StringBuilder builder = new StringBuilder();
                builder.Append(Path.GetFileNameWithoutExtension(inFile));
                builder.Append(".key.txt");
                keyFile = builder.ToString();
            }
            else if (mode.Equals("decrypt"))
            {
                inFile = args[1];

                CheckFile(inFile, "Входной", ".bin");

                keyFile = args[3];

                CheckFile(keyFile, "Ключевой", ".txt");

                outFile = args[4];

                if (!Path.GetExtension(outFile).Equals(".txt"))
                {
                    throw new Exception("Выходной файл должен иметь расширение txt.");
                }
            }
            else
            {
                throw new Exception("Первый аргумент может иметь значения: encrypt или decrypt");
            }
        }

        private static SymmetricAlgorithm GetAlgorithm(string algorithmName)
        {
            switch (algorithmName.ToLower())
            {
                case "aes":
                    {
                        return new AesCryptoServiceProvider();
                    }
                case "des":
                    {
                        return new DESCryptoServiceProvider();
                    }
                case "rc2":
                    {
                        return new RC2CryptoServiceProvider();
                    }
                case "rijndael":
                    {
                        return new RijndaelManaged();
                    }
                default:
                    {
                        throw new Exception("Некорректное название алгоритма.");
                    }
            }
        }

        private static void EncryptFile(SymmetricAlgorithm algorithm)
        {
            string iv, key;

            algorithm.GenerateIV();
            algorithm.GenerateKey();

            iv = Convert.ToBase64String(algorithm.IV);
            key = Convert.ToBase64String(algorithm.Key);

            using (StreamWriter keyStream = new StreamWriter(keyFile))
            {
                keyStream.WriteLine(key);
                keyStream.WriteLine(iv);
            }
            
            using (Stream inStream = new FileStream(inFile, FileMode.Open))
            {
                using (Stream outputStream = new FileStream(outFile, FileMode.OpenOrCreate))
                {
                    using (ICryptoTransform cryptoTransform = algorithm.CreateEncryptor())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(outputStream, cryptoTransform, CryptoStreamMode.Write))
                        {
                            inStream.CopyTo(cryptoStream);
                        }
                    }
                }
            }
        }

        private static void DecryptFile(SymmetricAlgorithm algorithm)
        {
            using (StreamReader keyStream = new StreamReader(keyFile))
            {
                algorithm.Key = Convert.FromBase64String(keyStream.ReadLine());
                algorithm.IV = Convert.FromBase64String(keyStream.ReadLine());
            }
            
            using (FileStream inputStream = new FileStream(inFile, FileMode.Open))
            {
                using (FileStream outputStream = new FileStream(outFile, FileMode.OpenOrCreate))
                {
                    using (ICryptoTransform cryptoTransform = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(inputStream, cryptoTransform, CryptoStreamMode.Read))
                        {
                            cryptoStream.CopyTo(outputStream);
                        }
                    }
                }
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                ParseArgs(args);

                using (SymmetricAlgorithm algorithm = GetAlgorithm(algorithmName))
                {
                    if (mode.Equals("encrypt"))
                    {
                        EncryptFile(algorithm);
                    }
                    else
                    {
                        DecryptFile(algorithm);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}