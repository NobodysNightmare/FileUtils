using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompareFS;
using System.IO;

namespace ComparePathCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length % 2 != 0)
            {
                Console.WriteLine("Usage: ComparePathCLI (-i extension)* [-m true|false] path1 path2");
                return;
            }

            var comparator = new PathComparator();
            var writer = new PathModificationConsoleWriter(comparator);

            for (int i = 0; i < args.Length - 2; i += 2)
            {
                if (args[i] == "-i")
                {
                    comparator.ExcludedFileExtensions.Add(args[i+1]);
                }
                else if (args[i] == "-m")
                {
                    comparator.CheckForModifications = args[i + 1] == "true";
                }
                else
                {
                    Console.WriteLine("Skipping unknown option \"{0}\"", args[i]);
                }
            }

            if (comparator.CheckForModifications)
            {
                Console.WriteLine("also checking for file-modifications");
            }

            comparator.Compare(new DirectoryInfo(args[args.Length - 2]), new DirectoryInfo(args[args.Length - 1]));
        }
    }
}
