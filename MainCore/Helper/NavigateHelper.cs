using MainCore.Services;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if TRAVIAN_OFFICIAL

using TravianOfficialCore.Parsers;
using TravianOfficialCore.FindElements;

#elif TRAVIAN_OFFICIAL_HEROUI

using TravianOfficialNewHeroUICore.Parsers;
using TravianOfficialNewHeroUICore.FindElements;

#elif TTWARS

using TTWarsCore.Parsers;
using TTWarsCore.FindElements;

#endif

namespace MainCore.Helper
{
    public static class NavigateHelper
    {
        public static readonly Random random = new();

        public static void SwitchVillage(AppDbContext context, IChromeBrowser chromeBrowser, int villageId)
        {
            while (!CheckHelper.IsCorrectVillage(context, chromeBrowser, villageId))
            {
                var html = chromeBrowser.GetHtml();

                var listNode = VillagesTable.GetVillageNodes(html);
                foreach (var node in listNode)
                {
                    var id = VillagesTable.GetId(node);
                    if (id != villageId) continue;

                    var chrome = chromeBrowser.GetChrome();
                    var elements = chrome.FindElements(By.XPath(node.XPath));
                    elements[0].Click();
                }
            }
        }

        public static bool ToDorf1(IChromeBrowser chromeBrowser)
        {
            var currentUrl = chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf1")) return true;
            var doc = chromeBrowser.GetHtml();
            var node = NavigationBar.GetResourceButton(doc);
            if (node is null)
            {
                throw new Exception("Cannot find Resources button");
            }

            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(node.XPath));
            if (elements.Count == 0)
            {
                throw new Exception("Cannot find Resources button");
            }

            elements[0].Click();
            var wait = chromeBrowser.GetWait();
            wait.Until(driver => driver.Url.Contains("dorf"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            return true;
        }

        public static bool ToDorf2(IChromeBrowser chromeBrowser)
        {
            var currentUrl = chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf2")) return true;
            var doc = chromeBrowser.GetHtml();
            var node = NavigationBar.GetBuildingButton(doc);
            if (node is null)
            {
                throw new Exception("Cannot find Buildings button");
            }

            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(node.XPath));
            if (elements.Count == 0)
            {
                throw new Exception("Cannot find Buildings button");
            }

            elements[0].Click();
            var wait = chromeBrowser.GetWait();
            wait.Until(driver => driver.Url.Contains("dorf"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            return true;
        }

        public static void GoRandomDorf(IChromeBrowser chromeBrowser)
        {
            var chanceDorf2 = random.Next(1, 100);
            if (chanceDorf2 >= 50)
            {
                ToDorf2(chromeBrowser);
            }
            else
            {
                ToDorf1(chromeBrowser);
            }
        }
    }
}