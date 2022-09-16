using MainCore.Tasks.Update;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace MainCore.Tasks.Attack
{
    public class StartFarmList : UpdateFarmList
    {
        public StartFarmList(int accountId, int farmId) : base(accountId)
        {
            _farmId = farmId;
        }

        private readonly int _farmId;
        public int FarmId => _farmId;
        private string _nameFarm;
        public override string Name => $"Start farmlist {_nameFarm}";

        private readonly Random rand = new();

        public override void CopyFrom(BotTask source)
        {
            base.CopyFrom(source);
            using var context = _contextFactory.CreateDbContext();
            var farm = context.Farms.Find(FarmId);
            if (farm is not null) _nameFarm = farm.Name;
            else _nameFarm = "unknow";
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
    }
}