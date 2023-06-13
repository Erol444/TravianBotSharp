using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public sealed class DatabaseHelper : IDatabaseHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IPlanManager _planManager;

        public DatabaseHelper(IDbContextFactory<AppDbContext> contextFactory, IPlanManager planManager)
        {
            _contextFactory = contextFactory;
            _planManager = planManager;
        }

        public Account GetAccount(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Accounts.Find(accountId);
        }

        public List<Access> GetAccesses(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Accesses.Where(a => a.AccountId == accountId).OrderBy(x => x.LastUsed).ToList();
        }

        public Hero GetHero(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Heroes.Find(accountId);
        }

        public List<Adventure> GetAdventures(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Adventures.Where(a => a.AccountId == accountId).ToList();
        }

        public AccountSetting GetAccountSetting(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.AccountsSettings.Find(accountId);
        }

        public Village GetVillage(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Villages.Find(villageId);
        }

        public List<VillageBuilding> GetVillageBuildings(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings
                .Where(x => x.VillageId == villageId)
                .OrderBy(x => x.Id)
                .ToList();
            return buildings;
        }

        public (PlanTask, VillCurrentBuilding) GetInProgressBuilding(int villageId, int buildingId)
        {
            using var context = _contextFactory.CreateDbContext();
            var currentlyBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Level > 0);
            var queueBuildings = _planManager.GetList(villageId);

            var plannedBuild = queueBuildings.Where(x => x.Location == buildingId).OrderByDescending(x => x.Level).FirstOrDefault();
            var currentBuild = currentlyBuildings.Where(x => x.Location == buildingId).OrderByDescending(x => x.Level).FirstOrDefault();
            return (plannedBuild, currentBuild);
        }

        public List<VillCurrentBuilding> GetVillageCurrentlyBuildings(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesCurrentlyBuildings
                .Where(x => x.CompleteTime != DateTime.MaxValue && x.VillageId == villageId)
                .OrderBy(x => x.Id)
                .ToList();
            return buildings;
        }
    }
}