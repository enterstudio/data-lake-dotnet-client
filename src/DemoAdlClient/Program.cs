﻿using System;
using System.Collections.Generic;
using System.Linq;
using MSADLA = Microsoft.Azure.Management.DataLake.Analytics;
using Microsoft.Azure.Management.DataLake.Analytics;

namespace DemoAdlClient
{
    class Program
    {
        private static void Main(string[] args)
        {
            // Setup authentication for this demo
            var auth = new AdlClient.Authentication("microsoft.onmicrosoft.com"); // change this to YOUR tenant
            auth.Authenticate();

            // Collect info about the Azure resources needed for this demo
            string subid = "045c28ea-c686-462f-9081-33c34e871ba3";
            string rg = "InsightServices";
            string adla_account = "datainsightsadhoc";
            string adls_account = "datainsightsadhoc";

            // Create the clients
            var az = new AdlClient.AzureClient(auth);
            var adla = az.Analytics.ConnectToAccount(subid, rg, adla_account);
            var adls = az.Store.ConnectToAccount(subid, rg, adls_account);


            var jobclient = adla.RestClients._JobRest.RestClient; // get the generated rest client

            EnumJobs1(jobclient, adla_account);
            EnumJobs2(jobclient, adla_account);
        }

        private static void EnumJobs1(DataLakeAnalyticsJobManagementClient jobclient, string adla_account)
        {
            var sw = new System.Diagnostics.Stopwatch();

            string opt_select = null;
            bool? opt_count = null;

            int item_count = 0;
            sw.Start();
            var jobs = jobclient.Job.List(adla_account, null, opt_select, opt_count);
            foreach (var job in jobs)
            {
                Console.WriteLine("ENUM1: {0}", item_count);
                item_count++;

                if (item_count > 1000)
                {
                    break;
                }
            }
            sw.Stop();
            Console.WriteLine("ENUM1 TOTAL TIME: {0}", sw.Elapsed.TotalSeconds);
        }

        private static void EnumJobs2(DataLakeAnalyticsJobManagementClient jobclient, string adla_account)
        {
            var sw = new System.Diagnostics.Stopwatch();

            string opt_select = null;
            bool? opt_count = null;

            int item_count = 0;
            sw.Start();
            
            var page= jobclient.Job.List(adla_account, null, opt_select, opt_count);

            while (true)
            {
                foreach (var job in page)
                {
                    Console.WriteLine("ENUM1: {0}", item_count);
                    item_count++;

                    if (item_count > 1000)
                    {
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(page.NextPageLink))
                {
                    page = jobclient.Job.ListNext(page.NextPageLink);
                }

                if (item_count > 1000)
                {
                    break;
                }
            }
            sw.Stop();
            Console.WriteLine("ENUM1 TOTAL TIME: {0}", sw.Elapsed.TotalSeconds);
        }

        private static void OldMain(string[] args)
        {
            // Setup authentication for this demo
            var auth = new AdlClient.Authentication("microsoft.onmicrosoft.com"); // change this to YOUR tenant
            auth.Authenticate();

            // Collect info about the Azure resources needed for this demo
            string subid = "045c28ea-c686-462f-9081-33c34e871ba3";
            string rg = "InsightServices";
            string adla_account = "datainsightsadhoc";
            string adls_account = "datainsightsadhoc";

            // Create the clients
            var az = new AdlClient.AzureClient(auth);
            var adla = az.Analytics.ConnectToAccount(subid, rg, adla_account);
            var adls = az.Store.ConnectToAccount(subid, rg, adls_account);
            
            // ------------------------------
            // Run the Demo
            // ------------------------------

            Demo_ListMySubscriptions(az);
            Demo_ListMyResourceGroups(az);

            Demo_JobsDetails(adla);
            Demo_Jobs_GetJobUrl(adla);

            RunDemos_Job_Summaries(adla);
            RunDemos_Job_Listing(adla);
            RunDemos_Catalog(adla);
            RunDemos_Analytics_Account_Management(adla);
            RunDemos_FileSystem(adls);
            RunDemos_Resource_Managementr(az);
        }

        private static void Demo_ListMySubscriptions(AdlClient.AzureClient az)
        {
            var subs = az.ListSubscriptions();
            foreach (var sub in subs)
            {
                Console.WriteLine("Sub: {0} {1}",sub.DisplayName, sub.Id);
            }
        }

        private static void Demo_ListMyResourceGroups(AdlClient.AzureClient az)
        {
            var subs = az.ListSubscriptions();
            var sub = subs.First();
            var rgs = az.ListResourceGroups(sub.SubscriptionId);
            foreach (var rg in rgs)
            {
                Console.WriteLine("Sub: {0}", rg.Name);
            }
        }

        private static void Demo_JobsDetails(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 5;
            opts.Filter.State.IsOneOf( MSADLA.Models.JobState.Ended );
            opts.Filter.Result.IsOneOf(MSADLA.Models.JobResult.Succeeded );
            var jobs = adla.Jobs.ListJobs(opts).ToList();

            var first_job = jobs[0];
            var jobdetails = adla.Jobs.GetJobDetails(first_job.Id, true);
        }

        private static void RunDemos_Analytics_Account_Management(AdlClient.AnalyticsClient adla)
        {
            Demo_AnalyticsAccount_List_LinkedStoreAccounts(adla);
        }

        private static void RunDemos_FileSystem(AdlClient.StoreClient adla)
        {
            Demo_FileSystem_ListFilesInFolder(adla);
        }

        private static void RunDemos_Resource_Managementr(AdlClient.AzureClient az)
        {
            Demo_Resource_List_AnalyticsAccounts(az);
            Demo_Resource_ListDataLakeStoreAccountsInSubscription(az);
        }

        private static void RunDemos_Catalog(AdlClient.AnalyticsClient adla)
        {
            Demo_Catalog_ListDatabases(adla);
        }

        private static void RunDemos_Job_Listing(AdlClient.AnalyticsClient adla)
        {
            Demo_Jobs_List_NeverStarted(adla);
            Demo_Jobs_List_Recent(adla);
            Demo_Jobs_List_SingleMostRecent(adla);
            Demo_Jobs_List_Oldest(adla);
            Demo_Jobs_List_Oldest_from_Submitter(adla);
            Demo_Jobs_List_Failed(adla);
            Demo_Jobs_List_SubmittedBy_AuthenticatedUser(adla);
            Demo_Jobs_List_SubmittedBy_Users(adla);
            Demo_Jobs_List_SubmittedBetween_MidnightAndNow(adla);
            Demo_Jobs_List_SubmittedBy_UserBeginsWith(adla);
            Demo_Jobs_List_SubmittedBy_UserContains(adla);
            Demo_Jobs_List_MostExpensive_In_Last24hours(adla);
        }

        private static void RunDemos_Job_Summaries(AdlClient.AnalyticsClient adla)
        {
            Demo_Jobs_Summarize_FailedAUHours_By_Submitter(adla);
            Demo_Jobs_Summarize_AUHours_By_JobResult_nad_Submitter(adla);
        }

        private static void Demo_FileSystem_ListFilesInFolder(AdlClient.StoreClient adls)
        {
            //var folder = ADLC.Store.FsPath.Root; // same as "/"
            var folder = new AdlClient.FileSystem.FsPath("/Samples");
            var lfo = new AdlClient.FileSystem.ListFilesOptions();
            foreach (var page in adls.FileSystem.ListFilesPaged(folder,lfo))
            {
                foreach (var fileitemn in page.FileItems)
                {
                    Console.WriteLine("path={0} filename={1}",page.Path,fileitemn.PathSuffix);                    
                }
            }
        }

        private static void Demo_Jobs_GetJobUrl(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 3;
            var jobs = adla.Jobs.ListJobs(opts).ToList();

            foreach (var job in jobs)
            {
                var joblink = job.GetJobReference();

                string joburi_string = joblink.GetUri();
                string job_portal_link_string = joblink.GetAzurePortalLink();

                Console.WriteLine(joburi_string);
                Console.WriteLine(job_portal_link_string);
            }
        }


        private static void Demo_Jobs_List_SingleMostRecent(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 1;
            var jobs = adla.Jobs.ListJobs(opts).ToList();

            PrintJobs(jobs);
        }

        private static void Demo_Jobs_List_SubmittedBy_AuthenticatedUser(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 10;
            opts.Filter.Submitter.IsOneOf(adla.Authentication.Token.DisplayableId);

            var jobs = adla.Jobs.ListJobs(opts).ToList();

            PrintJobs(jobs);
        }


        private static void Demo_Jobs_List_SubmittedBy_Users(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 10;
            opts.Filter.Submitter.IsOneOf("mrys@microsoft.com", "saveenr@microsoft.com");

            var jobs = adla.Jobs.ListJobs(opts).ToList();

            PrintJobs(jobs);
        }

        private static void Demo_Jobs_List_SubmittedBy_UserBeginsWith(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 10;
            opts.Filter.Submitter.BeginsWith("saa");

            var jobs = adla.Jobs.ListJobs(opts).ToList();

            PrintJobs(jobs);
        }

        private static void Demo_Jobs_List_SubmittedBy_UserContains(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 10;
            opts.Filter.Submitter.Contains("eenr");

            var jobs = adla.Jobs.ListJobs(opts).ToList();

            PrintJobs(jobs);
        }


        private static void Demo_Jobs_List_Recent(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 10;

            var jobfields = new AdlClient.Jobs.JobFields();
            opts.Sorting.Direction = AdlClient.OData.Enums.OrderByDirection.Descending;
            opts.Sorting.Field = jobfields.SubmitTime;

            var jobs = adla.Jobs.ListJobs(opts).ToList();

            PrintJobs(jobs);
        }

        private static void Demo_Jobs_List_Oldest(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 10;

            var jobfields = new AdlClient.Jobs.JobFields();
            opts.Sorting.Direction = AdlClient.OData.Enums.OrderByDirection.Ascending;
            opts.Sorting.Field = jobfields.SubmitTime;

            var jobs = adla.Jobs.ListJobs(opts).ToList();

            PrintJobs(jobs);
        }

        private static void Demo_Jobs_List_Oldest_from_Submitter(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 1;

            var jobfields = new AdlClient.Jobs.JobFields();
            opts.Sorting.Direction = AdlClient.OData.Enums.OrderByDirection.Ascending;
            opts.Sorting.Field = jobfields.SubmitTime;

            opts.Filter.Submitter.Contains("saveenr");

            var jobs = adla.Jobs.ListJobs(opts).ToList();

            PrintJobs(jobs);
        }


        private static void Demo_Jobs_List_Failed(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 5;

            opts.Filter.Result.IsOneOf(MSADLA.Models.JobResult.Failed);

            var jobs = adla.Jobs.ListJobs(opts).ToList();

            PrintJobs(jobs);
        }


        private static void Demo_GetJobsSubmitedInLast2hours(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Filter.SubmitTime.InRange(AdlClient.OData.Utils.RangeDateTime.InTheLastNHours(2));
            var jobs = adla.Jobs.ListJobs(opts).ToList();
            PrintJobs(jobs);
        }

        private static void Demo_Jobs_List_SubmittedBetween_MidnightAndNow(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Filter.SubmitTime.InRange(AdlClient.OData.Utils.RangeDateTime.SinceLocalMidnight());
            var jobs = adla.Jobs.ListJobs(opts).ToList();
            PrintJobs(jobs);
        }

        private static void Demo_Jobs_List_NeverStarted(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 2;
            opts.Filter.StartTime.IsNull();
            var jobs = adla.Jobs.ListJobs(opts).ToList().ToList();
            PrintJobs(jobs);
        }

        private static void Demo_Jobs_List_MostExpensive_In_Last24hours(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Filter.SubmitTime.InRange(AdlClient.OData.Utils.RangeDateTime.InTheLastNHours(24));
            var jobs = adla.Jobs.ListJobs(opts).OrderByDescending(j=>j.AUSeconds).Take(10).ToList();

            PrintJobs(jobs);
        }

        private static void PrintJobs(IEnumerable<AdlClient.Jobs.JobInfo> jobs)
        {
            foreach (var job in jobs)
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("Name = {0}", job.Name);
                Console.WriteLine("AUs = {0}", job.DegreeOfParallelism);
                Console.WriteLine("Priority = {0}", job.Priority);
                Console.WriteLine("Result = {0}; State = {1}", job.Result, job.State);
                Console.WriteLine("StartTime = {0} ", job.SubmitTime);
                Console.WriteLine("SubmitTime = {0} [ Local = {1} ] ", job.SubmitTime.Value, job.SubmitTime.Value.ToLocalTime());
                Console.WriteLine("Submitter = {0}", job.Submitter);
                Console.WriteLine("AUHours = {0}", job.AUSeconds / (60.0 * 60.0));
            }
        }

        private static void Demo_AnalyticsAccount_List_LinkedStoreAccounts(AdlClient.AnalyticsClient adla)
        {
            var storage_accounts = adla.Management.ListLinkedDataLakeStoreAccounts().ToList();
            foreach (var i in storage_accounts)
            {
                Console.WriteLine("----------------");
                Console.WriteLine("Name = {0}", i.Name);
                Console.WriteLine("Type = {0}", i.Type);
            }
        }

        private static void Demo_Resource_List_AnalyticsAccounts(AdlClient.AzureClient az)
        {
            var sub = "045c28ea-c686-462f-9081-33c34e871ba3";

            var storage_accounts = az.Analytics.ListAccountsInSubscription(sub).ToList();
            foreach (var i in storage_accounts)
            {
                Console.WriteLine("----------------");
                Console.WriteLine("Name = {0}", i.Name);
                Console.WriteLine("Location = {0}", i.Location);
                Console.WriteLine("Type = {0}", i.Type);
            }
        }

        private static void Demo_Catalog_ListDatabases(AdlClient.AnalyticsClient adla)
        {
            var databases = adla.Catalog.ListDatabases().ToList();
            foreach (var i in databases)
            {
                Console.WriteLine("----------------");
                Console.WriteLine("Name = {0}", i.Name);
                Console.WriteLine("Version = {0}", i.Version);
                Console.WriteLine("ComputeAccountName = {0}", i.ComputeAccountName);
            }
        }

        private static void Demo_Resource_ListDataLakeStoreAccountsInSubscription(AdlClient.AzureClient az)
        {
            var sub = "045c28ea-c686-462f-9081-33c34e871ba3";

            var storage_accounts = az.Store.ListAccountsInSubscription(sub).ToList();
            foreach (var i in storage_accounts)
            {
                Console.WriteLine("----------------");
                Console.WriteLine("Name = {0}", i.Name);
                Console.WriteLine("Location = {0}", i.Location);
                Console.WriteLine("Type = {0}", i.Type);
            }
        }




        private static void Demo_Jobs_Summarize_FailedAUHours_By_Submitter(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 300;

            opts.Filter.Result.IsOneOf(MSADLA.Models.JobResult.Failed);
            opts.Filter.StartTime.IsNotNull();

            var failed_jobs = adla.Jobs.ListJobs(opts).ToList();

            var results = from job in failed_jobs
                          group job by job.Submitter into job_group
                          select new 
                          {
                              Submitter = job_group.Key,
                              Count = job_group.Count(),
                              AUHours = job_group.Sum( j=> j.AUSeconds)/(60.0*60.0),
                          };

            foreach (var i in results)
            {
                Console.WriteLine("----------------");
                Console.WriteLine("Submitter = {0}", i.Submitter);
                Console.WriteLine("NumJobs = {0}", i.Count);
                Console.WriteLine("AU Hours = {0}", i.AUHours);
            }

        }

        private static void Demo_Jobs_Summarize_AUHours_By_JobResult_nad_Submitter(AdlClient.AnalyticsClient adla)
        {
            var opts = new AdlClient.Jobs.ListJobOptions();
            opts.Top = 300;

            //opts.Filter.Result.OneOf(MS_ADLA.Models.JobResult.Failed);

            var jobs = adla.Jobs.ListJobs(opts).Where(j => j.StartTime != null).ToList();

            var results = from job in jobs
                          group job by 
                            new {   job.Result,
                                    job.Submitter }  
                            into job_group
                          select new
                          {
                              Submitter = job_group.Key.Submitter,
                              Result = job_group.Key.Result,
                              Count = job_group.Count(),
                              AUHours = job_group.Sum(j => j.AUSeconds) / (60.0 * 60.0),
                          };

            foreach (var row in results)
            {
                Console.WriteLine("----------------");
                Console.WriteLine("Submitter = {0}", row.Submitter);
                Console.WriteLine("Result = {0}", row.Result);
                Console.WriteLine("NumJobs = {0}", row.Count);
                Console.WriteLine("AU Hours = {0}", row.AUHours);
            }

        }

    }
}
