using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Splat;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public sealed class StartAdventure : AccountBotTask
    {
        private readonly IAdventureHelper _adventureHelper;

        public StartAdventure(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _adventureHelper = Locator.Current.GetService<IAdventureHelper>();
        }

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            var result = _adventureHelper.StartAdventure(AccountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}