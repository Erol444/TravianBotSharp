using HtmlAgilityPack;
using MainCore.Services;
using OpenQA.Selenium;
using System;

#if TRAVIAN_OFFICIAL

using TravianOfficialCore.FindElements;

#elif TRAVIAN_OFFICIAL_HEROUI

using TravianOfficialNewHeroUICore.FindElements;

#elif TTWARS

using TTWarsCore.FindElements;

#else

#error You forgot to define Travian version here

#endif

namespace MainCore.Helper
{
    public static class ClickHelper
    {
        public static void ClickCompleteNow(IChromeBrowser chromeBrowser)
        {
            var html = chromeBrowser.GetHtml();
            var finishButton = InstantComplete.GetFinishButton(html);
            if (finishButton is null)
            {
                throw new Exception("Cannot find finish button");
            }
            var chrome = chromeBrowser.GetChrome();
            var finishElements = chrome.FindElements(By.XPath(finishButton.XPath));
            if (finishElements.Count == 0)
            {
                throw new Exception("Cannot find finish button");
            }
            finishElements[0].Click();
        }

        public static void WaitDialogFinishNow(IChromeBrowser chromeBrowser)
        {
            var wait = chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var confirmButton = InstantComplete.GetConfirmButton(html);
                return confirmButton is not null;
            });
        }

        public static void ClickConfirmFinishNow(IChromeBrowser chromeBrowser)
        {
            var html = chromeBrowser.GetHtml();
            var finishButton = InstantComplete.GetConfirmButton(html);
            if (finishButton is null)
            {
                throw new Exception("Cannot find confirm button");
            }
            var chrome = chromeBrowser.GetChrome();
            var finishElements = chrome.FindElements(By.XPath(finishButton.XPath));
            if (finishElements.Count == 0)
            {
                throw new Exception("Cannot find confirm button");
            }
            finishElements[0].Click();
        }

        public static void ClickStartAdventure(IChromeBrowser chromeBrowser, int x, int y)
        {
            var html = chromeBrowser.GetHtml();
            var finishButton = HeroPage.GetStartAdventureButton(html, x, y);
            if (finishButton is null)
            {
                throw new Exception("Cannot find start adventure button");
            }
            var chrome = chromeBrowser.GetChrome();
            var finishElements = chrome.FindElements(By.XPath(finishButton.XPath));
            if (finishElements.Count == 0)
            {
                throw new Exception("Cannot find start adventure button");
            }
            finishElements[0].Click();
        }
    }
}