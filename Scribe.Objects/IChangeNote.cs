using System;

namespace Scribe.Objects
{
    public interface IChangeNote
    {
        /// <summary>
        /// Optional data 
        /// </summary>
        string Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string ItemId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int OrgAreaInt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int RollingVersion { get; set; }

        string PartitionKey { get; set; }

        string RowKey { get; set; }

        DateTime Timestamp { get; set; }
    }
}