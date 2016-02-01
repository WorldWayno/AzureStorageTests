using System;
using Microsoft.WindowsAzure.StorageClient;
using Scribe.Objects;

namespace TableStorageAzureOld
{
    public class ChangeNote : TableServiceEntity, IChangeNote
    {
        public string Data { get; set; }
        public string ItemId { get; set; }
        public int OrgAreaInt { get; set; }
        public int RollingVersion { get; set; }
    }
}