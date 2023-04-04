using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public class SleepTask : AccountBotTask
    {
        private readonly IAccessHelper _accessHelper;

        private readonly IRestClientManager _restClientManager;

        private Access _nextAccess;
        private DateTime _sleepEnd;

        public SleepTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _accessHelper = Locator.Current.GetService<IAccessHelper>();

            _restClientManager = Locator.Current.GetService<IRestClientManager>();
        }

        public override Result Execute()
        {
            var commands = new List<Func<Result>>()
            {
                ChooseNextProxy,
                ChooseTimeWakeup,
                Sleep,
                WakeUp,
                NextExecute,
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

        private Result ChooseNextProxy()
        {
            _nextAccess = ChooseNextAccess();
            return Result.Ok();
        }

        private Result ChooseTimeWakeup()
        {
            if (_nextAccess is null || IsForceSleep())
            {
                var sleepTime = GetSleepTime();
                _sleepEnd = DateTime.Now.Add(sleepTime);
                _logManager.Information(AccountId, $"No proxy vaild or force sleep is acitve. Bot will sleep {(int)sleepTime.TotalMinutes} mins");
            }
            else
            {
                var sleepTime = TimeSpan.FromSeconds(Random.Shared.Next(14 * 60, 16 * 60));
                _sleepEnd = DateTime.Now.Add(sleepTime);
                var proxyHost = string.IsNullOrEmpty(_nextAccess.ProxyHost) ? "default" : _nextAccess.ProxyHost;
                _logManager.Information(AccountId, $"There is vaild proxy ({proxyHost}). Bot will sleep {(int)sleepTime.TotalMinutes} mins before switching to vaild proxy");
            }
            return Result.Ok();
        }

        private Result Sleep()
        {
            _chromeBrowser.Close();
            int lastMinute = 0;

            while (true)
            {
                if (CancellationToken.IsCancellationRequested)
                {
                    _logManager.Information(AccountId, "Cancellation requested");
                    return Result.Ok();
                }
                var timeRemaining = _sleepEnd - DateTime.Now;
                if (timeRemaining < TimeSpan.Zero) return Result.Ok();

                Thread.Sleep(TimeSpan.FromSeconds(1));
                var currentMinute = (int)timeRemaining.TotalMinutes;

                if (lastMinute != currentMinute)
                {
                    _logManager.Information(AccountId, $"Chrome will reopen in {currentMinute} mins");
                    lastMinute = currentMinute;
                }
            }
        }

        private Result WakeUp()
        {
            var status = _taskManager.GetAccountStatus(AccountId);
            if (status != Enums.AccountStatus.Stopping && status != Enums.AccountStatus.Pausing)
            {
                using var context = _contextFactory.CreateDbContext();
                // just use current access
                if (_nextAccess is null)
                {
                    var accesses = context.Accesses.Where(x => x.AccountId == AccountId).OrderBy(x => x.LastUsed).ToList();
                    _nextAccess = accesses.Last();
                }
                var setting = context.AccountsSettings.Find(AccountId);
                _chromeBrowser.Setup(_nextAccess, setting);
                var currentAccount = context.Accounts.Find(AccountId);
                _chromeBrowser.Navigate(currentAccount.Server);
                _taskManager.Add(AccountId, _taskFactory.GetLoginTask(AccountId), first: true);
            }
            return Result.Ok();
        }

        private Result NextExecute()
        {
            ExecuteAt = DateTime.Now.Add(GetWorkTime());
            return Result.Ok();
        }

        private Access ChooseNextAccess()
        {
            var context = _contextFactory.CreateDbContext();
            var accesses = context.Accesses.Where(x => x.AccountId == AccountId).OrderBy(x => x.LastUsed).ToList();
            var currentAccess = accesses.Last();
            accesses.Remove(currentAccess);
            var setting = context.AccountsSettings.Find(AccountId);

            foreach (var access in accesses)
            {
                if (string.IsNullOrEmpty(access.ProxyHost))
                {
                    return access;
                }

                var result = _accessHelper.IsValid(_restClientManager.Get(new(access)));
                if (result)
                {
                    access.LastUsed = DateTime.Now;
                    context.SaveChanges();
                    return access;
                }
                else
                {
                    _logManager.Information(AccountId, $"Proxy {access.ProxyHost} is not working");
                }
            }

            return null;
        }

        private bool IsForceSleep()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(AccountId);
            return setting.IsSleepBetweenProxyChanging;
        }

        private TimeSpan GetSleepTime()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(AccountId);
            (var min, var max) = (setting.SleepTimeMin, setting.SleepTimeMax);

            var time = TimeSpan.FromSeconds(Random.Shared.Next(min * 60, max * 60));

            return time;
        }

        private TimeSpan GetWorkTime()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(AccountId);
            (var min, var max) = (setting.WorkTimeMin, setting.WorkTimeMax);

            var time = TimeSpan.FromSeconds(Random.Shared.Next(min * 60, max * 60));

            return time;
        }
    }
}