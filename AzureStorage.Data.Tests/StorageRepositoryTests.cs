using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AzureStorage.Data.Tests
{
    [TestFixture]
    public class StorageRepositoryTests
    {
        private readonly AzureStorageRepository _storage;

        private readonly Stopwatch _timer = new Stopwatch();
        private readonly List<long> _totalDur = new List<long>();
        private readonly Dictionary<string,long> _testDur = new Dictionary<string, long>(); 

        public StorageRepositoryTests()
        {
            var settings = Config.GetSettings();
            _storage = new AzureStorageRepository(settings.Account, settings.Key, settings.Table, false);
        }

        [Test]
        public void CanParallel()
        {
            Parallel.For(0, 10, index =>
            {
                Console.WriteLine($"Hello {index} ");
            });
        }

        [Test(Description = "adds an entity to table storage")]
        [Repeat(10)]
        public void CanAdd()
        {
            var note = DataGenerator.GetRandomChangeNote();
            _storage.Add(note);

            Assert.IsNotNull(_storage);
        }

        [Test]
        [Repeat(10)]
        public void CanDelete()
        {
            var note = DataGenerator.GetRandomChangeNote();

            _storage.Add(note);

            _storage.Delete(note);
        }

        [Test]
        [Repeat(10)]
        public void CanQuery()
        {
            var note = DataGenerator.GetRandomChangeNote();

            _storage.Add(note);

            var result =
                _storage.Find(note.PartitionKey,note.RowKey)
                    .FirstOrDefault(n => n.OrgAreaInt == note.OrgAreaInt && n.ItemId == note.ItemId);

            //var result = (from e in _storage.Query()
            //              where e.PartitionKey == note.PartitionKey && e.OrgAreaInt == note.OrgAreaInt
            //                    && e.ItemId == note.ItemId
            //              select e).FirstOrDefault();

            Assert.IsNotNull(result);
        }

        [Test]
        [Repeat(10)]
        public void CanUpdateItem()
        {
            var note = DataGenerator.GetRandomChangeNote();

            _storage.Replace(note);
        }

        [SetUp]
        public void StartUp()
        {
            _timer.Restart();
        }

        [TearDown]
        public void TearDown()
        {
            _timer.Stop();
            var testName = TestContext.CurrentContext.Test.Name;
            var elapsedMs = _timer.ElapsedMilliseconds;
            _totalDur.Add(elapsedMs);

            Console.WriteLine(
                $"{testName}: {TestContext.CurrentContext.Result.Outcome.Status}: {elapsedMs/60.0:F}");
        }


        [OneTimeSetUp]
        public void Init()
        {
            _timer.Reset();
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            _timer.Stop();

            var result = _totalDur.Sum(i => i);

            Console.WriteLine(
          $"Total Duration: { result / 60.0:F}  Tests: {_totalDur.Count()}");
        }
    }
}
