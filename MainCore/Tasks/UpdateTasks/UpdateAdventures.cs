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
        private readonly IGeneralHelper _generalHelper;

        public UpdateAdventures(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _adventureHelper = Locator.Current.GetService<IAdventureHelper>();
            _generalHelper = Locator.Current.GetService<IGeneralHelper>();
        }

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            Result result;
            result = _adventureHelper.ToAdventure(AccountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            result = _adventureHelper.StartAdventure(AccountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            ExecuteAt = _adventureHelper.GetAdventureTimeLength(AccountId);
            return Result.Ok();
        }
    }
}