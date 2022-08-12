using MainCore.Services;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using HtmlAgilityPack;

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

        public static bool ToDorf1(IChromeBrowser chromeBrowser, bool isForce = false)

        {
            var currentUrl = chromeBrowser.GetCurrentUrl();

            if (currentUrl.Contains("dorf1"))
            {
                if (isForce) chromeBrowser.Navigate();

                return true;
            }

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

        public static bool ToDorf2(IChromeBrowser chromeBrowser, bool isForce = false)
        {
            var currentUrl = chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf2"))
            {
                if (isForce) chromeBrowser.Navigate();
                return true;
            }
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

        public static void GoToBuilding(IChromeBrowser chromeBrowser, int index)
        {
            var currentUrl = chromeBrowser.GetCurrentUrl();
#if TTWARS
            var uri = new Uri(currentUrl);
            var serverUrl = $"{uri.Scheme}://{uri.Host}";
            var url = $"{serverUrl}/build.php?id={index}";
            chromeBrowser.Navigate(url);
#else
            var dorf = BuildingsHelper.GetDorf(index);
            var html = chromeBrowser.GetHtml();
            var chrome = chromeBrowser.GetChrome();
            switch (dorf)
            {
                case 1:
                    {
                        var node = Building.GetResourceField(html, index);
                        if (node is null)
                        {
                            throw new Exception($"Cannot find resource field at {index}");
                        }
                        var elements = chrome.FindElements(By.XPath(node.XPath));
                        if (elements.Count == 0)
                        {
                            throw new Exception($"Cannot find resource field at {index}");
                        }
                        elements[0].Click();
                    }
                    break;

                case 2:
                    {
                        var node = Building.GetBuilding(html, index);
                        if (node is null)
                        {
                            throw new Exception($"Cannot find building at {index}");
                        }
                        var pathBuilding = node.Descendants("path").FirstOrDefault();
                        if (pathBuilding is null)
                        {
                            throw new Exception($"Cannot find building at {index}");
                        }
                        var href = pathBuilding.GetAttributeValue("onclick", "");
                        var script = href.Replace("&amp;", "&");
                        chrome.ExecuteScript(script);
                    }
                    break;

                default:
                    break;
            }
            var wait = chromeBrowser.GetWait();
            wait.Until(driver => driver.Url.Contains($"?id={index}"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

#endif
        }

        public static void SwitchTab(IChromeBrowser chromeBrowser, int index)
        {
            while (!CheckHelper.IsCorrectTab(chromeBrowser, index))
            {
                var html = chromeBrowser.GetHtml();
                var listNode = BuildingTab.GetBuildingTabNodes(html);
                var chrome = chromeBrowser.GetChrome();
                var elements = chrome.FindElements(By.XPath(listNode[index].XPath));
                elements[0].Click();
            }
        }

        public static void ToHeroInventory(IChromeBrowser chromeBrowser)
        {
            var html = chromeBrowser.GetHtml();
#if TRAVIAN_OFFICIAL_HEROUI
            var avatar = Hero.GetHeroAvatar(html);
            if (avatar is null)
            {
                throw new Exception("Cannot find hero avatar");
            }
            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(avatar.XPath));
            if (elements.Count == 0)
            {
                throw new Exception("Cannot find hero avatar");
            }
            elements[0].Click();
            var wait = chromeBrowser.GetWait();
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            wait.Until(driver =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var tab = Hero.GetHeroTab(doc, 1);
                if (tab is null) return false;
                return tab.IsCurrentTab();
            });

#else
            var inventory = HeroPage.GetHeroInventory(html);
            if (inventory is null)
            {
                throw new Exception("Cannot find hero inventory button");
            }
            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(inventory.XPath));
            if (elements.Count == 0)
            {
                throw new Exception("Cannot find hero inventory button");
            }
            elements[0].Click();

            var wait = chromeBrowser.GetWait();
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
#endif
        }
    }
}