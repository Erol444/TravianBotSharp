using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using Splat;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public abstract class NPCTask : VillageBotTask
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

        protected readonly Resources _ratio;

        public override Result Execute()
        {
            _npcHelper.Load(VillageId, AccountId, CancellationToken);
            var result = _npcHelper.Execute(_ratio);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            return Result.Ok();
        }
    }
}