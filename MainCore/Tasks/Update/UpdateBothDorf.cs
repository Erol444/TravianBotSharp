using FluentResults;
using MainCore.Errors;
using System;

namespace MainCore.Tasks.Update
{
    public class UpdateBothDorf : VillageBotTask
    {
        public UpdateBothDorf(int villageId, int accountId) : base(villageId, accountId)
        {
        }

        public override Result Execute()
        {
            var url = _chromeBrowser.GetCurrentUrl();
            if (url.Contains("dorf2"))
            {
                {
                    var updateDorf2 = new UpdateDorf2(VillageId, AccountId);
                    var result = updateDorf2.Execute();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                {
                    var updateDorf1 = new UpdateDorf1(VillageId, AccountId);
                    var result = updateDorf1.Execute();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
            }
            else if (url.Contains("dorf1"))
            {
                {
                    var updateDorf1 = new UpdateDorf1(VillageId, AccountId);
                    var result = updateDorf1.Execute();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
                {
                    var updateDorf2 = new UpdateDorf2(VillageId, AccountId);
                    var result = updateDorf2.Execute();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                }
            }
            else
            {
                if (Random.Shared.Next(0, 100) > 50)
                {
                    {
                        var updateDorf1 = new UpdateDorf1(VillageId, AccountId);
                        var result = updateDorf1.Execute();
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    {
                        var updateDorf2 = new UpdateDorf2(VillageId, AccountId);
                        var result = updateDorf2.Execute();
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                }
                else
                {
                    {
                        var updateDorf2 = new UpdateDorf2(VillageId, AccountId);
                        var result = updateDorf2.Execute();
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                    {
                        var updateDorf1 = new UpdateDorf1(VillageId, AccountId);
                        var result = updateDorf1.Execute();
                        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    }
                }
            }
            return Result.Ok();
        }
    }
}