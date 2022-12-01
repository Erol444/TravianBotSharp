using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using ServerModuleCore.Parser;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations
{
    public class ClickHelper : IClickHelper
    {
        private readonly IChromeManager _chromeManager;
        private readonly IVillageCurrentlyBuildingParser _villageCurrentlyBuildingParser;
        private readonly IHeroSectionParser _heroSectionParser;
        private readonly INavigateHelper _navigateHelper;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ClickHelper(IVillageCurrentlyBuildingParser villageCurrentlyBuildingParser, IChromeManager chromeManager, IHeroSectionParser heroSectionParser, INavigateHelper navigateHelper, IDbContextFactory<AppDbContext> contextFactory)
        {
            _villageCurrentlyBuildingParser = villageCurrentlyBuildingParser;
            _chromeManager = chromeManager;
            _heroSectionParser = heroSectionParser;
            _navigateHelper = navigateHelper;
            _contextFactory = contextFactory;
        }

        public Result ClickCompleteNow(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var finishButton = _villageCurrentlyBuildingParser.GetFinishButton(html);
            if (finishButton is null)
            {
                return Result.Fail(new MustRetry("Cannot find finish button"));
            }
            var chrome = chromeBrowser.GetChrome();
            var finishElements = chrome.FindElements(By.XPath(finishButton.XPath));
            if (finishElements.Count == 0)
            {
                return Result.Fail(new MustRetry("Cannot find finish button"));
            }
            {
                var result = _navigateHelper.Click(accountId, finishElements[0]);
                if (result.IsFailed) return result.WithError("from click finish now");
            }
            return Result.Ok();
        }

        public void WaitDialogFinishNow(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var wait = chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var confirmButton = _villageCurrentlyBuildingParser.GetConfirmFinishNowButton(html);
                return confirmButton is not null;
            });
        }

        public Result ClickConfirmFinishNow(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var finishButton = _villageCurrentlyBuildingParser.GetConfirmFinishNowButton(html);
            if (finishButton is null)
            {
                return Result.Fail(new MustRetry("Cannot find confirm button"));
            }
            var chrome = chromeBrowser.GetChrome();
            var finishElements = chrome.FindElements(By.XPath(finishButton.XPath));
            if (finishElements.Count == 0)
            {
                return Result.Fail(new MustRetry("Cannot find confirm button"));
            }
            {
                var result = _navigateHelper.Click(accountId, finishElements[0]);
                if (result.IsFailed) return result.WithError("from click confirm finish now");
            }
            return Result.Ok();
        }

        public Result ClickStartAdventure(int accountId, int x, int y)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var finishButton = _heroSectionParser.GetStartAdventureButton(html, x, y);
            if (finishButton is null)
            {
                return Result.Fail(new MustRetry("Cannot find start adventure button"));
            }
            var chrome = chromeBrowser.GetChrome();
            var finishElements = chrome.FindElements(By.XPath(finishButton.XPath));
            if (finishElements.Count == 0)
            {
                return Result.Fail(new MustRetry("Cannot find start adventure button"));
            }

            {
                var result = _navigateHelper.Click(accountId, finishElements[0]);
                if (result.IsFailed) return result.WithError("from click start adventure");
            }

            if (VersionDetector.IsTTWars())
            {
                var wait = chromeBrowser.GetWait();
                wait.Until(driver =>
                {
                    var elements = driver.FindElements(By.Id("start"));
                    if (elements.Count == 0) return false;
                    return elements[0].Enabled && elements[0].Displayed;
                });

                var elements = chrome.FindElements(By.Id("start"));
                elements[0].Click();

                wait.Until(driver =>
                {
                    var elements = driver.FindElements(By.Id("ok"));
                    if (elements.Count == 0) return false;
                    return elements[0].Enabled && elements[0].Displayed;
                });
            }
            return Result.Ok();
        }

        public Result ClickStartFarm(int accountId, int farmId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            if (VersionDetector.IsTravianOfficial())
            {
                var farmNode = html.GetElementbyId($"raidList{farmId}");
                if (farmNode is null)
                {
                    return Result.Fail(new MustRetry("Cannot found farm node"));
                }
                var startNode = farmNode.Descendants("button").FirstOrDefault(x => x.HasClass("startButton"));
                if (startNode is null)
                {
                    return Result.Fail(new MustRetry("Cannot found start button"));
                }
                var startElements = chromeBrowser.GetChrome().FindElements(By.XPath(startNode.XPath));
                if (startElements.Count == 0)
                {
                    return Result.Fail(new MustRetry("Cannot found start button"));
                }
                {
                    var result = _navigateHelper.Click(accountId, startElements[0]);
                    if (result.IsFailed) return result.WithError("from click start farm");
                }
            }
            else if (VersionDetector.IsTTWars())
            {
                using var context = _contextFactory.CreateDbContext();
                var setting = context.AccountsSettings.Find(accountId);

                var chrome = chromeBrowser.GetChrome();
                chrome.ExecuteScript($"Travian.Game.RaidList.toggleList({farmId});");

                var wait = chromeBrowser.GetWait();
                wait.Until(driver =>
                {
                    var waitHtml = new HtmlDocument();
                    waitHtml.LoadHtml(driver.PageSource);

                    var waitFarmNode = waitHtml.GetElementbyId($"list{farmId}");
                    var table = waitFarmNode.Descendants("div").FirstOrDefault(x => x.HasClass("listContent"));
                    return !table.HasClass("hide");
                });

                var delay = Random.Shared.Next(setting.ClickDelayMin, setting.ClickDelayMax);
                Thread.Sleep(delay);

                var checkboxAlls = chrome.FindElements(By.Id($"raidListMarkAll{farmId}"));
                if (checkboxAlls.Count == 0)
                {
                    return Result.Fail(new MustRetry("Cannot find check all check box"));
                }

                {
                    var result = _navigateHelper.Click(accountId, checkboxAlls[0]);
                    if (result.IsFailed) return result.WithError("from click check all");
                }

                delay = Random.Shared.Next(setting.ClickDelayMin, setting.ClickDelayMax);
                Thread.Sleep(delay);

                var farmNode = html.GetElementbyId($"list{farmId}");
                var buttonStartFarm = farmNode.Descendants("button").FirstOrDefault(x => x.HasClass("green") && x.GetAttributeValue("type", "").Contains("submit"));
                if (buttonStartFarm is null)
                {
                    return Result.Fail(new MustRetry("Cannot find button start farmlist"));
                }
                var buttonStartFarms = chrome.FindElements(By.XPath(buttonStartFarm.XPath));
                if (buttonStartFarms.Count == 0)
                {
                    return Result.Fail(new MustRetry("Cannot find button start farmlist"));
                }

                {
                    var result = _navigateHelper.Click(accountId, buttonStartFarms[0]);
                    if (result.IsFailed) return result.WithError("from click start farm");
                }
                {
                    var result = _navigateHelper.SwitchTab(accountId, 1);
                    if (result.IsFailed) return result.WithError("from switch tab");
                }
            }
            return Result.Ok();
        }
    }
}