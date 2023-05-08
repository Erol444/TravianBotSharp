using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class TrainTroopHelper : Base.TrainTroopHelper
    {
        public TrainTroopHelper(IDbContextFactory<AppDbContext> contextFactory, IGeneralHelper generalHelper, ILogManager logManager, IChromeManager chromeManager, ITrainTroopParser trainTroopParser) : base(contextFactory, generalHelper, logManager, chromeManager, trainTroopParser)
        {
        }
    }
}