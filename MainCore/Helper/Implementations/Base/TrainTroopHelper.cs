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

namespace MainCore.Helper.Implementations.Base
{
    public sealed class TrainTroopHelper : ITrainTroopHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly IGeneralHelper _generalHelper;
        private readonly ILogHelper _logHelper;

        private readonly IChromeManager _chromeManager;

        private readonly ITrainTroopParser _trainTroopParser;

        public TrainTroopHelper(IDbContextFactory<AppDbContext> contextFactory, IGeneralHelper generalHelper, ILogHelper logHelper, IChromeManager chromeManager, ITrainTroopParser trainTroopParser)
        {
            _contextFactory = contextFactory;
            _generalHelper = generalHelper;
            _logHelper = logHelper;
            _chromeManager = chromeManager;
            _trainTroopParser = trainTroopParser;
        }

        public Result Execute(int accountId, int villageId, BuildingEnums trainBuilding)
        {
            var result = _generalHelper.ToDorf2(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var buildingLoc = GetBuilding(villageId, trainBuilding);
            if (buildingLoc == -1)
            {
                DisableSetting(villageId, trainBuilding);
                _logHelper.Information(accountId, $"There is no {trainBuilding} in village");
                return Result.Ok();
            }

            result = _generalHelper.ToBuilding(accountId, villageId, buildingLoc);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var troop = GetTroopTraining(villageId, trainBuilding);

            var timeTrain = GetTroopTime(accountId, villageId, troop);
            var amountTroop = GetAmountTroop(accountId, villageId, trainBuilding, timeTrain);
            var maxTroop = GetMaxTroop(accountId, troop);
            if (maxTroop <= 0)
            {
                return Result.Fail(NoResource.Train(trainBuilding));
            }

            if (amountTroop > maxTroop)
            {
                using var context = _contextFactory.CreateDbContext();
                var setting = context.VillagesSettings.Find(villageId);
                if (setting.IsMaxTrain)
                {
                    amountTroop = maxTroop;
                }
                else
                {
                    return Result.Fail(NoResource.Train(trainBuilding));
                }
            }

            _logHelper.Information(accountId, $"Training {amountTroop} {troop}(s)");
            result = InputAmountTroop(accountId, troop, amountTroop);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public int GetBuilding(int villageId, BuildingEnums trainBuilding)
        {
            using var context = _contextFactory.CreateDbContext();
            var building = context.VillagesBuildings.Where(x => x.VillageId == villageId)
                                                    .FirstOrDefault(x => x.Type == trainBuilding && x.Level > 0);
            if (building is null) return -1;
            return building.Id;
        }

        public void DisableSetting(int villageId, BuildingEnums trainBuilding)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == villageId);
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

        public int GetTroopTraining(int villageId, BuildingEnums trainBuilding)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == villageId);
            return trainBuilding switch
            {
                BuildingEnums.Barracks or BuildingEnums.GreatBarracks => setting.BarrackTroop,
                BuildingEnums.Stable or BuildingEnums.GreatStable => setting.StableTroop,
                BuildingEnums.Workshop => setting.WorkshopTroop,
                _ => -1,
            };
        }

        public TimeSpan GetTroopTime(int accountId, int villageId, int troop)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
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

        public int GetAmountTroop(int accountId, int villageId, BuildingEnums trainBuilding, TimeSpan trainTime)
        {
            //var chromeBrowser = _chromeManager.Get(accountId);
            //var doc = chromeBrowser.GetHtml();
            //var timeRemaining = _trainTroopParser.GetQueueTrainTime(doc);

            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == villageId);
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

            //if (timeRemaining > timeTrain) return 0;

            //var timeLeft = timeTrain - timeRemaining;

            // quite confused but this comment will explain
            // we caclulate how many troop we will train
            // timeTrain aka time we want added into train queue
            // trainTime aka time to train 1 troop

            var result = (int)(timeTrain.TotalMilliseconds / trainTime.TotalMilliseconds);
            return result > 0 ? result + 1 : result;
        }

        public int GetMaxTroop(int accountId, int troop)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
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

        public Result InputAmountTroop(int accountId, int troop, int amountTroop)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
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
            var result = _generalHelper.Input(accountId, By.XPath(input.XPath), $"{amountTroop}");
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var button = _trainTroopParser.GetTrainButton(doc);
            result = _generalHelper.Click(accountId, By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}