using FluentResults;
using MainCore.Errors;
using System;
using System.Threading;

namespace MainCore.Tasks.Update
{
    public class UpdateBothDorf : VillageBotTask
    {
        public UpdateBothDorf(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
        }

        public override Result Execute()
        {
            var url = _chromeBrowser.GetCurrentUrl();
            var updateDorf1 = new UpdateDorf1(VillageId, AccountId, CancellationToken);
            var updateDorf2 = new UpdateDorf2(VillageId, AccountId, CancellationToken);
            if (url.Contains("dorf2"))
            {
                {
                    var result = updateDorf2.Execute();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                {
                    var result = updateDorf1.Execute();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
            }
            else if (url.Contains("dorf1"))
            {
                {
                    var result = updateDorf1.Execute();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                {
                    var result = updateDorf2.Execute();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
            }
            else
            {
                if (Random.Shared.Next(0, 100) > 50)
                {
                    {
                        var result = updateDorf1.Execute();
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    {
                        var result = updateDorf2.Execute();
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                }
                else
                {
                    {
                        var result = updateDorf2.Execute();
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    {
                        var result = updateDorf1.Execute();
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                }
            }
            return Result.Ok();
        }
    }
}