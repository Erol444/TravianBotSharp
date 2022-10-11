using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Helper;
using MainCore.Models.Runtime;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace MainCore.Tasks.Misc
{
    public class NPCTask : VillageBotTask
    {
        public NPCTask(int villageId, int accountId) : base(villageId, accountId, "NPC Task")
        {
        }

        public NPCTask(int villageId, int accountId, Resources ratio) : base(villageId, accountId, "NPC Task")
        {
            _ratio = ratio;
        }

        private readonly Resources _ratio;

        public override void Execute()
        {
            StopFlag = false;

            Update();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            if (!IsContinue()) return;

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

        private void Update()
        {
            using var context = _contextFactory.CreateDbContext();

            var decider = DateTime.Now.Ticks % 2 == 0;

            if (decider)
            {
                NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
                NavigateHelper.SwitchVillage(context, _chromeBrowser, VillageId, AccountId);
            }
            else
            {
                NavigateHelper.SwitchVillage(context, _chromeBrowser, VillageId, AccountId);
                NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
            }

            UpdateHelper.UpdateDorf2(context, _chromeBrowser, AccountId, VillageId);
        }

        private bool IsContinue()
        {
            using var context = _contextFactory.CreateDbContext();
            var info = context.AccountsInfo.Find(AccountId);

#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
            var result = info.Gold > 3;
#elif TTWARS
            var result = info.Gold > 5;

#else

#error You forgot to define Travian version here

#endif
            if (!result)
            {
                _logManager.Warning(AccountId, "Not enough gold");
            }
            return result;
        }

        private void ToMarketPlace()
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
                StopFlag = true;
                return;
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

            npcButtonElements[0].Click();

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
            distributeElements[0].Click();

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
            submitElements[0].Click();
        }
    }
}