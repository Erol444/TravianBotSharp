using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Helper.Implementations.TTWars
{
    public class CompleteNowHelper : Base.CompleteNowHelper
    {
        public CompleteNowHelper(IDbContextFactory<AppDbContext> contextFactory, IVillageCurrentlyBuildingParser villageCurrentlyBuildingParser, IGeneralHelper generalHelper) : base(contextFactory, villageCurrentlyBuildingParser, generalHelper)
        {
        }
    }
}