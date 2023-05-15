using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Splat;
using System.Threading;

namespace MainCore.Tasks.UpdateTasks
{
    public sealed class UpdateAdventures : AccountBotTask
    {
        private readonly IAdventureHelper _adventureHelper;

        public UpdateAdventures(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _adventureHelper = Locator.Current.GetService<IAdventureHelper>();
        }

        public override Result Execute()
        {
            _adventureHelper.Load(AccountId, CancellationToken);
            Result result;
            result = _adventureHelper.ToAdventure();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            result = _adventureHelper.StartAdventure();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            ExecuteAt = _adventureHelper.GetAdventureTimeLength();
            return Result.Ok();
        }
    }
}