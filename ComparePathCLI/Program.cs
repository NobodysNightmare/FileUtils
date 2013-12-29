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

            DirectoryInfo firstDirectory; 
            DirectoryInfo secondDirectory;
            try 
	        {	        
		        firstDirectory = GetDirectoryInfo(args[args.Length - 2]);
                secondDirectory =  GetDirectoryInfo(args[args.Length - 1]);
	        }
	        catch (Exception e)
	        {
                Console.WriteLine("Can't compare directories: {0}", e.Message);
                return;
	        }

            comparator.Compare(firstDirectory, secondDirectory);
        }

        private static DirectoryInfo GetDirectoryInfo(string path)
        {
            DirectoryInfo result = new DirectoryInfo(path);
            if (!result.Exists)
            {
                throw new ArgumentException(string.Format("The directory '{0}' does not exist", path));
            }

            return result;
        }
    }
}
