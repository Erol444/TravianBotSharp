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
        private readonly ILogHelper _logHelper;
        private readonly ITaskManager _taskManager;

        public SleepHelper(IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager, IAccessHelper accessHelper, ILogHelper logHelper, ITaskManager taskManager)
        {
            _contextFactory = contextFactory;
            _chromeManager = chromeManager;
            _accessHelper = accessHelper;
            _logHelper = logHelper;
            _taskManager = taskManager;
        }

        public Result Execute(int accountId, CancellationToken token)
        {
            var sleepEnd = DateTime.Now;
            var (nextAccess, isSameAccess) = _accessHelper.GetNextAccess(accountId);
            if (isSameAccess || IsForceSleep(accountId))
            {
                var sleepTime = GetSleepTime(accountId);
                sleepEnd = sleepEnd.Add(sleepTime);
                _logHelper.Information(accountId, $"Only 1 connection or force sleep is acitve. Bot will sleep {(int)sleepTime.TotalMinutes} mins");
            }
            else
            {
                var sleepTime = TimeSpan.FromSeconds(Random.Shared.Next(14 * 60, 16 * 60));
                sleepEnd = sleepEnd.Add(sleepTime);
                var proxyHost = string.IsNullOrEmpty(nextAccess.ProxyHost) ? "default" : nextAccess.ProxyHost;
                _logHelper.Information(accountId, $"There are more than 1 connection ({proxyHost}). Bot will sleep {(int)sleepTime.TotalMinutes} mins before switching to new connection");
            }

            var chromeBrowser = _chromeManager.Get(accountId);
            chromeBrowser.Close();

            var result = Sleep(accountId, sleepEnd, token);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = WakeUp(accountId, nextAccess);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result Sleep(int accountId, DateTime sleepEnd, CancellationToken token)
        {
            int lastMinute = 0;

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    _logHelper.Information(accountId, "Cancellation requested");
                    return Result.Ok();
                }
                var timeRemaining = sleepEnd - DateTime.Now;
                if (timeRemaining < TimeSpan.Zero) return Result.Ok();

                Thread.Sleep(TimeSpan.FromSeconds(1));
                var currentMinute = (int)timeRemaining.TotalMinutes;

                if (lastMinute != currentMinute)
                {
                    _logHelper.Information(accountId, $"Chrome will reopen in {currentMinute} mins");
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
                _taskManager.Add<LoginTask>(accountId, first: true);
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