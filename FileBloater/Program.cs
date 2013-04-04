using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FileBloater
{
    class Program
    {
        static void Main(string[] args)
        {
            int bloatSize = 8192;
            var bloatBuffer = new byte[bloatSize];
            new Random().NextBytes(bloatBuffer);
            long loopCount = 0;
            using (var file = File.OpenWrite("foo.bloat"))
            {
                try
                {
                    while (true)
                    {
                        file.Write(bloatBuffer, 0, bloatBuffer.Length);
                        if(++loopCount % 1024 == 0)
                            Console.WriteLine("{0} MiB written so far.", (loopCount*bloatSize)/(1024*1024)); ;
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error occured: {0}", e.Message);
                }
            }
        }
    }
}
