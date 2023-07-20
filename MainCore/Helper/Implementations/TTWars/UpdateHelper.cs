using MainCore.Enums;
using MainCore.Models.Database;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper.Implementations.TTWars
{
    public class UpdateHelper : Base.UpdateHelper
    {
        public UpdateHelper(IVillageCurrentlyBuildingParser villageCurrentlyBuildingParser, IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IVillageFieldParser villageFieldParser, IStockBarParser stockBarParser, ISubTabParser subTabParser, IHeroSectionParser heroSectionParser, IFarmListParser farmListParser, IEventManager eventManager, IVillagesTableParser villagesTableParser, ITaskManager taskManager, IRightBarParser rightBarParser, IVillageInfrastructureParser villageInfrastructureParser) : base(villageCurrentlyBuildingParser, chromeManager, contextFactory, villageFieldParser, stockBarParser, subTabParser, heroSectionParser, farmListParser, eventManager, villagesTableParser, taskManager, rightBarParser, villageInfrastructureParser)
        {
        }

        public override void UpdateBuildings(int accountId, int villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var buildingNodes = _villageInfrastructureParser.GetNodes(html);
            using var context = _contextFactory.CreateDbContext();

            var buildingUnderConstruction = new List<VillageBuilding>();
            foreach (var buildingNode in buildingNodes)
            {
                var id = _villageInfrastructureParser.GetId(buildingNode);
                var building = context.VillagesBuildings.Find(villageId, id);
                var level = _villageInfrastructureParser.GetLevel(buildingNode);
                int type = 0;
                if (id == 39)
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
                    building = new()
                    {
                        VillageId = villageId,
                        Id = id,
                        Level = level,
                        Type = (BuildingEnums)type,
                        IsUnderConstruction = isUnderConstruction,
                    };
                    context.VillagesBuildings.Add(building);
                }
                else
                {
                    building.Level = level;
                    building.Type = (BuildingEnums)type;
                    building.IsUnderConstruction = isUnderConstruction;
                    context.Update(building);
                }
                if (isUnderConstruction)
                {
                    buildingUnderConstruction.Add(building);
                }
            }

            if (buildingUnderConstruction.Any())
            {
                var currentlyBuilding = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Level > 0).ToList();
                var currentlyBuildings = currentlyBuilding.Where(x => !x.Type.IsResourceField());

                foreach (var building in buildingUnderConstruction)
                {
                    var currentlyField = currentlyBuildings.First(x => x.Level == building.Level + 1);
                    currentlyField.Location = building.Id;
                    context.Update(currentlyField);
                }
                {
                    var currentlyField = currentlyBuildings.FirstOrDefault(x => x.Location == -1);
                    if (currentlyField is not null)
                    {
                        var field = buildingUnderConstruction.First();
                        currentlyField.Location = field.Id;
                        context.Update(currentlyField);
                    }
                }
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
                context.Update(updateTime);
            }
            context.SaveChanges();

            _eventManager.OnVillageCurrentUpdate(villageId);
            _eventManager.OnVillageBuildsUpdate(villageId);
        }
    }
}