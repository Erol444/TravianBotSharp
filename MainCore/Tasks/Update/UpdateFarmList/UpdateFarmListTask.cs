using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Database;
using Splat;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Update.UpdateFarmList
{
    public abstract class UpdateFarmListTask : AccountBotTask
    {
        private readonly IUpdateHelper _updateHelper;
        private readonly ICheckHelper _checkHelper;
        protected int _villageHasRallyPoint;

        public UpdateFarmListTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
            _checkHelper = Locator.Current.GetService<ICheckHelper>();
        }

        public override Result Execute()
        {
            _villageHasRallyPoint = GetVillageHasRallyPoint();
            if (_villageHasRallyPoint == -1)
            {
                return Result.Fail(new Skip("There is no rallypoint in your villages"));
            }

            if (!IsFarmListPage())
            {
                {
                    var result = UpdateDorf2();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                }

                {
                    var result = GotoRallypoint();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                }

                if (!IsFarmListTab())
                {
                    var result = GotoFarmListTab();
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
                }
            }

            {
                var result = UpdateFarmLists();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }

            return Result.Ok();
        }

        protected abstract bool IsFarmListPage();

        private Result UpdateDorf2()
        {
            var taskUpdate = _taskFactory.CreateUpdateDorf2Task(_villageHasRallyPoint, AccountId, CancellationToken);
            var result = taskUpdate.Execute();
            return result;
        }

        protected abstract Result GotoRallypoint();

        protected abstract bool IsFarmListTab();

        protected abstract Result GotoFarmListTab();

        private Result UpdateFarmLists()
        {
            var chromeBrowser = _chromeManager.Get(AccountId);
            var html = chromeBrowser.GetHtml();

            var farmNodes = GetFarmNodes(html);
            var farms = new List<Farm>();
            foreach (var farmNode in farmNodes)
            {
                var name = GetName(farmNode);
                var id = GetId(farmNode);
                var count = GetNumOfFarms(farmNode);
                var farm = new Farm()
                {
                    AccountId = AccountId,
                    Id = id,
                    Name = name,
                    FarmCount = count,
                };
                farms.Add(farm);
            }
            using var context = _contextFactory.CreateDbContext();
            var farmsOld = context.Farms.Where(x => x.AccountId == AccountId).ToList();
            foreach (var farm in farms)
            {
                var existFarm = context.Farms.FirstOrDefault(x => x.Id == farm.Id);
                if (existFarm is null)
                {
                    context.Farms.Add(farm);
                    context.AddFarm(farm.Id);
                }
                else
                {
                    existFarm.Name = farm.Name;
                    existFarm.FarmCount = farm.FarmCount;
                    farmsOld.Remove(farmsOld.FirstOrDefault(x => x.Id == farm.Id));
                }
            }

            foreach (var farm in farmsOld)
            {
                context.Remove(farm);
                context.DeleteFarm(farm.Id);
            }

            context.SaveChanges();
            _eventManager.OnFarmListUpdate(AccountId);
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

        protected abstract List<HtmlNode> GetFarmNodes(HtmlDocument doc);

        protected abstract string GetName(HtmlNode node);

        protected abstract int GetId(HtmlNode node);

        protected abstract int GetNumOfFarms(HtmlNode node);
    }
}