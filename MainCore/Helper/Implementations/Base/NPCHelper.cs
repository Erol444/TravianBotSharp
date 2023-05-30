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

namespace MainCore.Helper.Implementations.Base
{
    public abstract class NPCHelper : INPCHelper
    {
        protected readonly IGeneralHelper _generalHelper;

        protected readonly IChromeManager _chromeManager;
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;

        public NPCHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory)
        {
            _chromeManager = chromeManager;
            _generalHelper = generalHelper;
            _contextFactory = contextFactory;
        }

        public abstract bool IsEnoughGold(int accountId);

        public abstract Result EnterNumber(int accountId, int villageId, Resources ratio);

        public Result Execute(int accountId, int villageId, Resources ratio)
        {
            var result = _generalHelper.SwitchVillage(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.ToDorf2(accountId, true);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = CheckGold(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = ToMarketPlace(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = ClickNPCButton(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = EnterNumber(accountId, villageId: villageId, ratio);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = ClickNPC(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result CheckGold(int accountId)
        {
            return Result.OkIf(IsEnoughGold(accountId), new Skip("Not enough gold"));
        }

        public Result ToMarketPlace(int accountId, int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var marketplace = context.VillagesBuildings.Where(x => x.VillageId == villageId).FirstOrDefault(x => x.Type == BuildingEnums.Marketplace && x.Level > 0);
            if (marketplace is null)
            {
                var setting = context.VillagesSettings.Find(villageId);
                setting.IsAutoNPC = false;
                context.Update(setting);
                context.SaveChanges();
                return Result.Fail(new Skip("Marketplace is missing"));
            }

            var result = _generalHelper.ToBuilding(accountId, marketplace.Id);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.SwitchTab(accountId, 0);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        public Result ClickNPCButton(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var npcMerchant = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("npcMerchant"));
            var npcButton = npcMerchant.Descendants("button").FirstOrDefault(x => x.HasClass("gold"));
            if (npcButton is null)
            {
                return Result.Fail(new Retry("NPC button is not found"));
            }

            var result = _generalHelper.Click(accountId, By.XPath(npcButton.XPath), waitPageLoaded: false);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Wait(accountId, driver =>
            {
                var waitHtml = new HtmlDocument();
                waitHtml.LoadHtml(driver.PageSource);
                return waitHtml.GetElementbyId("npc_market_button") is not null;
            });
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result ClickNPC(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var submit = html.GetElementbyId("submitText");
            var distribute = submit.Descendants("button").FirstOrDefault();
            if (distribute is null)
            {
                return Result.Fail(new Retry("NPC submit button is not found"));
            }
            var result = _generalHelper.Click(accountId, By.XPath(distribute.XPath), waitPageLoaded: false);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Wait(accountId, driver =>
            {
                var buttons = driver.FindElements(By.Id("npc_market_button"));
                if (buttons.Count == 0) return false;
                return buttons[0].Displayed && buttons[0].Enabled;
            });
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Click(accountId, By.Id("npc_market_button"), waitPageLoaded: false);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}