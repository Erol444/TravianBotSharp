using System;
using System.Linq;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Tasks.LowLevel;
using TravBotSharp.Files.Tasks.SecondLevel;

namespace TravBotSharp.Files.Helpers
{
    public static class AccountHelper
    {
        public static Village GetMainVillage(Account acc)
        {
            var main = acc.Villages.FirstOrDefault(x => x.Id == acc.Settings.MainVillage);
            // There is no main village, select it
            if (main == null)
            {
                main = acc.Villages.FirstOrDefault();
                acc.Settings.MainVillage = main?.Id ?? default;
            }
            return main;
        }
        public static Village GetHeroReviveVillage(Account acc)
        {
            var heroVill = acc.Villages.FirstOrDefault(x => x.Id == acc.Hero.ReviveInVillage);
            // There is no main village, select it
            if (heroVill == null)
            {
                heroVill = acc.Villages.FirstOrDefault();
                acc.Hero.ReviveInVillage = heroVill?.Id ?? default;
            }
            return heroVill;
        }

        public static Village GetQuestsClaimVillage(Account acc)
        {
            var questsClaimVill = acc.Villages.FirstOrDefault(x => x.Id == acc.Quests.VillToClaim);
            // There is no main village, select it
            if (questsClaimVill == null)
            {
                questsClaimVill = acc.Villages.FirstOrDefault();
                acc.Quests.VillToClaim = questsClaimVill?.Id ?? default;
            }
            return questsClaimVill;
        }

        /// <summary>
        /// Returns a random delay (click delay, ~0.5-1sec).
        /// </summary>
        /// <returns>Random delay in milliseconds</returns>
        public static int Delay()
        {
            //Return random delay
            Random rnd = new Random();
            return rnd.Next(500, 900);
        }
        public static void StartAccountTasks(Account acc)
        {
            Random ran = new Random();

            // Get the server info (on first running the account)
            if (acc.AccInfo.ServerSpeed == 0 || acc.AccInfo.MapSize == 0)
            {
                TaskExecutor.AddTaskIfNotExists(acc, new GetServerInfo() { ExecuteAt = DateTime.MinValue.AddHours(2) });
            }

            if (acc.AccInfo.Tribe == null)
            {
                TaskExecutor.AddTaskIfNotExists(acc, new GetTribe() { ExecuteAt = DateTime.MinValue.AddHours(3) });
            }

            //FL
            if (acc.Farming.Enabled) TaskExecutor.AddTaskIfNotExists(acc, new SendFLs() { ExecuteAt = DateTime.Now });

            // Bot sleep
            TaskExecutor.AddTaskIfNotExists(acc, new Sleep()
            {
                ExecuteAt = DateTime.Now + TimeHelper.GetWorkTime(acc),
                AutoSleep = true
            });

            // Access change
            var nextAccessChange = TimeHelper.GetNextProxyChange(acc);
            if (nextAccessChange != TimeSpan.MaxValue)
            {
                TaskExecutor.AddTaskIfNotExists(acc, new ChangeAccess() { ExecuteAt = DateTime.Now + nextAccessChange });
            }
            //research / improve / train troops
            foreach (var vill in acc.Villages)
            {
                //if (vill.Troops.Researched.Count == 0) TaskExecutor.AddTask(acc, new UpdateTroops() { ExecuteAt = DateTime.Now, vill = vill });
                TroopsHelper.ReStartResearchAndImprovement(acc, vill);
                if (!TroopsHelper.EverythingFilled(acc, vill)) TroopsHelper.ReStartTroopTraining(acc, vill);
                BuildingHelper.ReStartBuilding(acc, vill);
                BuildingHelper.ReStartDemolishing(acc, vill);
                MarketHelper.ReStartSendingToMain(acc, vill);
                ReStartCelebration(acc, vill);
            }

            // Hero update info
            if (acc.Hero.Settings.AutoRefreshInfo)
            {
                TaskExecutor.AddTask(acc, new HeroUpdateInfo()
                {
                    ExecuteAt = DateTime.Now.AddMinutes(ran.Next(40, 80)),
                    Priority = Tasks.BotTask.TaskPriority.Low
                });
            }
        }

        public static void ReStartCelebration(Account acc, Village vill)
        {
            // If we don't want auto-celebrations, return
            if (!vill.Expansion.AutoCelebrations) return;

            TaskExecutor.AddTaskIfNotExistInVillage(acc, vill, new Celebration()
            {
                ExecuteAt = vill.Expansion.CelebrationEnd.AddSeconds(7),
                Vill = vill,
                BigCelebration = vill.Expansion.BigCelebrations,
            });
        }
    }
}
