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
    public class SleepHelper : ISleepHelper
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

        public Result Execute(int accountId)
        {
            _nextAccess = ChooseNextAccess();

            if (_nextAccess is null || IsForceSleep())
            {
                var sleepTime = GetSleepTime();
                _sleepEnd = DateTime.Now.Add(sleepTime);
                _logManager.Information(accountId, $"No proxy vaild or force sleep is acitve. Bot will sleep {(int)sleepTime.TotalMinutes} mins");
            }
            else
            {
                var sleepTime = TimeSpan.FromSeconds(Random.Shared.Next(14 * 60, 16 * 60));
                _sleepEnd = DateTime.Now.Add(sleepTime);
                var proxyHost = string.IsNullOrEmpty(_nextAccess.ProxyHost) ? "default" : _nextAccess.ProxyHost;
                _logManager.Information(accountId, $"There is vaild proxy ({proxyHost}). Bot will sleep {(int)sleepTime.TotalMinutes} mins before switching to vaild proxy");
            }
            _result = Sleep();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            _result = WakeUp();
            if (_result.IsFailed) return _result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private Access ChooseNextAccess()
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
                    _logManager.Information(accountId, $"Proxy {access.ProxyHost} is not working");
                }
            }

            return null;
        }

        private Result Sleep()
        {
            _chromeBrowser.Close();
            int lastMinute = 0;

            while (true)
            {
                if (_token.IsCancellationRequested)
                {
                    _logManager.Information(accountId, "Cancellation requested");
                    return Result.Ok();
                }
                var timeRemaining = _sleepEnd - DateTime.Now;
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

        private Result WakeUp()
        {
            var status = _taskManager.GetAccountStatus(accountId);
            if (status != Enums.AccountStatus.Stopping && status != Enums.AccountStatus.Pausing)
            {
                using var context = _contextFactory.CreateDbContext();
                // just use current access
                if (_nextAccess is null)
                {
                    var accesses = context.Accesses.Where(x => x.AccountId == accountId).OrderBy(x => x.LastUsed).ToList();
                    _nextAccess = accesses.Last();
                }
                var setting = context.AccountsSettings.Find(accountId);
                _chromeBrowser.Setup(_nextAccess, setting);
                var currentAccount = context.Accounts.Find(accountId);
                _chromeBrowser.Navigate(currentAccount.Server);
                _taskManager.Add(accountId, new LoginTask(accountId), first: true);
            }
            return Result.Ok();
        }

        private bool IsForceSleep()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(accountId);
            return setting.IsSleepBetweenProxyChanging;
        }

        private TimeSpan GetSleepTime()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(accountId);
            (var min, var max) = (setting.SleepTimeMin, setting.SleepTimeMax);

            var time = TimeSpan.FromSeconds(Random.Shared.Next(min * 60, max * 60));

            return time;
        }

        public TimeSpan GetWorkTime()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(accountId);
            (var min, var max) = (setting.WorkTimeMin, setting.WorkTimeMax);

            var time = TimeSpan.FromSeconds(Random.Shared.Next(min * 60, max * 60));

            return time;
        }
    }
}