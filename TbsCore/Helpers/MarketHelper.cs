using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.Settings;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Parsers;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.LowLevel;
using TravBotSharp.Files.Tasks.SecondLevel;

namespace TravBotSharp.Files.Helpers
{
    public static class MarketHelper
    {
        private static readonly int[] MerchantSpeed = { 0, 16, 12, 24, 0, 0, 16, 20 };

        public static int GetMerchantsSpeed(Classificator.TribeEnum tribe) => MerchantSpeed[(int)tribe];

        public static (long, int) ParseMerchantsInfo(HtmlDocument html)
        {
            var merchantsCapacity = Parser.RemoveNonNumeric(html.GetElementbyId("merchantCapacityValue").InnerText);
            int merchantsNum = (int)Parser.RemoveNonNumeric(html.DocumentNode.Descendants("span").First(x => x.HasClass("merchantsAvailable")).InnerText);
            return (merchantsCapacity, merchantsNum);
        }

        /// <summary>
        /// Calculates the time it takes for resources to be transited from main village (supplying village) to target village
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="targetVillage">Target village to receive the resources</param>
        /// <returns></returns>
        public static TimeSpan CalculateTransitTimeMainVillage(Account acc, Village targetVillage)
        {
            return CalculateTransitTime(acc, targetVillage, AccountHelper.GetMainVillage(acc));
        }

        public static TimeSpan CalculateTransitTime(Account acc, Village vill1, Village vill2)
        {
            var distance = MapHelper.CalculateDistance(acc, vill1.Coordinates, vill2.Coordinates);
            //Speed is per hour
            var speed = GetMerchantsSpeed(acc.AccInfo.Tribe ?? Classificator.TribeEnum.Any);
            speed *= acc.AccInfo.ServerSpeed;

            var hours = distance / speed;
            return TimeSpan.FromHours(hours);
        }

        public static long[] NpcTargetResources(Village vill, long resSum = -1)
        {
            if (resSum == -1)
            {
                resSum = vill.Res.Stored.Resources.Sum();
            }
            var ratio = vill.Market.Npc.ResourcesRatio.ToArray();
            long ratioSum = vill.Market.Npc.ResourcesRatio.Sum();

            var onePoint = resSum / ratioSum;

            long[] resTarget = new long[4];
            for (int i = 0; i < 4; i++)
            {
                resTarget[i] = onePoint * ratio[i];
            }
            return resTarget;
        }

        public static bool NpcWillOverflow(Village vill, long[] targetRes = null)
        {
            if (targetRes == null) targetRes = NpcTargetResources(vill);

            for (int i = 0; i < 4; i++)
            {
                if (!vill.Market.Npc.NpcIfOverflow)
                {
                    //if resource would overflow the capacity
                    if (targetRes[i] > (i < 3 ? vill.Res.Capacity.WarehouseCapacity : vill.Res.Capacity.GranaryCapacity))
                    {
                        //TODO: log
                        return true;
                    }
                }
            }
            return false;
        }

        public static long[] SendResCapToStorage(Account acc, Resources resources)
        {
            var mainVill = AccountHelper.GetMainVillage(acc);
            var stored = mainVill.Res.Stored.Resources.ToArray();

            var resSend = resources.ToArray();
            long[] ret = new long[4];
            for (int i = 0; i < 4; i++)
            {
                ret[i] = stored[i] < resSend[i] ? stored[i] : resSend[i];
            }
            return ret;
        }

        /// <summary>
        /// Calculates how many resources should be sent to the main village based on configurable limit
        /// </summary>
        /// <param name="vill">Village</param>
        /// <returns>Resources to be sent</returns>
        public static Resources GetResToMainVillage(Village vill)
        {
            var ret = new long[4];
            var res = vill.Res.Stored.Resources.ToArray();
            var limit = vill.Market.Settings.Configuration.SendResLimit.ToArray();
            for (int i = 0; i < 4; i++)
            {
                // % into resource mode
                if (limit[i] < 100)
                {
                    var capacity = i == 3 ? vill.Res.Capacity.GranaryCapacity : vill.Res.Capacity.WarehouseCapacity;
                    limit[i] = (long)(limit[i] / 100.0 * capacity);
                }

                ret[i] = res[i] - limit[i];
                if (ret[i] < 0) ret[i] = 0;
            }
            return ResourcesHelper.ArrayToResources(ret);
        }

        public static void ReStartSendingToMain(Account acc, Village vill)
        {
            acc.Tasks.RemoveAll(x => x.GetType() == typeof(SendResToMain) && x.Vill == vill);

            if (vill.Settings.Type == VillType.Support && vill.Settings.SendRes)
            {
                TaskExecutor.AddTaskIfNotExistInVillage(acc, vill, new SendResToMain()
                {
                    ExecuteAt = DateTime.Now,
                    Vill = vill
                });
            }
        }
    }
}