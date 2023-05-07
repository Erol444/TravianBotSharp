using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class TrainTroopHelper : Base.TrainTroopHelper
    {
        public TrainTroopHelper(IDbContextFactory<AppDbContext> contextFactory, INavigateHelper navigateHelper, ILogManager logManager, IChromeManager chromeManager, ITrainTroopParser trainTroopParser) : base(contextFactory, navigateHelper, logManager, chromeManager, trainTroopParser)
        {
        }
    }
}