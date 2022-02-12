﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using OpenQA.Selenium;

using TbsCore.Extensions;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.JsObjects;
using TbsCore.Models.VillageModels;
using TbsCore.Parsers;
using TbsCore.Tasks;
using TbsCore.Tasks.LowLevel;
using static TbsCore.Tasks.BotTask;

namespace TbsCore.Helpers
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
            if (IsCaptcha(acc) || IsWWMsg(acc) || IsBanMsg(acc) || IsMaintanance(acc)) //Check if a captcha/ban/end of server/maintanance
            {
                acc.Logger.Warning("Captcha/WW/Ban/Maintanance found! Stopping bot for this account!");
                acc.TaskTimer.Stop();
                return;
            }
            if (CheckCookies(acc))
                await DriverHelper.ExecuteScript(acc, "document.getElementById('CybotCookiebotDialogBodyLevelButtonLevelOptinDeclineAll').click();");
            if (CheckCookiesNew(acc))
                await DriverHelper.ExecuteScript(acc, "document.getElementsByClassName('cmpboxbtnyes')[0].click();");

            if (CheckContextualHelp(acc) &&
                acc.AccInfo.ServerVersion == Classificator.ServerVersionEnum.T4_5)
            {
                acc.Tasks.Add(new EditPreferences()
                {
                    ExecuteAt = DateTime.Now.AddHours(-1),
                    TroopsPerPage = 99,
                    ContextualHelp = true
                }, true);
            }

            if (acc.AccInfo.Tribe == null && CheckSkipTutorial(acc)) await DriverHelper.ClickByClassName(acc, "questButtonSkipTutorial");

            if (IsLoginScreen(acc)) //Check if you are on login page -> Login task
            {
                acc.Tasks.Add(new LoginTask() { ExecuteAt = DateTime.MinValue });
                return;
            }

            if (IsSysMsg(acc)) //Check if there is a system message (eg. Artifacts/WW plans appeared)
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php?ok=1");
                await Task.Delay(AccountHelper.Delay(acc));
            }

            //TODO: limit this for performance reasons?
            PostLoadTasks(acc);
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
            // Before every execution, wait a random delay
            await Task.Delay(AccountHelper.Delay(acc));

            if (task.Vill == null) task.Vill = acc.Villages.FirstOrDefault(x => x.Active);

            acc.Logger.Information($"Executing task {task.GetName()}" + (task.Vill == null ? "" : $" in village {task.Vill.Name}"));

            try
            {
                switch (await task.Execute(acc))
                {
                    case TaskRes.Retry:
                        task.RetryCounter++;
                        if (task.NextExecute == null) task.NextExecute = DateTime.Now.AddMinutes(3);
                        break;

                    default:
                        task.RetryCounter = 0;
                        if (task.NextTask != null)
                        {
                            task.NextTask.ExecuteAt = DateTime.MinValue.AddHours(5);
                            task.NextTask.Stage = TaskStage.Start;
                            acc.Tasks.Add(task.NextTask);
                            task.NextTask = null;
                        }
                        break;
                }
            }
            catch (WebDriverException e) when (e.Message.Contains("chrome not reachable") || e.Message.Contains("no such window:"))
            {
                acc.Logger.Warning($"Chrome has problem while executing task {task.GetName()}! Vill {task.Vill?.Name}. Try reopen Chrome");

                acc.Tasks.Add(new ReopenDriver()
                {
                    ExecuteAt = DateTime.MinValue,
                    Priority = TaskPriority.High,
                    ReopenAt = DateTime.MinValue
                });

                //try exccute task after we reopen chrome 1 mintues

                if (task.NextExecute == null) task.NextExecute = DateTime.MinValue.AddMinutes(1); // make sure current task is excuted after reopen driver
            }
            catch (Exception e)
            {
                acc.Logger.Error(e, $"Error executing task {task.GetName()}! Vill {task.Vill?.Name}");
                task.RetryCounter++;
                if (task.NextExecute == null) task.NextExecute = DateTime.Now.AddMinutes(3);
            }

            //We want to re-execute the same task later
            if (task.NextExecute != null && task.RetryCounter < 3)
            {
                task.ExecuteAt = task.NextExecute ?? default;
                task.NextExecute = null;
                acc.Tasks.ReOrder();

                task.Stage = TaskStage.Start;
                acc.Logger.Warning($"Task {task.GetName()}" + (task.Vill == null ? "" : $" in village {task.Vill.Name} will be re-executed at {task.ExecuteAt}"));
                return;
            }
            // Remove the task from the task list
            acc.Tasks.Remove(task);
            if (task.RetryCounter >= 3)
            {
                acc.Logger.Warning($"Task {task.GetName()}" + (task.Vill == null ? "" : $" in village {task.Vill.Name} is already re-executed 3 times. Ignore it"));
            }
            else
            {
                acc.Logger.Information($"Task {task.GetName()}" + (task.Vill == null ? "" : $" in village {task.Vill.Name} is done."));
            }
        }

        /// <summary>
        /// Is called after every page load.
        /// TODO: don't execute all tasks every PostLoad due to performance?
        /// </summary>
        /// <param name="acc">Account</param>
        private static void PostLoadTasks(Account acc)
        {
            var tasks = PostLoadHelper.GetPostLoadTasks(acc);
            for (int i = 0; i < tasks.Count; i++)
            {
                try
                {
                    tasks[i].Invoke();
                }
                catch (Exception e)
                {
                    acc.Logger.Error(e, $"Error executing pre-task {PostLoadHelper.namePostTask[i]}!");
                }
            }
        }

        public static void UpdateDorf2Info(Account acc)
        {
            //update buildings, currentlyBuilding, resources, capacity
            var vill = acc.Villages.FirstOrDefault(x => x.Active);
            if (vill == null) return;

            //remove any further UpdateDorf2 BotTasks for this village (if below 5min)
            acc.Tasks.Remove(typeof(UpdateDorf2), vill, 5);

            UpdateCurrentlyBuilding(acc, vill);

            var buildings = InfrastructureParser.GetBuildings(acc, acc.Wb.Html);
            foreach (var field in buildings)
            {
                var building = vill.Build.Buildings.FirstOrDefault(x => x.Id == field.Id);
                building.Level = field.Level;
                building.Type = field.Type;
                building.UnderConstruction = field.UnderConstruction;
            }
        }

        public static void UpdateDorf1Info(Account acc)
        {
            var vill = acc.Villages.FirstOrDefault(x => x.Active);
            if (vill == null) return;

            //remove any further UpdateDorf1 BotTasks for this village (if below 5min)
            acc.Tasks.Remove(typeof(UpdateDorf1), vill, 5);

            UpdateCurrentlyBuilding(acc, vill);

            var dorf1Movements = TroopsMovementParser.ParseDorf1Movements(acc.Wb.Html);

            // Check attacks if there are incoming attacks and alerts aren't disabled and task isn't already on task list
            if (dorf1Movements.Any(x => x.Type == Classificator.MovementTypeDorf1.IncomingAttack) &&
                vill.Deffing.AlertType != Models.VillageModels.AlertTypeEnum.Disabled)
            {
                acc.Tasks.Add(new CheckAttacks()
                {
                    ExecuteAt = DateTime.Now,
                    Priority = TaskPriority.High
                }, true, vill);
            }
            vill.TroopMovements.Dorf1Movements = dorf1Movements;

            var resFields = ResourceParser.GetResourcefields(acc.Wb.Html, acc.AccInfo.ServerVersion);
            foreach (var field in resFields)
            {
                var building = vill.Build.Buildings.FirstOrDefault(x => x.Id == field.Id);
                building.Level = field.Level;
                building.Type = field.Type;
                building.UnderConstruction = field.UnderConstruction;
            }
        }

        private static void UpdateCurrentlyBuilding(Account acc, Village vill)
        {
            vill.Build.CurrentlyBuilding.Clear();
            var cb = InfrastructureParser.CurrentlyBuilding(acc.Wb.Html, acc);
            if (cb == null) return; // Nothing is currently building

            for (int i = 0; i < cb.Count; i++)
            {
                vill.Build.CurrentlyBuilding.Add(cb[i]);
            }
        }

        #region Game checks

        private static bool IsWWMsg(Account acc)
        {
            var wwImg = acc.Wb.Html.DocumentNode
                .Descendants("img")
                .FirstOrDefault(x => x.GetAttributeValue("src", "") == "/img/ww100.png");

            // This image is in the natars profile as well
            return wwImg != null && !acc.Wb.CurrentUrl.EndsWith("/spieler.php?uid=1");
        }

        private static bool CheckSkipTutorial(Account acc) =>
            acc.Wb.Html.DocumentNode.Descendants().Any(x => x.HasClass("questButtonSkipTutorial"));

        private static bool CheckContextualHelp(Account acc) =>
            acc.Wb.Html.GetElementbyId("contextualHelp") != null;

        /// <summary>
        /// Checks if account is banned (T4.5)
        /// </summary>
        private static bool IsBanMsg(Account acc) => acc.Wb.Html.GetElementbyId("punishmentMsgButtons") != null;

        /// <summary>
        /// Checks whether there is an ongoing maintanance
        /// </summary>
        private static bool IsMaintanance(Account acc) => acc.Wb.Html.DocumentNode.Descendants("img").Any(x => x.HasClass("fatalErrorImage"));

        /// <summary>
        /// Checks if there are cookies to be accepted
        /// </summary>
        private static bool CheckCookies(Account acc) =>
            acc.Wb.Html.GetElementbyId("CybotCookiebotDialogBodyLevelButtonLevelOptinDeclineAll") != null;

        private static bool CheckCookiesNew(Account acc) =>
            acc.Wb.Html.DocumentNode.Descendants("a").Any(x => x.HasClass("cmpboxbtn") && x.HasClass("cmpboxbtnyes"));

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

        #endregion Game checks
    }
}