using System;
using System.Text.RegularExpressions;
using Scribe.Objects;
using TableStorageAzureOld;

namespace TableStorageTests
{
    public class DataGenerator
    {
        private static readonly Random random = new Random();
        public static int GetRandomInt(int maxValue = 100)
        {
            var num = GetFirstNumber();
            return random.Next(num, num+1);
        }

        public static int GetFirstNumber()
        {
          return Int32.Parse(Regex.Match(GetRandomGuid().ToString(), @"\d+").Value);
        }

        public static Guid GetRandomGuid()
        {
            return Guid.NewGuid();
        }

        public static IChangeNote GetRandomChangeNote()
        {
            return new ChangeNote
            {
                PartitionKey = GetRandomGuid().ToString(),
                RowKey = GetRandomInt().ToString(),
                OrgAreaInt = GetRandomInt(),
                RollingVersion = GetRandomInt(),
                ItemId = GetRandomGuid().ToString().Replace("-",""),
                Data = GetRandomGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
        }
    }
}