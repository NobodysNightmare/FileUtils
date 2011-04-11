using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompareFS
{
    public abstract class PathModification
    {
        public abstract string Path { get; }

        public ModificationType Modification { get; private set; }

        public PathModification(ModificationType modificationType)
        {
            Modification = modificationType;
        }
    }
}
