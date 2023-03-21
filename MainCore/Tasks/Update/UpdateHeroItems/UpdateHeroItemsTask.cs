using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Tasks.Update.UpdateHeroItems
{
    public class UpdateHeroItemsTask : AccountBotTask
    {
        private readonly IUpdateHelper _updateHelper;

        public UpdateHeroItemsTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
        }

        public override Result Execute()
        {
            var commands = new List<Func<Result>>()
            {
                ToHeroInventory,
                Update,
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

        private Result ToHeroInventory()
        {
            var result = _navigateHelper.ToHeroInventory(AccountId);
            return result;
        }

        private Result Update()
        {
            var result = _updateHelper.UpdateHeroInventory(AccountId);
            return result;
        }
    }
}