using FluentResults;
using MainCore.Errors;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public class UpdateDorf1 : VillageBotTask
    {
        public UpdateDorf1(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
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
            var result = _generalHelper.ToDorf1(AccountId);
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