using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class NPCHelper : INPCHelper
    {
        protected readonly IGeneralHelper _generalHelper;

        protected readonly IChromeManager _chromeManager;
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;

        protected Result _result;
        protected int _villageId;
        protected int _accountId;
        protected CancellationToken _token;
        protected IChromeBrowser _chromeBrowser;

        public NPCHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory)
        {
            _chromeManager = chromeManager;
            _generalHelper = generalHelper;
            _contextFactory = contextFactory;
        }

        public void Load(int villageId, int accountId, CancellationToken cancellationToken)
        {
            _villageId = villageId;
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);

            _generalHelper.Load(villageId, accountId, cancellationToken);
        }

        public Result Execute(Resources ratio)
        {
            _result = _generalHelper.ToDorf2(true);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());

            _result = CheckGold();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());

            _result = ToMarketPlace();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());

            _result = ClickNPCButton();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());

            _result = EnterNumber(ratio);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());

            _result = ClickNPC();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());
            return Result.Ok();
        }

        protected abstract Result CheckGold();

        protected Result ToMarketPlace()
        {
            using var context = _contextFactory.CreateDbContext();
            var marketplace = context.VillagesBuildings.Where(x => x.VillageId == _villageId).FirstOrDefault(x => x.Type == BuildingEnums.Marketplace && x.Level > 0);
            if (marketplace is null)
            {
                var setting = context.VillagesSettings.Find(_villageId);
                setting.IsAutoNPC = false;
                context.Update(setting);
                context.SaveChanges();
                return Result.Fail(new Skip("Marketplace is missing"));
            }

            _result = _generalHelper.ToBuilding(marketplace.Id);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.SwitchTab(0);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

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

            _result = _generalHelper.Click(By.XPath(npcButton.XPath), waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.Wait(driver =>
            {
                var waitHtml = new HtmlDocument();
                waitHtml.LoadHtml(driver.PageSource);
                return waitHtml.GetElementbyId("npc_market_button") is not null;
            });
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        protected abstract Result EnterNumber(Resources ratio);

        protected Result ClickNPC()
        {
            var html = _chromeBrowser.GetHtml();
            var submit = html.GetElementbyId("submitText");
            var distribute = submit.Descendants("button").FirstOrDefault();
            if (distribute is null)
            {
                return Result.Fail(new Retry("NPC submit button is not found"));
            }
            _result = _generalHelper.Click(By.XPath(distribute.XPath), waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.Wait(driver =>
            {
                var buttons = driver.FindElements(By.Id("npc_market_button"));
                if (buttons.Count == 0) return false;
                return buttons[0].Displayed && buttons[0].Enabled;
            });
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));

            _result = _generalHelper.Click(By.Id("npc_market_button"), waitPageLoaded: false);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}