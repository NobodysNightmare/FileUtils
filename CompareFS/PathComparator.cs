using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace CompareFS
{
    public class PathComparator
    {
        private IPathModificationListener Listener;

        public List<string> ExcludedFileExtensions { get; private set; }

        public bool CheckForModifications { get; set; }

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

            if (CheckForModifications)
            {
                foreach (Pair<FileInfo> pair in EnumerateCommonFilesLeftAndRight(targetDirectory, referenceDirectory))
                {
                    if (!CompareFiles(pair.Left, pair.Right))
                    {
                        Listener.OnModification(new FileModification(pair.Left, ModificationType.Changed));
                    }
                }
            }
        }

        private IEnumerable<FileInfo> EnumerateLeftFilesMissingRight(DirectoryInfo leftFiles, DirectoryInfo rightFiles)
        {
            return leftFiles.EnumerateFiles().Where(f => !ExcludedFileExtensions.Contains(f.Extension)).Where(f => !rightFiles.HasFile(f));
        }

        private IEnumerable<Pair<FileInfo>> EnumerateCommonFilesLeftAndRight(DirectoryInfo leftFiles, DirectoryInfo rightFiles)
        {
            return leftFiles.EnumerateFiles().Where(f => !ExcludedFileExtensions.Contains(f.Extension)).Where(f => rightFiles.HasFile(f)).Select(left => new Pair<FileInfo>(left, rightFiles.GetCorrespondingFile(left)));
        }

        private bool CompareFiles(FileInfo fileInfo1, FileInfo fileInfo2)
        {
            SHA1 hashAlgorithm = SHA1.Create();
            using (var stream1 = fileInfo1.OpenRead())
            using (var stream2 = fileInfo2.OpenRead())
            {
                byte[] hash1 = hashAlgorithm.ComputeHash(stream1);
                byte[] hash2 = hashAlgorithm.ComputeHash(stream2);

                return ByteArrayCompare(hash1, hash2);
            }
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
    }
}
