using System.Collections.Specialized;
using System.Configuration;

namespace AzureTableStorageRunner
{
    internal class Config
    {
        private static readonly NameValueCollection Settings = ConfigurationManager.AppSettings;

        public static StorageSettings GetSettings()
        {
            return new StorageSettings()
            {
                Account = Settings["Account"],
                Key = Settings["Key"],
                Url = Settings["Url"],
                Table = Settings["Table"]
            };
        } 
    }
}