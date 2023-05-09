using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class UpdateHelper : IUpdateHelper
    {
        protected readonly IVillageCurrentlyBuildingParser _villageCurrentlyBuildingParser;
        protected readonly IChromeManager _chromeManager;
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IVillageFieldParser _villageFieldParser;
        protected readonly IVillageInfrastructureParser _villageInfrastructureParser;
        protected readonly IStockBarParser _stockBarParser;
        protected readonly IHeroSectionParser _heroSectionParser;
        protected readonly ISubTabParser _subTabParser;
        protected readonly IFarmListParser _farmListParser;
        protected readonly IEventManager _eventManager;

        protected Result _result;
        protected int _villageId;
        protected int _accountId;
        protected CancellationToken _token;
        protected IChromeBrowser _chromeBrowser;

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

        public void Load(int villageId, int accountId, CancellationToken cancellationToken)
        {
            _villageId = villageId;
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);
        }

        public Result UpdateCurrentlyBuilding()
        {
            var html = _chromeBrowser.GetHtml();
            var nodes = _villageCurrentlyBuildingParser.GetItems(html);
            using var context = _contextFactory.CreateDbContext();
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var building = context.VillagesCurrentlyBuildings.Find(_villageId, i);

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
                        VillageId = _villageId,
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
                var building = context.VillagesCurrentlyBuildings.Find(_villageId, i);
                if (building is null)
                {
                    context.VillagesCurrentlyBuildings.Add(new()
                    {
                        Id = i,
                        VillageId = _villageId,
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
            _eventManager.OnVillageCurrentUpdate(_villageId);
            return Result.Ok();
        }

        public Result UpdateDorf1()
        {
            var html = _chromeBrowser.GetHtml();
            var resFields = _villageFieldParser.GetNodes(html);
            using var context = _contextFactory.CreateDbContext();
            foreach (var fieldNode in resFields)
            {
                var id = _villageFieldParser.GetId(fieldNode);
                var resource = context.VillagesBuildings.Find(_villageId, id);
                var level = _villageFieldParser.GetLevel(fieldNode);
                var type = _villageFieldParser.GetBuildingType(fieldNode);
                var isUnderConstruction = _villageFieldParser.IsUnderConstruction(fieldNode);
                if (resource is null)
                {
                    context.VillagesBuildings.Add(new()
                    {
                        VillageId = _villageId,
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
                    context.Update(resource);
                }
            }

            var currentlyBuilding = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == _villageId && x.Level != -1).ToList();
            if (currentlyBuilding.Count > 0)
            {
                foreach (var building in currentlyBuilding)
                {
                    var build = context.VillagesBuildings.FirstOrDefault(x => x.IsUnderConstruction && x.Type == building.Type && x.Level - building.Level < 3);
                    if (build is null) continue;
                    building.Location = build.Id;
                    context.Update(building);
                }
            }

            var updateTime = context.VillagesUpdateTime.Find(_villageId);
            if (updateTime is null)
            {
                updateTime = new()
                {
                    VillageId = _villageId,
                    Dorf1 = DateTime.Now
                };

                context.VillagesUpdateTime.Add(updateTime);
            }
            else
            {
                updateTime.Dorf1 = DateTime.Now;
                context.Update(updateTime);
            }
            context.SaveChanges();

            _eventManager.OnVillageCurrentUpdate(_villageId);
            _eventManager.OnVillageBuildsUpdate(_villageId);
            return Result.Ok();
        }

        public abstract Result UpdateDorf2();

        public Result UpdateResource()
        {
            var html = _chromeBrowser.GetHtml();
            using var context = _contextFactory.CreateDbContext();
            var resource = context.VillagesResources.Find(_villageId);

            if (resource is null)
            {
                context.VillagesResources.Add(new()
                {
                    VillageId = _villageId,
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
                context.Update(resource);
            }
            context.SaveChanges();

            return Result.Ok();
        }

        public Result UpdateHeroInventory()
        {
            var foundItems = _heroSectionParser.GetItems(_chromeBrowser.GetHtml());
            using var context = _contextFactory.CreateDbContext();
            var heroItems = context.HeroesItems.Where(x => x.AccountId == _accountId).ToList();
            var addedItems = new List<HeroItem>();
            foreach (var item in foundItems)
            {
                (var type, var count) = ((HeroItemEnums)item.Item1, item.Item2);
                var existItem = heroItems.FirstOrDefault(x => x.Item == type);
                if (existItem is null)
                {
                    context.HeroesItems.Add(new()
                    {
                        AccountId = _accountId,
                        Item = type,
                        Count = count,
                    });
                }
                else
                {
                    existItem.Count = count;
                    addedItems.Add(existItem);
                    context.Update(existItem);
                }
            }

            foreach (var item in addedItems)
            {
                heroItems.Remove(item);
            }
            context.HeroesItems.RemoveRange(heroItems);
            context.SaveChanges();
            _eventManager.OnHeroInventoryUpdate(_accountId);
            return Result.Ok();
        }

        public Result UpdateAdventures()
        {
            var foundAdventures = _heroSectionParser.GetAdventures(_chromeBrowser.GetHtml());
            using var context = _contextFactory.CreateDbContext();
            var heroAdventures = context.Adventures.Where(x => x.AccountId == _accountId).ToList();
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
                        AccountId = _accountId,
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
            _eventManager.OnHeroAdventuresUpdate(_accountId);
            return Result.Ok();
        }

        public Result UpdateProduction()
        {
            var html = _chromeBrowser.GetHtml();
            using var context = _contextFactory.CreateDbContext();
            var production = context.VillagesProduction.Find(_villageId);

            var productionList = _subTabParser.GetProductions(html);
            var wood = _subTabParser.GetProduction(productionList[0]);
            var clay = _subTabParser.GetProduction(productionList[1]);
            var iron = _subTabParser.GetProduction(productionList[2]);
            var crop = _subTabParser.GetProduction(productionList[3]);

            if (production is null)
            {
                context.VillagesProduction.Add(new()
                {
                    VillageId = _villageId,
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
                context.Update(production);
            }
            context.SaveChanges();
            return Result.Ok();
        }

        public Result UpdateFarmList()
        {
            var html = _chromeBrowser.GetHtml();

            var farmNodes = _farmListParser.GetFarmNodes(html);
            var farms = new List<Farm>();
            foreach (var farmNode in farmNodes)
            {
                var name = _farmListParser.GetName(farmNode);
                var id = _farmListParser.GetId(farmNode);
                var count = _farmListParser.GetNumOfFarms(farmNode);
                var farm = new Farm()
                {
                    AccountId = _accountId,
                    Id = id,
                    Name = name,
                    FarmCount = count,
                };
                farms.Add(farm);
            }
            using var context = _contextFactory.CreateDbContext();
            var farmsOld = context.Farms.Where(x => x.AccountId == _accountId).ToList();
            foreach (var farm in farms)
            {
                var existFarm = context.Farms.FirstOrDefault(x => x.Id == farm.Id);
                if (existFarm is null)
                {
                    context.Farms.Add(farm);
                    context.AddFarm(farm.Id);
                }
                else
                {
                    existFarm.Name = farm.Name;
                    existFarm.FarmCount = farm.FarmCount;
                    farmsOld.Remove(farmsOld.FirstOrDefault(x => x.Id == farm.Id));
                    context.Update(existFarm);
                }
            }

            foreach (var farm in farmsOld)
            {
                context.Remove(farm);
                context.DeleteFarm(farm.Id);
            }

            context.SaveChanges();
            _eventManager.OnFarmListUpdate(_accountId);
            return Result.Ok();
        }
    }
}