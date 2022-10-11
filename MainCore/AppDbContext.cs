using MainCore.Models.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MainCore
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Account

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Accounts");

                entity.HasKey(e => e.Id)
                    .HasName("PK_ACCOUNTS");

                entity.HasIndex(e => new { e.Username, e.Server })
                    .IsUnique()
                    .HasDatabaseName("INDEX_ACCOUNTS");
            });

            #endregion Account

            #region Access

            modelBuilder.Entity<Access>(entity =>
            {
                entity.ToTable("Access");

                entity.HasKey(e => e.Id)
                    .HasName("PK_ACCESSES");
            });

            #endregion Access

            #region Village

            modelBuilder.Entity<Village>(entity =>
            {
                entity.ToTable("Villages");
                entity.HasKey(e => e.Id)
                    .HasName("PK_VILLAGES");
            });

            #endregion Village

            #region Village Building

            modelBuilder.Entity<VillageBuilding>(entity =>
            {
                entity.ToTable("VillagesBuildings");
                entity.HasKey(e => new { e.VillageId, e.Id })
                    .HasName("PK_VILLAGESBUILDINGS");
            });

            #endregion Village Building

            #region Village Resource

            modelBuilder.Entity<VillageResources>(entity =>
            {
                entity.ToTable("VillagesResources");
                entity.HasKey(e => e.VillageId)
                    .HasName("PK_VILLAGESRESOURCES");
            });

            #endregion Village Resource

            #region Village Update time

            modelBuilder.Entity<VillageUpdateTime>(entity =>
            {
                entity.ToTable("VillagesUpdateTime");
                entity.HasKey(e => e.VillageId)
                    .HasName("PK_VILLAGESUPDATETIME");
            });

            #endregion Village Update time

            #region Account setting

            modelBuilder.Entity<AccountSetting>(entity =>
            {
                entity.ToTable("AccountsSettings");
                entity.HasKey(e => e.AccountId)
                    .HasName("PK_ACCOUNTSSETTINGS");
            });

            #endregion Account setting

            #region Village setting

            modelBuilder.Entity<VillageSetting>(entity =>
            {
                entity.ToTable("VillagesSettings");
                entity.HasKey(e => e.VillageId)
                    .HasName("PK_VILLAGESSETTINGS");
            });

            #endregion Village setting

            #region Village Currently Building

            modelBuilder.Entity<VillCurrentBuilding>(entity =>
            {
                entity.ToTable("VillagesCurrentlyBuildings");
                entity.HasKey(e => new { e.VillageId, e.Id })
                    .HasName("PK_VILLAGESCURRENTLYBUILDINGS");
            });

            #endregion Village Currently Building

            #region Village Queue Building

            modelBuilder.Entity<VillageQueueBuilding>(entity =>
            {
                entity.ToTable("VillagesQueueBuildings");
                entity.HasKey(e => e.VillageId)
                    .HasName("PK_VILLAGESQUEUEBUILDINGS");
            });

            #endregion Village Queue Building

            #region Account info

            modelBuilder.Entity<AccountInfo>(entity =>
            {
                entity.ToTable("AccountsInfo");
                entity.HasKey(e => e.AccountId)
                    .HasName("PK_ACCOUNTSINFO");
            });

            #endregion Account info

            #region Hero

            modelBuilder.Entity<Hero>(entity =>
            {
                entity.ToTable("Heroes");
                entity.HasKey(e => e.AccountId)
                    .HasName("PK_HEROES");
            });

            #endregion Hero

            #region Adventures

            modelBuilder.Entity<Adventure>(entity =>
            {
                entity.ToTable("Adventures");
                entity.HasKey(e => e.Id)
                    .HasName("PK_ADVENTURES");
            });

            #endregion Adventures

            #region Inventory

            modelBuilder.Entity<HeroItem>(entity =>
            {
                entity.ToTable("HeroesItems");
                entity.HasKey(e => e.Id)
                    .HasName("PK_HEROESITEMS");
            });

            #endregion Inventory

            #region Village production

            modelBuilder.Entity<VillageProduction>(entity =>
            {
                entity.ToTable("VillagesProductions");
                entity.HasKey(e => e.VillageId)
                    .HasName("PK_VILLAGESPRODUCTIONS");
            });

            #endregion Village production

            #region Farm list

            modelBuilder.Entity<Farm>(entity =>
            {
                entity.ToTable("Farms");
                entity.HasKey(e => e.Id)
                    .HasName("PK_FARMS");
            });

            #endregion Farm list

            #region Farm settings

            modelBuilder.Entity<FarmSetting>(entity =>
            {
                entity.ToTable("FarmsSettings");
                entity.HasKey(e => e.Id)
                    .HasName("PK_FARMSSETTINGS");
            });

            #endregion Farm settings
        }

        public void AddAccount(int accountId)
        {
            AccountsInfo.Add(new AccountInfo { AccountId = accountId });

            AccountsSettings.Add(new AccountSetting
            {
                AccountId = accountId,
                ClickDelayMin = 500,
                ClickDelayMax = 900,
                TaskDelayMin = 1000,
                TaskDelayMax = 1500,
                WorkTimeMin = 340,
                WorkTimeMax = 380,
                SleepTimeMin = 480,
                SleepTimeMax = 600,
                IsClosedIfNoTask = false,
                IsDontLoadImage = false,
                IsMinimized = false,
                IsAutoAdventure = false,
            });
            Heroes.Add(new Hero { AccountId = accountId });

            //Accesses
            //Adventures
            //HeroesItems
        }

        public void AddVillage(int villageId)
        {
            VillagesResources.Add(new VillageResources { VillageId = villageId });
            VillagesUpdateTime.Add(new VillageUpdateTime { VillageId = villageId });
            VillagesProduction.Add(new VillageProduction { VillageId = villageId });
            VillagesSettings.Add(new VillageSetting
            {
                VillageId = villageId,
                IsAdsUpgrade = false,
                AdsUpgradeTime = 5,

                IsUseHeroRes = false,

                IsInstantComplete = false,
                InstantCompleteTime = 30,

                IsAutoRefresh = false,
                AutoRefreshTimeMin = 25,
                AutoRefreshTimeMax = 35,

                IsAutoNPC = false,
                AutoNPCPercent = 90,
                AutoNPCWood = 1,
                AutoNPCClay = 1,
                AutoNPCIron = 1,
                AutoNPCCrop = 0,
            });

            //VillagesQueueBuildings
            //VillagesCurrentlyBuildings
            //VillagesBuildings
        }

        public void AddFarm(int farmId)
        {
            FarmsSettings.Add(new FarmSetting
            {
                Id = farmId,
                IsActive = false,
                IntervalMin = 590,
                IntervalMax = 610,
            });
        }

        public void UpdateDatabase()
        {
            if (!AccountsInfo.Any())
            {
                foreach (var account in Accounts)
                {
                    var accountId = account.Id;
                    AccountsInfo.Add(new AccountInfo { AccountId = accountId });
                }
            }
            if (!AccountsSettings.Any())
            {
                foreach (var account in Accounts)
                {
                    var accountId = account.Id;
                    AccountsSettings.Add(new AccountSetting
                    {
                        AccountId = accountId,
                        ClickDelayMin = 500,
                        ClickDelayMax = 900,
                        TaskDelayMin = 1000,
                        TaskDelayMax = 1500,
                        WorkTimeMin = 340,
                        WorkTimeMax = 380,
                        SleepTimeMin = 480,
                        SleepTimeMax = 600,
                        IsClosedIfNoTask = false,
                        IsDontLoadImage = false,
                        IsMinimized = false,
                        IsAutoAdventure = false,
                    });
                }
            }

            if (!Heroes.Any())
            {
                foreach (var account in Accounts)
                {
                    var accountId = account.Id;
                    Heroes.Add(new Hero { AccountId = accountId });
                }
            }

            if (!VillagesResources.Any())
            {
                foreach (var village in Villages)
                {
                    var villageId = village.Id;
                    VillagesResources.Add(new VillageResources { VillageId = villageId });
                }
            }

            if (!VillagesUpdateTime.Any())
            {
                foreach (var village in Villages)
                {
                    var villageId = village.Id;
                    VillagesUpdateTime.Add(new VillageUpdateTime { VillageId = villageId });
                }
            }

            if (!VillagesSettings.Any())
            {
                foreach (var village in Villages)
                {
                    var villageId = village.Id;
                    VillagesSettings.Add(new VillageSetting
                    {
                        VillageId = villageId,
                        IsAdsUpgrade = false,
                        AdsUpgradeTime = 5,
                        IsUseHeroRes = false,
                        IsInstantComplete = false,
                        InstantCompleteTime = 30
                    });
                }
            }
            if (!VillagesProduction.Any())
            {
                foreach (var village in Villages)
                {
                    var villageId = village.Id;
                    VillagesProduction.Add(new VillageProduction { VillageId = villageId });
                }
            }

            if (!FarmsSettings.Any())
            {
                foreach (var farm in Farms)
                {
                    var farmId = farm.Id;
                    FarmsSettings.Add(new FarmSetting
                    {
                        Id = farmId,
                        IsActive = false,
                        IntervalMin = 590,
                        IntervalMax = 610,
                    });
                }
            }
        }

        public void DeleteAccount(int accountId)
        {
            var info = AccountsInfo.Find(accountId);
            AccountsInfo.Remove(info);
            var settings = AccountsSettings.Find(accountId);
            AccountsSettings.Remove(settings);
            var hero = Heroes.Find(accountId);
            Heroes.Remove(hero);

            var accesses = Accesses.Where(x => x.AccountId == accountId);
            Accesses.RemoveRange(accesses);
            var adventures = Adventures.Where(x => x.AccountId == accountId);
            Adventures.RemoveRange(adventures);
            var items = HeroesItems.Where(x => x.AccountId == accountId);
            HeroesItems.RemoveRange(items);

            var villages = Villages.Where(x => x.AccountId == accountId).ToList();
            foreach (var village in villages)
            {
                DeleteVillage(village.Id);
            }

            var farms = Farms.Where(x => x.AccountId == accountId).ToList();
            foreach (var farm in farms)
            {
                DeleteFarm(farm.Id);
            }

            var account = Accounts.Find(accountId);
            Accounts.Remove(account);
        }

        public void DeleteVillage(int villageId)
        {
            var resouce = VillagesResources.Find(villageId);
            VillagesResources.Remove(resouce);
            var updateTime = VillagesUpdateTime.Find(villageId);
            VillagesUpdateTime.Remove(updateTime);
            var settings = VillagesSettings.Find(villageId);
            VillagesSettings.Remove(settings);
            var production = VillagesProduction.Find(villageId);
            VillagesProduction.Remove(production);

            var currently = VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId);
            VillagesCurrentlyBuildings.RemoveRange(currently);
            var queue = VillagesQueueBuildings.Where(x => x.VillageId == villageId);
            VillagesQueueBuildings.RemoveRange(queue);
            var buildings = VillagesBuildings.Where(x => x.VillageId == villageId);
            VillagesBuildings.RemoveRange(buildings);

            var village = Villages.Find(villageId);
            Villages.Remove(village);
        }

        public void DeleteFarm(int farmId)
        {
            var setting = FarmsSettings.Find(farmId);
            FarmsSettings.Remove(setting);
            var farm = Farms.Find(farmId);
            Farms.Remove(farm);
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountInfo> AccountsInfo { get; set; }
        public DbSet<Access> Accesses { get; set; }
        public DbSet<Village> Villages { get; set; }
        public DbSet<VillageBuilding> VillagesBuildings { get; set; }
        public DbSet<VillageResources> VillagesResources { get; set; }
        public DbSet<VillageUpdateTime> VillagesUpdateTime { get; set; }
        public DbSet<AccountSetting> AccountsSettings { get; set; }
        public DbSet<VillageSetting> VillagesSettings { get; set; }
        public DbSet<VillCurrentBuilding> VillagesCurrentlyBuildings { get; set; }
        public DbSet<VillageQueueBuilding> VillagesQueueBuildings { get; set; }
        public DbSet<Hero> Heroes { get; set; }
        public DbSet<Adventure> Adventures { get; set; }
        public DbSet<HeroItem> HeroesItems { get; set; }
        public DbSet<VillageProduction> VillagesProduction { get; set; }
        public DbSet<Farm> Farms { get; set; }
        public DbSet<FarmSetting> FarmsSettings { get; set; }
    }
}