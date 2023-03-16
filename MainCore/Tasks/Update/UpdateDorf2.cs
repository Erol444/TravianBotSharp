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
            var commands = new List<Func<Result>>()
            {
                ToDorf,
                UpdateVillage,
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

        private Result ToDorf()
        {
            var result = _navigateHelper.ToDorf2(AccountId);
            return result;
        }

        private Result UpdateVillage()
        {
            var taskUpdate = new UpdateVillage(VillageId, AccountId, CancellationToken);
            var result = taskUpdate.Execute();
            return result;
        }
    }
}