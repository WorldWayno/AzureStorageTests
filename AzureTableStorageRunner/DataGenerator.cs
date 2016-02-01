using System;
using System.Text.RegularExpressions;
using AzureStorage.Data;

namespace AzureTableStorageRunner
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

        public static ChangeNote GetRandomChangeNote()
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