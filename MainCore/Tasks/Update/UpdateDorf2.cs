using MainCore.Helper;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf2 : UpdateVillage
    {
        public UpdateDorf2(int villageId, int accountId) : base(villageId, accountId)
        {
        }

        private string _name;
        public override string Name => _name;

        public override void CopyFrom(BotTask source)
        {
            base.CopyFrom(source);
            using var context = ContextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            _name = $"Update dorf 2 in {village.Name}";
        }

        public override void SetService(IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, IEventManager eventManager, ILogManager logManager, IPlanManager planManager, IRestClientManager restClientManager)
        {
            base.SetService(contextFactory, chromeBrowser, taskManager, eventManager, logManager, planManager, restClientManager);
            using var context = ContextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            _name = $"Update dorf 2 in {village.Name}";
        }

        public override void Execute()
        {
            NavigateHelper.ToDorf2(ChromeBrowser);
            base.Execute();
        }
    }
}