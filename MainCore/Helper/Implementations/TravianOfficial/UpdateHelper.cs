using FluentResults;
using MainCore.Enums;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class UpdateHelper : Base.UpdateHelper
    {
        public UpdateHelper(IVillageCurrentlyBuildingParser villageCurrentlyBuildingParser, IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IVillageFieldParser villageFieldParser, IVillageInfrastructureParser villageInfrastructureParser, IStockBarParser stockBarParser, ISubTabParser subTabParser, IHeroSectionParser heroSectionParser, IFarmListParser farmListParser, IEventManager eventManager, IVillagesTableParser villagesTableParser, ITaskManager taskManager, IRightBarParser rightBarParser) : base(villageCurrentlyBuildingParser, chromeManager, contextFactory, villageFieldParser, villageInfrastructureParser, stockBarParser, subTabParser, heroSectionParser, farmListParser, eventManager, villagesTableParser, taskManager, rightBarParser)
        {
        }

        protected override Result UpdateBuildings()
        {
            var html = _chromeBrowser.GetHtml();
            var buildingNodes = _villageInfrastructureParser.GetNodes(html);
            using var context = _contextFactory.CreateDbContext();
            foreach (var buildingNode in buildingNodes)
            {
                var id = _villageInfrastructureParser.GetId(buildingNode);
                var building = context.VillagesBuildings.Find(_villageId, id);
                var level = _villageInfrastructureParser.GetLevel(buildingNode);
                int type = 0;
                if (id == 26)
                {
                    type = (int)BuildingEnums.MainBuilding;
                }
                else if (id == 39)
                {
                    type = (int)BuildingEnums.RallyPoint;
                }
                else if (id == 40)
                {
                    var tribe = context.AccountsInfo.Find(_accountId).Tribe;

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
                        VillageId = _villageId,
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
                    context.Update(building);
                }
            }
            var currentlyBuilding = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == _villageId).ToList();
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

            _eventManager.OnVillageCurrentUpdate(_villageId);
            _eventManager.OnVillageBuildsUpdate(_villageId);
            return Result.Ok();
        }
    }
}