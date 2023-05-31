using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using MainCore.Tasks.FunctionTasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Helper.Implementations.Base
{
    public sealed class SleepHelper : ISleepHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IChromeManager _chromeManager;
        private readonly IAccessHelper _accessHelper;
        private readonly IRestClientManager _restClientManager;
        private readonly ILogManager _logManager;
        private readonly ITaskManager _taskManager;

        public SleepHelper(IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager, IAccessHelper accessHelper, IRestClientManager restClientManager, ILogManager logManager, ITaskManager taskManager)
        {
            _contextFactory = contextFactory;
            _chromeManager = chromeManager;
            _accessHelper = accessHelper;
            _restClientManager = restClientManager;
            _logManager = logManager;
            _taskManager = taskManager;
        }

        public Result Execute(int accountId, CancellationToken token)
        {
            var sleepEnd = DateTime.Now;
            var nextAccess = GetNextAccess(accountId);

            if (nextAccess is null || IsForceSleep(accountId))
            {
                var sleepTime = GetSleepTime(accountId);
                sleepEnd = sleepEnd.Add(sleepTime);
                _logManager.Information(accountId, $"No proxy vaild or force sleep is acitve. Bot will sleep {(int)sleepTime.TotalMinutes} mins");
            }
            else
            {
                var sleepTime = TimeSpan.FromSeconds(Random.Shared.Next(14 * 60, 16 * 60));
                sleepEnd = sleepEnd.Add(sleepTime);
                var proxyHost = string.IsNullOrEmpty(nextAccess.ProxyHost) ? "default" : nextAccess.ProxyHost;
                _logManager.Information(accountId, $"There is vaild proxy ({proxyHost}). Bot will sleep {(int)sleepTime.TotalMinutes} mins before switching to vaild proxy");
            }

            var result = Sleep(accountId, sleepEnd, token);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = WakeUp(accountId, nextAccess);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Access GetNextAccess(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var accesses = context.Accesses.Where(x => x.AccountId == accountId).OrderBy(x => x.LastUsed).ToList();
            var currentAccess = accesses.Last();
            accesses.Remove(currentAccess);

            var setting = context.AccountsSettings.Find(accountId);

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
                    _logManager.Warning(accountId, $"Proxy {access.ProxyHost} is not working");
                }
            }

            return null;
        }

        public Result Sleep(int accountId, DateTime sleepEnd, CancellationToken token)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            chromeBrowser.Close();

            int lastMinute = 0;

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    _logManager.Information(accountId, "Cancellation requested");
                    return Result.Ok();
                }
                var timeRemaining = sleepEnd - DateTime.Now;
                if (timeRemaining < TimeSpan.Zero) return Result.Ok();

                Thread.Sleep(TimeSpan.FromSeconds(1));
                var currentMinute = (int)timeRemaining.TotalMinutes;

                if (lastMinute != currentMinute)
                {
                    _logManager.Information(accountId, $"Chrome will reopen in {currentMinute} mins");
                    lastMinute = currentMinute;
                }
            }
        }

        public Result WakeUp(int accountId, Access nextAccess)
        {
            var status = _taskManager.GetAccountStatus(accountId);
            if (status != Enums.AccountStatus.Stopping && status != Enums.AccountStatus.Pausing)
            {
                using var context = _contextFactory.CreateDbContext();
                // just use current access
                if (nextAccess is null)
                {
                    var accesses = context.Accesses.Where(x => x.AccountId == accountId).OrderBy(x => x.LastUsed).ToList();
                    nextAccess = accesses.Last();
                }
                var setting = context.AccountsSettings.Find(accountId);
                var chromeBrowser = _chromeManager.Get(accountId);
                chromeBrowser.Setup(nextAccess, setting);
                var currentAccount = context.Accounts.Find(accountId);
                chromeBrowser.Navigate(currentAccount.Server);
                _taskManager.Add(accountId, new LoginTask(accountId), first: true);
            }
            return Result.Ok();
        }

        public bool IsForceSleep(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(accountId);
            return setting.IsSleepBetweenProxyChanging;
        }

        public TimeSpan GetSleepTime(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(accountId);
            (var min, var max) = (setting.SleepTimeMin, setting.SleepTimeMax);

            var time = TimeSpan.FromSeconds(Random.Shared.Next(min * 60, max * 60));

            return time;
        }

        public TimeSpan GetWorkTime(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(accountId);
            (var min, var max) = (setting.WorkTimeMin, setting.WorkTimeMax);

            var time = TimeSpan.FromSeconds(Random.Shared.Next(min * 60, max * 60));

            return time;
        }
    }
}