using FluentResults;
using MainCore.Enums;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Splat;
using System.Linq;
using ILogManager = MainCore.Services.Interface.ILogManager;

namespace MainCore.Tasks.Update
{
    public class UpdateFarmList : AccountBotTask
    {
        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private IChromeBrowser _chromeBrowser;

        private readonly ICheckHelper _checkHelper;
        private readonly INavigateHelper _navigateHelper;
        private readonly ILogManager _logManager;
        private readonly IUpdateHelper _updateHelper;
        private readonly IEventManager _eventManager;

        public UpdateFarmList(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, ICheckHelper checkHelper, ILogManager logManager)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _checkHelper = checkHelper;
            _logManager = logManager;
        }

        public override Result Execute()
        {
            _chromeBrowser = _chromeManager.Get(AccountId);

            var village = GetVillageHasRallyPoint();
            if (village == -1)
            {
                _logManager.Warning(AccountId, "There is no rallypoint in your villages");
                return Result.Ok();
            }
            {
                var result = GotoFarmListPage(village);
                if (result.IsFailed) return result.WithError("from GotoFarmListPage");
            }
            {
                var result = _updateHelper.UpdateFarmList(AccountId);
                if (result.IsFailed) return result.WithError("from UpdateFarmList");
                _eventManager.OnFarmListUpdate(AccountId);
            }
            return Result.Ok();
        }

        private int GetVillageHasRallyPoint()
        {
            using var context = _contextFactory.CreateDbContext();

            var currentVillage = _checkHelper.GetCurrentVillageId(AccountId);
            if (currentVillage != -1)
            {
                var building = context.VillagesBuildings
                   .Where(x => x.VillageId == currentVillage)
                   .FirstOrDefault(x => x.Type == BuildingEnums.RallyPoint && x.Level > 0);
                if (building is not null) return currentVillage;
            }

            var villages = context.Villages.Where(x => x.AccountId == AccountId);

            foreach (var village in villages)
            {
                var building = context.VillagesBuildings
                    .Where(x => x.VillageId == village.Id)
                    .FirstOrDefault(x => x.Type == BuildingEnums.RallyPoint && x.Level > 0);
                if (building is null) continue;
                return village.Id;
            }
            return -1;
        }

        private Result GotoFarmListPage(int village)
        {
            if (!_checkHelper.IsFarmListPage(AccountId))
            {
                using var context = _contextFactory.CreateDbContext();
                var url = _chromeBrowser.GetCurrentUrl();
                {
                    var taskUpdate = Locator.Current.GetService<UpdateDorf2>();
                    taskUpdate.SetAccountId(AccountId);
                    taskUpdate.SetVillageId(village);
                    var result = taskUpdate.Execute();
                    if (result.IsFailed) return result.WithError("from go to farmlist page");
                }
                {
                    var result = _navigateHelper.GoToBuilding(AccountId, 39);
                    if (result.IsFailed) return result.WithError("from go to farmlist page");
                }
                {
                    var result = _navigateHelper.SwitchTab(AccountId, 4);
                    if (result.IsFailed) return result.WithError("from go to farmlist page");
                }
            }
            return Result.Ok();
        }
    }
}