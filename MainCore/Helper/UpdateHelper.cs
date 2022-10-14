using MainCore.Enums;
using MainCore.Models.Database;
using MainCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;

#if TRAVIAN_OFFICIAL

using TravianOfficialCore.Parsers;

#elif TRAVIAN_OFFICIAL_HEROUI

using TravianOfficialNewHeroUICore.Parsers;

#elif TTWARS

using TTWarsCore.Parsers;

#else

#error You forgot to define Travian version here

#endif

namespace MainCore.Helper
{
    public static class UpdateHelper
    {
        public static void UpdateCurrentlyBuilding(AppDbContext context, IChromeBrowser chromeBrowser, int villageId)
        {
            var html = chromeBrowser.GetHtml();
            var nodes = VillageCurrentlyBuilding.GetNodes(html);
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var building = context.VillagesCurrentlyBuildings.Find(villageId, i);

                var strType = VillageCurrentlyBuilding.GetType(node);
                var level = VillageCurrentlyBuilding.GetLevel(node);
                var duration = VillageCurrentlyBuilding.GetDuration(node);

                var type = (BuildingEnums)Enum.Parse(typeof(BuildingEnums), strType);

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
        }

        public static void UpdateDorf1(AppDbContext context, IChromeBrowser chromeBrowser, int villageId)
        {
            var html = chromeBrowser.GetHtml();
            var resFields = VillageFields.GetResourceNodes(html);
            foreach (var fieldNode in resFields)
            {
                var id = VillageFields.GetId(fieldNode);
                var resource = context.VillagesBuildings.Find(villageId, id);
                var level = VillageFields.GetLevel(fieldNode);
                var type = VillageFields.GetType(fieldNode);
                var isUnderConstruction = VillageFields.IsUnderConstruction(fieldNode);
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
        }

        public static void UpdateDorf2(AppDbContext context, IChromeBrowser chromeBrowser, int accountId, int villageId)
        {
            var html = chromeBrowser.GetHtml();
            var buildingNodes = VillageInfrastructure.GetBuildingNodes(html);
            foreach (var buildingNode in buildingNodes)
            {
                var id = VillageInfrastructure.GetId(buildingNode);
                var building = context.VillagesBuildings.Find(villageId, id);
                var level = VillageInfrastructure.GetLevel(buildingNode);
                var type = VillageInfrastructure.GetType(buildingNode);
                switch (id)
                {
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
                    case 26:
                        type = (int)BuildingEnums.MainBuilding;
                        break;
#elif TTWARS

#else

#error You forgot to define Travian version here

#endif

                    case 39:
                        type = (int)BuildingEnums.RallyPoint;
                        break;

                    case 40:
                        {
                            var tribe = context.AccountsInfo.Find(accountId).Tribe;

                            var wall = tribe.GetTribesWall();
                            type = (int)wall;
                        }
                        break;

                    default:
                        break;
                }
                var isUnderConstruction = VillageInfrastructure.IsUnderConstruction(buildingNode);
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
        }

        public static void UpdateResource(AppDbContext context, IChromeBrowser chromeBrowser, int villageId)
        {
            var html = chromeBrowser.GetHtml();
            var resource = context.VillagesResources.Find(villageId);

            if (resource is null)
            {
                context.VillagesResources.Add(new()
                {
                    VillageId = villageId,
                    Wood = StockBar.GetWood(html),
                    Clay = StockBar.GetClay(html),
                    Iron = StockBar.GetIron(html),
                    Crop = StockBar.GetCrop(html),
                    Warehouse = StockBar.GetWarehouseCapacity(html),
                    Granary = StockBar.GetGranaryCapacity(html),
                    FreeCrop = StockBar.GetFreeCrop(html),
                });
            }
            else
            {
                resource.Wood = StockBar.GetWood(html);
                resource.Clay = StockBar.GetClay(html);
                resource.Iron = StockBar.GetIron(html);
                resource.Crop = StockBar.GetCrop(html);
                resource.Warehouse = StockBar.GetWarehouseCapacity(html);
                resource.Granary = StockBar.GetGranaryCapacity(html);
                resource.FreeCrop = StockBar.GetFreeCrop(html);
            }
            context.SaveChanges();
        }

        public static void UpdateHeroInventory(AppDbContext context, IChromeBrowser chromeBrowser, int accountId)
        {
            var foundItems = HeroInfo.GetItems(chromeBrowser.GetHtml());
            if (foundItems.Count == 0) return;
            var heroItems = context.HeroesItems.Where(x => x.AccountId == accountId).ToList();
            var addedItems = new List<Models.Database.HeroItem>();
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
        }

        public static void UpdateAdventures(AppDbContext context, IChromeBrowser chromeBrowser, int accountId)
        {
            var foundAdventures = HeroInfo.GetAdventures(chromeBrowser.GetHtml());
            var heroAdventures = context.Adventures.Where(x => x.AccountId == accountId).ToList();
            if (foundAdventures.Count == 0)
            {
                context.Adventures.RemoveRange(heroAdventures);
                context.SaveChanges();
                return;
            }
            var addedAdventures = new List<Models.Database.Adventure>();
            foreach (var adventure in foundAdventures)
            {
                (var x, var y) = HeroInfo.GetAdventureCoordinates(adventure);
                var difficulty = HeroInfo.GetAdventureDifficult(adventure);
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
        }

        public static void UpdateProduction(AppDbContext context, IChromeBrowser chromeBrowser, int villageId)
        {
            var html = chromeBrowser.GetHtml();
            var production = context.VillagesProduction.Find(villageId);

            var productionList = SubTab.GetProductionNum(html);
            var wood = SubTab.GetNum(productionList[0]);
            var clay = SubTab.GetNum(productionList[1]);
            var iron = SubTab.GetNum(productionList[2]);
            var crop = SubTab.GetNum(productionList[3]);

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
        }

        public static void UpdateFarmList(AppDbContext context, IChromeBrowser chromeBrowser, int accountId)
        {
            var html = chromeBrowser.GetHtml();

            var farmNodes = FarmList.GetFarmNodes(html);
            var farms = new List<Farm>();
            foreach (var farmNode in farmNodes)
            {
                var name = FarmList.GetName(farmNode);
                var id = FarmList.GetId(farmNode);
                var count = FarmList.GetNumOfFarms(farmNode);
                var farm = new Farm()
                {
                    AccountId = accountId,
                    Id = id,
                    Name = name,
                    FarmCount = count,
                };
                farms.Add(farm);
            }

            var farmsOld = context.Farms.Where(x => x.AccountId == accountId).ToList();
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
                }
            }

            foreach (var farm in farmsOld)
            {
                context.Remove(farm);
                context.DeleteFarm(farm.Id);
            }

            context.SaveChanges();
        }
    }
}