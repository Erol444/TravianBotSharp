using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Models.Runtime;
using MainCore.Tasks.Update;
using ModuleCore.Parser;
using OpenQA.Selenium;
using Splat;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Misc
{
    public class NPCTask : VillageBotTask
    {
        private readonly ISystemPageParser _systemPageParser;

        public NPCTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _systemPageParser = Locator.Current.GetService<ISystemPageParser>();
        }

        public NPCTask(int villageId, int accountId, Resources ratio, CancellationToken cancellationToken = default) : this(villageId, accountId, cancellationToken)

        {
            _ratio = ratio;
        }

        private readonly Resources _ratio;

        public override Result Execute()
        {
            {
                var updateDorf2 = new UpdateDorf2(VillageId, AccountId, CancellationToken);
                var result = updateDorf2.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            if (!IsContinue()) return Result.Ok();

            {
                var result = ToMarketPlace();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            {
                var result = ClickNPCButton();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            EnterNumber();
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            {
                var result = ClickNPC();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        private bool IsContinue()
        {
            using var context = _contextFactory.CreateDbContext();
            var info = context.AccountsInfo.Find(AccountId);

            var goldNeed = 0;
            if (VersionDetector.IsTravianOfficial())
            {
                goldNeed = 3;
            }
            else if (VersionDetector.IsTTWars())
            {
                goldNeed = 5;
            }
            var result = info.Gold > goldNeed;
            if (!result)
            {
                _logManager.Warning(AccountId, "Not enough gold", this);
            }
            return result;
        }

        private Result ToMarketPlace()
        {
            using var context = _contextFactory.CreateDbContext();
            var marketplace = context.VillagesBuildings.Where(x => x.VillageId == VillageId).FirstOrDefault(x => x.Type == BuildingEnums.Marketplace && x.Level > 0);
            if (marketplace is null)
            {
                _logManager.Information(AccountId, "Marketplace is missing. Turn off auto NPC to prevent bot detector", this);
                var setting = context.VillagesSettings.Find(VillageId);
                setting.IsAutoNPC = false;
                context.Update(setting);
                context.SaveChanges();
                return Result.Fail(new Skip());
            }

            {
                var result = _navigateHelper.GoToBuilding(AccountId, marketplace.Id);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = _navigateHelper.SwitchTab(AccountId, 0);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        private Result ClickNPCButton()
        {
            var html = _chromeBrowser.GetHtml();
            var npcMerchant = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("npcMerchant"));
            var npcButton = npcMerchant.Descendants("button").FirstOrDefault(x => x.HasClass("gold"));
            if (npcButton is null)
            {
                return Result.Fail(new Retry("NPC button is not found"));
            }
            var chrome = _chromeBrowser.GetChrome();
            var npcButtonElements = chrome.FindElements(By.XPath(npcButton.XPath));
            if (npcButtonElements.Count == 0)
            {
                return Result.Fail(new Retry("NPC button is not found"));
            }
            _navigateHelper.Click(AccountId, npcButtonElements[0]);
            var wait = _chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var waitHtml = new HtmlDocument();
                waitHtml.LoadHtml(driver.PageSource);
                return waitHtml.GetElementbyId("npc_market_button") is not null;
            });
            return Result.Ok();
        }

        private void EnterNumber()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);
            var ratio = new int[4];
            if (_ratio is null)
            {
                ratio[0] = setting.AutoNPCWood;
                ratio[1] = setting.AutoNPCClay;
                ratio[2] = setting.AutoNPCIron;
                ratio[3] = setting.AutoNPCCrop;
            }
            else
            {
                ratio[0] = _ratio.Wood;
                ratio[1] = _ratio.Clay;
                ratio[2] = _ratio.Iron;
                ratio[3] = _ratio.Crop;
            }
            var ratioSum = ratio.Sum();

            if (ratioSum == 0)
            {
                Array.ForEach(ratio, x => x = 1);
                ratioSum = 4;
            }

            var html = _chromeBrowser.GetHtml();
            var nodeSum = _systemPageParser.GetNpcSumNode(html);
            var sumCurrent = nodeSum.InnerText.ToNumeric();
            var current = new long[4];
            for (var i = 0; i < 4; i++)
            {
                current[i] = (long)(sumCurrent * ratio[i]) / ratioSum;
            }
            var sum = current.Sum();
            var diff = sumCurrent - sum;
            current[3] += diff;

            var chrome = _chromeBrowser.GetChrome();
            var script = "";
            if (VersionDetector.IsTravianOfficial())
            {
                for (int i = 0; i < 4; i++)
                {
                    script = $"document.getElementsByName('desired{i}')[0].value = {current[i]};";
                    chrome.ExecuteScript(script);
                }
            }
            else if (VersionDetector.IsTTWars())
            {
                for (int i = 0; i < 4; i++)
                {
                    script = $"document.getElementById('m2[{i}]').value = {current[i]};";
                    chrome.ExecuteScript(script);
                }
            }
        }

        private Result ClickNPC()
        {
            var html = _chromeBrowser.GetHtml();
            var submit = html.GetElementbyId("submitText");
            var distribute = submit.Descendants("button").FirstOrDefault();
            if (distribute is null)
            {
                return Result.Fail(new Retry("NPC submit button is not found"));
            }
            var chrome = _chromeBrowser.GetChrome();
            var distributeElements = chrome.FindElements(By.XPath(distribute.XPath));
            if (distributeElements.Count == 0)
            {
                return Result.Fail(new Retry("NPC submit button is not found"));
            }
            {
                var result = _navigateHelper.Click(AccountId, distributeElements[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            var wait = _chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var buttons = driver.FindElements(By.Id("npc_market_button"));
                if (buttons.Count == 0) return false;
                return buttons[0].Displayed && buttons[0].Enabled;
            });

            var submitElements = chrome.FindElements(By.Id("npc_market_button"));
            if (submitElements.Count == 0)
            {
                return Result.Fail(new Retry("NPC submit button is not found"));
            }
            {
                var result = _navigateHelper.Click(AccountId, submitElements[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }
    }
}