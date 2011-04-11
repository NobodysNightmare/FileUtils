using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CompareFS
{
    class FileModification : PathModification
    {
        public override string Path
        {
            get { return FileInfo.FullName; }
        }

        public FileInfo FileInfo { get; private set; }
    }
}
