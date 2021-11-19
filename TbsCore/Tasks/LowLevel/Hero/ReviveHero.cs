using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;

namespace TbsCore.Tasks.LowLevel
{
    public class ReviveHero : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await HeroHelper.NavigateToHeroAttributes(acc);

            //heroRegeneration
            var reviveButton = acc.Wb.Html.GetElementbyId("heroRegeneration");
            if (reviveButton == null)
            {
                acc.Logger.Warning("No revive button found!");
                return TaskRes.Executed;
            }
            if (reviveButton.HasClass("green"))
            {
                acc.Wb.ExecuteScript("document.getElementById('heroRegeneration').click()"); //revive hero
                return TaskRes.Executed;
            }
            else
            {
                //no resources?
                this.NextExecute = DateTime.Now.AddMinutes(10);
                return TaskRes.Executed;
            }
        }
    }
}