using MainCore.Helper.Interface;
using MainCore.Models.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public class DatabaseHelper : IDatabaseHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public DatabaseHelper(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Account GetAccount(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Accounts.Find(accountId);
        }

        public List<Access> GetAccesses(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Accesses.Where(a => a.AccountId == accountId).OrderByDescending(x => x.LastUsed).ToList();
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
            return context.VillagesBuildings.Where(vb => vb.VillageId == villageId).ToList();
        }
    }
}