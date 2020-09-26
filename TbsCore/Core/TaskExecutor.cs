
using Elasticsearch.Net;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.LowLevel;
using static TravBotSharp.Files.Tasks.BotTask;

namespace TravBotSharp.Files.Helpers
{
    public static class TaskExecutor
    {
        /// <summary>
        /// Is called whenever a web browser loaded a new page.
        /// Usage: After navigation (has to execute) and after execution (click on button etc.)
        /// In first case execute the task, in second remove it.
        /// </summary>
        /// <param name="acc"></param>
        public static async Task PageLoaded(Account acc)
        {
            if (IsCaptcha(acc) || IsWWMsg(acc)) //Check if a captcha is detected or WW there is a WW msg
            {
                acc.TaskTimer.Stop();
                return;
            }
            if (CheckCookies(acc))
            {
                acc.Wb.Driver.ExecuteScript("document.getElementById('CybotCookiebotDialogBodyLevelButtonLevelOptinDeclineAll').click()");
            }
            if (IsLoginScreen(acc)) //Check if you are on login page -> Login task
            {
                var login = new LoginTask();
                await login.Execute(acc);
            }
            if (IsSysMsg(acc)) //Check if there is a system message (eg. Artifacts/WW plans appeared)
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php?ok");
                await Task.Delay(AccountHelper.Delay());
            }

            //TODO: limit this for performance reasons?
            PreTaskRefresh(acc);
        }

        /// <summary>
        /// Checks if there are cookies to be accepted
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>Whether there are cookies to be accepted</returns>
        private static bool CheckCookies(Account acc) =>
            acc.Wb.Html.GetElementbyId("CybotCookiebotDialogBodyLevelButtonLevelOptinDeclineAll") != null;

        /// <summary>
        /// Invoke all PostTasks that a task might have.
        /// </summary>
        /// <param name="task">The task</param>
        /// <param name="acc">Account</param>
        private static void PostTask(BotTask task, Account acc)
        {
            if (task.PostTaskCheck != null)
            {
                foreach (var postTask in task.PostTaskCheck)
                {
                    postTask.Invoke(acc.Wb.Html, acc);
                }
                task.PostTaskCheck.Clear();
            }

        }

        /// <summary>
        /// Called PageLoaded (after navigating to a specific url) or from
        /// Task timer, if there is no url/bot is already on the url
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="task">Task to be executed</param>
        /// <returns></returns>
        public static async Task Execute(Account acc, BotTask task)
        {
            //Before every execution, wait a random delay. TODO: needed?
            if (task.PostTaskCheck == null) task.PostTaskCheck = new List<Action<HtmlDocument, Account>>();
            if (acc.Wb?.CurrentUrl == null &&
                task.GetType() != typeof(CheckProxy))
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php");
            }
            task.ErrorMessage = null;
            //Console.WriteLine($"Executing task {task.GetType()}");
            if (task.Vill == null) task.Vill = acc.Villages.FirstOrDefault(x => x.Active);
            try
            {
                switch (await task.Execute(acc))
                {
                    case TaskRes.Retry:
                        if (task.ErrorMessage != null)
                        {
                            Utils.log.Warning(LogHelper(acc, task, "warning") + "\n" + task.ErrorMessage);
                        }

                        // There was probably a problem, retry executing the task later.
                        task.RetryCounter++;
                        if (task.NextExecute == null) task.NextExecute = DateTime.Now.AddMinutes(3);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Utils.log.Error(LogHelper(acc, task, "error") + $"\nStack Trace:\n{e.StackTrace}\n\nMessage:" + e.Message + "\n------------------------\n");
                task.RetryCounter++;
                if (task.NextExecute == null) task.NextExecute = DateTime.Now.AddMinutes(3);
            }

            // Execute post tasks
            PostTask(task, acc);

            //We want to re-execute the same task later
            if (task.NextExecute != null && task.RetryCounter < 3)
            {
                task.ExecuteAt = task.NextExecute ?? default;
                task.NextExecute = null;
                task.Stage = TaskStage.Start;
                task.RetryCounter = 0;
                ReorderTaskList(acc);
                return;
            }
            // Remove the task from the task list
            acc.Tasks.Remove(task);
        }

        private static string LogHelper(Account acc, BotTask task, string type)
        {
            var msg = $"Account {acc.AccInfo.Nickname}, \nserver {acc.AccInfo.ServerUrl}, \ncurrent url {acc.Wb.CurrentUrl}\n";
            return msg + $"Task: {task.GetType()}, village {task.Vill?.Name} encountered a {type}";
        }

        /// <summary>
        /// Will be called before each task to update resources/msg?,villages,quests,hero health, adventures num, gold/silver
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>True if successful, false if error</returns>
        private static bool PreTaskRefresh(Account acc)
        {
            var html = acc.Wb.Html;
            try
            {
                //check & update dorf1/dorf2
                if (!UpdateAccountObject.UpdateVillages(html, acc)) return false; //Web browser not initialized
                var activeVill = acc.Villages.FirstOrDefault(x => x.Active);

                //update server version
                acc.AccInfo.ServerVersion = (acc.Wb.Html.GetElementbyId("sidebarBoxDailyquests") == null ? Classificator.ServerVersionEnum.T4_5 : Classificator.ServerVersionEnum.T4_4);

                //update dorf1/dorf2
                if (acc.Wb.CurrentUrl.Contains("dorf1")) UpdateDorf1Info(acc);
                else if (acc.Wb.CurrentUrl.Contains("dorf2")) UpdateDorf2Info(acc);


                acc.AccInfo.CulturePoints = RightBarParser.GetCulurePoints(html, acc.AccInfo.ServerVersion);

                var villExpansionReady = acc.Villages.FirstOrDefault(x => x.Expansion.ExpensionAvailable);
                if (acc.AccInfo.CulturePoints.MaxVillages > acc.AccInfo.CulturePoints.VillageCount &&
                    villExpansionReady != null)
                {
                    villExpansionReady.Expansion.ExpensionAvailable = false;
                    TaskExecutor.AddTaskIfNotExists(acc, new SendSettlers() { ExecuteAt = DateTime.Now, Vill = villExpansionReady });
                }
                // Beginner Quests
                acc.Quests.Quests = RightBarParser.GetBeginnerQuests(html, acc.AccInfo.ServerVersion);
                var claimQuest = acc.Quests?.Quests?.FirstOrDefault(x => x.finished);
                if (claimQuest != null
                    && acc.Quests.ClaimBeginnerQuests
                    )
                {
                    TaskExecutor.AddTaskIfNotExists(acc, new ClaimBeginnerTask()
                    {
                        ExecuteAt = DateTime.Now,
                        QuestToClaim = claimQuest,
                        Vill = VillageHelper.VillageFromId(acc, acc.Quests.VillToClaim)
                    });
                }
                // Daily quest
                if (acc.AccInfo.ServerVersion == Classificator.ServerVersionEnum.T4_5 &&
                    RightBarParser.CheckDailyQuest(html) &&
                    acc.Quests.ClaimDailyQuests)
                {
                    TaskExecutor.AddTaskIfNotExists(acc, new ClaimDailyTask()
                    {
                        ExecuteAt = DateTime.Now,
                        Vill = VillageHelper.VillageFromId(acc, acc.Quests.VillToClaim)
                    });
                }


                var goldSilver = RightBarParser.GetGoldAndSilver(html, acc.AccInfo.ServerVersion);
                acc.AccInfo.Gold = goldSilver[0];
                acc.AccInfo.Silver = goldSilver[1];
                acc.AccInfo.PlusAccount = RightBarParser.HasPlusAccount(html, acc.AccInfo.ServerVersion);
                //Check reports/msg count
                if (MsgParser.UnreadMessages(html, acc.AccInfo.ServerVersion) > 0
                    && !acc.Wb.CurrentUrl.Contains("messages.php")
                    && acc.Settings.AutoReadIgms)
                {
                    var ran = new Random();
                    TaskExecutor.AddTaskIfNotExists(acc, new ReadMessage() {
                        ExecuteAt = DateTime.Now.AddSeconds(ran.Next(10, 600)), // Read msg in next 10-600 seconds
                        Priority = TaskPriority.Low
                    });
                }

                //update loyalty of village


                activeVill.Res.FreeCrop = RightBarParser.GetFreeCrop(html);
                activeVill.Res.Capacity = ResourceParser.GetResourceCapacity(html, acc.AccInfo.ServerVersion);
                activeVill.Res.Stored = ResourceParser.GetResources(html);
                activeVill.Timings.LastVillRefresh = DateTime.Now;

                float ratio = (float)activeVill.Res.Stored.Resources.Crop / activeVill.Res.Capacity.GranaryCapacity;
                if (ratio >= 0.99 &&
                    acc.AccInfo.Gold >= 3 &&
                    activeVill.Market.Npc.Enabled &&
                    (activeVill.Market.Npc.NpcIfOverflow || !MarketHelper.NpcWillOverflow(activeVill)))
                {  //npc crop!
                    TaskExecutor.AddTaskIfNotExistInVillage(acc, activeVill, new NPC()
                    {
                        ExecuteAt = DateTime.MinValue,
                        Vill = activeVill
                    });
                }
                if (acc.Settings.AutoActivateProductionBoost && CheckProductionBoost(acc)) { TaskExecutor.AddTask(acc, new TTWarsPlusAndBoost() { ExecuteAt = DateTime.Now.AddSeconds(1) }); }

                acc.Hero.AdventureNum = HeroParser.GetAdventureNum(html, acc.AccInfo.ServerVersion);
                acc.Hero.Status = HeroParser.HeroStatus(html, acc.AccInfo.ServerVersion);
                acc.Hero.HeroInfo.Health = HeroParser.GetHeroHealth(html, acc.AccInfo.ServerVersion);

                bool heroReady = (acc.Hero.HeroInfo.Health > acc.Hero.Settings.MinHealth &&
                    acc.Hero.Settings.AutoSendToAdventure &&
                    acc.Hero.Status == Hero.StatusEnum.Home &&
                    acc.Hero.NextHeroSend < DateTime.Now);
                // Update adventures
                if (heroReady
                    && (acc.Hero.AdventureNum != acc.Hero.Adventures.Count() || HeroHelper.AdventureInRange(acc))
                    ) //update adventures
                {
                    AddTaskIfNotExists(acc, new StartAdventure() { ExecuteAt = DateTime.Now.AddSeconds(10) });
                }
                if (acc.Hero.AdventureNum == 0 && acc.Hero.Settings.BuyAdventures) //for UNL servers, buy adventures
                {
                    AddTaskIfNotExists(acc, new TTWarsBuyAdventure() { ExecuteAt = DateTime.Now.AddSeconds(5) });
                }
                if (acc.Hero.Status == Hero.StatusEnum.Dead && acc.Hero.Settings.AutoReviveHero) //if hero is dead, revive him
                {
                    AddTaskIfNotExists(acc, new ReviveHero() { ExecuteAt = DateTime.Now.AddSeconds(5), Vill = AccountHelper.GetHeroReviveVillage(acc) });
                }
                if (HeroParser.LeveledUp(html, acc.AccInfo.ServerVersion) && acc.Hero.Settings.AutoSetPoints)
                {
                    AddTaskIfNotExists(acc, new HeroSetPoints() { ExecuteAt = DateTime.Now });
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in PreTask " + e.Message + "\n\nStack Trace: " + e.StackTrace + "\n-----------------------");
                return false;
            }
        }

        public static void UpdateDorf2Info(Account acc)
        {
            //update buildings, currentlyBuilding, resources, capacity
            var activeVill = acc.Villages.FirstOrDefault(x => x.Active);
            if (activeVill == null) return;

            //remove any further UpdateDorf1 BotTasks for this village (if below 5min)
            acc.Tasks.RemoveAll(x =>
                x.GetType() == typeof(UpdateDorf2) &&
                x.Vill == activeVill &&
                x.ExecuteAt < DateTime.Now.AddMinutes(5)
            );

            UpdateCurrentlyBuilding(acc, activeVill);

            var buildings = InfrastructureParser.GetBuildings(acc, acc.Wb.Html);
            foreach (var field in buildings)
            {
                var building = activeVill.Build.Buildings.FirstOrDefault(x => x.Id == field.Id);
                building.Level = field.Level;
                building.Type = field.Type;
                building.UnderConstruction = field.UnderConstruction;
            }
        }

        private static void UpdateCurrentlyBuilding(Account acc, Village vill)
        {
            vill.Build.CurrentlyBuilding.Clear();
            var currentlyb = InfrastructureParser.CurrentlyBuilding(acc.Wb.Html, acc);
            if (currentlyb != null)
                foreach (var b in currentlyb) vill.Build.CurrentlyBuilding.Add(b);
        }

        public static void UpdateDorf1Info(Account acc)
        {
            var activeVill = acc.Villages.FirstOrDefault(x => x.Active);
            if (activeVill == null) return;

            //remove any further UpdateDorf1 BotTasks for this village (if below 5min)
            acc.Tasks.RemoveAll(x =>
                x.GetType() == typeof(UpdateDorf1) &&
                x.Vill == activeVill &&
                x.ExecuteAt < DateTime.Now.AddMinutes(5)
            );

            UpdateCurrentlyBuilding(acc, activeVill);

            activeVill.Res.Production = ResourceParser.GetProduction(acc.Wb.Html);

            var resFields = ResourceParser.GetResourcefields(acc.Wb.Html, acc.AccInfo.ServerVersion);
            foreach (var field in resFields)
            {
                var building = activeVill.Build.Buildings.FirstOrDefault(x => x.Id == field.Id);
                building.Level = field.Level;
                building.Type = field.Type;
                building.UnderConstruction = field.UnderConstruction;
            }
        }
        private static bool IsWWMsg(Account acc)
        {
            var wwImg = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.GetAttributeValue("src", "") == "/img/ww100.png");
            // This image is in the natars profile as well
            return wwImg != null && !acc.Wb.CurrentUrl.EndsWith("/spieler.php?uid=1");
        }
        private static bool IsCaptcha(Account acc)
        {
            var captcha = acc.Wb.Html.GetElementbyId("recaptchaImage");
            return captcha != null;
        }
        //will be called before executing PreTaskRefresh
        internal static bool IsLoginScreen(Account acc)
        {
            var outerLoginBox = acc.Wb.Html.DocumentNode.Descendants("form").FirstOrDefault(x => x.GetAttributeValue("name", "") == "login");
            if (outerLoginBox != null)
            {
                if (!IsCaptcha(acc)) return true;
            }
            return false;
        }
        private static bool IsSysMsg(Account acc)
        { //End of server/gold promotions/arts
            var msg = acc.Wb.Html.GetElementbyId("sysmsg");
            return msg != null;
        }

        public static void AddTask(Account acc, BotTask task)
        {
            //acc.Tasks.FirstOrDefault(x=>x.GetType == task.GetType)
            acc.Tasks.Add(task);
            //Console.WriteLine($"{DateTime.Now.ToString()}] Adding task {task.GetType()} for village {task.vill?.Name}, will get executed in {(task.ExecuteAt - DateTime.Now).TotalSeconds}s");
            ReorderTaskList(acc);
        }
        public static void AddTask(Account acc, List<BotTask> tasks)
        {
            foreach (var task in tasks)
            {
                acc.Tasks.Add(task);
            }
            ReorderTaskList(acc);
        }
        public static void ReorderTaskList(Account acc)
        {
            acc.Tasks = acc.Tasks.OrderBy(x => x.ExecuteAt).ToList();
        }
        public static void AddTaskIfNotExists(Account acc, BotTask task)
        {
            if (!acc.Tasks.Any(x => x.GetType() == task.GetType()))
                AddTask(acc, task);
        }
        public static void AddTaskIfNotExistInVillage(Account acc, Village vill, BotTask task)
        {
            var taskExists = acc.Tasks.Any(x =>
                        x.GetType() == task.GetType() &&
                        x.Vill == vill
                    );
            if (!taskExists)
            {
                AddTask(acc, task);
            }
        }
        /// <summary>
        /// Removes all pending BotTasks of specific type for specific village except for the task that called it
        /// Called by UpdateDorf1/2 since they are called a lot.
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="vill"></param>
        /// <param name="type"></param>
        /// <param name="thisTask"></param>
        public static void RemoveSameTasksForVillage(Account acc, Village vill, Type type, BotTask thisTask)
        {
            acc.Tasks.RemoveAll(x =>
                x.Vill == vill &&
                x.GetType() == type &&
                x != thisTask
            );
        }
        public static bool CheckProductionBoost(Account acc)
        {
            var boostProduction = acc.Wb.Html.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("productionBoostButton"));
            return (boostProduction != null);
        }
    }
}
