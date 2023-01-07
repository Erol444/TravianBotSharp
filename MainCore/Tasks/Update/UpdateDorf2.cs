using FluentResults;
using MainCore.Errors;
using System.Threading;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf2 : VillageBotTask
    {
        public UpdateDorf2(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
        }

        public override Result Execute()
        {
            {
                var result = _navigateHelper.ToDorf2(AccountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var taskUpdate = new UpdateVillage(VillageId, AccountId, CancellationToken);
                var result = taskUpdate.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }
    }
}