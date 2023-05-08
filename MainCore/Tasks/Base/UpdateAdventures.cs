using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public abstract class UpdateAdventures : AccountBotTask
    {
        protected readonly IUpdateHelper _updateHelper;

        protected readonly ISystemPageParser _systemPageParser;

        public UpdateAdventures(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
            _systemPageParser = Locator.Current.GetService<ISystemPageParser>();
        }

        public override Result Execute()
        {
            var commands = new List<Func<Result>>()
            {
                ToAdventure,
                UpdateAdventureList,
                UpdateInfo,
                SendAdventures,
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

        protected Result ToAdventure()
        {
            var result = _generalHelper.ToAdventure(AccountId);
            return result;
        }

        protected Result UpdateAdventureList()
        {
            var result = _updateHelper.UpdateAdventures(AccountId);
            return result;
        }

        protected Result UpdateInfo()
        {
            var updateInfo = new UpdateInfo(AccountId);
            var result = updateInfo.Execute();
            return result;
        }

        protected Result SendAdventures()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(AccountId);
            if (!setting.IsAutoAdventure)
            {
                return Result.Ok();
            }
            var hero = context.Heroes.Find(AccountId);
            if (hero.Status != Enums.HeroStatusEnums.Home)
            {
                _logManager.Warning(AccountId, "Hero is not at home. Cannot start adventures", this);
                return Result.Ok();
            }
            var adventures = context.Adventures.Where(a => a.AccountId == AccountId);
            if (!adventures.Any())
            {
                return Result.Ok();
            }

            {
                var taskAutoSend = new StartAdventure(AccountId, CancellationToken);
                var result = taskAutoSend.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var taskUpdate = new UpdateInfo(AccountId, CancellationToken);
                var result = taskUpdate.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            NextExecute();
            return Result.Ok();
        }

        protected abstract void NextExecute();
    }
}