using FluentResults;
using MainCore.Errors;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Tasks.Update.UpdateBothDorf
{
    public class UpdateBothDorfTask : VillageBotTask
    {
        public UpdateBothDorfTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
        }

        public override Result Execute()
        {
            var commands = new List<Func<Result>>();
            var url = _chromeBrowser.GetCurrentUrl();
            if (url.Contains("dorf2"))
            {
                commands.Add(UpdateDorf2);
                commands.Add(UpdateDorf1);
            }
            else if (url.Contains("dorf1"))
            {
                commands.Add(UpdateDorf1);
                commands.Add(UpdateDorf2);
            }
            else
            {
                if (Random.Shared.Next(0, 100) > 50)
                {
                    commands.Add(UpdateDorf1);
                    commands.Add(UpdateDorf2);
                }
                else
                {
                    commands.Add(UpdateDorf2);
                    commands.Add(UpdateDorf1);
                }
            }

            foreach (var command in commands)
            {
                _logManager.Information(AccountId, $"[{GetName()}] Execute {command.Method.Name}");
                var result = command.Invoke();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }
            return Result.Ok();
        }

        private Result UpdateDorf1()
        {
            var updateDorf1 = _taskFactory.CreateUpdateDorf1Task(VillageId, AccountId, CancellationToken);
            var result = updateDorf1.Execute();
            return result;
        }

        private Result UpdateDorf2()
        {
            var updateDorf2 = _taskFactory.CreateUpdateDorf2Task(VillageId, AccountId, CancellationToken);
            var result = updateDorf2.Execute();
            return result;
        }
    }
}