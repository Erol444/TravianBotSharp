using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Tasks.Base;
using Splat;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public sealed class NPCTask : VillageBotTask
    {
        private readonly INPCHelper _npcHelper;

        public NPCTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _npcHelper = Locator.Current.GetService<INPCHelper>();
        }

        public NPCTask(int villageId, int accountId, Resources ratio, CancellationToken cancellationToken = default) : this(villageId, accountId, cancellationToken)

        {
            _ratio = ratio;
        }

        private readonly Resources _ratio;

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            var result = _npcHelper.Execute(AccountId, VillageId, _ratio);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}