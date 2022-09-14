using MainCore.Helper;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using System;

namespace MainCore.Tasks.Update
{
    public class UpdateBothDorf : UpdateVillage
    {
        public UpdateBothDorf(int villageId, int accountId) : base(villageId, accountId)
        {
        }

        private string _name;
        public override string Name => _name;

        public override void CopyFrom(BotTask source)
        {
            base.CopyFrom(source);
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            if (village is null)
            {
                _name = $"Update both dorf in {VillageId}";
            }
            else
            {
                _name = $"Update both dorf in {village.Name}";
            }
        }

        public override void SetService(IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, IEventManager eventManager, ILogManager logManager, IPlanManager planManager, IRestClientManager restClientManager)
        {
            base.SetService(contextFactory, chromeBrowser, taskManager, eventManager, logManager, planManager, restClientManager);
            using var context = _contextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            if (village is null)
            {
                _name = $"Update both dorf in {VillageId}";
            }
            else
            {
                _name = $"Update both dorf in {village.Name}";
            }
        }

        public override void Execute()
        {
            var url = _chromeBrowser.GetCurrentUrl();
            if (url.Contains("dorf2"))
            {
                NavigateHelper.ToDorf2(_chromeBrowser);
                base.Execute();

                NavigateHelper.ToDorf1(_chromeBrowser);
                base.Execute();
            }
            else if (url.Contains("dorf1"))
            {
                NavigateHelper.ToDorf1(_chromeBrowser);
                base.Execute();

                NavigateHelper.ToDorf2(_chromeBrowser);
                base.Execute();
            }
            else
            {
                var random = new Random(DateTime.Now.Second);
                if (random.Next(0, 100) > 50)
                {
                    NavigateHelper.ToDorf1(_chromeBrowser);
                    base.Execute();

                    NavigateHelper.ToDorf2(_chromeBrowser);
                    base.Execute();
                }
                else
                {
                    NavigateHelper.ToDorf2(_chromeBrowser);
                    base.Execute();

                    NavigateHelper.ToDorf1(_chromeBrowser);
                    base.Execute();
                }
            }
        }
    }
}