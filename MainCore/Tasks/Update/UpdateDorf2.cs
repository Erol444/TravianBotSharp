using MainCore.Helper;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf2 : UpdateVillage
    {
        public UpdateDorf2(int villageId, int accountId) : base(villageId, accountId, "Update Buildings page")
        {
        }

        public override void Execute()
        {
            {
                using var context = _contextFactory.CreateDbContext();
                NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
            }
            base.Execute();
        }
    }
}