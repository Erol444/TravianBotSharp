using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Sim;
using ModuleCore.Parser;
using Splat;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Update
{
    public class UpdateAdventures : AccountBotTask
    {
        private readonly IUpdateHelper _updateHelper;

        private readonly ISystemPageParser _systemPageParser;

        public UpdateAdventures(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
            _systemPageParser = Locator.Current.GetService<ISystemPageParser>();
        }

        public override Result Execute()
        {
            {
                var result = _navigateHelper.ToAdventure(AccountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = _updateHelper.UpdateAdventures(AccountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var updateInfo = new UpdateInfo(AccountId);
                var result = updateInfo.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = SendAdventures();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        private Result SendAdventures()
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

        private void NextExecute()
        {
            var html = _chromeBrowser.GetHtml();
            var tileDetails = _systemPageParser.GetAdventuresDetail(html);
            if (tileDetails is null)
            {
                ExecuteAt = DateTime.Now.AddMinutes(Random.Shared.Next(5, 10));
                return;
            }
            var timer = tileDetails.Descendants("span").FirstOrDefault(x => x.HasClass("timer"));
            if (timer is null)
            {
                ExecuteAt = DateTime.Now.AddMinutes(Random.Shared.Next(5, 10));
                return;
            }

            int sec = int.Parse(timer.GetAttributeValue("value", "0"));
            if (sec < 0) sec = 0;
            if (VersionDetector.IsTravianOfficial())
            {
                ExecuteAt = DateTime.Now.AddSeconds(sec * 2 + Random.Shared.Next(20, 40));
            }
            else if (VersionDetector.IsTTWars())
            {
                ExecuteAt = DateTime.Now.AddSeconds(sec * 2 + 1);
            }
        }
    }
}