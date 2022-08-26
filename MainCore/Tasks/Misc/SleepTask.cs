using MainCore.Helper;
using MainCore.Models.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MainCore.Tasks.Misc
{
    public class SleepTask : BotTask
    {
        private readonly Random random = new();

        public SleepTask(int accountId) : base(accountId)
        {
        }

        public override string Name => "Sleep";

        public override void Execute()
        {
            var context = ContextFactory.CreateDbContext();
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

                var result = AccessHelper.CheckAccess(RestClientManager.Get(access.Id), access.ProxyHost);
                if (result)
                {
                    selectedAccess = access;
                    access.LastUsed = DateTime.Now;
                    context.SaveChanges();
                    break;
                }
                else
                {
                    LogManager.Information(AccountId, $"Proxy {access.ProxyHost} is not working");
                }
            }
            if (selectedAccess is null || selectedAccess.Id == currentAccess.Id)
            {
                (var min, var max) = (setting.SleepTimeMin, setting.SleepTimeMax);

                var time = TimeSpan.FromMinutes(random.Next(min, max));
                ChromeBrowser.Close();
                LogManager.Information(AccountId, $"Bot is sleeping in {time} minute(s)");
                Task.Delay(time, Cts.Token).Wait();
                if (Cts.IsCancellationRequested) return;
            }
            else
            {
                ChromeBrowser.Close();
                LogManager.Information(AccountId, $"Bot is sleeping in {3} minute(s)");
                Task.Delay(TimeSpan.FromMinutes(3), Cts.Token).Wait();
            }
            ChromeBrowser.Setup(selectedAccess, setting);
            var currentAccount = context.Accounts.Find(AccountId);
            ChromeBrowser.Navigate(currentAccount.Server);
            TaskManager.Add(AccountId, new LoginTask(AccountId), true);
        }
    }
}