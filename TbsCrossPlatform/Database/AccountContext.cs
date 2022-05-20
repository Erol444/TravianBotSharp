using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(GetConnectionString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}