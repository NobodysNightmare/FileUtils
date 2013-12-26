using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CompareFS
{
    public static class DirectoryInfoExtensions
    {
        public static bool HasFile(this DirectoryInfo self, FileInfo file)
        {
            return self.EnumerateFiles(file.Name).Any();
        }

        public static FileInfo GetCorrespondingFile(this DirectoryInfo self, FileInfo file)
        {
            return self.EnumerateFiles(file.Name).Single();
        }

        public static bool HasSubdirectory(this DirectoryInfo self, DirectoryInfo subDirectory)
        {
            return self.EnumerateDirectories(subDirectory.Name).Any();
        }
    }
}
