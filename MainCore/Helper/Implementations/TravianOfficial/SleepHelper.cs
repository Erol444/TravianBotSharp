using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class SleepHelper : Base.SleepHelper
    {
        public SleepHelper(IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager, IAccessHelper accessHelper, IRestClientManager restClientManager, ILogManager logManager, ITaskManager taskManager) : base(contextFactory, chromeManager, accessHelper, restClientManager, logManager, taskManager)
        {
        }
    }
}