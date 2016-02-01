using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Scribe.Objects;
using CloudTableClient = Microsoft.WindowsAzure.StorageClient.CloudTableClient;

namespace TableStorageAzureOld
{
    public class AzureStorageOld
    {
        private readonly CloudStorageAccount _account;
        private CloudTableClient _client;
        private readonly string _table;
        private TableServiceContext _context;



        public AzureStorageOld(string account, string key, string table, bool nagling = true)
        {
            this._account = this.GetStorageAccount(account, key);

            EnableNagling(nagling);
            this._client = _account.CreateCloudTableClient();

            this._table = table;

            this.CreateTable(table);

            this._context = _client.GetDataServiceContext();
        }

        public void EnableNagling(bool enable)
        {
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(_account.TableEndpoint);
            tableServicePoint.UseNagleAlgorithm = enable;
        }

        private void Init()
        {
            _client.RetryPolicy = RetryPolicies.Retry(20, TimeSpan.FromSeconds(5));
        }

        private StorageCredentialsAccountAndKey GetCredentials(string account, string key)
        {
            return new StorageCredentialsAccountAndKey(account, key);
        }

        private CloudStorageAccount GetStorageAccount(string account, string key)
        {
            var creds = this.GetCredentials(account, key);
            return new CloudStorageAccount(creds, true);
        }

        public void CreateTableClient()
        {
           _client = _account.CreateCloudTableClient();
        }

        public void GetTableClient()
        {
            _client = new CloudTableClient(_account.TableEndpoint,_account.Credentials);
        }

        public void CreateTable(string table)
        {
            _client.CreateTableIfNotExist(table);
        }

        public bool TableExists(string table)
        {
            return _client.DoesTableExist(table);
        }

        public IQueryable<IChangeNote> Query()
        {
            return _context.CreateQuery<IChangeNote>(this._table);
        }

        public void Add(IChangeNote note)
        {
            _context.AddObject(this._table, note);
        }

        public void Attach(IChangeNote note)
        {
            _context.AttachTo(this._table, note);
        }
        public void Update(IChangeNote note)
        {
            _context.UpdateObject(note);
        }

        public void Commit()
        {
            _context.RetryPolicy = RetryPolicies.NoRetry();
            _context.SaveChanges(SaveChangesOptions.ContinueOnError| SaveChangesOptions.ReplaceOnUpdate);
        }

        public void Delete(IChangeNote note)
        {
            _context.DeleteObject(note);
        }
    }
}
