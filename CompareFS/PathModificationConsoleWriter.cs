using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompareFS
{
    public class PathModificationConsoleWriter : IPathModificationListener
    {
        public void OnModification(PathModification modification)
        {
            if (modification == null)
            {
                throw new ArgumentNullException("modification");
            }

            switch (modification.Modification)
            {
                case ModificationType.Added:
                    Console.Write("ADDED: ");
                    break;
                case ModificationType.Removed:
                    Console.Write("REMOVED: ");
                    break;
                default:
                    break;
            }
            Console.WriteLine(modification.Path);
        }
    }
}
