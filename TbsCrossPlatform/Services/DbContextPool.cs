using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TbsCrossPlatform.Database;

namespace TbsCrossPlatform.Services
{
    public class DbContextPool : IDbContextPool
    {
        private readonly PooledDbContextFactory<AccountContext> _factory;

        public static string GetConnectionString() => new SqliteConnectionStringBuilder($"Data Source=TBS.db;Cache=Shared")
        {
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString();

        public DbContextPool()
        {
            var options = new DbContextOptionsBuilder<AccountContext>()
                .UseSqlite(GetConnectionString())
                .Options;

            _factory = new(options);
        }

        public AccountContext CreateDbContext()
        {
            return _factory.CreateDbContext();
        }
    }
}