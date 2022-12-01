using FluentResults;
using MainCore.Helper.Interface;
using Microsoft.EntityFrameworkCore;
using Splat;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf2 : VillageBotTask
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly INavigateHelper _navigateHelper;

        public UpdateDorf2(INavigateHelper navigateHelper, IDbContextFactory<AppDbContext> contextFactory)
        {
            _navigateHelper = navigateHelper;
            _contextFactory = contextFactory;
        }

        public override Result Execute()
        {
            {
                using var context = _contextFactory.CreateDbContext();
                var village = context.Villages.Find(VillageId);
                if (village is null) Name = $"Update dorf2 in {VillageId}";
                else Name = $"Update dorf2 in {village.Name}";
            }

            {
                var result = _navigateHelper.ToDorf2(AccountId);
                if (result.IsFailed) return result.WithError("from Update dorf2");
            }
            {
                var result = UpdateVillage();
                if (result.IsFailed) return result.WithError("from Update dorf2");
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