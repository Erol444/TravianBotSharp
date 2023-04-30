using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Parsers.Interface;
using OpenQA.Selenium;
using Splat;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public class TrainTroopsTask : VillageBotTask
    {
        private readonly BuildingEnums _trainingBuilding;
        protected BuildingEnums TrainingBuilding => _trainingBuilding;

        private readonly ITrainTroopParser _trainTroopParser;

        public TrainTroopsTask(int villageId, int accountId, BuildingEnums trainingBuilding, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _trainingBuilding = trainingBuilding;
            _trainTroopParser = Locator.Current.GetService<ITrainTroopParser>();
        }

        public override Result Execute()
        {
            {
                var result = SwitchVillage();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }
            {
                var result = UpdateDorf2();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }

            {
                var result = HasBuilding();
                if (!result)
                {
                    DisableSetting();
                    _logManager.Information(AccountId, "There is no building for training troops");
                    return Result.Ok();
                }
            }
            {
                var result = EnterBuilding(false);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }
            {
                var timeTrain = GetTroopTime();
                var amountTroop = GetAmountTroop(timeTrain);
                var maxTroop = GetMaxTroop();
                if (maxTroop == 0)
                {
                    _logManager.Information(AccountId, $"Don't have enough resource to train troops");
                    ExecuteAt = DateTime.Now.AddHours(1);
                    {
                        var result = UpdateDorf2();
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                        if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                    }
                    return Result.Ok();
                }
                if (maxTroop < amountTroop)
                {
                    amountTroop = maxTroop;
                }
                if (amountTroop == 0)
                {
                    NextExecute();
                    {
                        var result = UpdateDorf2();
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                        if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                    }
                    return Result.Ok();
                }
                else
                {
                    InputAmountTroop(amountTroop);
                }
            }
            NextExecute();
            {
                var result = UpdateDorf2();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }

            return Result.Ok();
        }

        private Result SwitchVillage()
        {
            return _navigateHelper.SwitchVillage(AccountId, VillageId);
        }

        private Result UpdateDorf2()
        {
            return _navigateHelper.ToDorf2(AccountId);
        }

        private bool HasBuilding(bool great = false)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == VillageId);

            var buildingType = great ? TrainingBuilding.GetGreatVersion() : TrainingBuilding;

            return buildings.Any(x => x.Type == TrainingBuilding && x.Level > 0);
        }

        private void DisableSetting(bool great = false)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == VillageId);
            switch (TrainingBuilding)
            {
                case BuildingEnums.Barracks:
                    if (great)
                    {
                        setting.IsGreatBarrack = false;
                    }
                    else
                    {
                        setting.BarrackTroop = 0;
                    }
                    break;

                case BuildingEnums.Stable:
                    if (great)
                    {
                        setting.IsGreatStable = false;
                    }
                    else
                    {
                        setting.StableTroop = 0;
                    }
                    break;

                case BuildingEnums.Workshop:
                    setting.WorkshopTroop = 0;
                    break;

                default:
                    break;
            }
            context.Update(setting);
            context.SaveChanges();
        }

        private Result EnterBuilding(bool great)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == VillageId);

            var buildingType = great ? TrainingBuilding.GetGreatVersion() : TrainingBuilding;
            var building = buildings.FirstOrDefault(x => x.Type == buildingType);
            return _navigateHelper.GoToBuilding(AccountId, building.Id);
        }

        private TimeSpan GetTroopTime()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == VillageId);
            var troopType = 0;
            switch (TrainingBuilding)
            {
                case BuildingEnums.Barracks:
                    troopType = setting.BarrackTroop;
                    break;

                case BuildingEnums.Stable:
                    troopType = setting.StableTroop;
                    break;

                case BuildingEnums.Workshop:
                    troopType = setting.WorkshopTroop;
                    break;

                default:
                    break;
            }

            var doc = _chromeBrowser.GetHtml();
            var nodes = _trainTroopParser.GetTroopNodes(doc);
            HtmlNode troopNode = null;
            foreach (var node in nodes)
            {
                if (_trainTroopParser.GetTroopType(node) == troopType)
                {
                    troopNode = node;
                    break;
                }
            }

            return _trainTroopParser.GetTrainTime(troopNode);
        }

        private int GetAmountTroop(TimeSpan trainTime)
        {
            var doc = _chromeBrowser.GetHtml();
            var timeRemaining = _trainTroopParser.GetQueueTrainTime(doc);

            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == VillageId);
            var timeTrainMinutes = 0;
            switch (TrainingBuilding)
            {
                case BuildingEnums.Barracks:
                    timeTrainMinutes = Random.Shared.Next(setting.BarrackTroopTimeMin, setting.BarrackTroopTimeMax);
                    break;

                case BuildingEnums.Stable:
                    timeTrainMinutes = Random.Shared.Next(setting.StableTroopTimeMin, setting.StableTroopTimeMax);
                    break;

                case BuildingEnums.Workshop:
                    timeTrainMinutes = Random.Shared.Next(setting.WorkshopTroopTimeMin, setting.WorkshopTroopTimeMax);
                    break;

                default:
                    break;
            }

            var timeTrain = TimeSpan.FromMinutes(timeTrainMinutes);

            if (timeRemaining > timeTrain) return 0;

            var timeLeft = timeTrain - timeRemaining;

            var result = (int)(timeLeft.TotalMilliseconds / trainTime.TotalMilliseconds);
            return result > 0 ? result + 1 : result;
        }

        private int GetMaxTroop()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == VillageId);
            var troopType = 0;
            switch (TrainingBuilding)
            {
                case BuildingEnums.Barracks:
                    troopType = setting.BarrackTroop;
                    break;

                case BuildingEnums.Stable:
                    troopType = setting.StableTroop;
                    break;

                case BuildingEnums.Workshop:
                    troopType = setting.WorkshopTroop;
                    break;

                default:
                    break;
            }

            var doc = _chromeBrowser.GetHtml();
            var nodes = _trainTroopParser.GetTroopNodes(doc);
            HtmlNode troopNode = null;
            foreach (var node in nodes)
            {
                if (_trainTroopParser.GetTroopType(node) == troopType)
                {
                    troopNode = node;
                    break;
                }
            }

            return _trainTroopParser.GetMaxAmount(troopNode);
        }

        private void InputAmountTroop(int amountTroop)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == VillageId);
            var troopType = 0;
            switch (TrainingBuilding)
            {
                case BuildingEnums.Barracks:
                    troopType = setting.BarrackTroop;
                    break;

                case BuildingEnums.Stable:
                    troopType = setting.StableTroop;
                    break;

                case BuildingEnums.Workshop:
                    troopType = setting.WorkshopTroop;
                    break;

                default:
                    break;
            }

            var doc = _chromeBrowser.GetHtml();
            var nodes = _trainTroopParser.GetTroopNodes(doc);
            HtmlNode troopNode = null;
            foreach (var node in nodes)
            {
                if (_trainTroopParser.GetTroopType(node) == troopType)
                {
                    troopNode = node;
                    break;
                }
            }

            var input = _trainTroopParser.GetInputBox(troopNode);
            var chrome = _chromeBrowser.GetChrome();
            var inputElements = chrome.FindElements(By.XPath(input.XPath));

            inputElements[0].SendKeys(Keys.Home);
            inputElements[0].SendKeys(Keys.Shift + Keys.End);
            inputElements[0].SendKeys($"{amountTroop}");

            doc = _chromeBrowser.GetHtml();
            var button = _trainTroopParser.GetTrainButton(doc);
            var buttonElements = chrome.FindElements(By.XPath(button.XPath));

            buttonElements[0].Click();
        }

        private void NextExecute()
        {
            var doc = _chromeBrowser.GetHtml();
            var timeRemaining = _trainTroopParser.GetQueueTrainTime(doc);
            var timePass = timeRemaining.TotalMilliseconds * 0.9;
            ExecuteAt = DateTime.Now.AddMilliseconds(timePass);
        }
    }

    public class BarrackTrainTroopsTask : TrainTroopsTask
    {
        public BarrackTrainTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, BuildingEnums.Barracks, cancellationToken)
        {
        }
    }

    public class StableTrainTroopsTask : TrainTroopsTask
    {
        public StableTrainTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, BuildingEnums.Stable, cancellationToken)
        {
        }
    }

    public class WorkshopTrainTroopsTask : TrainTroopsTask
    {
        public WorkshopTrainTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, BuildingEnums.Workshop, cancellationToken)
        {
        }
    }
}