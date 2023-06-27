using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using MainCore.Tasks.Base;
using Splat;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public sealed class NPCTask : VillageBotTask
    {
        private readonly INPCHelper _npcHelper;
        private readonly ITaskManager _taskManager;

        public NPCTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _npcHelper = Locator.Current.GetService<INPCHelper>();
            _taskManager = Locator.Current.GetService<ITaskManager>();
        }

        public NPCTask(int villageId, int accountId, Resources ratio, CancellationToken cancellationToken = default) : this(villageId, accountId, cancellationToken)

        {
            _ratio = ratio;
        }

        private readonly Resources _ratio;

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            if (!_npcHelper.IsEnoughGold(AccountId)) return Result.Fail(new Skip("Don't have enough gold to NPC"));

            var result = _npcHelper.Execute(AccountId, VillageId, _ratio);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            TriggerTask();

            return Result.Ok();
        }

        private void TriggerTask()
        {
            var tasks = _taskManager.GetList(AccountId);
            //var improveTroopTask = tasks.OfType<ImproveTroopsTask>().FirstOrDefault(x => x.VillageId == VillageId);
            //if (improveTroopTask is not null)
            //{
            //    improveTroopTask.ExecuteAt = DateTime.Now;
            //    _taskManager.Update(AccountId);
            //}
            var upgradeTask = tasks.OfType<UpgradeBuilding>().FirstOrDefault(x => x.VillageId == VillageId);
            if (upgradeTask is not null)
            {
                upgradeTask.ExecuteAt = DateTime.Now;
                _taskManager.ReOrder(AccountId);
            }
            var trainTroopTask = tasks.OfType<TrainTroopsTask>().FirstOrDefault(x => x.VillageId == VillageId);
            if (trainTroopTask is not null)
            {
                trainTroopTask.ExecuteAt = DateTime.Now;
                _taskManager.ReOrder(AccountId);
            }
        }
    }
}