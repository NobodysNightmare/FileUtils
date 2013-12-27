using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompareFS
{
    public class PathModificationConsoleWriter
    {
        public PathModificationConsoleWriter(PathComparator comparator)
        {
            comparator.Modification += HandleModification;
        }

        private void HandleModification(object sender, ModificationEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            switch (args.Type)
            {
                case ModificationType.Added:
                    WriteColored(ConsoleColor.Green, "ADDED:   ");
                    break;
                case ModificationType.Removed:
                    WriteColored(ConsoleColor.Red,   "REMOVED: ");
                    break;
                case ModificationType.Changed:
                    WriteColored(ConsoleColor.Yellow, "CHANGED: ");
                    break;
                default:
                    break;
            }
            Console.WriteLine(args.Path);
        }

        private static void WriteColored(ConsoleColor color, string value, params object[] args)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.Write(value, args);

            Console.ForegroundColor = oldColor;
        }
    }
}
