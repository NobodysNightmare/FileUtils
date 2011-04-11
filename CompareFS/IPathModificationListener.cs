using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompareFS
{
    public interface IPathModificationListener
    {
        void OnModification(PathModification modification);
    }
}
