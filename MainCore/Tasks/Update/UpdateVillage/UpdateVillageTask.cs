using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Tasks.Update.UpdateVillage
{
    public class UpdateVillage : VillageBotTask
    {
        private readonly IUpdateHelper _updateHelper;

        public UpdateVillage(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
        }

        public override Result Execute()
        {
            var commands = new List<Func<Result>>()
            {
                SwitchVillage,
                UpdateInfo,
                UpdateVillageInfo,
            };

            foreach (var command in commands)
            {
                _logManager.Information(AccountId, $"[{GetName()}] Execute {command.Method.Name}");
                var result = command.Invoke();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }
            return Result.Ok();
        }

        private Result SwitchVillage()
        {
            var result = _navigateHelper.SwitchVillage(AccountId, VillageId);
            return result;
        }

        private Result UpdateInfo()
        {
            var updateTask = _taskFactory.CreateUpdateInfoTask(AccountId, CancellationToken);
            var result = updateTask.Execute();
            return result;
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