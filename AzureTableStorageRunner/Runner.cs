using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorage.Data;

namespace AzureTableStorageRunner
{
    class Runner
    {
        static void Main(string[] args)
        {
            Console.WriteLine("How hany threads?");
            String[] cmds = Environment.GetCommandLineArgs();
            var exceptions = new ConcurrentQueue<Exception>();

            var key = Console.ReadLine();
            int i = 1;
            if (!String.IsNullOrEmpty(key))
                i = Int32.Parse(key);

            Console.WriteLine($"threads: {key}");

            Parallel.For(0, i, index =>
            {
                var watch = Stopwatch.StartNew();
                var passed = true;
                var errors = 0;

                var settings = Config.GetSettings();
                var storage = new AzureStorageRepository(settings.Account, settings.Key, settings.Table, false);
        
                var note = DataGenerator.GetRandomChangeNote();

                note.PartitionKey = "N " + note.PartitionKey;

                storage.Add(note);

                var retrived = storage.Retrieve(note.PartitionKey, note.RowKey);
                if (retrived == null) passed = false;

                // updating
                try
                {
                    note.Data = DataGenerator.GetRandomGuid().ToString();
                    storage.Update(note);
                    var updated = storage.Retrieve(note.PartitionKey, note.RowKey);
                    if (updated.Data != note.Data) passed = false;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                    passed = false;
                    errors++;
                }
               

                var query = storage.Find(note.PartitionKey).Where(n => n.OrgAreaInt == note.OrgAreaInt);
                if (!query.Any()) passed = false;

                try
                {
                    storage.Delete(note);
                    var deleted = storage.Retrieve(note.PartitionKey, note.RowKey);
                    if (deleted != null) passed = false;
                }
                catch(Exception e)
                {
                    exceptions.Enqueue(e);
                    passed = false;
                    errors++;
                }

                watch.Stop();
                Console.WriteLine($"Test:{index + 1} StartTime: {DateTime.UtcNow:g} Passed:{passed} Errors:{errors} Duration:{watch.ElapsedMilliseconds / 60.0:F}");
            });

            Console.WriteLine("Completed");
            Console.ReadLine();
        }
    }
}
