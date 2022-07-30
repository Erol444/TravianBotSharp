using MainCore.Models.Database;
using Microsoft.EntityFrameworkCore;

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

            modelBuilder.Entity<VillageCurrentlyBuilding>(entity =>
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
                entity.HasKey(e => new { e.VillageId, e.Id })
                    .HasName("PK_VILLAGESQUEUEBUILDINGS");
            });

            #endregion Village Queue Building
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Access> Accesses { get; set; }
        public DbSet<Village> Villages { get; set; }
        public DbSet<VillageBuilding> VillagesBuildings { get; set; }
        public DbSet<VillageResources> VillagesResources { get; set; }
        public DbSet<VillageUpdateTime> VillagesUpdateTime { get; set; }
        public DbSet<AccountSetting> AccountsSettings { get; set; }
        public DbSet<VillageSetting> VillagesSettings { get; set; }
        public DbSet<VillageCurrentlyBuilding> VillagesCurrentlyBuildings { get; set; }
        public DbSet<VillageQueueBuilding> VillagesQueueBuildings { get; set; }
    }
}