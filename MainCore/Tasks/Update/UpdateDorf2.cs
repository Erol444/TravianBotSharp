using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf2 : VillageBotTask
    {
        private readonly INavigateHelper _navigateHelper;

        public UpdateDorf2(int villageId, int accountId) : base(villageId, accountId)
        {
            _navigateHelper = Locator.Current.GetService<INavigateHelper>();
        }

        public override Result Execute()
        {
            {
                var result = _navigateHelper.ToDorf2(AccountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var taskUpdate = new UpdateVillage(VillageId, AccountId);
                var result = taskUpdate.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }
    }
}