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

            #region Village Update time

            modelBuilder.Entity<VillageUpdateTime>(entity =>
            {
                entity.ToTable("VillagesUpdateTime");
                entity.HasKey(e => e.VillageId)
                    .HasName("PK_VILLAGESUPDATETIME");
            });

            #endregion Village Update time
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Access> Accesses { get; set; }
        public DbSet<Village> Villages { get; set; }
        public DbSet<VillageUpdateTime> VillagesUpdateTime { get; set; }
    }
}