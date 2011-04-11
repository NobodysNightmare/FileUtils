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
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: ComparePathCLI path1 path2");
                return;
            }

            var comparator = new PathComparator(new PathModificationConsoleWriter());
            comparator.Compare(new DirectoryInfo(args[0]), new DirectoryInfo(args[1]));
        }
    }
}
