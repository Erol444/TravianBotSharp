using System;
using System.Threading.Tasks;
using System.Linq;
using HtmlAgilityPack;
using TbsCore.Helpers;
using TbsCore.Parsers;
using TbsCore.Models.AccModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.VillageModels;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.LowLevel
{
    public class SendResource : Update
    {
        /// <summary>
        /// How much resources will bot send
        /// </summary>
        protected Resources Resources { get; set; }

        /// <summary>
        /// Target village's coordinate
        /// </summary>
        protected Coordinates Coordinates { get; set; }

        /// <summary>
        /// Duration after bot click "Prepare" button.
        /// There is delay between this and real duration after clicking "Send" button
        /// </summary>
        protected TimeSpan? Duration { get; set; }

        //public int RunTimes { get; set; } //once / twice / 3 times

        public SendResource(Village vill, Resources resources, Coordinates coordinates, DateTime executeAt, TaskPriority priority = TaskPriority.Medium) : base(vill, executeAt, BuildingEnum.Marketplace, priority)
        {
            Resources = resources;
            Coordinates = coordinates;
        }

        public override async Task<TaskRes> Execute(Account acc)
        {
            // Checking before entering building to avoid bot detector
            if (Coordinates == Vill.Coordinates)
            {
                acc.Logger.Warning($"{this.Vill.Name} cannot send resource to itself");
                return TaskRes.Executed;
            }

            if (!ResourcesHelper.IsEnough(Vill, Resources))
            {
                acc.Logger.Warning($"{Vill.Name} doesn't have enough resource to send.");
                acc.Logger.Warning($"Wood: {Resources.Wood}/{Vill.Res.Stored.Resources.Wood}, Clay: {Resources.Clay}/{Vill.Res.Stored.Resources.Clay}, Iron: {Resources.Iron}/{Vill.Res.Stored.Resources.Iron}, Crop: {Resources.Crop}/{Vill.Res.Stored.Resources.Crop}");
                return TaskRes.Executed;
            }

            await base.Execute(acc);

            if (!IsInBuilding)
            {
                return TaskRes.Executed;
            }

            var merchantsCapacity = Vill.Market.MerchantInfo.Capacity;
            var merchantsNum = Vill.Market.MerchantInfo.Free;
            if (Resources.Sum() > merchantsCapacity * merchantsNum)
            {
                return TaskRes.Executed;
            }

            var sendRes = Resources.ToArray();

            for (int i = 0; i < 4; i++)
            {
                await DriverHelper.WriteById(acc, "r" + (i + 1), sendRes[i]);
                await Task.Delay(AccountHelper.Delay(acc));
            }

            await DriverHelper.WriteCoordinates(acc, Coordinates);
            await Task.Delay(AccountHelper.Delay(acc));
            await DriverHelper.ClickById(acc, "enabledButton");

            // Prepare state
            HtmlNode durNode = null;
            while (durNode == null)
            {
                acc.Wb.UpdateHtml();
                durNode = acc.Wb.Html.GetElementbyId("target_validate");
                await Task.Delay(100);
            }
            var dur = durNode.Descendants("td").ToList()[3].InnerText.Replace("\t", "").Replace("\n", "");
            Duration = TimeParser.ParseDuration(dur);

            await DriverHelper.ClickById(acc, "enabledButton");

            await Task.Delay(2000);
            acc.Wb.UpdateHtml();
            (Vill.Market.MerchantInfo.Capacity, Vill.Market.MerchantInfo.Free) = MarketHelper.ParseMerchantsInfo(acc.Wb.Html);

            await TaskExecutor.PageLoaded(acc);

            return TaskRes.Executed;
        }
    }
}