using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations.TTWars
{
    public class ClickHelper : Base.ClickHelper
    {
        public ClickHelper(IVillageCurrentlyBuildingParser villageCurrentlyBuildingParser, IChromeManager chromeManager, IHeroSectionParser heroSectionParser, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory) : base(villageCurrentlyBuildingParser, chromeManager, heroSectionParser, generalHelper, contextFactory)
        {
        }

        public override Result ClickStartAdventure(int accountId, int x, int y)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var finishButton = _heroSectionParser.GetStartAdventureButton(html, x, y);
            if (finishButton is null)
            {
                return Result.Fail(new Retry("Cannot find start adventure button"));
            }
            var chrome = chromeBrowser.GetChrome();
            var finishElements = chrome.FindElements(By.XPath(finishButton.XPath));
            if (finishElements.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find start adventure button"));
            }

            {
                var result = _generalHelper.Click(accountId, finishElements[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            var wait = chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var elements = driver.FindElements(By.Id("start"));
                if (elements.Count == 0) return false;
                return elements[0].Enabled && elements[0].Displayed;
            });

            {
                var elements = chrome.FindElements(By.Id("start"));
                var result = _generalHelper.Click(accountId, elements[0]);
                if (result.IsFailed) return result.WithError("from click start adventure");
            }

            wait.Until(driver =>
            {
                var elements = driver.FindElements(By.Id("ok"));
                if (elements.Count == 0) return false;
                return elements[0].Enabled && elements[0].Displayed;
            });

            return Result.Ok();
        }

        public override Result ClickStartFarm(int accountId, int farmId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

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
                return Result.Fail(new Retry("Cannot find check all check box"));
            }

            {
                var result = _generalHelper.Click(accountId, checkboxAlls[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            delay = Random.Shared.Next(setting.ClickDelayMin, setting.ClickDelayMax);
            Thread.Sleep(delay);

            var farmNode = html.GetElementbyId($"list{farmId}");
            var buttonStartFarm = farmNode.Descendants("button").FirstOrDefault(x => x.HasClass("green") && x.GetAttributeValue("type", "").Contains("submit"));
            if (buttonStartFarm is null)
            {
                return Result.Fail(new Retry("Cannot find button start farmlist"));
            }
            var buttonStartFarms = chrome.FindElements(By.XPath(buttonStartFarm.XPath));
            if (buttonStartFarms.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find button start farmlist"));
            }

            {
                var result = _generalHelper.Click(accountId, buttonStartFarms[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = _generalHelper.SwitchTab(accountId, 1);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }
    }
}