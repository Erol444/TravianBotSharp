using MainCore.Helper;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf2 : VillageBotTask
    {
        public UpdateDorf2(int villageId, int accountId) : base(villageId, accountId, "Update Buildings page")
        {
        }

        public override void Execute()
        {
            IsFail = true;
            ToDorf2();
            if (IsUpdateFail()) return;
            IsFail = false;
        }

        private void ToDorf2()
        {
            using var context = _contextFactory.CreateDbContext();
            NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
        }

        private bool IsUpdateFail()
        {
            var taskUpdate = new UpdateVillage(VillageId, AccountId);
            taskUpdate.CopyFrom(this);
            taskUpdate.Execute();
            return taskUpdate.IsFail;
        }
    }
}