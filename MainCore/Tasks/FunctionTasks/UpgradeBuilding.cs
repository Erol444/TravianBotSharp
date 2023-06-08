using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
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

        public UpgradeBuilding(int VillageId, int AccountId, CancellationToken cancellationToken = default) : base(VillageId, AccountId, cancellationToken)
        {
            _upgradeBuildingHelper = Locator.Current.GetService<IUpgradeBuildingHelper>();
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
                }
                else if (result.HasError<NoResource>())
                {
                    ExecuteAt = DateTime.Now.AddMinutes(Random.Shared.Next(30, 40));
                }
                return result.WithError(new Trace(Trace.TraceMessage()));
            }

            NextExecute();
            return Result.Ok();
        }

        public void NextExecute()
        {
            using var context = _contextFactory.CreateDbContext();
            var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId && x.Level != -1).OrderBy(x => x.CompleteTime);
            var firstComplete = currentBuildings.FirstOrDefault();
            if (firstComplete is null) return;

            ExecuteAt = _upgradeBuildingHelper.GetNextExecute(firstComplete.CompleteTime);
        }
    }
}