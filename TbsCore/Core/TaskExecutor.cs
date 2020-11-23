
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Extensions;
using TbsCore.Helpers;
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
            if (IsCaptcha(acc) || IsWWMsg(acc) || IsBanMsg(acc)) //Check if a captcha/ban/end of server
            {
                acc.Wb.Log("Captcha/WW/Ban found! Stopping bot for this account!");
                acc.TaskTimer.Stop();
                return;
            }
            if (CheckCookies(acc))
                await acc.Wb.Driver.FindElementById("CybotCookiebotDialogBodyLevelButtonLevelOptinDeclineAll").Click(acc);
            if (acc.AccInfo.Tribe == null && CheckSkipTutorial(acc))
                await acc.Wb.Driver.FindElementByClassName("questButtonSkipTutorial").Click(acc);

            if (IsLoginScreen(acc)) //Check if you are on login page -> Login task
            {
                AddTask(acc, new LoginTask() { ExecuteAt = DateTime.MinValue });
                return;
            }
            if (IsSysMsg(acc)) //Check if there is a system message (eg. Artifacts/WW plans appeared)
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php?ok");
                await Task.Delay(AccountHelper.Delay());
            }

            //TODO: limit this for performance reasons?
            PostLoadTasks(acc);
        }
        private static bool CheckSkipTutorial(Account acc) =>
            acc.Wb.Html.DocumentNode.Descendants().Any(x => x.HasClass("questButtonSkipTutorial"));

        /// <summary>
        /// Checks if account is banned (T4.5)
        /// </summary>
        private static bool IsBanMsg(Account acc) => acc.Wb.Html.GetElementbyId("punishmentMsgButtons") != null;

        /// <summary>
        /// Checks if there are cookies to be accepted
        /// </summary>
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
            
            if (acc.Wb?.CurrentUrl == null && task.GetType() != typeof(CheckProxy))
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php");
            }
            
            if (task.Vill == null) task.Vill = acc.Villages.FirstOrDefault(x => x.Active);

            try
            {
                acc.Wb.Log($"Executing task {task.GetName()}" + (task.Vill == null ? "" : $" in village {task.Vill.Name}"));

                switch (await task.Execute(acc))
                {
                    case TaskRes.Retry:
                        task.RetryCounter++;
                        if (task.NextExecute == null) task.NextExecute = DateTime.Now.AddMinutes(3);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                if (acc.Wb != null) acc.Wb.Log($"Error executing task {task.GetName()}! Vill {task.Vill?.Name}", e);
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

        /// <summary>
        /// Is called after every page load.
        /// TODO: don't execute all tasks every PostLoad due to performance?
        /// </summary>
        /// <param name="acc">Account</param>
        private static void PostLoadTasks(Account acc)
        {
            foreach (var task in GetPostLoadTasks(acc))
            {
                try
                {
                    task.Invoke();
                }
                catch (Exception e)
                {
                    acc.Wb.Log($"Error in PreTask {task.GetType()}", e);
                }
            }
        }

        /// <summary>
        /// Gets tasks that should be executed after loading a page
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>List of tasks</returns>
        private static List<Action> GetPostLoadTasks(Account acc)
        {
            var html = acc.Wb.Html;

            //Web browser not initialized
            if (!UpdateAccountObject.UpdateVillages(html, acc)) return new List<Action>();
            var activeVill = acc.Villages.FirstOrDefault(x => x.Active);

            return new List<Action>() {
                () => acc.AccInfo.ServerVersion = (acc.Wb.Html.GetElementbyId("sidebarBoxDailyquests") == null ? Classificator.ServerVersionEnum.T4_5 : Classificator.ServerVersionEnum.T4_4),
                () => {
                    if (acc.Wb.CurrentUrl.Contains("dorf1")) UpdateDorf1Info(acc);
                    else if (acc.Wb.CurrentUrl.Contains("dorf2")) UpdateDorf2Info(acc);
                },
                () => acc.AccInfo.CulturePoints = RightBarParser.GetCulurePoints(html, acc.AccInfo.ServerVersion),
                () => // Village expansion
                {
                    var villExpansionReady = acc.Villages.FirstOrDefault(x => x.Expansion.ExpansionAvailable);
                    if (acc.AccInfo.CulturePoints.MaxVillages > acc.AccInfo.CulturePoints.VillageCount &&
                        villExpansionReady != null)
                    {
                        villExpansionReady.Expansion.ExpansionAvailable = false;
                        TaskExecutor.AddTaskIfNotExists(acc, new SendSettlers() { ExecuteAt = DateTime.Now, Vill = villExpansionReady });
                    }
                },
                () => // Beginner Quests
                {
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
                },
                () => // Daily Quest
                {
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
                },
                () =>
                {
                    var goldSilver = RightBarParser.GetGoldAndSilver(html, acc.AccInfo.ServerVersion);
                    acc.AccInfo.Gold = goldSilver[0];
                    acc.AccInfo.Silver = goldSilver[1];
                },
                () => acc.AccInfo.PlusAccount = RightBarParser.HasPlusAccount(html, acc.AccInfo.ServerVersion),
                () => // Check messages
                {
                    if (MsgParser.UnreadMessages(html, acc.AccInfo.ServerVersion) > 0
                        && !acc.Wb.CurrentUrl.Contains("messages.php")
                        && acc.Settings.AutoReadIgms)
                    {
                        var ran = new Random();
                        TaskExecutor.AddTaskIfNotExists(acc, new ReadMessage()
                        {
                            ExecuteAt = DateTime.Now.AddSeconds(ran.Next(10, 600)), // Read msg in next 10-600 seconds
                            Priority = TaskPriority.Low
                        });
                    }
                },
                () => {
                    activeVill.Res.FreeCrop = RightBarParser.GetFreeCrop(html);
                    },
                () => activeVill.Res.Capacity = ResourceParser.GetResourceCapacity(html, acc.AccInfo.ServerVersion),
                () => activeVill.Res.Stored = ResourceParser.GetResources(html),
                () => activeVill.Timings.LastVillRefresh = DateTime.Now,
                () => // NPC
                {
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
                },
                () => {
                    if (acc.Settings.AutoActivateProductionBoost && CheckProductionBoost(acc))
                    {
                        TaskExecutor.AddTask(acc, new TTWarsPlusAndBoost() {
                            ExecuteAt = DateTime.Now.AddSeconds(1)
                        });
                    }
                },
                () =>
                {
                    if (activeVill.Build.InstaBuild &&
                        acc.AccInfo.Gold >= 2 &&
                        activeVill.Build.CurrentlyBuilding.Count >= (acc.AccInfo.PlusAccount ? 2 : 1) &&
                        activeVill.Build.CurrentlyBuilding.LastOrDefault().Duration
                            >= DateTime.Now.AddHours(activeVill.Build.InstaBuildHours))
                    {
                        TaskExecutor.AddTaskIfNotExistInVillage(acc, activeVill, new InstaUpgrade()
                        {
                            Vill = activeVill,
                            ExecuteAt = DateTime.Now.AddHours(-1)
                        });
                    }
                },
                () => acc.Hero.AdventureNum = HeroParser.GetAdventureNum(html, acc.AccInfo.ServerVersion),
                () => acc.Hero.Status = HeroParser.HeroStatus(html, acc.AccInfo.ServerVersion),
                () => acc.Hero.HeroInfo.Health = HeroParser.GetHeroHealth(html, acc.AccInfo.ServerVersion),
                () =>
                {
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
                        AddTaskIfNotExists(acc, new ReviveHero() { 
                            ExecuteAt = DateTime.Now.AddSeconds(5),
                            Vill = AccountHelper.GetHeroReviveVillage(acc) 
                        });
                    }
                    if (HeroParser.LeveledUp(html, acc.AccInfo.ServerVersion) && acc.Hero.Settings.AutoSetPoints)
                    {
                        AddTaskIfNotExists(acc, new HeroSetPoints() { ExecuteAt = DateTime.Now });
                    }
                }
            };
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
            var wwImg = acc.Wb.Html.DocumentNode
                .Descendants("img")
                .FirstOrDefault(x => x.GetAttributeValue("src", "") == "/img/ww100.png");

            // This image is in the natars profile as well
            return wwImg != null && !acc.Wb.CurrentUrl.EndsWith("/spieler.php?uid=1");
        }
        private static bool IsCaptcha(Account acc) => acc.Wb.Html.GetElementbyId("recaptchaImage") != null;
        
        //will be called before executing PreTaskRefresh
        internal static bool IsLoginScreen(Account acc)
        {
            var outerLoginBox = acc.Wb.Html.DocumentNode
                .Descendants("form")
                .FirstOrDefault(x => x.GetAttributeValue("name", "") == "login");

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
            acc.Tasks.Add(task);
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
        /// Removes all pending BotTasks of specific type for specific village except for the task calling it
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
        /// <summary>
        /// Removes all pending BotTasks of specific type except for the task calling it
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="type">Type of bot tasks to remove</param>
        /// <param name="thisTask">Task not to remove</param>
        public static void RemoveSameTasks(Account acc, Type type, BotTask thisTask)
        {
            acc.Tasks.RemoveAll(x =>
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
