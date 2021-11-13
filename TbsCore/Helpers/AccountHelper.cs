using Discord.Webhook;
using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.AccModels;
using TbsCore.Models.Logging;
using TbsCore.Models.VillageModels;
using TbsCore.Tasks.LowLevel;
using TbsCore.Tasks.SecondLevel;

namespace TbsCore.Helpers
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
        /// Returns a random delay (click delay, ~0.5-1.6sec).
        /// </summary>
        /// <returns>Random delay in milliseconds</returns>
        public static int Delay(Account acc)
        {
            //Return random delay
            Random rnd = new Random();
            return rnd.Next(acc.Settings.DelayClickingMin, acc.Settings.DelayClickingMax);
        }

        public static void StartAccountTasks(Account acc)
        {
            // Get the server info (on first running the account)
            if (acc.AccInfo.ServerSpeed == 0 || acc.AccInfo.MapSize == 0)
            {
                acc.Tasks.Add(new GetServerInfo()
                {
                    ExecuteAt = DateTime.MinValue.AddHours(2)
                }, true);
            }

            if (acc.AccInfo.Tribe == null)
            {
                acc.Tasks.Add(new GetTribe()
                {
                    ExecuteAt = DateTime.MinValue.AddHours(3)
                }, true);
            }

            // Access change
            var nextAccessChange = TimeHelper.GetNextProxyChange(acc);
            if (nextAccessChange != TimeSpan.MaxValue)
            {
                acc.Tasks.Add(new ChangeAccess()
                {
                    ExecuteAt = DateTime.Now + nextAccessChange
                }, true);
            }

            var rand = new Random();
            var vills = acc.Villages.OrderBy((item) => rand.Next());
            foreach (var vill in vills)
            {
                // update info
                var min = vill.Settings.RefreshMin * 60;
                var max = vill.Settings.RefreshMax * 60;
                int timeUpdate;
                var task = acc.Tasks?.FindTask(typeof(UpdateDorf1), vill);
                if (task == null)
                {
                    timeUpdate = rand.Next(min, max);
                    acc.Tasks.Add(new UpdateDorf1()
                    {
                        ExecuteAt = DateTime.Now.AddSeconds(timeUpdate),
                        Vill = vill,
                    }, true, vill);
                }
                // this is for task delay, i will add this in next time ~ VINAGHOST
                min = 0;
                max = 3;
                // building
                task = acc.Tasks?.FindTask(typeof(UpgradeBuilding), vill);
                if (task == null)
                {
                    if (vill.Build.Tasks.Count > 0)
                    {
                        timeUpdate = rand.Next(min, max);

                        acc.Tasks.Add(new UpgradeBuilding()
                        {
                            ExecuteAt = DateTime.Now.AddMilliseconds(timeUpdate),
                            Vill = vill,
                        }, true, vill);
                    }
                }

                // demolish
                task = acc.Tasks?.FindTask(typeof(DemolishBuilding), vill);
                if (task == null)
                {
                    if (vill.Build.DemolishTasks.Count > 0)
                    {
                        timeUpdate = rand.Next(min, max);
                        acc.Tasks.Add(new DemolishBuilding()
                        {
                            ExecuteAt = DateTime.Now.AddMilliseconds(timeUpdate),
                            Vill = vill,
                        }, true, vill);
                    }
                }

                TroopsHelper.ReStartResearchAndImprovement(acc, vill);
                TroopsHelper.ReStartTroopTraining(acc, vill);
            }
        }

        public static void Loaded(Account acc)
        {
            if (acc.Settings.DiscordWebhook && !string.IsNullOrEmpty(acc.AccInfo.WebhookUrl))
            {
                acc.WebhookClient = new DiscordWebhookClient(acc.AccInfo.WebhookUrl);
            }

            SerilogSingleton.LogOutput.AddUsername(acc.AccInfo.Nickname);
            acc.Logger = new Logger(acc.AccInfo.Nickname);
            acc.Tasks = new TaskList(acc);
            acc.Tasks.LoadFromFile(IoHelperCore.GetTaskFileUrl(acc.AccInfo.Nickname, acc.AccInfo.ServerUrl));
            var sleepTask = acc.Tasks?.FindTask(typeof(Sleep));
            if (sleepTask != null)
            {
                sleepTask.ExecuteAt = DateTime.Now + TimeHelper.GetWorkTime(acc);
            }
            else
            {
                acc.Tasks.Add(new Sleep()
                {
                    AutoSleep = true,
                    ExecuteAt = DateTime.Now + TimeHelper.GetWorkTime(acc),
                }, true);
            }

            var rand = new Random();
            var min = 0;
            var max = 3;
            foreach (var task in acc.Tasks.ToList())
            {
                if (task.GetType() == typeof(Sleep)) continue;
                //reset Vill, obj Vill we save is different with current Vill object account hold
                task.Vill = acc.Villages.FirstOrDefault(x => x.Id == task.Vill?.Id);
                //reset time execute
                if (task.ExecuteAt < DateTime.Now)
                {
                    var timeUpdate = rand.Next(min, max);
                    task.ExecuteAt = DateTime.Now.AddMilliseconds(timeUpdate);
                }
            }

            acc.Villages.ForEach(vill => vill.UnfinishedTasks = new List<VillUnfinishedTask>());
        }
    }
}