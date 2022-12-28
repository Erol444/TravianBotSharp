using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System.Threading;

namespace MainCore.Tasks.Update
{
    public class UpdateVillage : VillageBotTask
    {
        private readonly INavigateHelper _navigateHelper;
        private readonly IUpdateHelper _updateHelper;

        public UpdateVillage(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _navigateHelper = Locator.Current.GetService<INavigateHelper>();
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
        }

        public override Result Execute()
        {
            {
                var result = _navigateHelper.SwitchVillage(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var updateTask = new UpdateInfo(AccountId, CancellationToken);
                var result = updateTask.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = UpdateVillageInfo();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }

        private Result UpdateVillageInfo()
        {
            var currentUrl = _chromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf"))
            {
                var result = _updateHelper.UpdateCurrentlyBuilding(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                _eventManager.OnVillageCurrentUpdate(VillageId);
            }
            if (currentUrl.Contains("dorf1"))
            {
                {
                    var result = _updateHelper.UpdateDorf1(AccountId, VillageId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                _eventManager.OnVillageBuildsUpdate(VillageId);

                {
                    var result = _updateHelper.UpdateProduction(AccountId, VillageId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
            }
            else if (currentUrl.Contains("dorf2"))
            {
                {
                    var result = _updateHelper.UpdateDorf2(AccountId, VillageId);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                _eventManager.OnVillageBuildsUpdate(VillageId);
            }

            {
                var result = _updateHelper.UpdateResource(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }
    }
}