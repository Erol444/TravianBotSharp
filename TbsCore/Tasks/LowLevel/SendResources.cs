using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.VillageModels;
using TbsCore.Models.ResourceModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendResources : BotTask
    {
        public Resources Resources { get; set; }
        public Coordinates Coordinates { get; set; }
        public Village TargetVill { get; set; } = null;

        public override async Task<TaskRes> Execute(Account acc)
        {
            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.Marketplace, "&t=5"))
                return TaskRes.Executed;

            // if send resouce to our villages
            if (TargetVill != null)
            {
                Coordinates = TargetVill.Coordinates;
            }

            if (Vill.Coordinates.Equals(Coordinates))
            {
                acc.Wb.Log("Same coordinates with target");
                return TaskRes.Executed;
            }

            (var merchantsCapacity, var merchantsNum) = MarketHelper.ParseMerchantsInfo(acc.Wb.Html);
            // We don't have any merchants.
            if (merchantsNum == 0)
            {
                acc.Wb.Log("All merchants are not in village now. Try later");
                var time = MarketParser.GetSoonestMerchant(acc.Wb.Html);
                NextExecute = time.AddSeconds(20);
                return TaskRes.Executed;
            }

            var times = 1;
            if (acc.AccInfo.GoldClub ?? false) times = 3;
            else if (acc.AccInfo.PlusAccount) times = 2;

            var res = Resources.ToArray();

            if (times > 1)
            {
                res = res.Select(x =>
                {
                    var resSend = (long)Math.Ceiling((double)x / times);
                    var tmp = resSend % 100;
                    return tmp > 0 ? resSend + 100 - tmp : resSend;
                }).ToArray();
            }

            // Check if we have enough merchants to carry
            var sum = ResourcesHelper.ArrayToResources(res).Sum();
            var diff = (sum - merchantsCapacity);
            if (diff > 0)
            {
                // if EriK or someone else look at this section and has another way better, pls change ._.
                // so my way (vinaghost's way) is reduce one by one untill we can send,
                // start from iron -> wood -> clay -> crop
                // why crop last ? i dont know ._.
                long tmp;
                // iron
                if (diff != 0)
                {
                    tmp = res[2];
                    res[2] = (diff > res[2]) ? 0 : res[2] - diff;
                    diff = (res[2] == 0) ? (diff - tmp) : 0;
                }
                // wood
                if (diff != 0)
                {
                    tmp = res[0];
                    res[0] = (diff > res[0]) ? 0 : res[0] - diff;
                    diff = (res[0] == 0) ? (diff - tmp) : 0;
                }
                // clay
                if (diff != 0)
                {
                    tmp = res[1];
                    res[1] = (diff > res[1]) ? 0 : res[1] - diff;
                    diff = (res[1] == 0) ? (diff - tmp) : 0;
                }
                // crop
                if (diff != 0)
                {
                    tmp = res[3];
                    res[3] = (diff > res[3]) ? 0 : res[3] - diff;
                    diff = (res[3] == 0) ? (diff - tmp) : 0;
                }

                acc.Wb.Log("Not have enough merchants, village will send as much as posible");
            }

            var wb = acc.Wb.Driver;
            for (int i = 0; i < 4; i++)
            {
                if (res[i] <= 0) continue;
                await DriverHelper.TextById(acc, $"r{(i + 1)}", res[i]);
            }

            // Input coordinates
            await DriverHelper.WriteCoordinates(acc, Coordinates);

            // Update target village
            TimeSpan timeTransit;
            if (TargetVill != null)
            {
                timeTransit = MarketHelper.CalculateTransitTime(acc, Vill, TargetVill);
            }

            //Select x2/x3
            if (times != 1)
            {
                if (TargetVill != null)
                {
                    timeTransit = TimeSpan.FromSeconds(timeTransit.TotalSeconds * ((2 * times) - 1));
                }
                wb.ExecuteScript($"document.getElementById('x2').value='{times}'");
                await Task.Delay(AccountHelper.Delay() / 3);
            }

            if (TargetVill != null)
            {
                TaskExecutor.AddTask(acc, new UpdateTaskUseRes()
                {
                    Vill = TargetVill,
                    ExecuteAt = DateTime.Now.AddSeconds(timeTransit.TotalSeconds + 5)
                });
            }

            // Send
            await DriverHelper.ClickById(acc, "enabledButton");

            await Task.Delay(AccountHelper.Delay());
            wb.ExecuteScript("marketPlace.sendRessources()");

            return TaskRes.Executed;
        }
    }
}