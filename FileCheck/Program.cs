using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FileCheck
{
    class Program
    {
        private const string HashFileExtension = ".sha1";

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintHelp();
                return;
            }

            string command = args[0].ToLower();
            var files = GetFiles(args.Skip(1).ToArray());

            switch (command)
            {
                case "create":
                    CreateHashes(files.Where(file => file.Extension != HashFileExtension));
                    break;
                case "verify":
                    VerifyHashes(files.Where(file => file.Extension == HashFileExtension));
                    break;
                default:
                    Console.WriteLine("Unknown command '{0}'", command);
                    PrintHelp();
                    break;
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("     FileCheck create <file>");
            Console.WriteLine("     FileCheck create [-r] <directory>");
            Console.WriteLine("");
            Console.WriteLine("     FileCheck verify <file>");
            Console.WriteLine("     FileCheck verify [-r] <directory>");
        }

        private static IEnumerable<FileInfo> GetFiles(string[] args)
        {
            string path = args.Last();
            bool recurse = false;
            if (args.Length > 1 && args[0] == "-r")
            {
                recurse = true;
            }

            if (Directory.Exists(path))
            {
                var directory = new DirectoryInfo(path);
                foreach (FileInfo file in EnumerateDirectory(directory, recurse))
                {
                    yield return file;
                }
            }
            else if (File.Exists(path))
            {
                yield return new FileInfo(path);
            }
            else
            {
                Console.WriteLine("Could not find the path '{0}'", path);
            }
        }

        private static IEnumerable<FileInfo> EnumerateDirectory(DirectoryInfo directory, bool recurse)
        {
            if (recurse)
            {
                foreach (DirectoryInfo subDirectory in directory.EnumerateDirectories())
                {
                    foreach (FileInfo file in EnumerateDirectory(subDirectory, true))
                    {
                        yield return file;
                    }
                }
            }

            foreach (FileInfo file in directory.EnumerateFiles())
            {
                yield return file;
            }
        }

        private static void CreateHashes(IEnumerable<FileInfo> files)
        {
            Console.WriteLine("Generating Checksums...");
            foreach (var file in files)
            {
                Console.Write("...  {0}", file.FullName);
                GenerateChecksum(file);
                WriteColored(ConsoleColor.Green, "\rOK ");
                Console.WriteLine();
            }
        }

        private static void GenerateChecksum(FileInfo file)
        {
            SHA1 hashAlgorithm = SHA1.Create();
            byte[] hash;
            using (var inStream = file.OpenRead())
            using (var outStream = new FileStream(file.FullName + HashFileExtension, FileMode.Create))
            {
                hash = hashAlgorithm.ComputeHash(inStream);
                outStream.Write(hash, 0, hash.Length);
                outStream.Flush();
            }
        }

        private static void VerifyHashes(IEnumerable<FileInfo> hashFiles)
        {
            Console.WriteLine("Verifying checksums...");
            foreach (var hashFile in hashFiles)
            {
                string checkedFileName = hashFile.FullName.Substring(0, hashFile.FullName.Length - HashFileExtension.Length);
                Console.Write("...  {0}", checkedFileName);

                FileInfo checkedFile = new FileInfo(checkedFileName);
                if (!checkedFile.Exists)
                {
                    WriteColored(ConsoleColor.Yellow, "\rMISS");
                    Console.WriteLine();
                }
                else
                {
                    if (VerifyFileHash(checkedFile, hashFile))
                    {
                        WriteColored(ConsoleColor.Green, "\rOK ");
                        Console.WriteLine();
                    }
                    else
                    {
                        WriteColored(ConsoleColor.Red, "\rFAIL");
                        Console.WriteLine();
                    }
                }
            }
        }

        private static bool VerifyFileHash(FileInfo checkedFile, FileInfo hashFile)
        {
            byte[] fileHash;
            using (var fileStream = checkedFile.OpenRead())
            {
                SHA1 hashAlgorithm = SHA1.Create();
                fileHash = hashAlgorithm.ComputeHash(fileStream);
            }

            byte[] referenceHash;
            using (var fileStream = hashFile.OpenRead())
            using (var reader = new BinaryReader(fileStream))
            {
                referenceHash = reader.ReadBytes(fileHash.Length);
            }

            return ByteArrayCompare(fileHash, referenceHash);
        }

        private static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
            {
                return false;
            }

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }

        private static void WriteColored(ConsoleColor color, string value, params object[] args)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.Write(value, args);

            Console.ForegroundColor = oldColor;
        }
    }
}
