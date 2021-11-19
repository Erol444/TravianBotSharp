﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Database;
using TbsCore.Models.Access;
using TbsCore.Models.AccModels;
using TbsCore.Models.BuildingModels;
using TbsCore.Models.VillageModels;

using TbsCore.Tasks;
using static TbsCore.Models.TB;

namespace TbsCore.Helpers
{
    public static class IoHelperCore
    {
        public static string AccountsPath => Path.Combine(TbsPath, "accounts.txt");
        public static string CachePath => Path.Combine(TbsPath, "cache");
        public static string TaskPath => Path.Combine(TbsPath, "task");
        public static string SqlitePath => Path.Combine(TbsPath, "db.sqlite");

        public static string TbsPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TravBotSharp");

        public static bool SQLiteExists() => File.Exists(SqlitePath);

        public static bool AccountsTxtExists() => File.Exists(AccountsPath);

        public static string GetCacheDir(string username, string server, Access access)
        {
            return Path.Combine(CachePath, GetCacheFolder(username, server, access.Proxy));
        }

        public static string GetTaskFileUrl(string username, string server)
        {
            Directory.CreateDirectory(TaskPath);
            return Path.Combine(TaskPath, GetTaskFile(username, server));
        }

        /// <summary>
        /// Gets set by WinForms on startup, so TbsCore can alert user (sound+popup)
        /// </summary>
        public static Func<string, bool> AlertUser { get; set; }

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
            catch (Exception) { return; } // User canceled

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
                        task.TaskType = Classificator.BuildingType.AutoUpgradeResFields;
                        task.BuildingStrategy = BuildingStrategyEnum.BasedOnLevel;
                        task.ResourceType = GetTrBuilderResType(cmd.gid);
                        break;

                    case 5: // Based on production
                        task.TaskType = Classificator.BuildingType.AutoUpgradeResFields;
                        task.BuildingStrategy = BuildingStrategyEnum.BasedOnProduction;
                        task.ResourceType = GetTrBuilderResType(cmd.gid);
                        break;

                    case 6: // Based on storage
                        task.TaskType = Classificator.BuildingType.AutoUpgradeResFields;
                        task.BuildingStrategy = BuildingStrategyEnum.BasedOnRes;
                        task.ResourceType = GetTrBuilderResType(cmd.gid);
                        break;

                    default: // Normal build?
                        task.TaskType = Classificator.BuildingType.General;
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

        public static void RemoveTask(Account acc)
        {
            var userFile = IoHelperCore.GetTaskFileUrl(acc.AccInfo.Nickname, acc.AccInfo.ServerUrl);

            if (!File.Exists(userFile)) return;

            File.Delete(userFile);
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
        /// TODO: remove in future version
        /// </summary>
        /// <returns>Accounts saved in the file</returns>
        public static List<Account> ReadAccounts()
        {
            var accounts = new List<Account>();
            try
            {
                // Open the text file using a stream reader.
                System.IO.Directory.CreateDirectory(IoHelperCore.TbsPath);

                using (StreamReader sr = new StreamReader(IoHelperCore.AccountsPath))
                {
                    accounts = JsonConvert.DeserializeObject<List<Account>>(sr.ReadToEnd());
                }
                accounts = accounts ?? new List<Account>();

                accounts.ForEach(x => ObjectHelper.FixAccObj(x, x));
            }
            catch (IOException e)
            {
                Console.WriteLine(", Exception thrown: " + e.Message);
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
        public static string GetCacheFolder(string username, string server, string proxy)
        {
            return $"{username}_{UrlRemoveHttp(server)}_{proxy}";
        }

        public static string GetTaskFile(string username, string server)
        {
            return $"{username}_{UrlRemoveHttp(server)}.json";
        }

        /// <summary>
        /// Saves accounts into the SQLite DB
        /// </summary>
        /// <param name="accounts"></param>
        public static async Task SaveAccounts(List<Account> accounts, bool logout)
        {
            if (logout)
            {
                var list = new List<Task>();
                foreach (var acc in accounts)
                {
                    if (acc.IsLogged)
                    {
                        var logoutAwait = Logout(acc);
                        list.Add(logoutAwait);
                    }
                }

                await Task.WhenAll(list);
            }
            foreach (var acc in accounts)
            {
                DbRepository.SaveAccount(acc);
                acc.Tasks.SaveToFile(GetTaskFileUrl(acc.AccInfo.Nickname, acc.AccInfo.ServerUrl));
            }
        }

        /// <summary>
        /// Login into account and initialize everything
        /// </summary>
        /// <param name="acc">Account</param>
        public static async Task LoginAccount(Account acc)
        {
            if (acc.IsLogged) return;

            if (acc.Wb == null)
            {
                acc.Wb = new WebBrowserInfo();
                await acc.Wb.InitSelenium(acc);
                acc.TaskTimer = new TaskTimer(acc);
            }

            if (acc.Settings.DiscordWebhook && !string.IsNullOrEmpty(acc.AccInfo.WebhookUrl))
            {
                if (acc.Settings.DiscordOnlineAnnouncement)
                {
                    DiscordHelper.SendMessage(acc, "TravianBotSharp is online now");
                }
            }

            AccountHelper.StartAccountTasks(acc);

            acc.IsLogged = true;
        }

        /// <summary>
        /// Logout from the account. Closes web driver.
        /// </summary>
        /// <param name="acc"></param>
        public static async Task Logout(Account acc)
        {
            if (!acc.IsLogged) return;
            acc.IsLogged = false;
            while (acc.Tasks?.IsTaskExcuting() ?? true)
            {
                await Task.Delay(1000);
                acc.Logger.Information("Waiting current task complete ...");
            }
            acc.Logger.Information("Logged out.");

            if (acc.TaskTimer != null)
            {
                acc.TaskTimer.Dispose();
                acc.TaskTimer = default;
            }
            if (acc.Wb != null)
            {
                acc.Wb.Dispose();
                acc.Wb = default;
            }
            acc.Tasks = default;
        }
    }
}