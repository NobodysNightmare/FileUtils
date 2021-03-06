﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CompareFS
{
    public class PathComparator
    {
        public delegate void ModificationEventHandler(object sender, ModificationEventArgs args);

        public event ModificationEventHandler Modification;

        public List<string> ExcludedFileExtensions { get; private set; }

        public bool CheckForModifications { get; set; }

        public PathComparator()
        {
            ExcludedFileExtensions = new List<string>();
        }

        protected void OnModification(ModificationEventArgs args)
        {
            var handler = Modification;
            if (handler != null)
            {
                handler(this, args);
            }
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
                OnModification(new ModificationEventArgs(di.FullName, ModificationType.Added));
            }
            foreach (DirectoryInfo di in referenceDirectory.EnumerateDirectories().Where(d => !targetDirectory.HasSubdirectory(d)))
            {
                OnModification(new ModificationEventArgs(di.FullName, ModificationType.Removed));
            }
        }

        private void CompareFiles(DirectoryInfo referenceDirectory, DirectoryInfo targetDirectory)
        {
            foreach (FileInfo fi in EnumerateLeftFilesMissingRight(targetDirectory, referenceDirectory))
            {
                OnModification(new ModificationEventArgs(fi.FullName, ModificationType.Added));
            }
            foreach (FileInfo fi in EnumerateLeftFilesMissingRight(referenceDirectory, targetDirectory))
            {
                OnModification(new ModificationEventArgs(fi.FullName, ModificationType.Removed));
            }

            if (CheckForModifications)
            {
                foreach (Pair<FileInfo> pair in EnumerateCommonFilesLeftAndRight(targetDirectory, referenceDirectory))
                {
                    if (!CompareFilesAsync(pair.Left, pair.Right).Result)
                    {
                        OnModification(new ModificationEventArgs(pair.Left.FullName, ModificationType.Changed));
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

        private async Task<bool> CompareFilesAsync(FileInfo fileInfo1, FileInfo fileInfo2)
        {
            
            using (var stream1 = fileInfo1.OpenRead())
            using (var stream2 = fileInfo2.OpenRead())
            {
                Task<byte[]> hash1 = ComputeHashAsync(stream1);
                Task<byte[]> hash2 = ComputeHashAsync(stream2);

                return ByteArrayCompare(await hash1, await hash2);
            }
        }

        private async Task<byte[]> ComputeHashAsync(Stream source)
        {
            using (SHA1 hashAlgorithm = SHA1.Create())
            {
                return await Task.Factory.StartNew<byte[]>(() => hashAlgorithm.ComputeHash(source));
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
