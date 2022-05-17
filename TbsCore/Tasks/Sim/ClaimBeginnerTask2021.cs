using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Tasks.Update;

namespace TbsCore.Tasks.Sim
{
    public class ClaimBeginnerTask2021 : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            StopFlag = false;

            {
                if (acc.Hero.Settings.AutoRefreshInfo)
                {
                    acc.Tasks.Add(new HeroUpdateInfo() { ExecuteAt = DateTime.Now });
                }
            }

            {
                acc.Logger.Information($"Checking current village ...");
                var result = await NavigationHelper.SwitchVillage(acc, Vill);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }

            {
                var result = await Update(acc);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }

            {
                var questMasterNode = acc.Wb.Html.GetElementbyId("questmasterButton");
                if (questMasterNode == null)
                {
                    acc.Logger.Warning("Cannot find quest master");
                    return TaskRes.Executed;
                }
                var questMasterElement = acc.Wb.Driver.FindElement(By.XPath(questMasterNode.XPath));
                questMasterElement.Click();
            }

            {
                var result = await Update(acc);
                if (!result) return TaskRes.Executed;
            }

            {
                var result = await ClaimRewards(acc);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }

            {
                var result = await Update(acc);
                if (!result) return TaskRes.Executed;
            }

            {
                var sidebarBox = acc.Wb.Html.GetElementbyId("sidebarBoxQuestmaster");
                if (sidebarBox != null && sidebarBox.Descendants().Any(x => x.HasClass("newQuestSpeechBubble")))
                {
                    var tabNode = acc.Wb.Html.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass("tabItem") && x.GetAttributeValue("data-tab", "").Equals("2"));
                    if (tabNode != null)
                    {
                        var tabElement = acc.Wb.Driver.FindElement(By.XPath(tabNode.XPath));
                        tabElement.Click();

                        var result = await ClaimRewards(acc);
                        if (StopFlag) return TaskRes.Executed;
                        if (!result) return TaskRes.Executed;
                    }
                }
            }

            return TaskRes.Executed;
        }

        private async Task<bool> ClaimRewards(Account acc)
        {
            int count = 0;
            do
            {
                {
                    var result = await Update(acc);
                    if (!result) return false;
                }

                var collectNodes = acc.Wb.Html.DocumentNode.Descendants("button").Where(x => x.HasClass("collect"));
                if (collectNodes.Count() == 0) return true;

                foreach (var node in collectNodes)
                {
                    var collectElement = acc.Wb.Driver.FindElement(By.XPath(node.XPath));
                    collectElement.Click();
                    if (StopFlag) return false;

                    await AccountHelper.DelayWait(acc, 5);
                }

                {
                    var result = await Update(acc);
                    if (!result) return false;
                }

                count++;
                if (count > 50) return true; // infinite loop ( i dont think there is over 50 quest waiting bot )
            }
            while (true);
        }
    }
}