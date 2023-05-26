using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using MainCore.Tasks.Base;
using Splat;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public sealed class LoginTask : AccountBotTask
    {
        private readonly IPlanManager _planManager;
        private readonly ITaskManager _taskManager;

        private readonly ILoginHelper _loginHelper;
        private readonly IUpdateHelper _updateHelper;

        public LoginTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _planManager = Locator.Current.GetService<IPlanManager>();

            _loginHelper = Locator.Current.GetService<ILoginHelper>();
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
            _taskManager = Locator.Current.GetService<ITaskManager>();
        }

        public override Result Execute()
        {
            _loginHelper.Load(AccountId, CancellationToken);
            _updateHelper.Load(-1, AccountId, CancellationToken);

            var result = _loginHelper.Execute();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = _updateHelper.Update();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            AddTask();

            return Result.Ok();
        }

        private void AddTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == AccountId);
            var listTask = _taskManager.GetList(AccountId);

            var upgradeBuildingList = listTask.OfType<UpgradeBuilding>();
            var updateList = listTask.OfType<RefreshVillage>();
            var trainTroopList = listTask.OfType<TrainTroopsTask>();

            foreach (var village in villages)
            {
                var queue = _planManager.GetList(village.Id);
                if (queue.Any())
                {
                    var task = upgradeBuildingList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (task is null)
                    {
                        _taskManager.Add(AccountId, new UpgradeBuilding(village.Id, AccountId));
                    }
                }
                var setting = context.VillagesSettings.Find(village.Id);
                if (setting.IsAutoRefresh)
                {
                    var task = updateList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (task is null)
                    {
                        _taskManager.Add(AccountId, new RefreshVillage(village.Id, AccountId));
                    }
                }

                if (setting.BarrackTroop != 0 || setting.StableTroop != 0 || setting.WorkshopTroop != 0)
                {
                    var task = trainTroopList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (task is null)
                    {
                        _taskManager.Add(AccountId, new TrainTroopsTask(village.Id, AccountId));
                    }
                }
            }
        }
    }
}