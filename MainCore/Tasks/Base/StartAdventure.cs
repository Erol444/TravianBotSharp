using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public class StartAdventure : AccountBotTask
    {
        private readonly IClickHelper _clickHelper;
        private Adventure _adventure;

        public StartAdventure(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _clickHelper = Locator.Current.GetService<IClickHelper>();
        }

        public override Result Execute()
        {
            var commands = new List<Func<Result>>()
            {
                ChooseAdventures,
                StartAdventures,
            };

            foreach (var command in commands)
            {
                _logManager.Information(AccountId, $"[{GetName()}] Execute {command.Method.Name}");
                var result = command.Invoke();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }

            return Result.Ok();
        }

        private Result ChooseAdventures()
        {
            _adventure = GetAdventures();
            if (_adventure is null) return Result.Fail(new Skip("No adventure available"));
            return Result.Ok();
        }

        private Adventure GetAdventures()
        {
            using var context = _contextFactory.CreateDbContext();
            var adventures = context.Adventures.Where(a => a.AccountId == AccountId);
            return adventures.FirstOrDefault();
        }

        private Result StartAdventures()
        {
            var x = _adventure.X;
            var y = _adventure.Y;
            var result = _clickHelper.ClickStartAdventure(AccountId, x, y);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}