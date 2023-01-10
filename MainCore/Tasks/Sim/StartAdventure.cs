using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using Splat;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Sim
{
    public class StartAdventure : AccountBotTask
    {
        private readonly IClickHelper _clickHelper;

        public StartAdventure(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _clickHelper = Locator.Current.GetService<IClickHelper>();
        }

        public override Result Execute()
        {
            var adventure = GetAdventures();
            if (adventure is null) return Result.Ok();
            var result = StartAdventures(adventure);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private Adventure GetAdventures()
        {
            using var context = _contextFactory.CreateDbContext();
            var adventures = context.Adventures.Where(a => a.AccountId == AccountId);
            return adventures.FirstOrDefault();
        }

        private Result StartAdventures(Adventure adventure)
        {
            var x = adventure.X;
            var y = adventure.Y;
            var result = _clickHelper.ClickStartAdventure(AccountId, x, y);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}