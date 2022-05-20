using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TbsCrossPlatform.Models;

namespace TbsCrossPlatform.Database
{
    public class AccountContext : DbContext
    {
        public static string GetConnectionString() => new SqliteConnectionStringBuilder($"Data Source=TBS.db;Cache=Shared")
        {
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString();

        public AccountContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Account> Accounts;
        public DbSet<Access> Accesses;

        // village related table
        public DbSet<Village> Villages;

        public DbSet<StoredResources> StoredResources;
        public DbSet<ProduceResources> ProduceResources;
        public DbSet<Building> Buildings;
        public DbSet<BuildingTask> BuildingTasks;
        public DbSet<DemolishTask> DemolishTasks;

        // hero related table
        public DbSet<Hero> Heroes;

        public DbSet<Adventure> Adventures;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(GetConnectionString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().ToTable("Accounts");
            modelBuilder.Entity<Access>().ToTable("Accesses").HasKey(p => new { p.AccountId, p.Id });
            modelBuilder.Entity<Village>().ToTable("Villages").HasKey(p => new { p.AccountId, p.Id });
            modelBuilder.Entity<StoredResources>().ToTable("StoredResources");
            modelBuilder.Entity<ProduceResources>().ToTable("ProduceResources");
            modelBuilder.Entity<Building>().ToTable("Buildings").HasKey(p => new { p.VillageId, p.Location });
            modelBuilder.Entity<BuildingTask>().ToTable("BuildingTasks").HasKey(p => new { p.VillageId, p.Position });
            modelBuilder.Entity<DemolishTask>().ToTable("DemolishTasks").HasKey(p => new { p.VillageId, p.Position });
        }
    }
}