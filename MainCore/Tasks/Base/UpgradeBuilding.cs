using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using Splat;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public abstract class UpgradeBuilding : VillageBotTask
    {
        protected readonly IUpgradeBuildingHelper _upgradeBuildingHelper;

        protected PlanTask _chosenTask;
        protected bool _isNewBuilding;

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