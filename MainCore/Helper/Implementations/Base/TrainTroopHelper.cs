using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public class TrainTroopHelper : ITrainTroopHelper
    {
        private Result _result;
        private int _villageId;
        private int _accountId;
        private CancellationToken _token;

        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly IGeneralHelper _generalHelper;
        private readonly ILogManager _logManager;

        private readonly IChromeManager _chromeManager;

        private readonly ITrainTroopParser _trainTroopParser;

        private IChromeBrowser _chromeBrowser;

        public TrainTroopHelper(IDbContextFactory<AppDbContext> contextFactory, IGeneralHelper generalHelper, ILogManager logManager, IChromeManager chromeManager, ITrainTroopParser trainTroopParser)
        {
            _contextFactory = contextFactory;
            _generalHelper = generalHelper;
            _logManager = logManager;
            _chromeManager = chromeManager;
            _trainTroopParser = trainTroopParser;
        }

        public void Load(int villageId, int accountId, CancellationToken cancellationToken)
        {
            _villageId = villageId;
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);
        }

        public Result Execute(BuildingEnums trainBuilding)
        {
            _result = _generalHelper.ToDorf2(_accountId);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());

            var buildingLoc = GetBuilding(trainBuilding);
            if (buildingLoc == -1)
            {
                DisableSetting(trainBuilding);
                _logManager.Information(_accountId, $"There is no {trainBuilding} in village");
                return Result.Ok();
            }

            _result = _generalHelper.ToBuilding(_accountId, buildingLoc);
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            if (_token.IsCancellationRequested) return Result.Fail(new Cancel());

            var troop = GetTroopTraining(trainBuilding);

            var timeTrain = GetTroopTime(troop);
            var amountTroop = GetAmountTroop(trainBuilding, timeTrain);
            var maxTroop = GetMaxTroop(troop);
            if (maxTroop == 0)
            {
                return Result.Fail(NoResource.Train(trainBuilding));
            }

            if (maxTroop < amountTroop)
            {
                amountTroop = maxTroop;
            }

            if (amountTroop == 0)
            {
                return Result.Fail(NoResource.Train(trainBuilding));
            }

            InputAmountTroop(troop, amountTroop);
            return Result.Ok();
        }

        private int GetBuilding(BuildingEnums trainBuilding)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == _villageId);

            var building = buildings.FirstOrDefault(x => x.Type == trainBuilding && x.Level > 0);
            if (building is null) return -1;
            return building.Id;
        }

        private void DisableSetting(BuildingEnums trainBuilding)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == _villageId);
            switch (trainBuilding)
            {
                case BuildingEnums.Barracks:
                    setting.BarrackTroop = 0;
                    break;

                case BuildingEnums.GreatBarracks:
                    setting.IsGreatBarrack = false;
                    break;

                case BuildingEnums.Stable:
                    setting.StableTroop = 0;
                    break;

                case BuildingEnums.GreatStable:
                    setting.IsGreatStable = false;
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

        private int GetTroopTraining(BuildingEnums trainBuilding)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == _villageId);
            return trainBuilding switch
            {
                BuildingEnums.Barracks or BuildingEnums.GreatBarracks => setting.BarrackTroop,
                BuildingEnums.Stable or BuildingEnums.GreatStable => setting.StableTroop,
                BuildingEnums.Workshop => setting.WorkshopTroop,
                _ => -1,
            };
        }

        private TimeSpan GetTroopTime(int troop)
        {
            var doc = _chromeBrowser.GetHtml();
            var nodes = _trainTroopParser.GetTroopNodes(doc);
            HtmlNode troopNode = null;
            foreach (var node in nodes)
            {
                if (_trainTroopParser.GetTroopType(node) == troop)
                {
                    troopNode = node;
                    break;
                }
            }

            return _trainTroopParser.GetTrainTime(troopNode);
        }

        private int GetAmountTroop(BuildingEnums trainBuilding, TimeSpan trainTime)
        {
            var doc = _chromeBrowser.GetHtml();
            var timeRemaining = _trainTroopParser.GetQueueTrainTime(doc);

            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == _villageId);
            var timeTrainMinutes = 0;
            switch (trainBuilding)
            {
                case BuildingEnums.Barracks:
                case BuildingEnums.GreatBarracks:
                    timeTrainMinutes = Random.Shared.Next(setting.BarrackTroopTimeMin, setting.BarrackTroopTimeMax);
                    break;

                case BuildingEnums.Stable:
                case BuildingEnums.GreatStable:
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

        private int GetMaxTroop(int troop)
        {
            var doc = _chromeBrowser.GetHtml();
            var nodes = _trainTroopParser.GetTroopNodes(doc);
            HtmlNode troopNode = null;
            foreach (var node in nodes)
            {
                if (_trainTroopParser.GetTroopType(node) == troop)
                {
                    troopNode = node;
                    break;
                }
            }

            return _trainTroopParser.GetMaxAmount(troopNode);
        }

        private void InputAmountTroop(int troop, int amountTroop)
        {
            var doc = _chromeBrowser.GetHtml();
            var nodes = _trainTroopParser.GetTroopNodes(doc);
            HtmlNode troopNode = null;
            foreach (var node in nodes)
            {
                if (_trainTroopParser.GetTroopType(node) == troop)
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
    }
}