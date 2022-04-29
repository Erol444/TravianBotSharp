using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TbsCore.Tasks.Browser;
using TbsCore.Tasks.LowLevel;
using TbsCore.Tasks.SecondLevel;

namespace TbsCore.Helpers
{
    public static class AccountHelper
    {
        private static readonly Random rnd = new Random();

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
        /// Returns a random
        ///
        /// (click delay, ~0.5-1.6sec).
        /// </summary>
        /// <returns>Random delay in milliseconds</returns>
        public static int Delay(Account acc) => rnd.Next(acc.Settings.DelayClickingMin, acc.Settings.DelayClickingMax);

        public static Task DelayWait(Account acc) => Task.Delay(Delay(acc));

        public static void StartAccountTasks(Account acc)
        {
            // Get the server info (on first running the account)
            if (acc.AccInfo.ServerSpeed == 0 || acc.AccInfo.MapSize == 0)
            {
                acc.Tasks.Add(new GetServerInfo() { ExecuteAt = DateTime.MinValue.AddHours(2) }, true);
            }

            if (acc.AccInfo.Tribe == null)
            {
                acc.Tasks.Add(new GetTribe() { ExecuteAt = DateTime.MinValue.AddHours(3) }, true);
            }

            //FL
            if (acc.Farming.Enabled) acc.Tasks.Add(new SendFLs() { ExecuteAt = DateTime.Now }, true);

            // Bot sleep
            acc.Tasks.Add(new TimeSleep()
            {
                ExecuteAt = DateTime.Now + TimeHelper.GetWorkTime(acc),
            }, true);

            // Access change
            var nextAccessChange = TimeHelper.GetNextProxyChange(acc);
            if (nextAccessChange != TimeSpan.MaxValue)
            {
                acc.Tasks.Add(new ChangeAccess() { ExecuteAt = DateTime.Now + nextAccessChange }, true);
            }
            //research / improve / train troops
            foreach (var vill in acc.Villages)
            {
                //if (vill.Troops.Researched.Count == 0) acc.Tasks.Add( new UpdateTroops() { ExecuteAt = DateTime.Now, vill = vill });
                TroopsHelper.ReStartResearchAndImprovement(acc, vill);
                TroopsHelper.ReStartTroopTraining(acc, vill);
                UpgradeBuildingHelper.ReStartBuilding(acc, vill);
                BuildingHelper.ReStartDemolishing(acc, vill);
                MarketHelper.ReStartSendingToMain(acc, vill);
                ReStartCelebration(acc, vill);
                VillageHelper.SetNextRefresh(acc, vill);
                if (vill.FarmingNonGold.OasisFarmingEnabled)
                {
                    acc.Tasks.Add(new AttackOasis() { Vill = vill }, true, vill);
                }

                // Remove in later updates!
                if (vill.Settings.RefreshMin == 0) vill.Settings.RefreshMin = 30;
                if (vill.Settings.RefreshMax == 0) vill.Settings.RefreshMax = 60;
            }
            // Remove in later updates!
            if (acc.Hero.Settings.MinUpdate == 0) acc.Hero.Settings.MinUpdate = 40;
            if (acc.Hero.Settings.MaxUpdate == 0) acc.Hero.Settings.MaxUpdate = 80;

            // Hero update info
            if (acc.Hero.Settings.AutoRefreshInfo)
            {
                Random ran = new Random();
                acc.Tasks.Add(new HeroUpdateInfo()
                {
                    ExecuteAt = DateTime.Now.AddMinutes(ran.Next(40, 80)),
                    Priority = Tasks.BotTask.TaskPriority.Low
                }, true);
            }
        }

        public static void ReStartCelebration(Account acc, Village vill)
        {
            // If we don't want auto-celebrations, return
            if (vill.Expansion.Celebrations == CelebrationEnum.None) return;

            acc.Tasks.Add(new Celebration()
            {
                ExecuteAt = vill.Expansion.CelebrationEnd.AddSeconds(7),
                Vill = vill
            }, true, vill);
        }
    }
}