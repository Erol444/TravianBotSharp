using FluentResults;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Splat;
using System;

namespace MainCore.Tasks.Update
{
    public class UpdateBothDorf : VillageBotTask
    {
        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly INavigateHelper _navigateHelper;
        private IChromeBrowser _chromeBrowser;

        public UpdateBothDorf(INavigateHelper navigateHelper, IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager)
        {
            _navigateHelper = navigateHelper;
            _contextFactory = contextFactory;
            _chromeManager = chromeManager;
        }

        public override Result Execute()
        {
            _chromeBrowser = _chromeManager.Get(AccountId);
            {
                using var context = _contextFactory.CreateDbContext();
                var village = context.Villages.Find(VillageId);
                if (village is null) Name = $"Update both dorf in {VillageId}";
                else Name = $"Update both dorf in {village.Name}";
            }

            var url = _chromeBrowser.GetCurrentUrl();
            if (url.Contains("dorf2"))
            {
                {
                    var result = _navigateHelper.ToDorf2(AccountId);
                    if (result.IsFailed) return result.WithError("from Update both dorf");
                }
                {
                    var result = _navigateHelper.ToDorf1(AccountId);
                    if (result.IsFailed) return result.WithError("from Update both dorf");
                }
                {
                    var result = UpdateVillage();
                    if (result.IsFailed) return result.WithError("from Update both dorf");
                }
            }
            else if (url.Contains("dorf1"))
            {
                {
                    var result = _navigateHelper.ToDorf1(AccountId);
                    if (result.IsFailed) return result.WithError("from Update both dorf");
                }
                {
                    var result = _navigateHelper.ToDorf2(AccountId);
                    if (result.IsFailed) return result.WithError("from Update both dorf");
                }
                {
                    var result = UpdateVillage();
                    if (result.IsFailed) return result.WithError("from Update both dorf");
                }
            }
            else
            {
                var random = new Random(DateTime.Now.Second);
                if (random.Next(0, 100) > 50)
                {
                    {
                        var result = _navigateHelper.ToDorf1(AccountId);
                        if (result.IsFailed) return result.WithError("from Update both dorf");
                    }
                    {
                        var result = _navigateHelper.ToDorf2(AccountId);
                        if (result.IsFailed) return result.WithError("from Update both dorf");
                    }
                    {
                        var result = UpdateVillage();
                        if (result.IsFailed) return result.WithError("from Update both dorf");
                    }
                }
                else
                {
                    {
                        var result = _navigateHelper.ToDorf2(AccountId);
                        if (result.IsFailed) return result.WithError("from Update both dorf");
                    }
                    {
                        var result = _navigateHelper.ToDorf1(AccountId);
                        if (result.IsFailed) return result.WithError("from Update both dorf");
                    }
                    {
                        var result = UpdateVillage();
                        if (result.IsFailed) return result.WithError("from Update both dorf");
                    }
                }
            }
            return Result.Ok();
        }

        private Result UpdateVillage()
        {
            var taskUpdate = Locator.Current.GetService<UpdateVillage>();
            taskUpdate.SetAccountId(AccountId);
            taskUpdate.SetVillageId(VillageId);
            return taskUpdate.Execute();
        }
    }
}