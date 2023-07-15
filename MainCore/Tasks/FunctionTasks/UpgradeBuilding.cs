using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using MainCore.Tasks.Base;
using Splat;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public sealed class UpgradeBuilding : VillageBotTask
    {
        private readonly IUpgradeBuildingHelper _upgradeBuildingHelper;
        private readonly ITaskManager _taskManager;

        public UpgradeBuilding(int VillageId, int AccountId, CancellationToken cancellationToken = default) : base(VillageId, AccountId, cancellationToken)
        {
            _upgradeBuildingHelper = Locator.Current.GetService<IUpgradeBuildingHelper>();
            _taskManager = Locator.Current.GetService<ITaskManager>();
        }

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            var result = _upgradeBuildingHelper.Execute(AccountId, VillageId);
            if (result.IsFailed)
            {
                if (result.HasError<BuildingQueue>())
                {
                    NextExecute();
                    return Result.Fail(result.Errors.First());
                }
                else if (result.HasError<NoResource>())
                {
                    ExecuteAt = DateTime.Now.AddMinutes(Random.Shared.Next(30, 40));
                    return Result.Fail(result.Errors.First());
                }
                return Result.Fail(result.Errors);
            }

            NextExecute();
            return Result.Ok();
        }

        public void NextExecute()
        {
            using var context = _contextFactory.CreateDbContext();
            var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId && x.Level != -1).OrderBy(x => x.CompleteTime);
            var firstComplete = currentBuildings.FirstOrDefault();
            if (firstComplete is null)
            {
                ExecuteAt = DateTime.Now.AddSeconds(1);
                return;
            }

            ExecuteAt = _upgradeBuildingHelper.GetNextExecute(firstComplete.CompleteTime);

            var setting = context.VillagesSettings.Find(VillageId);
            if (setting.IsInstantComplete)
            {
                var info = context.AccountsInfo.Find(AccountId);
                if (info.Gold < 2) return;

                var listTask = _taskManager.GetList(AccountId);
                var tasks = listTask.OfType<InstantUpgrade>();
                if (tasks.Any(x => x.VillageId == VillageId)) return;
                var lastComplete = currentBuildings.Last();
                if (lastComplete.CompleteTime < DateTime.Now.AddMinutes(setting.InstantCompleteTime)) return;
                _taskManager.Add<InstantUpgrade>(AccountId, VillageId);
            }
        }
    }
}