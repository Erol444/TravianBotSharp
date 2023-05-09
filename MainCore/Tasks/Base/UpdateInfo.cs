using FluentResults;
using MainCore.Errors;
using MainCore.Parsers.Interface;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public class UpdateInfo : AccountBotTask
    {
        private readonly IRightBarParser _rightBarParser;
        private readonly IStockBarParser _stockBarParser;

        public UpdateInfo(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _rightBarParser = Locator.Current.GetService<IRightBarParser>();
            _stockBarParser = Locator.Current.GetService<IStockBarParser>();
        }

        public override Result Execute()
        {
            var commands = new List<Func<Result>>()
            {
                UpdateAccountInfo,
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

        private Result UpdateAccountInfo()
        {
            var html = _chromeBrowser.GetHtml();
            //var tribe = _rightBarParser.GetTribe(html);
            var hasPlusAccount = _rightBarParser.HasPlusAccount(html);
            var gold = _stockBarParser.GetGold(html);
            var silver = _stockBarParser.GetSilver(html);

            using var context = _contextFactory.CreateDbContext();
            var account = context.AccountsInfo.Find(AccountId);

            account.HasPlusAccount = hasPlusAccount;
            account.Gold = gold;
            account.Silver = silver;

            //if (account.Tribe == TribeEnums.Any) account.Tribe = (TribeEnums)tribe;

            context.SaveChanges();
            return Result.Ok();
        }
    }
}