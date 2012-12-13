using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CompareFS
{
    public class PathComparator
    {
        private IPathModificationListener Listener;

        public List<string> ExcludedFileExtensions { get; private set; }

        public PathComparator(IPathModificationListener listener)
        {
            Listener = listener;

            ExcludedFileExtensions = new List<string>();
        }

        public void Compare(DirectoryInfo referenceDirectory, DirectoryInfo targetDirectory)
        {
            CompareSubdirectories(referenceDirectory, targetDirectory);
            CompareFiles(referenceDirectory, targetDirectory);

            foreach(DirectoryInfo di in referenceDirectory.EnumerateDirectories().Where( d => targetDirectory.HasSubdirectory(d) ))
            {
                Compare(di, targetDirectory.EnumerateDirectories(di.Name).Single());
            }
        }

        private void CompareSubdirectories(DirectoryInfo referenceDirectory, DirectoryInfo targetDirectory)
        {
            foreach (DirectoryInfo di in targetDirectory.EnumerateDirectories().Where(d => !referenceDirectory.HasSubdirectory(d)))
            {
                Listener.OnModification(new DirectoryModification(di, ModificationType.Added));
            }
            foreach (DirectoryInfo di in referenceDirectory.EnumerateDirectories().Where(d => !targetDirectory.HasSubdirectory(d)))
            {
                Listener.OnModification(new DirectoryModification(di, ModificationType.Removed));
            }
        }

        private void CompareFiles(DirectoryInfo referenceDirectory, DirectoryInfo targetDirectory)
        {
            foreach (FileInfo fi in EnumerateLeftFilesMissingRight(targetDirectory, referenceDirectory))
            {
                Listener.OnModification(new FileModification(fi, ModificationType.Added));
            }
            foreach (FileInfo fi in EnumerateLeftFilesMissingRight(referenceDirectory, targetDirectory))
            {
                Listener.OnModification(new FileModification(fi, ModificationType.Removed));
            }
        }

        private IEnumerable<FileInfo> EnumerateLeftFilesMissingRight(DirectoryInfo leftFiles, DirectoryInfo rightFiles)
        {
            return leftFiles.EnumerateFiles().Where(f => !ExcludedFileExtensions.Contains(f.Extension)).Where(f => !rightFiles.HasFile(f));
        }
    }
}
