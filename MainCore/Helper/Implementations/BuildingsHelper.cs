using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper.Implementations
{
    public class BuildingsHelper : IBuildingsHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IPlanManager _planManager;

        public BuildingsHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager)
        {
            _contextFactory = contextFactory;
            _planManager = planManager;
        }

        public int GetDorf(int index) => index < 19 ? 1 : 2;

        public int GetDorf(BuildingEnums building) => GetDorf((int)building);

        public List<BuildingEnums> GetCanBuild(int accountId, int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var result = new List<BuildingEnums>();
            var tribe = context.AccountsInfo.Find(accountId).Tribe;
            for (var i = BuildingEnums.Sawmill; i <= BuildingEnums.Hospital; i++)
            {
                if (CanBuild(villageId, tribe, i))
                {
                    result.Add(i);
                }
            }
            return result;
        }

        public bool CanBuild(int villageId, TribeEnums tribe, BuildingEnums building)
        {
            if (IsExists(villageId, building))
            {
                //check cranny/warehouse/grannary/trapper/GG/GW
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

        private bool IsExists(int villageId, BuildingEnums building)
        {
            using var context = _contextFactory.CreateDbContext();
            var b = context.VillagesBuildings.Where(x => x.VillageId == villageId).FirstOrDefault(x => x.Type == building);
            if (b is not null) return true;
            var c = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).FirstOrDefault(x => x.Type == building);
            if (c is not null) return true;
            var q = _planManager.GetList(villageId).FirstOrDefault(x => x.Building == building);
            if (q is not null) return true;
            return false;
        }

        private bool IsBuildingAboveLevel(int villageId, BuildingEnums building, int lvl)
        {
            using var context = _contextFactory.CreateDbContext();
            var b = context.VillagesBuildings.Where(x => x.VillageId == villageId).Any(x => x.Type == building && lvl <= x.Level);
            if (b) return true;
            var c = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).Any(x => x.Type == building && lvl <= x.Level);
            if (c) return true;
            var q = _planManager.GetList(villageId).Any(x => x.Building == building && lvl <= x.Level);
            if (q) return true;
            return false;
        }

        private bool IsAutoResourceFieldAboveLevel(int villageId, int lvl)
        {
            return _planManager.GetList(villageId).Any(x => (x.ResourceType == ResTypeEnums.AllResources || x.ResourceType == ResTypeEnums.ExcludeCrop) && lvl <= x.Level);
        }

        private bool IsAutoCropFieldAboveLevel(int villageId, int lvl)
        {
            return _planManager.GetList(villageId).Any(x => (x.ResourceType == ResTypeEnums.AllResources || x.ResourceType == ResTypeEnums.OnlyCrop) && lvl <= x.Level);
        }
    }
}