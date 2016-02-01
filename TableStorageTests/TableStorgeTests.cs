using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Scribe.Objects;
using TableStorageAzureOld;

namespace TableStorageTests
{
    [TestFixture]
    public class TableStorgeOldTests
    {
        private readonly Stopwatch _timer = new Stopwatch();
        private Dictionary<string, long> _testAvges = new Dictionary<string, long>();
        private List<long> _totalAvg = new List<long>(); 
        private readonly AzureStorageOld _storage;
        public TableStorgeOldTests()
        {
            var settings = Config.GetSettings();
            _storage = new AzureStorageOld(settings.Account, settings.Key, settings.Table, false);
        }

        [OneTimeSetUp]
        public void Init()
        {
            _timer.Reset();
        }

        [OneTimeTearDown]
        public void Dispose()
        {
           var result = _testAvges.Average(t => t.Value);
            var total = _totalAvg.Sum(l => l);
           

            Console.WriteLine(
          $"Total Duration: { total / 60.0:F} Tests: {_totalAvg.Count()}");
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
            _totalAvg.Add(elapsedMs);

            Console.WriteLine(
                $"{testName}: {TestContext.CurrentContext.Result.Outcome.Status}: {elapsedMs / 60.0:F}");

            if (!_testAvges.ContainsKey(testName))
            {
                _testAvges.Add(testName, elapsedMs);
            }
            else
            {
                long result = _testAvges[testName];
                result += elapsedMs;
                _testAvges[testName] = result;
            }

            //if (_testAvges.Values. == 3)
            //{
            //    var val = _testAvges[testName];
            //    Console.WriteLine($"Total: {val}");
            //}
           

        }

        [Test]
        [Repeat(10)]
        public void CanAddItem()
        {
            var note = DataGenerator.GetRandomChangeNote();

            _storage.Add(note);

            _storage.Commit();
        }

        [Test]
        [Repeat(10)]
        public void CanDeleteItem()
        {
            var note = DataGenerator.GetRandomChangeNote();

            _storage.Add(note);

            _storage.Commit();

            _storage.Delete(note);

            _storage.Commit();
        }

        [Test]
        [Repeat(10)]
        public void CanQueryForItem()
        {
            var note = DataGenerator.GetRandomChangeNote();

            _storage.Add(note);

            _storage.Commit();

            var result = (from e in _storage.Query()
                where e.PartitionKey == note.PartitionKey && e.OrgAreaInt == note.OrgAreaInt
                      && e.ItemId == note.ItemId
                select e).FirstOrDefault();

            Assert.IsNotNull(result);
        }

        [Test]
        [Repeat(10)]
        public void CanUpdateItem()
        {
            var note = DataGenerator.GetRandomChangeNote();

            _storage.Attach(note);

            _storage.Update(note);

            _storage.Commit();

            Assert.IsNotNull(_storage);
        }
    }
}
