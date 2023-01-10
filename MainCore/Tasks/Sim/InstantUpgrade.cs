using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Misc;
using MainCore.Tasks.Update;
using Splat;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Sim
{
    public class InstantUpgrade : VillageBotTask
    {
        private readonly IClickHelper _clickHelper;

        public InstantUpgrade(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _clickHelper = Locator.Current.GetService<IClickHelper>();
        }

        public override Result Execute()
        {
            {
                var result = _navigateHelper.SwitchVillage(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            var currentUrl = _chromeBrowser.GetCurrentUrl();
            if (!currentUrl.Contains("dorf"))
            {
                var result = _navigateHelper.GoRandomDorf(AccountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            {
                var result = _clickHelper.ClickCompleteNow(AccountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            var tasks = _taskManager.GetList(AccountId);
            var improveTroopTask = tasks.OfType<ImproveTroopsTask>().FirstOrDefault(x => x.VillageId == VillageId);
            if (improveTroopTask is not null)
            {
                improveTroopTask.ExecuteAt = DateTime.Now;
                _taskManager.Update(AccountId);
            }
            var upgradeTask = tasks.OfType<UpgradeBuilding>().FirstOrDefault(x => x.VillageId == VillageId);
            if (upgradeTask is not null)
            {
                upgradeTask.ExecuteAt = DateTime.Now;
                _taskManager.Update(AccountId);
            }
            {
                var updateTask = new UpdateVillage(VillageId, AccountId, CancellationToken);
                var result = updateTask.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }
    }
}