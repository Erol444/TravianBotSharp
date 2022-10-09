using MainCore.Services;
using System;
using OpenQA.Selenium;
using MainCore.Exceptions;
using System.Threading;

#if TRAVIAN_OFFICIAL

using System.Linq;
using TravianOfficialCore.Parsers;
using TravianOfficialCore.FindElements;

#elif TRAVIAN_OFFICIAL_HEROUI

using HtmlAgilityPack;
using System.Linq;
using TravianOfficialNewHeroUICore.Parsers;
using TravianOfficialNewHeroUICore.FindElements;

#elif TTWARS

using TTWarsCore.Parsers;
using TTWarsCore.FindElements;

#else

#error You forgot to define Travian version here

#endif

namespace MainCore.Helper
{
    public static class NavigateHelper
    {
        private static readonly Random rand = new();

        public static void Sleep(int min, int max) => Thread.Sleep(rand.Next(min, max));

        public static void WaitPageLoaded(IChromeBrowser chromeBrowser)
        {
            var wait = chromeBrowser.GetWait();
            try
            {
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            }
            catch (TimeoutException)
            {
                throw new StopNowException("Page not loaded in 3 mins. Considering that network has problem");
            }
        }

        public static void WaitPageChanged(IChromeBrowser chromeBrowser, string path)
        {
            var wait = chromeBrowser.GetWait();
            try
            {
                wait.Until(driver => driver.Url.Contains(path));
            }
            catch (TimeoutException)
            {
                WaitPageLoaded(chromeBrowser);
            }
        }

        public static int GetDelayClick(AppDbContext context, int accountId)
        {
            var setting = context.AccountsSettings.Find(accountId);
            return rand.Next(setting.ClickDelayMin, setting.ClickDelayMax);
        }

        public static void AfterClicking(IChromeBrowser chromeBrowser, AppDbContext context, int accountId)
        {
            if (!chromeBrowser.IsOpen())
            {
                throw new ChromeMissingException();
            }
            var html = chromeBrowser.GetHtml();
            if (CheckHelper.IsCaptcha(html))
            {
                throw new StopNowException("Captcha found! Bot must be stopped.");
            }
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
            if (CheckHelper.IsWWMsg(html))
#elif TTWARS
            if (CheckHelper.IsWWMsg(html) && CheckHelper.IsWWPage(chromeBrowser))
#else

#error You forgot to define Travian version here

#endif
            {
                throw new StopNowException("WW complete page found! Bot must be stopped.");
            }

            if (CheckHelper.IsBanMsg(html))
            {
                throw new StopNowException("Ban page found! Bot must be stopped.");
            }

            if (CheckHelper.IsMaintanance(html))
            {
                throw new StopNowException("Maintanance page found! Bot must be stopped.");
            }

            if (CheckHelper.IsLoginScreen(html))
            {
                throw new LoginNeedException();
            }
            if (CheckHelper.IsSysMsg(html))
            {
                var url = chromeBrowser.GetCurrentUrl();
                var serverUrl = new Uri(url);
                var chrome = chromeBrowser.GetChrome();
                chromeBrowser.Navigate($"{serverUrl.Scheme}://{serverUrl.Host}/dorf1.php?ok=1");
                var delay = GetDelayClick(context, accountId);
                Thread.Sleep(delay);
                WaitPageChanged(chromeBrowser, "dorf1");
                WaitPageLoaded(chromeBrowser);
            }
        }

        public static void SwitchVillage(AppDbContext context, IChromeBrowser chromeBrowser, int villageId, int accountId)
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
                    var delay = GetDelayClick(context, accountId);
                    Thread.Sleep(delay);
                    WaitPageLoaded(chromeBrowser);
                    AfterClicking(chromeBrowser, context, accountId);
                }
            }
        }

        public static bool ToDorf1(IChromeBrowser chromeBrowser, AppDbContext context, int accountId, bool isForce = false)

        {
            var currentUrl = chromeBrowser.GetCurrentUrl();
            var delay = GetDelayClick(context, accountId);
            if (currentUrl.Contains("dorf1"))
            {
                if (isForce)
                {
                    chromeBrowser.Navigate();
                    Thread.Sleep(delay);
                    WaitPageChanged(chromeBrowser, "dorf1");
                    if (!chromeBrowser.GetCurrentUrl().Contains("dorf1")) return ToDorf1(chromeBrowser, context, accountId, isForce);
                    WaitPageLoaded(chromeBrowser);
                    AfterClicking(chromeBrowser, context, accountId);
                }

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

            Thread.Sleep(delay);
            WaitPageChanged(chromeBrowser, "dorf1");
            if (!chromeBrowser.GetCurrentUrl().Contains("dorf1")) return ToDorf1(chromeBrowser, context, accountId, isForce);
            WaitPageLoaded(chromeBrowser);
            AfterClicking(chromeBrowser, context, accountId);
            return true;
        }

        public static bool ToDorf2(IChromeBrowser chromeBrowser, AppDbContext context, int accountId, bool isForce = false)
        {
            var currentUrl = chromeBrowser.GetCurrentUrl();
            var delay = GetDelayClick(context, accountId);

            if (currentUrl.Contains("dorf2"))
            {
                if (isForce)
                {
                    chromeBrowser.Navigate();
                    Thread.Sleep(delay);
                    WaitPageChanged(chromeBrowser, "dorf2");
                    if (!chromeBrowser.GetCurrentUrl().Contains("dorf2")) return ToDorf2(chromeBrowser, context, accountId, isForce);
                    WaitPageLoaded(chromeBrowser);
                    AfterClicking(chromeBrowser, context, accountId);
                }
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

            Thread.Sleep(delay);
            WaitPageChanged(chromeBrowser, "dorf2");
            if (!chromeBrowser.GetCurrentUrl().Contains("dorf2")) return ToDorf2(chromeBrowser, context, accountId, isForce);
            WaitPageLoaded(chromeBrowser);
            AfterClicking(chromeBrowser, context, accountId);
            return true;
        }

        public static void GoRandomDorf(IChromeBrowser chromeBrowser, AppDbContext context, int accountId)
        {
            var chanceDorf2 = DateTime.Now.Ticks % 100;
            if (chanceDorf2 >= 50)
            {
                ToDorf2(chromeBrowser, context, accountId);
            }
            else
            {
                ToDorf1(chromeBrowser, context, accountId);
            }
        }

        public static bool GoToBuilding(IChromeBrowser chromeBrowser, int index, AppDbContext context, int accountId)
        {
            var currentUrl = chromeBrowser.GetCurrentUrl();
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
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
#elif TTWARS
            var uri = new Uri(currentUrl);
            var serverUrl = $"{uri.Scheme}://{uri.Host}";
            var url = $"{serverUrl}/build.php?id={index}";
            chromeBrowser.Navigate(url);
#else

#error You forgot to define Travian version here

#endif
            var delay = GetDelayClick(context, accountId);

            Thread.Sleep(delay);
            WaitPageChanged(chromeBrowser, $"?id={index}");
            if (!chromeBrowser.GetCurrentUrl().Contains($"?id={index}")) return GoToBuilding(chromeBrowser, index, context, accountId);
            WaitPageLoaded(chromeBrowser);
            AfterClicking(chromeBrowser, context, accountId);
            return true;
        }

        public static void SwitchTab(IChromeBrowser chromeBrowser, int index, AppDbContext context, int accountId)
        {
            while (!CheckHelper.IsCorrectTab(chromeBrowser, index))
            {
                var html = chromeBrowser.GetHtml();
                var listNode = BuildingTab.GetBuildingTabNodes(html);
                if (listNode.Count == 0)
                {
                    throw new Exception("Cannot find building tabs");
                }
                if (index > listNode.Count)
                {
                    throw new Exception($"Tab {index} is invalid, this building only has {listNode.Count} tabs");
                }
                var chrome = chromeBrowser.GetChrome();
                var elements = chrome.FindElements(By.XPath(listNode[index].XPath));
                if (elements.Count == 0)
                {
                    throw new Exception("Cannot find building tabs");
                }
                elements[0].Click();

                var delay = GetDelayClick(context, accountId);
                Thread.Sleep(delay);
                WaitPageLoaded(chromeBrowser);
                AfterClicking(chromeBrowser, context, accountId);
            }
        }

        public static void ToHeroInventory(IChromeBrowser chromeBrowser)
        {
            var html = chromeBrowser.GetHtml();
#if TRAVIAN_OFFICIAL_HEROUI
            var avatar = HeroPage.GetHeroAvatar(html);
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
                var tab = HeroPage.GetHeroTab(doc, 1);
                if (tab is null) return false;
                return tab.IsCurrentTab();
            });
#elif TRAVIAN_OFFICIAL || TTWARS

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
#else

#error You forgot to define Travian version here

#endif
        }

        public static void ToAdventure(IChromeBrowser chromeBrowser)
        {
            var html = chromeBrowser.GetHtml();
#if TRAVIAN_OFFICIAL_HEROUI
            var node = HeroPage.GetAdventuresButton(html);
            if (node is null)
            {
                throw new Exception("Cannot find adventures button");
            }
            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(node.XPath));

            if (elements.Count == 0)
            {
                throw new Exception("Cannot find adventures button");
            }

            elements[0].Click();
            var wait = chromeBrowser.GetWait();
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            wait.Until(driver =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var adventureDiv = doc.GetElementbyId("heroAdventure");
                if (adventureDiv is null) return false;
                var heroState = adventureDiv.Descendants("div").FirstOrDefault(x => x.HasClass("heroState"));
                if (heroState is null) return false;
                return driver.FindElements(By.XPath(heroState.XPath)).Count > 0;
            });
#elif TTWARS || TRAVIAN_OFFICIAL
            var inventory = HeroPage.GetAdventuresButton(html);
            if (inventory is null)
            {
                throw new Exception("Cannot find adventures button");
            }
            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(inventory.XPath));
            if (elements.Count == 0)
            {
                throw new Exception("Cannot find adventures button");
            }
            elements[0].Click();
            var wait = chromeBrowser.GetWait();
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
#else

#error You forgot to define Travian version here

#endif
        }
    }
}