using System;
using System.Linq;
using TravBotSharp.Files.Models.AccModels;
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

        /// <summary>
        /// Returns a random delay.
        /// </summary>
        /// <returns></returns>
        internal static int Delay()
        {
            //Return random delay
            Random rnd = new Random();
            return rnd.Next(500, 900);
        }
        public static void StartAccountTasks(Account acc)
        {
            // If we don't know server speed, go and get it
            if (acc.AccInfo.ServerSpeed == 0) TaskExecutor.AddTaskIfNotExists(acc, new GetServerSpeed() { ExecuteAt = DateTime.MinValue.AddHours(2) });
            if (acc.AccInfo.MapSize == 0) TaskExecutor.AddTaskIfNotExists(acc, new GetMapSize() { ExecuteAt = DateTime.MinValue.AddHours(2) });
            //FL
            if (acc.Farming.Enabled) TaskExecutor.AddTaskIfNotExists(acc, new SendFLs() { ExecuteAt = DateTime.Now });

            TaskExecutor.AddTaskIfNotExists(acc, new Sleep() { ExecuteAt = DateTime.Now + TimeHelper.GetWorkTime(acc) });

            //research / improve / train troops
            foreach (var vill in acc.Villages)
            {
                //if (vill.Troops.Researched.Count == 0) TaskExecutor.AddTask(acc, new UpdateTroops() { ExecuteAt = DateTime.Now, vill = vill });
                TroopsHelper.ReStartResearchAndImprovement(acc, vill);
                if (!TroopsHelper.EverythingFilled(acc, vill)) TroopsHelper.ReStartTroopTraining(acc, vill);
                BuildingHelper.ReStartBuilding(acc, vill);
                BuildingHelper.ReStartDemolishing(acc, vill);
                MarketHelper.ReStartSendingToMain(acc, vill);
                //todo
            }
        }
    }
}
