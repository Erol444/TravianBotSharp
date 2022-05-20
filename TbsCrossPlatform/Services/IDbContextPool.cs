using TbsCrossPlatform.Database;

namespace TbsCrossPlatform.Services
{
    public interface IDbContextPool
    {
        public AccountContext CreateDbContext();
    }
}