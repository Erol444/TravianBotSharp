using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Splat;
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
            _upgradeBuildingHelper.Load(VillageId, AccountId, CancellationToken);

            var result = _upgradeBuildingHelper.Execute();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}