using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;
using OpenQA.Selenium;

namespace TbsCore.Tasks.LowLevel
{
    public class ReviveHero : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            StopFlag = false;
            acc.Logger.Information("Navigate to revive page", this);
            await NavigationHelper.ToHero(acc, NavigationHelper.HeroTab.Attributes);
            if (StopFlag) return TaskRes.Executed;

            acc.Logger.Information("Find revive button to click", this);
            //heroRegeneration
            var reviveButton = acc.Wb.Html.GetElementbyId("heroRegeneration");
            if (reviveButton == null)
            {
                acc.Logger.Information("No revive button found!");
                return TaskRes.Executed;
            }

            if (reviveButton.HasClass("green"))
            {
                var element = acc.Wb.Driver.FindElement(By.XPath(reviveButton.XPath));

                if (element == null)
                {
                    acc.Logger.Information("Cannot click revive button!");
                    return TaskRes.Executed;
                }

                element.Click();
                return TaskRes.Executed;
            }
            else
            {
                //no resources?
                acc.Logger.Information("Don't have enough resource. Retry after 10 minutes!");
                NextExecute = DateTime.Now.AddMinutes(10);
                return TaskRes.Executed;
            }
        }
    }
}