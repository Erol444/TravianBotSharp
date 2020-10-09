using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
            if (acc.AccInfo.MapSize == 0 ||
                acc.AccInfo.Tribe == Classificator.TribeEnum.Any)
            {
                TaskExecutor.AddTaskIfNotExists(acc, new GetMapSizeAndTribe() { ExecuteAt = DateTime.MinValue.AddHours(2) });
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
            if(nextAccessChange != TimeSpan.MaxValue)
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
                //todo
            }
        }


        public static async Task CheckProxies(List<Access> access)
        {
            List<Task> tasks = new List<Task>();
            access.ForEach(a =>
            {
                tasks.Add(Task.Run(() =>
                {
                    Console.WriteLine(DateTime.Now.ToString()+"]Start ip "+a.Proxy);
                    var restClient = new RestClient
                    {
                        BaseUrl = new Uri("https://api.ipify.org/"),
                    };

                    if (!string.IsNullOrEmpty(a.Proxy))
                    {
                        if (!string.IsNullOrEmpty(a.ProxyUsername)) // Proxy auth
                        {
                            ICredentials credentials = new NetworkCredential(a.ProxyUsername, a.ProxyPassword);
                            restClient.Proxy = new WebProxy($"{a.Proxy}:{a.ProxyPort}", false, null, credentials);
                        }
                        else // Without proxy auth
                        {
                            restClient.Proxy = new WebProxy(a.Proxy, a.ProxyPort);
                        }
                    }

                    var response = restClient.Execute(new RestRequest
                    {
                        Resource = "api/ip",
                        Method = Method.GET,
                        Timeout = 5000,
                    });
                    Console.WriteLine(DateTime.Now.ToString() + "] Complete ip" + a.Proxy + $", Credentials: {!string.IsNullOrEmpty(a.ProxyUsername)}, content:"+response.Content);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(response.Content);

                    var ip = doc.DocumentNode.InnerText;

                    a.Ok = ip == a.Proxy;
                }));
            });
            await Task.WhenAll(tasks);
            Console.WriteLine(DateTime.Now.ToString() + "]all tasks complete");
        }
    }
}
