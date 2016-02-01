// <copyright file="AzureStorageRepository.cs" company="Scribe Software Corporation">
// //   Copyright © 1996-2016 Scribe Software Corp. All rights reserved.
// // </copyright>
// // <summary>
// //   This is a generated file please do not make modifications to this file. It was created using the SurrogateGenerator Tool. This file contains the partial class definition for the known types involved in synchronizations.
// // </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureStorage.Data
{
    public class AzureStorageRepository
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTable _table;
        private readonly CloudTableClient _tableClient;

        public AzureStorageRepository(string account, string key, string tableName, bool nagling = true)
        {
            _storageAccount = GetStorageAccount(account, key);

            _tableClient = _storageAccount.CreateCloudTableClient();

            _table = _tableClient.GetTableReference(tableName);

            _table.CreateIfNotExistsAsync();

            EnableNagling(nagling);
        }

        public void Add(ITableEntity note)
        {
            var insertOperation = TableOperation.Insert(note);
            _table.Execute(insertOperation);
        }

        public async void Add(IList<ITableEntity> notes)
        {
            var batchOperation = new TableBatchOperation();

            notes.ToList().ForEach(n => batchOperation.Insert(n));
            await _table.ExecuteBatchAsync(batchOperation);
        }

        public ChangeNote Retrieve(string partionKey, string rowKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<ChangeNote>(partionKey, rowKey);

            // Execute the retrieve operation.
            TableResult result = _table.Execute(retrieveOperation);

            return (ChangeNote) result.Result;
        }

        public IEnumerable<ChangeNote> Find(string partionKey)
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            var query =
                new TableQuery<ChangeNote>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                    partionKey));

            return _table.ExecuteQuery(query).ToList();
        }

        public IEnumerable<ChangeNote> Find(string partionKey, string rowKey)
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            var query = new TableQuery<ChangeNote>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partionKey),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey)));

            return _table.ExecuteQuery(query).ToList();
        }

        public ChangeNote GetNote(ChangeNote note)
        {
            var query = new TableQuery<ChangeNote>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, note.PartitionKey),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, note.RowKey)));

            return _table.ExecuteQuery(query).ToList().Single();
        }

        public void Delete(ITableEntity note)
        {
            // Create the Delete TableOperation.
            var deleteOperation = TableOperation.Delete(note);
            // Execute the operation.
            _table.Execute(deleteOperation);
        }

        public void Update(ChangeNote note)
        {
            // Create the InsertOrReplace TableOperation.
            TableOperation updateOperation = TableOperation.Replace(note);

            // Execute the operation.
            _table.Execute(updateOperation);

        }

        public void Replace(ChangeNote note)
        {
            // Assign the result to a CustomerEntity object.

            var insertOrReplaceOperation = TableOperation.InsertOrReplace(note);

            // Execute the operation.
            _table.Execute(insertOrReplaceOperation);
        }

        public void EnableNagling(bool enable)
        {
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(_storageAccount.TableEndpoint);
            tableServicePoint.UseNagleAlgorithm = enable;
        }


        private CloudStorageAccount GetStorageAccount(string account, string key)
        {
            var creds = new StorageCredentials(account, key);

            return new CloudStorageAccount(creds, true);
        }
    }
}