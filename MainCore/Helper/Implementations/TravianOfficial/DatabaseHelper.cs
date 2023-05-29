using Microsoft.EntityFrameworkCore;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class DatabaseHelper : Base.DatabaseHelper
    {
        public DatabaseHelper(IDbContextFactory<AppDbContext> contextFactory) : base(contextFactory)
        {
        }
    }
}