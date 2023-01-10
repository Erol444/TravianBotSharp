using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System.Threading;

namespace MainCore.Tasks.Update
{
    public class UpdateHeroItems : AccountBotTask
    {
        private readonly IUpdateHelper _updateHelper;

        public UpdateHeroItems(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
        }

        public override Result Execute()
        {
            {
                var result = _navigateHelper.ToHeroInventory(AccountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = _updateHelper.UpdateHeroInventory(AccountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }
    }
}