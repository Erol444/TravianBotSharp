using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{

    /// <summary>
    /// Send all resources (except 30k crop(TODO: SELECTABLE)) above 20% (todo: selectable) to main village.
    /// If we have auto celebration selected, leave res for that (calculate based on production)
    /// </summary>
    public class SendResToMain : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;

            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.Marketplace, "&t=5"))
                return TaskRes.Executed;

            if (this.Vill.Settings.Type == Models.Settings.VillType.Support && this.Vill.Settings.SendRes)
            {
                // Repeat this task
                this.NextExecute = DateTime.Now.AddHours(1);
            }

            var mainVill = AccountHelper.GetMainVillage(acc);

            var res = MarketHelper.GetResToMainVillage(Vill);

            var ret = await MarketHelper.MarketSendResource(acc, res, mainVill, this);
            return TaskRes.Executed;
        }
    }
}
