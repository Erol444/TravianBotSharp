using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public class BuildingsHelper : IBuildingsHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IPlanManager _planManager;
        protected readonly IChromeManager _chromeManager;

        public BuildingsHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager, IChromeManager chromeManager)
        {
            _contextFactory = contextFactory;
            _planManager = planManager;
            _chromeManager = chromeManager;
        }

        public int GetDorf(int index) => index < 19 ? 1 : 2;

        public List<BuildingEnums> GetCanBuild(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(villageId);
            var tribe = context.AccountsInfo.Find(village.AccountId).Tribe;

            var buildings = new List<BuildingEnums>();
            for (var i = BuildingEnums.Sawmill; i <= BuildingEnums.Hospital; i++)
            {
                if (CanBuild(villageId, tribe, i)) buildings.Add(i);
            }
            return buildings;
        }

        public bool CanBuild(int villageId, TribeEnums tribe, BuildingEnums building)
        {
            if (IsExists(villageId, building))
            {
                return building switch
                {
                    BuildingEnums.Warehouse => IsBuildingAboveLevel(villageId, BuildingEnums.Warehouse, 20),
                    BuildingEnums.Granary => IsBuildingAboveLevel(villageId, BuildingEnums.Granary, 20),
                    BuildingEnums.GreatWarehouse => IsBuildingAboveLevel(villageId, BuildingEnums.GreatWarehouse, 20),
                    BuildingEnums.GreatGranary => IsBuildingAboveLevel(villageId, BuildingEnums.GreatGranary, 20),
                    BuildingEnums.Trapper => IsBuildingAboveLevel(villageId, BuildingEnums.Trapper, 20),
                    BuildingEnums.Cranny => IsBuildingAboveLevel(villageId, BuildingEnums.Cranny, 10),
                    _ => false,
                };
            }

            (var reqTribe, var prerequisites) = building.GetPrerequisiteBuildings();
            if (reqTribe != TribeEnums.Any && reqTribe != tribe) return false;
            foreach (var prerequisite in prerequisites)
            {
                if (prerequisite.Building.IsResourceField())
                {
                    if (prerequisite.Building == BuildingEnums.Cropland)
                    {
                        if (IsAutoCropFieldAboveLevel(villageId, prerequisite.Level)) return true;
                    }
                    else
                    {
                        if (IsAutoResourceFieldAboveLevel(villageId, prerequisite.Level)) return true;
                    }
                }
                if (!IsBuildingAboveLevel(villageId, prerequisite.Building, prerequisite.Level)) return false;
            }
            return true;
        }

        public bool IsExists(int villageId, BuildingEnums building)
        {
            using var context = _contextFactory.CreateDbContext();
            var b = context.VillagesBuildings.Where(x => x.VillageId == villageId).Any(x => x.Type == building);
            if (b) return true;
            var c = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).Any(x => x.Type == building);
            if (c) return true;
            var q = _planManager.GetList(villageId).Any(x => x.Building == building);
            if (q) return true;
            return false;
        }

        public bool IsBuildingAboveLevel(int villageId, BuildingEnums building, int level)
        {
            using var context = _contextFactory.CreateDbContext();
            var b = context.VillagesBuildings.Where(x => x.VillageId == villageId).Any(x => x.Type == building && level <= x.Level);
            if (b) return true;
            var c = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).Any(x => x.Type == building && level <= x.Level);
            if (c) return true;
            var q = _planManager.GetList(villageId).Any(x => x.Building == building && level <= x.Level);
            if (q) return true;
            return false;
        }

        public bool IsAutoResourceFieldAboveLevel(int villageId, int level)
        {
            return _planManager.GetList(villageId).Any(x => (x.ResourceType == ResTypeEnums.AllResources || x.ResourceType == ResTypeEnums.ExcludeCrop) && level <= x.Level);
        }

        public bool IsAutoCropFieldAboveLevel(int villageId, int level)
        {
            return _planManager.GetList(villageId).Any(x => (x.ResourceType == ResTypeEnums.AllResources || x.ResourceType == ResTypeEnums.OnlyCrop) && level <= x.Level);
        }

        public int GetDorf(BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Woodcutter or BuildingEnums.ClayPit or BuildingEnums.IronMine or BuildingEnums.Cropland => 1,
                _ => 2,
            };
        }
    }
}