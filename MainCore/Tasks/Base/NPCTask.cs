using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Models.Runtime;
using MainCore.Parsers.Interface;
using OpenQA.Selenium;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public abstract class NPCTask : VillageBotTask
    {
        protected readonly ISystemPageParser _systemPageParser;

        public NPCTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _systemPageParser = Locator.Current.GetService<ISystemPageParser>();
        }

        public NPCTask(int villageId, int accountId, Resources ratio, CancellationToken cancellationToken = default) : this(villageId, accountId, cancellationToken)

        {
            _ratio = ratio;
        }

        protected readonly Resources _ratio;

        public override Result Execute()
        {
            var commands = new List<Func<Result>>()
            {
                Update,
                CheckGold,
                ToMarketPlace,
                ClickNPCButton,
                EnterNumber,
                ClickNPC,
            };

            foreach (var command in commands)
            {
                _logManager.Information(AccountId, $"[{GetName()}] Execute {command.Method.Name}");
                var result = command.Invoke();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }

            return Result.Ok();
        }

        protected Result Update()
        {
            var updateDorf2 = new UpdateDorf2(VillageId, AccountId, CancellationToken);
            var result = updateDorf2.Execute();
            return result;
        }

        protected abstract Result CheckGold();

        protected Result ToMarketPlace()
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
                return Result.Fail(new Skip("Marketplace is missing"));
            }

            {
                var result = _generalHelper.GoToBuilding(AccountId, marketplace.Id);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = _generalHelper.SwitchTab(AccountId, 0);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        protected Result ClickNPCButton()
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
            _generalHelper.Click(AccountId, npcButtonElements[0]);
            var wait = _chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var waitHtml = new HtmlDocument();
                waitHtml.LoadHtml(driver.PageSource);
                return waitHtml.GetElementbyId("npc_market_button") is not null;
            });
            return Result.Ok();
        }

        protected abstract Result EnterNumber();

        protected Result ClickNPC()
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
                var result = _generalHelper.Click(AccountId, distributeElements[0]);
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
                var result = _generalHelper.Click(AccountId, submitElements[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }
    }
}