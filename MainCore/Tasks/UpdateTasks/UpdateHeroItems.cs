using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Splat;
using System.Threading;

namespace MainCore.Tasks.UpdateTasks
{
    public class UpdateHeroItems : AccountBotTask
    {
        private readonly IGeneralHelper _generalHelper;

        public UpdateHeroItems(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _generalHelper = Locator.Current.GetService<IGeneralHelper>();
        }

        public override Result Execute()
        {
            _generalHelper.Load(-1, AccountId, CancellationToken);
            var result = _generalHelper.ToHeroInventory();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}