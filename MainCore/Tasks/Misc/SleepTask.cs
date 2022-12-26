using FluentResults;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using Splat;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Misc
{
    public class SleepTask : AccountBotTask
    {
        private readonly IAccessHelper _accessHelper;

        private readonly IRestClientManager _restClientManager;

        public SleepTask(int accountId) : base(accountId)
        {
            _accessHelper = Locator.Current.GetService<IAccessHelper>();

            _restClientManager = Locator.Current.GetService<IRestClientManager>();
        }

        public override Result Execute()
        {
            var context = _contextFactory.CreateDbContext();
            var accesses = context.Accesses.Where(x => x.AccountId == AccountId).OrderBy(x => x.LastUsed);
            var currentAccess = accesses.Last();
            var setting = context.AccountsSettings.Find(AccountId);

            Access selectedAccess = null;
            foreach (var access in accesses)
            {
                if (string.IsNullOrEmpty(access.ProxyHost))
                {
                    selectedAccess = access;
                    break;
                }

                var result = _accessHelper.IsValid(_restClientManager.Get(new(access)));
                if (result)
                {
                    selectedAccess = access;
                    access.LastUsed = DateTime.Now;
                    context.SaveChanges();
                    break;
                }
                else
                {
                    _logManager.Information(AccountId, $"Proxy {access.ProxyHost} is not working");
                }
            }
            if (selectedAccess is null || selectedAccess.Id == currentAccess.Id)
            {
                (var min, var max) = (setting.SleepTimeMin, setting.SleepTimeMax);

                var time = TimeSpan.FromMinutes(Random.Shared.Next(min, max));
                _chromeBrowser.Close();
                _logManager.Information(AccountId, $"Bot is sleeping in {time} minute(s)");
                Thread.Sleep(time * 60 * 1000);
            }
            else
            {
                _chromeBrowser.Close();
                _logManager.Information(AccountId, $"Bot is sleeping in {3} minute(s)");
                Thread.Sleep(3 * 60 * 1000);
            }
            _chromeBrowser.Setup(selectedAccess, setting);
            var currentAccount = context.Accounts.Find(AccountId);
            _chromeBrowser.Navigate(currentAccount.Server);
            _taskManager.Add(AccountId, new LoginTask(AccountId), true);

            var nextExecute = Random.Shared.Next(setting.SleepTimeMin, setting.SleepTimeMax);
            ExecuteAt = DateTime.Now.AddMinutes(nextExecute);
            return Result.Ok();
        }
    }
}