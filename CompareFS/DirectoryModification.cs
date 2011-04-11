using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CompareFS
{
    class DirectoryModification : PathModification
    {
        public override string Path
        {
            get { return DirectoryInfo.FullName; }
        }

        public DirectoryInfo DirectoryInfo { get; private set; }

        public DirectoryModification(DirectoryInfo directoryInfo, ModificationType modificationType)
            : base(modificationType)
        {
            DirectoryInfo = directoryInfo;
        }
    }
}
