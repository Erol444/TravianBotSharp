using FluentResults;
using MainCore.Services.Interface;
using MainCore.Tasks.Sim;
using MainCore.Tasks.Update;
using Splat;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Misc.Login
{
    public abstract class LoginTask : AccountBotTask
    {
        private readonly IPlanManager _planManager;

        public LoginTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _planManager = Locator.Current.GetService<IPlanManager>();
        }

        protected Result AddTask()
        {
            _taskManager.Add(AccountId, new UpdateInfo(AccountId));

            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == AccountId);
            var listTask = _taskManager.GetList(AccountId);
            var upgradeBuildingList = listTask.OfType<UpgradeBuilding>();
            var updateList = listTask.OfType<UpdateDorf1>();
            foreach (var village in villages)
            {
                var queue = _planManager.GetList(village.Id);
                if (queue.Any())
                {
                    var upgradeBuilding = upgradeBuildingList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (upgradeBuilding is null)
                    {
                        _taskManager.Add(AccountId, new UpgradeBuilding(village.Id, AccountId));
                    }
                }
                var setting = context.VillagesSettings.Find(village.Id);
                if (setting.IsAutoRefresh)
                {
                    var update = updateList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (update is null)
                    {
                        _taskManager.Add(AccountId, new RefreshVillage(village.Id, AccountId));
                    }
                }
            }
            return Result.Ok();
        }
    }
}