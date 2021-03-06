﻿using System.Collections.Generic;
using Microsoft.Azure.Management.DataLake.Analytics; // have to have this using clause to get the extension methods
using MSADL = Microsoft.Azure.Management.DataLake;

namespace AdlClient.Rest
{
    public class AnalyticsAccountManagmentRestWrapper
    {
        public MSADL.Analytics.DataLakeAnalyticsAccountManagementClient RestClient;

        public AnalyticsAccountManagmentRestWrapper(string sub, Microsoft.Rest.ServiceClientCredentials creds)
        {
            this.RestClient = new MSADL.Analytics.DataLakeAnalyticsAccountManagementClient(creds);
            this.RestClient.SubscriptionId = sub;
        }

        public IEnumerable<MSADL.Analytics.Models.DataLakeAnalyticsAccount> ListAccounts()
        {
            var initial_page = this.RestClient.Account.List();
            foreach (var acc in RestUtil.EnumItemsInPages(initial_page, p => this.RestClient.Account.ListNext(p.NextPageLink)))
            {
                yield return acc;
            }
        }

        public IEnumerable<MSADL.Analytics.Models.DataLakeAnalyticsAccount> ListAccounts(string rg)
        {
            var initial_page = this.RestClient.Account.ListByResourceGroup(rg);
            foreach (var acc in RestUtil.EnumItemsInPages(initial_page, p => this.RestClient.Account.ListByResourceGroupNext(p.NextPageLink)))
            {
                yield return acc;
            }
        }

        public MSADL.Analytics.Models.DataLakeAnalyticsAccount GetAccount(string rg, AnalyticsAccount account)
        {
            var adls_account = this.RestClient.Account.Get(rg, account.Name);
            return adls_account;
        }

        public bool ExistsAccount(string rg, AnalyticsAccount account_name)
        {
            return this.RestClient.Account.Exists(rg, account_name.Name);
        }

        public void UpdateAccount(string rg, AnalyticsAccount account, MSADL.Analytics.Models.DataLakeAnalyticsAccountUpdateParameters parameters)
        {
            this.RestClient.Account.Update(rg, account.Name, parameters);
        }

        public void AddStorageAccount(string rg, AnalyticsAccount account, string storage_account, MSADL.Analytics.Models.AddStorageAccountParameters parameters)
        {
            this.RestClient.StorageAccounts.Add(rg, account.Name, storage_account, parameters);
        }

        public void AddDataLakeStoreAccount(string rg, AnalyticsAccount account, string storage_account, MSADL.Analytics.Models.AddDataLakeStoreParameters parameters)
        {
            this.RestClient.DataLakeStoreAccounts.Add(rg, account.Name, storage_account, parameters);
        }

        public IEnumerable<MSADL.Analytics.Models.DataLakeStoreAccountInfo> ListStoreAccounts(AnalyticsAccount account)
        {
            var initial_page = this.RestClient.DataLakeStoreAccounts.ListByAccount(account.ResourceGroup, account.Name);
            foreach (var acc in RestUtil.EnumItemsInPages(initial_page, p => this.RestClient.DataLakeStoreAccounts.ListByAccountNext(p.NextPageLink)))
            {
                yield return acc;
            }
        }

        public IEnumerable<MSADL.Analytics.Models.StorageAccountInfo> ListStorageAccounts(AnalyticsAccount account)
        {
            var initial_page = this.RestClient.StorageAccounts.ListByAccount(account.ResourceGroup, account.Name);
            foreach (var acc in RestUtil.EnumItemsInPages(initial_page, p => this.RestClient.StorageAccounts.ListByAccountNext(p.NextPageLink)))
            {
                yield return acc;
            }
        }

        public IEnumerable<MSADL.Analytics.Models.StorageContainer> ListStorageContainers(AnalyticsAccount account, string storage_account)
        {
            var initial_page = this.RestClient.StorageAccounts.ListStorageContainers(account.Name, account.Name, storage_account);
            foreach (var acc in RestUtil.EnumItemsInPages(initial_page, p => this.RestClient.StorageAccounts.ListStorageContainersNext(p.NextPageLink)))
            {
                yield return acc;
            }
        }

        public void DeleteStorageAccount(AnalyticsAccount account, string storage_account)
        {
            this.RestClient.StorageAccounts.Delete(account.ResourceGroup, account.Name, storage_account);
        }

        public void DeleteDataLakeStoreAccount(AnalyticsAccount account, string storage_account)
        {
            this.RestClient.DataLakeStoreAccounts.Delete(account.ResourceGroup, account.Name, storage_account);
        }

        public IEnumerable<MSADL.Analytics.Models.SasTokenInfo> ListSasTokens(AnalyticsAccount account, string storage_account, string container)
        {
            var initial_page = this.RestClient.StorageAccounts.ListSasTokens(account.ResourceGroup, account.Name, storage_account, container);
            foreach (var acc in RestUtil.EnumItemsInPages(initial_page, p => this.RestClient.StorageAccounts.ListSasTokensNext(p.NextPageLink)))
            {
                yield return acc;
            }
        }
    }
}
