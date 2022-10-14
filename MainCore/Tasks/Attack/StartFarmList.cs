using MainCore.Services;
using MainCore.Tasks.Update;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Linq;

#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI

#elif TTWARS

using MainCore.Helper;
using HtmlAgilityPack;
using System.Threading;

#else

#error You forgot to define Travian version here

#endif

namespace MainCore.Tasks.Attack
{
    public class StartFarmList : UpdateFarmList
    {
        public StartFarmList(int accountId, int farmId) : base(accountId, "Start farmlist")
        {
            _farmId = farmId;
        }

        private readonly int _farmId;
        public int FarmId => _farmId;
        private string _nameFarm;

        private readonly Random rand = new();

        public override void CopyFrom(BotTask source)
        {
            base.CopyFrom(source);
            using var context = _contextFactory.CreateDbContext();
            var farm = context.Farms.Find(FarmId);
            if (farm is not null) _nameFarm = farm.Name;
            else _nameFarm = "unknow";

            Name = $"{Name} {_nameFarm}";
        }

        public override void SetService(IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, EventManager EventManager, ILogManager logManager, IPlanManager planManager, IRestClientManager restClientManager)
        {
            base.SetService(contextFactory, chromeBrowser, taskManager, EventManager, logManager, planManager, restClientManager);
            using var context = _contextFactory.CreateDbContext();
            var farm = context.Farms.Find(FarmId);
            if (farm is not null) _nameFarm = farm.Name;
            else _nameFarm = "unknow";

            Name = $"{Name} {_nameFarm}";
        }

        public override void Execute()
        {
            base.Execute();

            if (!IsFarmExist())
            {
                _logManager.Warning(AccountId, $"Farm {FarmId} is missing. Remove this farm from queue");
                return;
            }
            if (IsFarmDeactive())
            {
                _logManager.Warning(AccountId, $"Farm {FarmId} is deactive. Remove this farm from queue");
                return;
            }
            if (Cts.IsCancellationRequested) return;

            ClickStartFarm();
            if (Cts.IsCancellationRequested) return;

            {
                using var context = _contextFactory.CreateDbContext();
                var setting = context.FarmsSettings.Find(FarmId);
                var time = rand.Next(setting.IntervalMin, setting.IntervalMax);
                ExecuteAt = DateTime.Now.AddSeconds(time);
                _logManager.Information(AccountId, $"Farmlist {_nameFarm} was sent.");
            }
        }

        private bool IsFarmExist()
        {
            using var context = _contextFactory.CreateDbContext();
            var farm = context.Farms.Find(FarmId);
            return farm is not null;
        }

        private bool IsFarmDeactive()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.FarmsSettings.Find(FarmId);
            if (!setting.IsActive) return true;
            return false;
        }

#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI

        private void ClickStartFarm()
        {
            var html = _chromeBrowser.GetHtml();
            var farmNode = html.GetElementbyId($"raidList{FarmId}");
            if (farmNode is null) throw new Exception("Cannot found farm node");
            var startNode = farmNode.Descendants("button").FirstOrDefault(x => x.HasClass("startButton"));
            if (startNode is null) throw new Exception("Cannot found start button");
            var startElements = _chromeBrowser.GetChrome().FindElements(By.XPath(startNode.XPath));
            if (startElements.Count == 0) throw new Exception("Cannot found start button");
            startElements[0].Click();
        }

#elif TTWARS

        private void ClickStartFarm()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(AccountId);

            var chrome = _chromeBrowser.GetChrome();
            chrome.ExecuteScript($"Travian.Game.RaidList.toggleList({FarmId});");

            var wait = _chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var waitHtml = new HtmlDocument();
                waitHtml.LoadHtml(driver.PageSource);

                var waitFarmNode = waitHtml.GetElementbyId($"list{FarmId}");
                var table = waitFarmNode.Descendants("div").FirstOrDefault(x => x.HasClass("listContent"));
                return !table.HasClass("hide");
            });

            var delay = rand.Next(setting.ClickDelayMin, setting.ClickDelayMax);
            Thread.Sleep(delay);

            var checkboxAlls = chrome.FindElements(By.Id($"raidListMarkAll{FarmId}"));
            if (checkboxAlls.Count == 0)
            {
                throw new Exception("Cannot find check all check box");
            }
            checkboxAlls[0].Click();

            delay = rand.Next(setting.ClickDelayMin, setting.ClickDelayMax);
            Thread.Sleep(delay);

            var html = _chromeBrowser.GetHtml();
            var farmNode = html.GetElementbyId($"list{FarmId}");
            var buttonStartFarm = farmNode.Descendants("button").FirstOrDefault(x => x.HasClass("green") && x.GetAttributeValue("type", "").Contains("submit"));
            if (buttonStartFarm is null)
            {
                throw new Exception("Cannot find button start farmlist");
            }
            var buttonStartFarms = chrome.FindElements(By.XPath(buttonStartFarm.XPath));
            if (buttonStartFarms.Count == 0)
            {
                throw new Exception("Cannot find button start farmlist");
            }
            buttonStartFarms[0].Click();

            NavigateHelper.SwitchTab(_chromeBrowser, 1, context, AccountId);
        }

#else

#error You forgot to define Travian version here

#endif
    }
}