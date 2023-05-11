using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public class StartAdventure : AccountBotTask
    {
        private readonly IAdventureHelper _adventureHelper;

        public StartAdventure(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _adventureHelper = Locator.Current.GetService<IAdventureHelper>();
        }

        public override Result Execute()
        {
            _adventureHelper.Load(AccountId, CancellationToken);

            var result = _adventureHelper.StartAdventure();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}