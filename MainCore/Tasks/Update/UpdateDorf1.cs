using MainCore.Helper.Implementations;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf1 : VillageBotTask
    {
        public UpdateDorf1(int villageId, int accountId) : base(villageId, accountId, "Update Resources page")
        {
        }

        public override void Execute()
        {
            {
                using var context = _contextFactory.CreateDbContext();
                NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);
            }
            IsFail = true;
            ToDorf1();
            if (IsUpdateFail()) return;
            IsFail = false;
        }

        private void ToDorf1()
        {
            using var context = _contextFactory.CreateDbContext();
            NavigateHelper.ToDorf1(_chromeBrowser, context, AccountId);
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