using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompareFS
{
    public enum ModificationType
    {
        None,
        Added,
        Removed,
        Changed
    }

    public class ModificationEventArgs : EventArgs
    {
        public string Path { get; private set; }

        public ModificationType Type { get; private set; }

        public ModificationEventArgs(string path, ModificationType type)
        {
            Path = path;
            Type = type;
        }
    }
}
