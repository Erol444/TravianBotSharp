using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Helper.Implementations;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace MainCore.Tasks.Misc
{
    public class NPCTask : VillageBotTask
    {
        private Resources _ratio;
        public Resources Ratio => _ratio;

        public void SetRatio(Resources ratio)
        {
            _ratio = ratio;
        }

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly INavigateHelper _navigateHelper;
        private readonly IUpdateHelper _updateHelper;
        private readonly ILogManager _logManager;

        public NPCTask(IDbContextFactory<AppDbContext> contextFactory, INavigateHelper navigateHelper, IUpdateHelper updateHelper, ILogManager logManager)
        {
            _contextFactory = contextFactory;
            _navigateHelper = navigateHelper;
            _updateHelper = updateHelper;
            _logManager = logManager;
        }

        public override Result Execute()
        {
            {
                var result = Update();
                if (result.IsFailed) return result.WithError("from update");
            }

            if (!IsEnoughGold()) return Result.Ok();

            ToMarketPlace();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            ClickNPCButton();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            EnterNumber();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            ClickNPC();
        }

        private Result Update()
        {
            using var context = _contextFactory.CreateDbContext();

            var decider = DateTime.Now.Ticks % 2 == 0;

            if (decider)
            {
                {
                    var result = _navigateHelper.ToDorf2(AccountId);
                    if (result.IsFailed) return result.WithError("from to dorf2");
                }
                {
                    var result = _navigateHelper.SwitchVillage(AccountId, VillageId);
                    if (result.IsFailed) return result.WithError("from switch village");
                }
            }
            else
            {
                {
                    var result = _navigateHelper.SwitchVillage(AccountId, VillageId);
                    if (result.IsFailed) return result.WithError("from switch village");
                }
                {
                    var result = _navigateHelper.ToDorf2(AccountId);
                    if (result.IsFailed) return result.WithError("from to dorf2");
                }
            }

            {
                var result = _updateHelper.UpdateDorf2(AccountId, VillageId);
                if (result.IsFailed) return result.WithError("from update dorf2");
            }
            return Result.Ok();
        }

        private bool IsEnoughGold()
        {
            using var context = _contextFactory.CreateDbContext();
            var info = context.AccountsInfo.Find(AccountId);

            var goldNeeded = 3;
            if (VersionDetector.IsTTWars()) goldNeeded = 5;

            var result = info.Gold > goldNeeded;
            if (!result)
            {
                _logManager.Warning(AccountId, "Not enough gold");
            }
            return result;
        }

        private Result ToMarketPlace()
        {
            using var context = _contextFactory.CreateDbContext();
            var marketplace = context.VillagesBuildings.Where(x => x.VillageId == VillageId).FirstOrDefault(x => x.Type == BuildingEnums.Marketplace && x.Level > 0);
            if (marketplace is null)
            {
                _logManager.Information(AccountId, "Marketplace is missing. Turn off auto NPC to prevent bot detector");
                var setting = context.VillagesSettings.Find(VillageId);
                setting.IsAutoNPC = false;
                context.Update(setting);
                context.SaveChanges();
                return Result.Ok();
            }
            NavigateHelper.GoToBuilding(_chromeBrowser, marketplace.Id, context, AccountId);
            NavigateHelper.SwitchTab(_chromeBrowser, 0, context, AccountId);
        }

        private void ClickNPCButton()
        {
            var html = _chromeBrowser.GetHtml();
            var npcMerchant = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("npcMerchant"));
            var npcButton = npcMerchant.Descendants("button").FirstOrDefault(x => x.HasClass("gold"));
            if (npcButton is null)
            {
                throw new Exception("NPC button is not found");
            }
            var chrome = _chromeBrowser.GetChrome();
            var npcButtonElements = chrome.FindElements(By.XPath(npcButton.XPath));
            if (npcButtonElements.Count == 0)
            {
                throw new Exception("NPC button is not found");
            }
            using var context = _contextFactory.CreateDbContext();
            npcButtonElements.Click(_chromeBrowser, context, AccountId);

            var wait = _chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                if (Cts.IsCancellationRequested) return true;
                var waitHtml = new HtmlDocument();
                waitHtml.LoadHtml(driver.PageSource);
                return waitHtml.GetElementbyId("npc_market_button") is not null;
            });
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
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
            var nodeSum = html.GetElementbyId($"sum");
#elif TTWARS
            var nodeSum = html.GetElementbyId($"org4");
#else
#error You forgot to define Travian version here
#endif
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
            for (int i = 0; i < 4; i++)
            {
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
                var script = $"document.getElementsByName('desired{i}')[0].value = {current[i]};";

#elif TTWARS
                var script = $"document.getElementById('m2[{i}]').value = {current[i]};";
#else
#error You forgot to define Travian version here
#endif
                chrome.ExecuteScript(script);
            }
        }

        private void ClickNPC()
        {
            var html = _chromeBrowser.GetHtml();
            var submit = html.GetElementbyId("submitText");
            var distribute = submit.Descendants("button").FirstOrDefault();
            if (distribute is null)
            {
                throw new Exception("NPC submit button is not found");
            }
            var chrome = _chromeBrowser.GetChrome();
            var distributeElements = chrome.FindElements(By.XPath(distribute.XPath));
            if (distributeElements.Count == 0)
            {
                throw new Exception("NPC submit button is not found");
            }
            using var context = _contextFactory.CreateDbContext();
            distributeElements.Click(_chromeBrowser, context, AccountId);

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
                throw new Exception("NPC submit button is not found");
            }
            submitElements.Click(_chromeBrowser, context, AccountId);
        }
    }
}