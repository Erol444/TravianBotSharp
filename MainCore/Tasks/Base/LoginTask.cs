using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Splat;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public abstract class LoginTask : AccountBotTask
    {
        private readonly IPlanManager _planManager;

        private readonly ILoginHelper _loginHelper;

        public LoginTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _planManager = Locator.Current.GetService<IPlanManager>();

            _loginHelper = Locator.Current.GetService<ILoginHelper>();
        }

        public override Result Execute()
        {
            _loginHelper.Load(AccountId, CancellationToken);

            var result = _loginHelper.Execute();
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            AddTask();
            return Result.Ok();
        }

        private void AddTask()
        {
            _taskManager.Add(AccountId, new UpdateInfo(AccountId));

            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == AccountId);
            var listTask = _taskManager.GetList(AccountId);
            var upgradeBuildingList = listTask.OfType<UpgradeBuilding>();
            var updateList = listTask.OfType<UpdateDorf1>();

            var trainTroopList = listTask.OfType<TrainTroopsTask>();
            foreach (var village in villages)
            {
                var queue = _planManager.GetList(village.Id);
                if (queue.Any())
                {
                    var upgradeBuilding = upgradeBuildingList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (upgradeBuilding is null)
                    {
                        _taskManager.Add(AccountId, _taskFactory.GetUpgradeBuildingTask(village.Id, AccountId));
                    }
                }
                var setting = context.VillagesSettings.Find(village.Id);
                if (setting.IsAutoRefresh)
                {
                    var update = updateList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (update is null)
                    {
                        _taskManager.Add(AccountId, _taskFactory.GetRefreshVillageTask(village.Id, AccountId));
                    }
                }

                if (setting.BarrackTroop != 0 || setting.StableTroop != 0 || setting.WorkshopTroop != 0)
                {
                    var update = trainTroopList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (update is null)
                    {
                        _taskManager.Add(AccountId, _taskFactory.GetTrainTroopTask(village.Id, AccountId));
                    }
                }
            }
        }
    }
}