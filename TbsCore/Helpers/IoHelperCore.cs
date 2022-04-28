using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.Webhook;
using TbsCore.Database;
using TbsCore.Models.Access;
using TbsCore.Models.Logging;
using TbsCore.Models.AccModels;
using TbsCore.Models.BuildingModels;
using TbsCore.Models.VillageModels;

using TbsCore.Tasks;
using static TbsCore.Models.TB;

namespace TbsCore.Helpers
{
    public static class IoHelperCore
    {
        public static string TbsPath => Path.Combine(AppContext.BaseDirectory, "Data");
        public static string SqlitePath => Path.Combine(TbsPath, "db.sqlite");
        public static string UseragentPath => Path.Combine(TbsPath, "useragent.json");

        public static string UserDataPath(string username, string server) => Path.Combine(TbsPath, UrlRemoveHttp(server), username);

        public static string UserTaskPath(string username, string server) => Path.Combine(UserDataPath(username, server), "tasks.json");

        public static string UserCachePath(string username, string server) => Path.Combine(UserDataPath(username, server), "Cache");

        public static string UserCachePath(string username, string server, string host) => Path.Combine(UserCachePath(username, server), string.IsNullOrWhiteSpace(host) ? "default" : host);

        public static bool SQLiteExists() => File.Exists(SqlitePath);

        public static bool UserAgentExists() => File.Exists(UseragentPath);

        public static bool UserTaskExists(string username, string server) => File.Exists(UserTaskPath(username, server));

        public static bool UserDataExists(string username, string server) => Directory.Exists(UserDataPath(username, server));

        public static void CreateUserData(string username, string server) => Directory.CreateDirectory(UserDataPath(username, server));

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
                UpgradeBuildingHelper.AddBuildingTask(acc, vill, task);
            }
            UpgradeBuildingHelper.RemoveCompletedTasks(vill);
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
            var userCacheFolder = UserCachePath(acc.AccInfo.Nickname, acc.AccInfo.ServerUrl);
            if (!UserDataExists(acc.AccInfo.Nickname, acc.AccInfo.ServerUrl)) return;

            try
            {
                var removeFolders = Directory.GetDirectories(userCacheFolder);

                for (int i = 0; i < removeFolders.Count(); i++)
                {
                    Directory.Delete(removeFolders[i], true);
                }
            }
            catch { }
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
        /// Saves accounts into the SQLite DB
        /// </summary>
        /// <param name="accounts"></param>
        public static void SaveAccounts(List<Account> accounts)
        {
            foreach (var acc in accounts)
            {
                acc.Tasks.Save();
                DbRepository.SaveAccount(acc);
            }
        }

        /// <summary>
        /// Login into account and initialize everything
        /// </summary>
        /// <param name="acc">Account</param>
        public static async Task<bool> Login(Account acc)
        {
            var opened = await acc.Wb.Init(acc);
            if (!opened)
            {
                acc.Logger.Warning("Cannot open browser. Check warning below");
                return false;
            }
            AccountHelper.StartAccountTasks(acc);
            acc.TaskTimer.Start();

            return true;
        }

        /// <summary>
        /// Logout from the account. Closes web driver.
        /// </summary>
        /// <param name="acc"></param>
        public static async Task Logout(Account acc)
        {
            acc.TaskTimer.Stop();
            await acc.TaskTimer.WaitStop();
            acc.Status = Status.Stopping;
            acc.Wb.Close();
            acc.Status = Status.Offline;
        }
    }
}