using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ModuleCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper.Implementations
{
    public class UpdateHelper : IUpdateHelper
    {
        private readonly IVillageCurrentlyBuildingParser _villageCurrentlyBuildingParser;
        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IVillageFieldParser _villageFieldParser;
        private readonly IVillageInfrastructureParser _villageInfrastructureParser;
        private readonly IStockBarParser _stockBarParser;
        private readonly IHeroSectionParser _heroSectionParser;
        private readonly ISubTabParser _subTabParser;
        private readonly IFarmListParser _farmListParser;
        private readonly IEventManager _eventManager;

        public UpdateHelper(IVillageCurrentlyBuildingParser villageCurrentlyBuildingParser, IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IVillageFieldParser villageFieldParser, IVillageInfrastructureParser villageInfrastructureParser, IStockBarParser stockBarParser, ISubTabParser subTabParser, IHeroSectionParser heroSectionParser, IFarmListParser farmListParser, IEventManager eventManager)
        {
            _villageCurrentlyBuildingParser = villageCurrentlyBuildingParser;
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _villageFieldParser = villageFieldParser;
            _villageInfrastructureParser = villageInfrastructureParser;
            _stockBarParser = stockBarParser;
            _subTabParser = subTabParser;
            _heroSectionParser = heroSectionParser;
            _farmListParser = farmListParser;
            _eventManager = eventManager;
        }

        public Result UpdateCurrentlyBuilding(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var nodes = _villageCurrentlyBuildingParser.GetItems(html);
            using var context = _contextFactory.CreateDbContext();
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var building = context.VillagesCurrentlyBuildings.Find(villageId, i);

                var strType = _villageCurrentlyBuildingParser.GetBuildingType(node);
                var level = _villageCurrentlyBuildingParser.GetLevel(node);
                var duration = _villageCurrentlyBuildingParser.GetDuration(node);

                var result = Enum.TryParse(strType, false, out BuildingEnums type);
                if (!result) return Result.Fail(new Stop($"Cannot parse {strType}. Is language English ?"));
                if (building is null)
                {
                    context.VillagesCurrentlyBuildings.Add(new()
                    {
                        Id = i,
                        VillageId = villageId,
                        Type = type,
                        Level = level,
                        CompleteTime = DateTime.Now.Add(duration),
                    });
                }
                else
                {
                    building.Type = type;
                    building.Level = level;
                    building.CompleteTime = DateTime.Now.Add(duration);
                }
            }
            for (int i = nodes.Count; i < 4; i++) // we will save 3 slot for each village, Roman can build 3 building in one time
            {
                var building = context.VillagesCurrentlyBuildings.Find(villageId, i);
                if (building is null)
                {
                    context.VillagesCurrentlyBuildings.Add(new()
                    {
                        Id = i,
                        VillageId = villageId,
                        Type = 0,
                        Level = -1,
                        CompleteTime = DateTime.MaxValue,
                    });
                }
                else
                {
                    building.Type = 0;
                    building.Level = -1;
                    building.CompleteTime = DateTime.MaxValue;
                }
            }
            context.SaveChanges();
            _eventManager.OnVillageCurrentUpdate(villageId);
            return Result.Ok();
        }

        public Result UpdateDorf1(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var resFields = _villageFieldParser.GetNodes(html);
            using var context = _contextFactory.CreateDbContext();
            foreach (var fieldNode in resFields)
            {
                var id = _villageFieldParser.GetId(fieldNode);
                var resource = context.VillagesBuildings.Find(villageId, id);
                var level = _villageFieldParser.GetLevel(fieldNode);
                var type = _villageFieldParser.GetBuildingType(fieldNode);
                var isUnderConstruction = _villageFieldParser.IsUnderConstruction(fieldNode);
                if (resource is null)
                {
                    context.VillagesBuildings.Add(new()
                    {
                        VillageId = villageId,
                        Id = id,
                        Level = level,
                        Type = (BuildingEnums)type,
                        IsUnderConstruction = isUnderConstruction,
                    });
                }
                else
                {
                    resource.Level = level;
                    resource.Type = (BuildingEnums)type;
                    resource.IsUnderConstruction = isUnderConstruction;
                }
            }
            context.SaveChanges();

            var currentlyBuilding = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).Where(x => x.Level != -1).ToList();
            if (currentlyBuilding.Count > 0)
            {
                foreach (var building in currentlyBuilding)
                {
                    var build = context.VillagesBuildings.FirstOrDefault(x => x.IsUnderConstruction && x.Type == building.Type && x.Level - building.Level < 3);
                    if (build is null) continue;
                    building.Location = build.Id;
                }
                context.SaveChanges();
            }

            var updateTime = context.VillagesUpdateTime.Find(villageId);
            if (updateTime is null)
            {
                updateTime = new()
                {
                    VillageId = villageId,
                    Dorf1 = DateTime.Now
                };

                context.VillagesUpdateTime.Add(updateTime);
            }
            else
            {
                updateTime.Dorf1 = DateTime.Now;
            }
            context.SaveChanges();

            _eventManager.OnVillageCurrentUpdate(villageId);
            _eventManager.OnVillageBuildsUpdate(villageId);
            return Result.Ok();
        }

        public Result UpdateDorf2(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var buildingNodes = _villageInfrastructureParser.GetNodes(html);
            using var context = _contextFactory.CreateDbContext();
            foreach (var buildingNode in buildingNodes)
            {
                var id = _villageInfrastructureParser.GetId(buildingNode);
                var building = context.VillagesBuildings.Find(villageId, id);
                var level = _villageInfrastructureParser.GetLevel(buildingNode);
                int type = 0;
                if (VersionDetector.IsTravianOfficial() && id == 26)
                {
                    type = (int)BuildingEnums.MainBuilding;
                }
                else if (id == 39)
                {
                    type = (int)BuildingEnums.RallyPoint;
                }
                else if (id == 40)
                {
                    var tribe = context.AccountsInfo.Find(accountId).Tribe;

                    var wall = tribe.GetTribesWall();
                    type = (int)wall;
                }
                else
                {
                    type = _villageInfrastructureParser.GetBuildingType(buildingNode);
                }
                var isUnderConstruction = _villageInfrastructureParser.IsUnderConstruction(buildingNode);
                if (building is null)
                {
                    context.VillagesBuildings.Add(new()
                    {
                        VillageId = villageId,
                        Id = id,
                        Level = level,
                        Type = (BuildingEnums)type,
                        IsUnderConstruction = isUnderConstruction,
                    });
                }
                else
                {
                    building.Level = level;
                    building.Type = (BuildingEnums)type;
                    building.IsUnderConstruction = isUnderConstruction;
                }
            }
            context.SaveChanges();
            var currentlyBuilding = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).ToList();
            if (currentlyBuilding.Count > 0)
            {
                foreach (var building in currentlyBuilding)
                {
                    var build = context.VillagesBuildings.FirstOrDefault(x => x.IsUnderConstruction && x.Type == building.Type && x.Level - building.Level < 3);
                    if (build is null) continue;
                    building.Location = build.Id;
                }
                context.SaveChanges();
            }

            var updateTime = context.VillagesUpdateTime.Find(villageId);
            if (updateTime is null)
            {
                updateTime = new()
                {
                    VillageId = villageId,
                    Dorf2 = DateTime.Now
                };

                context.VillagesUpdateTime.Add(updateTime);
            }
            else
            {
                updateTime.Dorf2 = DateTime.Now;
            }
            context.SaveChanges();

            _eventManager.OnVillageCurrentUpdate(villageId);
            _eventManager.OnVillageBuildsUpdate(villageId);
            return Result.Ok();
        }

        public Result UpdateResource(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            using var context = _contextFactory.CreateDbContext();
            var resource = context.VillagesResources.Find(villageId);

            if (resource is null)
            {
                context.VillagesResources.Add(new()
                {
                    VillageId = villageId,
                    Wood = _stockBarParser.GetWood(html),
                    Clay = _stockBarParser.GetClay(html),
                    Iron = _stockBarParser.GetIron(html),
                    Crop = _stockBarParser.GetCrop(html),
                    Warehouse = _stockBarParser.GetWarehouseCapacity(html),
                    Granary = _stockBarParser.GetGranaryCapacity(html),
                    FreeCrop = _stockBarParser.GetFreeCrop(html),
                });
            }
            else
            {
                resource.Wood = _stockBarParser.GetWood(html);
                resource.Clay = _stockBarParser.GetClay(html);
                resource.Iron = _stockBarParser.GetIron(html);
                resource.Crop = _stockBarParser.GetCrop(html);
                resource.Warehouse = _stockBarParser.GetWarehouseCapacity(html);
                resource.Granary = _stockBarParser.GetGranaryCapacity(html);
                resource.FreeCrop = _stockBarParser.GetFreeCrop(html);
            }
            context.SaveChanges();

            return Result.Ok();
        }

        public Result UpdateHeroInventory(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var foundItems = _heroSectionParser.GetItems(chromeBrowser.GetHtml());
            using var context = _contextFactory.CreateDbContext();
            var heroItems = context.HeroesItems.Where(x => x.AccountId == accountId).ToList();
            var addedItems = new List<HeroItem>();
            foreach (var item in foundItems)
            {
                (var type, var count) = ((HeroItemEnums)item.Item1, item.Item2);
                var existItem = heroItems.FirstOrDefault(x => x.Item == type);
                if (existItem is null)
                {
                    context.HeroesItems.Add(new()
                    {
                        AccountId = accountId,
                        Item = type,
                        Count = count,
                    });
                }
                else
                {
                    existItem.Count = count;
                    addedItems.Add(existItem);
                }
            }

            foreach (var item in addedItems)
            {
                heroItems.Remove(item);
            }
            context.HeroesItems.RemoveRange(heroItems);
            context.SaveChanges();
            _eventManager.OnHeroInventoryUpdate(accountId);
            return Result.Ok();
        }

        public Result UpdateAdventures(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var foundAdventures = _heroSectionParser.GetAdventures(chromeBrowser.GetHtml());
            using var context = _contextFactory.CreateDbContext();
            var heroAdventures = context.Adventures.Where(x => x.AccountId == accountId).ToList();
            if (foundAdventures.Count == 0)
            {
                context.Adventures.RemoveRange(heroAdventures);
                context.SaveChanges();
                return Result.Ok();
            }
            var addedAdventures = new List<Adventure>();
            foreach (var adventure in foundAdventures)
            {
                (var x, var y) = _heroSectionParser.GetAdventureCoordinates(adventure);
                var difficulty = _heroSectionParser.GetAdventureDifficult(adventure);
                var existItem = heroAdventures.FirstOrDefault(a => a.X == x && a.Y == y);
                if (existItem is null)
                {
                    context.Adventures.Add(new()
                    {
                        AccountId = accountId,
                        X = x,
                        Y = y,
                        Difficulty = (DifficultyEnums)difficulty,
                    });
                }
                else
                {
                    addedAdventures.Add(existItem);
                }
            }

            foreach (var item in addedAdventures)
            {
                heroAdventures.Remove(item);
            }
            context.Adventures.RemoveRange(heroAdventures);
            context.SaveChanges();
            _eventManager.OnHeroAdventuresUpdate(accountId);
            return Result.Ok();
        }

        public Result UpdateProduction(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            using var context = _contextFactory.CreateDbContext();
            var production = context.VillagesProduction.Find(villageId);

            var productionList = _subTabParser.GetProductions(html);
            var wood = _subTabParser.GetProduction(productionList[0]);
            var clay = _subTabParser.GetProduction(productionList[1]);
            var iron = _subTabParser.GetProduction(productionList[2]);
            var crop = _subTabParser.GetProduction(productionList[3]);

            if (production is null)
            {
                context.VillagesProduction.Add(new()
                {
                    VillageId = villageId,
                    Wood = wood,
                    Clay = clay,
                    Iron = iron,
                    Crop = crop,
                });
            }
            else
            {
                production.Wood = wood;
                production.Clay = clay;
                production.Iron = iron;
                production.Crop = crop;
            }
            context.SaveChanges();
            return Result.Ok();
        }

        public Result UpdateFarmList(int accountId)
        {
        }
    }
}