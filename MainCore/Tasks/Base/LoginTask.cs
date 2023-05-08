using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using OpenQA.Selenium;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public abstract class LoginTask : AccountBotTask
    {
        protected readonly ICheckHelper _checkHelper;

        protected readonly ISystemPageParser _systemPageParser;

        protected readonly IPlanManager _planManager;

        public LoginTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _checkHelper = Locator.Current.GetService<ICheckHelper>();
            _systemPageParser = Locator.Current.GetService<ISystemPageParser>();
            _planManager = Locator.Current.GetService<IPlanManager>();
        }

        public override Result Execute()
        {
            var commands = new List<Func<Result>>()
            {
                AcceptCookie,
                Login,
                AddTask,
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

        protected Result AcceptCookie()
        {
            var html = _chromeBrowser.GetHtml();

            if (html.DocumentNode.Descendants("a").Any(x => x.HasClass("cmpboxbtn") && x.HasClass("cmpboxbtnyes")))
            {
                var driver = _chromeBrowser.GetChrome();
                var acceptCookie = driver.FindElements(By.ClassName("cmpboxbtnyes"));
                var result = _generalHelper.Click(AccountId, acceptCookie[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        protected abstract Result Login();

        protected Result AddTask()
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

            return Result.Ok();
        }
    }
}