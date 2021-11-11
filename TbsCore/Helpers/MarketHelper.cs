using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.Settings;
using TbsCore.Models.VillageModels;

using TbsCore.Parsers;
using TbsCore.Tasks;
using TbsCore.Tasks.LowLevel;

namespace TbsCore.Helpers
{
    public static class MarketHelper
    {
        private static readonly int[] MerchantSpeed = { 0, 16, 12, 24, 0, 0, 16, 20 };

        public static int GetMerchantsSpeed(Classificator.TribeEnum tribe) => MerchantSpeed[(int)tribe];

        #region Trade route

        public static void AddTradeRoute(Village vill, TradeRoute trade)
        {
            vill.Market.TradeRoute.TradeRoutes.Add(trade);
        }

        public static void UpdateTradeRoute(Village vill, TradeRoute trade, int index)
        {
            if (index < 0) return;
            if (index >= vill.Market.TradeRoute.TradeRoutes.Count) return;
            vill.Market.TradeRoute.TradeRoutes[index] = trade;
        }

        public static void RemoveTradeRoute(Village vill, int index)
        {
            if (index < 0) return;
            if (index >= vill.Market.TradeRoute.TradeRoutes.Count) return;
            vill.Market.TradeRoute.TradeRoutes.RemoveAt(index);

            if (vill.Market.TradeRoute.Next >= vill.Market.TradeRoute.TradeRoutes.Count)
            {
                vill.Market.TradeRoute.Next = 0;
            }
        }

        public static int UpdateNextTradeRoute(Village vill)
        {
            vill.Market.TradeRoute.Next++;

            if (vill.Market.TradeRoute.Next >= vill.Market.TradeRoute.TradeRoutes.Count)
            {
                vill.Market.TradeRoute.Next = 0;
            }
            return vill.Market.TradeRoute.Next;
        }

        #endregion Trade route

        #region Send resource

        public static Resources GetResource(Village vill, Resources resources) =>
              new Resources
              {
                  Wood = resources.Wood > 100 ? resources.Wood : vill.Res.Stored.Resources.Wood * resources.Wood / 100,
                  Clay = resources.Clay > 100 ? resources.Clay : vill.Res.Stored.Resources.Clay * resources.Clay / 100,
                  Iron = resources.Iron > 100 ? resources.Iron : vill.Res.Stored.Resources.Iron * resources.Iron / 100,
                  Crop = resources.Crop > 100 ? resources.Crop : vill.Res.Stored.Resources.Crop * resources.Crop / 100,
              };

        public static Resources Round(Resources resources)
        {
            var res = resources.ToArray();
            for (int i = 0; i < 4; i++)
            {
                // To avoid exception devide by zero
                if (50 <= res[i])
                {
                    //round the number to about -1%, for rounder numbers
                    var digits = Math.Ceiling(Math.Log10(res[i]));
                    var remainder = res[i] % (long)Math.Pow(10, digits - 2);
                    res[i] -= remainder;
                }
            }

            return ResourcesHelper.ArrayToResources(res);
        }

        #endregion Send resource

        /// <summary>
        /// Used by BotTasks to insert resources/coordinates into the page.
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="resources">Target resources</param>
        /// <param name="coordinates">Target coordinates</param>
        /// <returns>Time it will take for transit to complete</returns>
        public static async Task<TimeSpan> MarketSendResource(Account acc, Resources resources, Village targetVillage, BotTask botTask) =>
            await MarketSendResource(acc, resources.ToArray(), targetVillage, botTask);

        /// <summary>
        /// Used by BotTasks to insert resources/coordinates into the page.
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="resources">Target resources</param>
        /// <param name="coordinates">Target coordinates</param>
        /// <returns>Time it will take for transit to complete</returns>
        public static async Task<TimeSpan> MarketSendResource(Account acc, long[] resources, Village targetVillage, BotTask botTask)
        {
            var times = 1;
            if (acc.AccInfo.GoldClub ?? false) times = 3;
            else if (acc.AccInfo.PlusAccount) times = 2;

            // No resources to send
            if (resources.Sum() == 0) return TimeSpan.Zero;

            var sendRes = resources.Select(x => x / times).ToArray();

            //round the resources that we want to send, so it looks less like a bot

            (var merchantsCapacity, var merchantsNum) = MarketHelper.ParseMerchantsInfo(acc.Wb.Html);
            // We don't have any merchants.
            if (merchantsNum == 0)
            {
                //Parse currently ongoing transits
                var transits = MarketParser.ParseTransits(acc.Wb.Html);
                var activeVill = acc.Villages.FirstOrDefault(x => x.Active); // Could also just pass that in params

                var nextTry = SoonestAvailableMerchants(acc, activeVill, targetVillage, transits);
                if (nextTry != DateTime.MaxValue) nextTry = nextTry.AddSeconds(5);

                botTask.NextExecute = nextTry;
                // Just return something, will get overwritten anyways.
                return new TimeSpan((int)(nextTry - DateTime.Now).TotalHours + 1, 0, 0);
            }

            var maxRes = merchantsCapacity * times;
            var allRes = resources.Sum();
            if (allRes > maxRes)
            {
                // We don't have enough merchants to transit all the resources. Divide all resources by some divider.
                var resDivider = (float)allRes / maxRes;
                float[] resFloat = sendRes.Select(x => x / resDivider).ToArray();
                sendRes = resFloat.Select(x => (long)Math.Floor(x)).ToArray();
            }

            for (int i = 0; i < 4; i++)
            {
                // To avoid exception devide by zero
                if (50 <= sendRes[i])
                {
                    //round the number to about -1%, for rounder numbers
                    var digits = Math.Ceiling(Math.Log10(sendRes[i]));
                    var remainder = sendRes[i] % (long)Math.Pow(10, digits - 2);
                    sendRes[i] -= remainder;
                    await DriverHelper.WriteById(acc, "r" + (i + 1), sendRes[i]);
                }
                await Task.Delay(AccountHelper.Delay(acc) / 5);
            }

            // Input coordinates
            await DriverHelper.WriteCoordinates(acc, targetVillage.Coordinates);

            //Select x2/x3
            if (times != 1)
            {
                acc.Wb.ExecuteScript($"document.getElementById('x2').value='{times}'");
                await Task.Delay(AccountHelper.Delay(acc) / 5);
            }
            await DriverHelper.ClickById(acc, "enabledButton");

            var durNode = acc.Wb.Html.GetElementbyId("target_validate");

            if (durNode == null && acc.Wb.Html.GetElementbyId("prepareError") != null)
            {
                // Error "Abuse! You have not enough resources." is displayed.
            }
            //get duration of transit
            var dur = durNode.Descendants("td").ToList()[3].InnerText.Replace("\t", "").Replace("\n", "");

            // Will NOT trigger a page reload! Thus we should await some time before continuing.
            await DriverHelper.ClickById(acc, "enabledButton");

            targetVillage.Market.LastTransit = DateTime.Now;

            var duration = TimeParser.ParseDuration(dur);
            return TimeSpan.FromTicks(duration.Ticks * (times * 2 - 1));
        }

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
        private static TimeSpan CalculateTransitTimeMainVillage(Account acc, Village targetVillage)
        {
            return CalculateTransitTime(acc, targetVillage, AccountHelper.GetMainVillage(acc));
        }

        private static TimeSpan CalculateTransitTime(Account acc, Village vill1, Village vill2)
        {
            var mainVill = AccountHelper.GetMainVillage(acc);
            if (mainVill == null) acc.Villages.First();

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
        /// Finds out which transits will be over soonest and returns the datetime.
        /// </summary>
        /// <param name="transitsO"></param>
        /// <returns></returns>
        public static DateTime SoonestAvailableMerchants(Account acc, Village vill1, Village vill2, List<MerchantsUnderWay> transitsO)
        {
            var transits = transitsO
                .Where(x => x.Transit == TransitType.Outgoin || x.Transit == TransitType.Returning)
                .ToList();

            var ret = DateTime.MaxValue;
            foreach (var transit in transits)
            {
                DateTime time = DateTime.MaxValue;

                switch (transit.Transit)
                {
                    case TransitType.Outgoin:
                        var oneTransitTime = MarketHelper.CalculateTransitTime(acc, vill1, vill2);
                        time = transit.Arrival.Add(oneTransitTime);
                        if (transit.RepeatTimes > 1)
                        {
                            time += TimeHelper.MultiplyTimespan(
                                oneTransitTime,
                                (2 * (transit.RepeatTimes - 1))
                                );
                        }
                        break;

                    case TransitType.Returning:
                        time = transit.Arrival;
                        if (transit.RepeatTimes > 1)
                        {
                            time += TimeHelper.MultiplyTimespan(
                                MarketHelper.CalculateTransitTime(acc, vill1, vill2),
                                (2 * (transit.RepeatTimes - 1))
                                );
                        }
                        break;
                }
                if (ret > time) ret = time;
            }
            return ret;
        }

        public static void ReStartSendingToMain(Account acc, Village vill)
        {
            acc.Tasks.Remove(typeof(SendResToMain), vill);

            if (vill.Settings.Type == VillType.Support && vill.Settings.SendRes)
            {
                acc.Tasks.Add(new SendResToMain()
                {
                    ExecuteAt = DateTime.Now,
                    Vill = vill
                }, true, vill);
            }
        }
    }
}