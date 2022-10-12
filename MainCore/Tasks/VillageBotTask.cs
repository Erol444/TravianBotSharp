using MainCore.Services;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Tasks
{
    public abstract class VillageBotTask : AccountBotTask
    {
        private readonly int _villageId;
        public int VillageId => _villageId;

        public VillageBotTask(int villageID, int accountId, string name) : base(accountId, name)
        {
            _villageId = villageID;
        }

        public override void CopyFrom(BotTask source)
        {
            base.CopyFrom(source);
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            if (village is null)
            {
                Name = $"{Name} in {VillageId}";
            }
            else
            {
                Name = $"{Name} in {village.Name}";
            }
        }

        public override void SetService(IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, EventManager EventManager, ILogManager logManager, IPlanManager planManager, IRestClientManager restClientManager)
        {
            base.SetService(contextFactory, chromeBrowser, taskManager, EventManager, logManager, planManager, restClientManager);
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            if (village is null)
            {
                Name = $"{Name} in {VillageId}";
            }
            else
            {
                Name = $"{Name} in {village.Name}";
            }
        }
    }
}