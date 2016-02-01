using System;
using Microsoft.WindowsAzure.Storage.Table;
using Scribe.Objects;

namespace AzureStorage.Data
{
    public class ChangeNote : TableEntity, IChangeNote
    {
        public ChangeNote() 
        {
            base.Timestamp = DateTimeOffset.UtcNow;
        }

        public ChangeNote(string partitionKey, string rowKey, DateTime timeStamp)
        {
            base.PartitionKey = partitionKey;
            base.RowKey = rowKey;
            base.Timestamp = timeStamp;
        }

        public string Data { get; set; }
        public string ItemId { get; set; }
        public int OrgAreaInt { get; set; }
        public int RollingVersion { get; set; }
        public new DateTime Timestamp { get; set; }
    }
}