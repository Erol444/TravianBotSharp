using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Parsers;

namespace TbsCore.Tasks.Others
{
    public class HeroSetPoints : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await NavigationHelper.ToHero(acc, NavigationHelper.HeroTab.Attributes);

            acc.Hero.HeroInfo = HeroParser.GetHeroAttributes(acc.Wb.Html);
            acc.Hero.HeroArrival = DateTime.Now + HeroParser.GetHeroArrivalInfo(acc.Wb.Html);

            float sum = 0;
            for (int i = 0; i < 4; i++) sum += acc.Hero.Settings.Upgrades[i];
            var points = acc.Hero.HeroInfo.AvaliblePoints;
            var wait = new WebDriverWait(acc.Wb.Driver, TimeSpan.FromMinutes(1));
            var pointUsed = 0;
            for (int i = 0; i < 4; i++)
            {
                var amount = Math.Ceiling(acc.Hero.Settings.Upgrades[i] * points / sum);
                if (amount == 0) continue;

                for (int j = 0; j < (int)amount; j++)
                {
                    {
                        var attributesDiv = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroAttributes"));
                        var pointButtons = attributesDiv.Descendants("button").Where(x => x.HasClass("plus")).ToArray();
                        var elements = acc.Wb.Driver.FindElements(By.XPath(pointButtons[i].XPath));
                        elements[0].Click();
                    }
                    wait.Until(driver =>
                    {
                        acc.Wb.UpdateHtml();
                        var attributesDiv = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("heroAttributes"));
                        if (attributesDiv == null) return false;
                        var avaliblePoints = attributesDiv.Descendants("div").FirstOrDefault(x => x.HasClass("pointsAvailable"));
                        if (avaliblePoints == null) return false;
                        var currentPoint = (int)Parser.RemoveNonNumeric(avaliblePoints.InnerText);
                        if (acc.Hero.HeroInfo.AvaliblePoints - pointUsed - currentPoint == 0) return false;
                        pointUsed++;
                        return true;
                    });
                }
            }
            var buttonElements = acc.Wb.Driver.FindElements(By.Id("savePoints"));
            buttonElements[0].Click();
            wait.Until(driver =>
            {
                var button = acc.Wb.Driver.FindElements(By.Id("savePoints"));
                if (button.Count == 0) return false;
                return !button[0].Enabled;
            });
            acc.Hero.HeroInfo = HeroParser.GetHeroAttributes(acc.Wb.Html);
            return TaskRes.Executed;
        }
    }
}