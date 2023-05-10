using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using OpenQA.Selenium;
using Splat;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public abstract class LoginTask : AccountBotTask
    {
        private readonly ISystemPageParser _systemPageParser;
        private readonly IPlanManager _planManager;

        private readonly IGeneralHelper _generalHelper;

        private IChromeBrowser _chromeBrowser;

        public LoginTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _systemPageParser = Locator.Current.GetService<ISystemPageParser>();
            _planManager = Locator.Current.GetService<IPlanManager>();

            _generalHelper = Locator.Current.GetService<IGeneralHelper>();
        }

        public override Result Execute()
        {
            _generalHelper.Load(-1, AccountId, CancellationToken);
            _chromeBrowser = _chromeManager.Get(AccountId);

            Result result;

            result = AcceptCookie();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            result = Login();
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            AddTask();
            return Result.Ok();
        }

        private Result AcceptCookie()
        {
            var html = _chromeBrowser.GetHtml();

            if (html.DocumentNode.Descendants("a").Any(x => x.HasClass("cmpboxbtn") && x.HasClass("cmpboxbtnyes")))
            {
                var result = _generalHelper.Click(By.ClassName("cmpboxbtnyes"), false);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        private Result Login()
        {
            var html = _chromeBrowser.GetHtml();

            var usernameNode = _systemPageParser.GetUsernameNode(html);

            var passwordNode = _systemPageParser.GetPasswordNode(html);

            var buttonNode = _systemPageParser.GetLoginButton(html);
            if (buttonNode is null)
            {
                _logManager.Information(AccountId, "Account is already logged in. Skip login task");
                return Result.Ok();
            }

            if (usernameNode is null)
            {
                return Result.Fail(new Retry("Cannot find username box"));
            }

            if (passwordNode is null)
            {
                return Result.Fail(new Retry("Cannot find password box"));
            }

            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(AccountId);
            var access = context.Accesses.Where(x => x.AccountId == AccountId).OrderByDescending(x => x.LastUsed).FirstOrDefault();

            Result result;

            result = _generalHelper.Input(By.XPath(usernameNode.XPath), account.Username);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Input(By.XPath(passwordNode.XPath), access.Password);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Click(By.XPath(buttonNode.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private void AddTask()
        {
            _taskManager.Add(AccountId, new UpdateInfo(AccountId));

            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == AccountId);
            var listTask = _taskManager.GetList(AccountId);
            var upgradeBuildingList = listTask.OfType<UpgradeBuilding>();
            var updateList = listTask.OfType<UpdateDorf1>();

            var trainTroopList = listTask.OfType<TrainTroopsTask>();
            foreach (var village in villages)
            {
                var queue = _planManager.GetList(village.Id);
                if (queue.Any())
                {
                    var upgradeBuilding = upgradeBuildingList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (upgradeBuilding is null)
                    {
                        _taskManager.Add(AccountId, _taskFactory.GetUpgradeBuildingTask(village.Id, AccountId));
                    }
                }
                var setting = context.VillagesSettings.Find(village.Id);
                if (setting.IsAutoRefresh)
                {
                    var update = updateList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (update is null)
                    {
                        _taskManager.Add(AccountId, _taskFactory.GetRefreshVillageTask(village.Id, AccountId));
                    }
                }

                if (setting.BarrackTroop != 0 || setting.StableTroop != 0 || setting.WorkshopTroop != 0)
                {
                    var update = trainTroopList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (update is null)
                    {
                        _taskManager.Add(AccountId, _taskFactory.GetTrainTroopTask(village.Id, AccountId));
                    }
                }
            }
        }
    }
}