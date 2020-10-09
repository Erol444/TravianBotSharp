using Elasticsearch.Net.Specification.IndexLifecycleManagementApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Resources;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Tasks;
using static TbsCore.Models.TB;

namespace TravBotSharp.Files.Helpers
{
    public static class IoHelperCore
    {
        public static string AccountsPath => Path.Combine(TbsPath(), "accounts.txt");
        public static string CachePath => Path.Combine(TbsPath(), "cache");
        public static string GetCacheDir(string username, string server, Access access)
        {
            return Path.Combine(IoHelperCore.CachePath, GetCacheFolder(username, server, access.Proxy));
        }

        public static void AddBuildTasksFromFile(Account acc, Village vill, string location)
        {
            List<BuildingTask> tasks = new List<BuildingTask>();
            try
            {
                using (StreamReader sr = new StreamReader(location))
                {
                    // If .trbc file, decode into List<BuildTask>
                    if (Path.GetExtension(location).Equals(".TRBC", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var trbc = JsonConvert.DeserializeObject<TbRoot>(sr.ReadToEnd());
                        tasks = DecodeTrbc(trbc);
                    }
                    else tasks = JsonConvert.DeserializeObject<List<BuildingTask>>(sr.ReadToEnd());
                }
            }
            catch (Exception e) { return; } // User canceled

            foreach (var task in tasks)
            {
                BuildingHelper.AddBuildingTask(acc, vill, task);
            }
            BuildingHelper.RemoveCompletedTasks(vill, acc);
        }

        private static List<BuildingTask> DecodeTrbc(TbRoot root)
        {
            var tasks = new List<BuildingTask>();

            foreach (var cmd in root.commands)
            {
                var task = new BuildingTask
                {
                    Level = cmd.level
                };
                if (cmd.bid > 0) task.BuildingId = (byte)cmd.bid;

                switch (cmd.cmdType)
                {
                    case 4: // Based on level
                        task.TaskType = BuildingHelper.BuildingType.AutoUpgradeResFields;
                        task.BuildingStrategy = BuildingStrategyEnum.BasedOnLevel;
                        task.ResourceType = GetTrBuilderResType(cmd.gid);
                        break;
                    case 5: // Based on production
                        task.TaskType = BuildingHelper.BuildingType.AutoUpgradeResFields;
                        task.BuildingStrategy = BuildingStrategyEnum.BasedOnProduction;
                        task.ResourceType = GetTrBuilderResType(cmd.gid);
                        break;
                    case 6: // Based on storage
                        task.TaskType = BuildingHelper.BuildingType.AutoUpgradeResFields;
                        task.BuildingStrategy = BuildingStrategyEnum.BasedOnRes;
                        task.ResourceType = GetTrBuilderResType(cmd.gid);
                        break;
                    default: // Normal build?
                        task.TaskType = BuildingHelper.BuildingType.General;
                        task.Building = (Classificator.BuildingEnum)cmd.gid;
                        break;
                }

                tasks.Add(task);
            }
            return tasks;
        }
        private static ResTypeEnum GetTrBuilderResType(int gid)
        {
            switch (gid)
            {
                case 60: return ResTypeEnum.AllResources;
                case 61: return ResTypeEnum.ExcludeCrop;
                case 62: return ResTypeEnum.OnlyCrop;
            }
            return ResTypeEnum.AllResources;
        }

        public static string TbsPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "TravBotSharp");
        }

        /// <summary>
        /// Removes the cache folders that were created by Selenium driver, since they take a lot of space (70MB+)
        /// </summary>
        /// <param name="acc">Account</param>
        public static void RemoveCache(Account acc)
        {
            var userFolder = IoHelperCore.GetCacheFolder(acc.AccInfo.Nickname, acc.AccInfo.ServerUrl, "");

            var removeFolders = Directory
                .GetDirectories(CachePath + "\\")
                .Where(x => x.Replace(CachePath + "\\", "").StartsWith(userFolder))
                .ToArray();

            if (removeFolders == null) return;

            for (int i = 0; i < removeFolders.Count(); i++)
            {
                Directory.Delete(removeFolders[i], true);
            }
        }

        /// <summary>
        /// Removes the protocol (http/https) text from the url
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Shortened url</returns>
        public static string UrlRemoveHttp(string url)
        {
            return url.Replace("https://", "").Replace("http://", "");
        }

        /// <summary>
        /// Read accounts from the accounts.txt file
        /// </summary>
        /// <returns>Accounts saved in the file</returns>
        public static List<Account> ReadAccounts()
        {
            var accounts = new List<Account>();
            try
            {
                // Open the text file using a stream reader.
                var folder = IoHelperCore.TbsPath();
                System.IO.Directory.CreateDirectory(folder);

                using (StreamReader sr = new StreamReader(IoHelperCore.AccountsPath))
                {
                    accounts = JsonConvert.DeserializeObject<List<Account>>(sr.ReadToEnd());
                }
                if (accounts == null) accounts = new List<Account>();

                accounts.ForEach(x => ObjectHelper.FixAccObj(x, x));
            }
            catch (IOException e)
            {

                Console.WriteLine("Can't read accounts.txt, Exception thrown: " + e.Message);
            }
            return accounts;
        }

        /// <summary>
        /// Cache folder selenium will use for this account
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="server">Server url</param>
        /// <param name="proxy">Proxy ip</param>
        /// <returns></returns>
        internal static string GetCacheFolder(string username, string server, string proxy)
        {
            return $"{username}_{IoHelperCore.UrlRemoveHttp(server)}_{proxy}";
        }
        /// <summary>
        /// Quit (stop) the program. This will logout (close drivers) from all accounts and save them into the file
        /// </summary>
        /// <param name="accounts"></param>
        public static void Quit(List<Account> accounts)
        {
            foreach (var acc in accounts)
            {
                Logout(acc);
            }
            using (StreamWriter sw = new StreamWriter(AccountsPath))
            {
                sw.Write(JsonConvert.SerializeObject(accounts));
            }
        }
        /// <summary>
        /// Login into account and initialize everything
        /// </summary>
        /// <param name="acc">Account</param>
        public static async Task LoginAccount(Account acc)
        {
            if (acc.Wb == null)
            { // If Agent doesn't exist yet
                acc.Tasks = new List<BotTask>();
                acc.Wb = new WebBrowserInfo();
                await acc.Wb.InitSelenium(acc);
                acc.TaskTimer = new TaskTimer(acc);

                AccountHelper.StartAccountTasks(acc);
            }
        }
        /// <summary>
        /// Logout from the account. Closes web driver.
        /// </summary>
        /// <param name="acc"></param>
        public static void Logout(Account acc)
        {
            if (acc.TaskTimer != null)
            {
                acc.TaskTimer.Stop();
                acc.TaskTimer = null;
            }
            if (acc.Wb != null)
            {
                acc.Wb.Close();
                acc.Wb = null;
            }
            acc.Tasks = null; //TODO: somehow save tasks, JSON cant parse/stringify abstract classes :(
        }

        /// <summary>
        /// Gets a random useragent. Useragents written higher in the file are more popular, thus should be
        /// used by the bot more frequently.
        /// </summary>
        /// <returns>Random useragent string</returns>
        public static string GetUseragent()
        {
            Random rnd = new Random();
            var agents = Resources.useragents.Split('\n');
            for (int i = 0; i < agents.Length; i++)
            {
                int limit = agents.Length - i;
                int num = rnd.Next(1, limit);
                if(num <= 1 + limit / 10)
                {
                    return agents[i].Replace("\r", "");
                }
            }
            return "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.70 Safari/537.36";
        }
    }
}
