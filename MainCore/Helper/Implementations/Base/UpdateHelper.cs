using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using MainCore.Tasks.Base;
using MainCore.Tasks.FunctionTasks;
using MainCore.Tasks.UpdateTasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public abstract class UpdateHelper : IUpdateHelper
    {
        protected readonly IVillageCurrentlyBuildingParser _villageCurrentlyBuildingParser;
        protected readonly IChromeManager _chromeManager;
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IVillageFieldParser _villageFieldParser;
        protected readonly IStockBarParser _stockBarParser;
        protected readonly IHeroSectionParser _heroSectionParser;
        protected readonly ISubTabParser _subTabParser;
        protected readonly IFarmListParser _farmListParser;
        protected readonly IEventManager _eventManager;
        protected readonly IVillagesTableParser _villagesTableParser;
        protected readonly IRightBarParser _rightBarParser;
        protected readonly ITaskManager _taskManager;
        protected readonly IVillageInfrastructureParser _villageInfrastructureParser;

        public UpdateHelper(IVillageCurrentlyBuildingParser villageCurrentlyBuildingParser, IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IVillageFieldParser villageFieldParser, IStockBarParser stockBarParser, ISubTabParser subTabParser, IHeroSectionParser heroSectionParser, IFarmListParser farmListParser, IEventManager eventManager, IVillagesTableParser villagesTableParser, ITaskManager taskManager, IRightBarParser rightBarParser, IVillageInfrastructureParser villageInfrastructureParser)
        {
            _villageCurrentlyBuildingParser = villageCurrentlyBuildingParser;
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _villageFieldParser = villageFieldParser;
            _stockBarParser = stockBarParser;
            _subTabParser = subTabParser;
            _heroSectionParser = heroSectionParser;
            _farmListParser = farmListParser;
            _eventManager = eventManager;
            _villagesTableParser = villagesTableParser;
            _taskManager = taskManager;
            _rightBarParser = rightBarParser;
            _villageInfrastructureParser = villageInfrastructureParser;
        }

        public abstract void UpdateBuildings(int accountId, int villageId);

        public void Update(int accountId, int villageId = -1)
        {
            UpdateAccountInfo(accountId);

            UpdateHeroInfo(accountId);

            UpdateVillageList(accountId);

            if (villageId != -1)
            {
                UpdateResource(accountId, villageId);
                TriggerAutoNPC(accountId, villageId);
            }
        }

        public Result UpdateCurrentDorf(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var url = chromeBrowser.GetCurrentUrl();
            if (url.Contains("dorf1"))
            {
                var result = UpdateDorf1(accountId, villageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            else if (url.Contains("dorf2"))
            {
                var result = UpdateDorf2(accountId, villageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        public Result UpdateDorf1(int accountId, int villageId)
        {
            Update(accountId, villageId);

            var result = UpdateCurrentlyBuilding(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            TriggerInstantUpgrade(accountId, villageId);

            UpdateResourceFields(accountId, villageId);

            UpdateProduction(accountId, villageId);
            return Result.Ok();
        }

        public Result UpdateDorf2(int accountId, int villageId)
        {
            Update(accountId, villageId);

            var result = UpdateCurrentlyBuilding(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            TriggerInstantUpgrade(accountId, villageId);

            UpdateBuildings(accountId, villageId);
            return Result.Ok();
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

        public void UpdateResourceFields(int accountId, int villageId)
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
                    context.Update(resource);
                }
            }

            var currentlyBuilding = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Level != -1).ToList();
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
                context.Update(updateTime);
            }
            context.SaveChanges();

            _eventManager.OnVillageCurrentUpdate(villageId);
            _eventManager.OnVillageBuildsUpdate(villageId);
        }

        public void UpdateResource(int accountId, int villageId)
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
                context.Update(resource);
            }
            context.SaveChanges();
        }

        public void UpdateHeroInventory(int accountId)
        {
            Update(accountId);

            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var foundItems = _heroSectionParser.GetItems(html);
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
                    context.Update(existItem);
                }
            }

            foreach (var item in addedItems)
            {
                heroItems.Remove(item);
            }
            context.HeroesItems.RemoveRange(heroItems);
            context.SaveChanges();
            _eventManager.OnHeroInventoryUpdate(accountId);
        }

        public void UpdateAdventures(int accountId)
        {
            Update(accountId);

            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var foundAdventures = _heroSectionParser.GetAdventures(html);
            using var context = _contextFactory.CreateDbContext();
            var heroAdventures = context.Adventures.Where(x => x.AccountId == accountId).ToList();
            if (foundAdventures.Count == 0)
            {
                context.Adventures.RemoveRange(heroAdventures);
                context.SaveChanges();
                return;
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
        }

        public void UpdateProduction(int accountId, int villageId)
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
                context.Update(production);
            }
            context.SaveChanges();
        }

        public void UpdateFarmList(int accountId)
        {
            Update(accountId);
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            var farmNodes = _farmListParser.GetFarmNodes(html);
            var farms = new List<Farm>();
            foreach (var farmNode in farmNodes)
            {
                var name = _farmListParser.GetName(farmNode);
                var id = _farmListParser.GetId(farmNode);
                var count = _farmListParser.GetNumOfFarms(farmNode);
                var farm = new Farm()
                {
                    AccountId = accountId,
                    Id = id,
                    Name = name,
                    FarmCount = count,
                };
                farms.Add(farm);
            }
            using var context = _contextFactory.CreateDbContext();
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
                    context.Update(existFarm);
                }
            }

            foreach (var farm in farmsOld)
            {
                context.Remove(farm);
                context.DeleteFarm(farm.Id);
            }

            context.SaveChanges();
            _eventManager.OnFarmListUpdate(accountId);
        }

        public void UpdateVillageList(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            var listNode = _villagesTableParser.GetVillages(html);
            var foundVills = new List<Village>();
            foreach (var node in listNode)
            {
                var id = _villagesTableParser.GetId(node);
                var name = _villagesTableParser.GetName(node);
                var x = _villagesTableParser.GetX(node);
                var y = _villagesTableParser.GetY(node);
                foundVills.Add(new()
                {
                    AccountId = accountId,
                    Id = id,
                    Name = name,
                    X = x,
                    Y = y,
                });
            }

            using var context = _contextFactory.CreateDbContext();
            var currentVills = context.Villages.Where(x => x.AccountId == accountId).ToList();

            var missingVills = new List<Village>();
            var taskList = _taskManager.GetList(accountId).OfType<VillageBotTask>();
            for (var i = 0; i < currentVills.Count; i++)
            {
                var currentVillage = currentVills[i];
                var foundVillage = foundVills.FirstOrDefault(x => x.Id == currentVillage.Id);

                if (foundVillage is null)
                {
                    missingVills.Add(currentVillage);
                    var tasks = taskList.Where(x => x.VillageId == currentVillage.Id).ToList();
                    foreach (var task in tasks)
                    {
                        if (task.Stage == TaskStage.Executing) _taskManager.StopCurrentTask(accountId);
                        _taskManager.Remove(accountId, task);
                    }
                    continue;
                }

                currentVillage.Name = foundVillage.Name;
                foundVills.Remove(foundVillage);
            }
            bool villageChange = missingVills.Count > 0 || foundVills.Count > 0;
            foreach (var item in missingVills)
            {
                context.DeleteVillage(item.Id);
            }

            var tribe = context.AccountsInfo.Find(accountId).Tribe;
            foreach (var newVill in foundVills)
            {
                context.Villages.Add(new Village()
                {
                    Id = newVill.Id,
                    Name = newVill.Name,
                    AccountId = newVill.AccountId,
                    X = newVill.X,
                    Y = newVill.Y,
                });
                context.AddVillage(newVill.Id);
                context.AddTroop(newVill.Id, tribe);

                _taskManager.Add<UpdateBothDorf>(accountId, newVill.Id);
            }
            context.SaveChanges();
            if (villageChange)
            {
                _eventManager.OnVillagesUpdate(accountId);
            }
        }

        public void UpdateHeroInfo(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var health = _heroSectionParser.GetHealth(html);
            var status = _heroSectionParser.GetStatus(html);
            var numberAdventure = _heroSectionParser.GetAdventureNum(html);

            using var context = _contextFactory.CreateDbContext();
            var account = context.Heroes.Find(accountId);
            account.Health = health;
            account.Status = (HeroStatusEnums)status;
            context.Update(account);

            var adventures = context.Adventures.Count(x => x.AccountId == accountId);
            if (numberAdventure == 0)
            {
                if (adventures != 0)
                {
                    var heroAdventures = context.Adventures.Where(x => x.AccountId == accountId).ToList();
                    context.Adventures.RemoveRange(heroAdventures);
                }
            }
            else if (adventures != numberAdventure)
            {
                var setting = context.AccountsSettings.Find(accountId);
                if (setting.IsAutoAdventure)
                {
                    var listTask = _taskManager.GetList(accountId);
                    var task = listTask.OfType<UpdateAdventures>();
                    if (!task.Any())
                    {
                        _taskManager.Add<UpdateAdventures>(accountId);
                    }
                }
            }

            context.SaveChanges();
        }

        public void UpdateAccountInfo(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            //var tribe = _rightBarParser.GetTribe(html);
            var hasPlusAccount = _rightBarParser.HasPlusAccount(html);
            var gold = _stockBarParser.GetGold(html);
            var silver = _stockBarParser.GetSilver(html);

            using var context = _contextFactory.CreateDbContext();
            var account = context.AccountsInfo.Find(accountId);

            account.HasPlusAccount = hasPlusAccount;
            account.Gold = gold;
            account.Silver = silver;

            //if (account.Tribe == TribeEnums.Any) account.Tribe = (TribeEnums)tribe;
            context.Update(account);
            context.SaveChanges();
        }

        private void TriggerInstantUpgrade(int accountId, int villageId)
        {
            using var context = _contextFactory.CreateDbContext();

            var setting = context.VillagesSettings.Find(villageId);
            if (!setting.IsInstantComplete) return;
            var info = context.AccountsInfo.Find(accountId);
            if (info.Gold < 2) return;

            var listTask = _taskManager.GetList(accountId);
            var tasks = listTask.OfType<InstantUpgrade>();
            if (tasks.Any(x => x.VillageId == villageId)) return;

            var currentlyBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Level > 0).ToList();

            var tribe = context.AccountsInfo.Find(accountId).Tribe;
            if (tribe == TribeEnums.Romans)
            {
                if (currentlyBuildings.Count < (info.HasPlusAccount ? 3 : 2)) return;
            }
            else
            {
                if (currentlyBuildings.Count < (info.HasPlusAccount ? 2 : 1)) return;
            }
            var notInstantBuildings = currentlyBuildings.Where(x => x.Type.IsNotAdsUpgrade());
            foreach (var building in notInstantBuildings)
            {
                currentlyBuildings.Remove(building);
            }
            if (!currentlyBuildings.Any()) return;

            if (currentlyBuildings.Max(x => x.CompleteTime) < DateTime.Now.AddMinutes(setting.InstantCompleteTime)) return;

            _taskManager.Add<InstantUpgrade>(accountId, villageId);
        }

        private void TriggerAutoNPC(int accountId, int villageId)
        {
            var listTask = _taskManager.GetList(accountId);
            var tasks = listTask.OfType<NPCTask>();
            if (tasks.Any(x => x.VillageId == villageId)) return;

            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(villageId);

            var resource = context.VillagesResources.Find(villageId);
            if (setting.IsAutoNPC && setting.AutoNPCPercent != 0)
            {
                var ratio = resource.Crop * 100.0f / resource.Granary;
                if (ratio < setting.AutoNPCPercent) return;
                _taskManager.Add<NPCTask>(accountId, villageId);
            }
            if (setting.IsAutoNPCWarehouse && setting.AutoNPCWarehousePercent != 0)
            {
                var maxResource = Math.Max(resource.Wood, Math.Max(resource.Clay, resource.Iron));
                var ratio = maxResource * 100.0f / resource.Warehouse;
                if (ratio < setting.AutoNPCWarehousePercent) return;
                _taskManager.Add<NPCTask>(accountId, villageId);
            }
        }
    }
}