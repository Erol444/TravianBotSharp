using FluentResults;
using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public class BuildingsHelper : IBuildingsHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IPlanManager _planManager;
        protected readonly IChromeManager _chromeManager;

        protected Result _result;
        protected int _villageId;
        protected int _accountId;
        protected CancellationToken _token;
        protected IChromeBrowser _chromeBrowser;

        public BuildingsHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager, IChromeManager chromeManager)
        {
            _contextFactory = contextFactory;
            _planManager = planManager;
            _chromeManager = chromeManager;
        }

        public void Load(int villageId, int accountId, CancellationToken cancellationToken)
        {
            _villageId = villageId;
            _accountId = accountId;
            _token = cancellationToken;
            _chromeBrowser = _chromeManager.Get(_accountId);
        }

        public int GetDorf(int index) => index < 19 ? 1 : 2;

        public List<BuildingEnums> GetCanBuild()
        {
            using var context = _contextFactory.CreateDbContext();
            var result = new List<BuildingEnums>();
            var tribe = context.AccountsInfo.Find(_accountId).Tribe;
            for (var i = BuildingEnums.Sawmill; i <= BuildingEnums.Hospital; i++)
            {
                if (CanBuild(tribe, i))
                {
                    result.Add(i);
                }
            }
            return result;
        }

        public bool CanBuild(BuildingEnums building)
        {
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(_villageId);
            var tribe = context.AccountsInfo.Find(village.AccountId).Tribe;
            return CanBuild(tribe, building);
        }

        private bool CanBuild(TribeEnums tribe, BuildingEnums building)
        {
            if (IsExists(building))
            {
                //check cranny/warehouse/grannary/trapper/GG/GW
                return building switch
                {
                    BuildingEnums.Warehouse => IsBuildingAboveLevel(BuildingEnums.Warehouse, 20),
                    BuildingEnums.Granary => IsBuildingAboveLevel(BuildingEnums.Granary, 20),
                    BuildingEnums.GreatWarehouse => IsBuildingAboveLevel(BuildingEnums.GreatWarehouse, 20),
                    BuildingEnums.GreatGranary => IsBuildingAboveLevel(BuildingEnums.GreatGranary, 20),
                    BuildingEnums.Trapper => IsBuildingAboveLevel(BuildingEnums.Trapper, 20),
                    BuildingEnums.Cranny => IsBuildingAboveLevel(BuildingEnums.Cranny, 10),
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
                        if (IsAutoCropFieldAboveLevel(prerequisite.Level)) return true;
                    }
                    else
                    {
                        if (IsAutoResourceFieldAboveLevel(prerequisite.Level)) return true;
                    }
                }
                if (!IsBuildingAboveLevel(prerequisite.Building, prerequisite.Level)) return false;
            }
            return true;
        }

        private bool IsExists(BuildingEnums building)
        {
            using var context = _contextFactory.CreateDbContext();
            var b = context.VillagesBuildings.Where(x => x.VillageId == _villageId).FirstOrDefault(x => x.Type == building);
            if (b is not null) return true;
            var c = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == _villageId).FirstOrDefault(x => x.Type == building);
            if (c is not null) return true;
            var q = _planManager.GetList(_villageId).FirstOrDefault(x => x.Building == building);
            if (q is not null) return true;
            return false;
        }

        private bool IsBuildingAboveLevel(BuildingEnums building, int lvl)
        {
            using var context = _contextFactory.CreateDbContext();
            var b = context.VillagesBuildings.Where(x => x.VillageId == _villageId).Any(x => x.Type == building && lvl <= x.Level);
            if (b) return true;
            var c = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == _villageId).Any(x => x.Type == building && lvl <= x.Level);
            if (c) return true;
            var q = _planManager.GetList(_villageId).Any(x => x.Building == building && lvl <= x.Level);
            if (q) return true;
            return false;
        }

        private bool IsAutoResourceFieldAboveLevel(int lvl)
        {
            return _planManager.GetList(_villageId).Any(x => (x.ResourceType == ResTypeEnums.AllResources || x.ResourceType == ResTypeEnums.ExcludeCrop) && lvl <= x.Level);
        }

        private bool IsAutoCropFieldAboveLevel(int lvl)
        {
            return _planManager.GetList(_villageId).Any(x => (x.ResourceType == ResTypeEnums.AllResources || x.ResourceType == ResTypeEnums.OnlyCrop) && lvl <= x.Level);
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