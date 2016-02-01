using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageOldRunner
{
    class OldRunner
    {
        static void Main(string[] args)
        {
            Console.WriteLine("How hany threads?");
            String[] cmds = Environment.GetCommandLineArgs();

            var key = Console.ReadLine();
            int i = 1;
            if (!String.IsNullOrEmpty(key))
                i = Int32.Parse(key);

            Console.WriteLine($"threads: {key}");

            var exceptions = new ConcurrentQueue<Exception>();
            Parallel.For(0, i, index =>
            {
                var watch = Stopwatch.StartNew();
                var passed = true;
                var errors = 0;

                watch.Stop();
                Console.WriteLine($"Test:{index + 1} Passed:{passed} Errors:{errors} Duration:{watch.ElapsedMilliseconds / 60.0:F}");
            });

            Console.WriteLine("Completed");
            Console.ReadLine();
        }
    }
}
