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

        /// <summary>
        /// Will send resources from main village to the target village
        /// </summary>
        /// <param name="vill">(target) Village to get the resources</param>
        /// <returns>Returns DateTime when approximately will resources get transited to target village </returns>
        public static DateTime TransitResourcesFromMain(Account acc, Village vill)
        {
            // Transit resources for this village is disabled.
            if (!vill.Market.Settings.Configuration.Enabled) return DateTime.MaxValue;

            // There already is a sendResources BotTask for this village
            var transitTask = (SendResources)acc.Tasks.FirstOrDefault(x =>
                x.GetType() == typeof(SendResources) &&
                ((SendResources)x).Coordinates == vill.Coordinates
            );
            //vill.Market.Settings.Configuration.
            if (transitTask != null) return transitTask.Configuration.TransitArrival;
            //Less than 5min ago we already sent resources. Just to catch bugs.
            //if(vill.Market.)

            // Merchants are on their way
            if (vill.Market.Settings.Configuration.TransitArrival > DateTime.Now) return vill.Market.Settings.Configuration.TransitArrival;

            //send resources
            var sendRes = new Resources();
            var conf = vill.Market.Settings.Configuration;
            var currentRes = vill.Res.Stored.Resources;
            var cap = vill.Res.Capacity;

            var woodNeeded = (long)(cap.WarehouseCapacity * conf.TargetLimit.Wood / 100.0);
            sendRes.Wood = (woodNeeded > conf.FillLimit.Wood ? conf.FillLimit.Wood : woodNeeded) - currentRes.Wood;
            sendRes.Wood = (sendRes.Wood < 0 ? 0 : sendRes.Wood);

            var clayNeeded = (long)(cap.WarehouseCapacity * conf.TargetLimit.Clay / 100.0);
            sendRes.Clay = (clayNeeded > conf.FillLimit.Clay ? conf.FillLimit.Clay : clayNeeded) - currentRes.Clay;
            sendRes.Clay = (sendRes.Clay < 0 ? 0 : sendRes.Clay);

            var ironNeeded = (long)(cap.WarehouseCapacity * conf.TargetLimit.Iron / 100.0);
            sendRes.Iron = (ironNeeded > conf.FillLimit.Iron ? conf.FillLimit.Iron : ironNeeded) - currentRes.Iron;
            sendRes.Iron = (sendRes.Iron < 0 ? 0 : sendRes.Iron);

            var cropNeeded = (long)(cap.GranaryCapacity * conf.TargetLimit.Crop / 100.0);
            sendRes.Crop = (cropNeeded > conf.FillLimit.Crop ? conf.FillLimit.Crop : cropNeeded) - currentRes.Crop;
            sendRes.Crop = (sendRes.Crop < 0 ? 0 : sendRes.Crop);

            if (ResourcesHelper.IsZeroResources(sendRes)) //we have enough res :)
                return DateTime.MinValue;

            // Send resources to a village only once per 5 minutes
            TimeSpan transitAfter = vill.Market.LastTransit.AddMinutes(5) - DateTime.Now;
            if (transitAfter < TimeSpan.Zero) transitAfter = TimeSpan.Zero;

            var sendResTask = new SendResources
            {
                Configuration = conf,
                Coordinates = vill.Coordinates,
                ExecuteAt = DateTime.Now + transitAfter,
                Vill = AccountHelper.GetMainVillage(acc),
                Resources = sendRes
            };

            TaskExecutor.AddTask(acc, sendResTask);

            //AddMinutes(1) since bot has to wait for the SendResources task and then
            //go to the marketplace and send resources
            //TransitArrival will get updated to more specific time

            return DateTime.Now.Add(transitAfter + CalculateTransitTimeMainVillage(acc, vill)).AddMinutes(1);
        }

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

            var wb = acc.Wb.Driver;
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
                await Task.Delay(AccountHelper.Delay() / 5);
            }

            // Input coordinates
            await DriverHelper.WriteCoordinates(acc, targetVillage.Coordinates);

            //Select x2/x3
            if (times != 1)
            {
                wb.ExecuteScript($"document.getElementById('x2').value='{times}'");
                await Task.Delay(AccountHelper.Delay() / 5);
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

        private static (long, int) ParseMerchantsInfo(HtmlDocument html)
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
        /// Calculates how many resources should be sent to the main village based on configurable limit
        /// </summary>
        /// <param name="vill">Village</param>
        /// <returns>Resources to be sent</returns>
        public static long[] GetResToMainVillage(Village vill)
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