using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Parsers;

namespace TbsCore.Tasks.Others
{
    public class StartAdventure : BotTask
    {
        /// <summary>
        /// In case we want to only update adventures
        /// </summary>
        public bool UpdateOnly { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            await NavigationHelper.ToAdventure(acc);

            acc.Hero.Adventures = AdventureParser.GetAdventures(acc.Wb.Html);

            if (acc.Hero.Adventures == null || acc.Hero.Adventures.Count == 0 || UpdateOnly) return TaskRes.Executed;
            if (acc.Hero.Status != Hero.StatusEnum.Home)
            {
                acc.Logger.Warning("Hero isn't in home village.");
                return TaskRes.Executed;
            }

            var adventures = acc.Wb.Html.GetElementbyId("heroAdventure");
            if (adventures == null)
            {
                acc.Logger.Warning("Cannot find adventures table");
                return TaskRes.Executed;
            }
            var tbody = adventures.Descendants("tbody").FirstOrDefault();
            if (adventures == null)
            {
                acc.Logger.Warning("Cannot find adventures body table");
                return TaskRes.Executed;
            }
            var buttons = tbody.Descendants("button").ToArray();
            if (buttons.Length == 0)
            {
                acc.Logger.Warning("Cannot find button to start adventure");
                return TaskRes.Executed;
            }
            var buttonElements = acc.Wb.Driver.FindElements(By.XPath(buttons[0].XPath));
            if (buttons.Length == 0)
            {
                acc.Logger.Warning("Cannot find button to start adventure");
                return TaskRes.Executed;
            }
            buttonElements[0].Click();
            await Task.Delay(900); //random magic numer

            // Check hero outgoing time
            var outTime = HeroParser.GetHeroArrival(acc.Wb.Html);
            // At least 1.5x longer (if hero has Large map)
            acc.Hero.NextHeroSend = DateTime.Now + TimeSpan.FromTicks((long)(outTime.Ticks * 1.5));

            if (DateTime.Now.Millisecond % 2 == 0)
            {
                await NavigationHelper.ToDorf1(acc);
            }
            else
            {
                await NavigationHelper.ToDorf2(acc);
            }

            return TaskRes.Executed;
        }
    }
}