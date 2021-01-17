using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.Settings;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{

    /// <summary>
    /// Send all resources (except 30k crop(TODO: SELECTABLE)) above 20% (todo: selectable) to main village.
    /// If we have auto celebration selected, leave res for that (calculate based on production)
    /// </summary>
    public class SendResToMain : UpdateDorf2
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await base.Execute(acc);

            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.Marketplace, "&t=5"))
                return TaskRes.Executed;

            if (this.Vill.Settings.Type == VillType.Support && this.Vill.Settings.SendRes)
            {
                // Repeat this task
                var ran = new Random();
                this.NextExecute = DateTime.Now.AddMinutes(ran.Next(30, 60));
            }

            var mainVill = AccountHelper.GetMainVillage(acc);

            var res = MarketHelper.GetResToMainVillage(Vill);

            var ret = await MarketHelper.MarketSendResource(acc, res, mainVill, this);
            return TaskRes.Executed;
        }
    }
}
