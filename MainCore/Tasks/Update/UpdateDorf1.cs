using MainCore.Helper;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf1 : UpdateVillage
    {
        public UpdateDorf1(int villageId, int accountId) : base(villageId, accountId, "Update Resources page")
        {
        }

        public override void Execute()
        {
            {
                using var context = _contextFactory.CreateDbContext();
                NavigateHelper.ToDorf1(_chromeBrowser, context, AccountId);
            }
            base.Execute();
        }
    }
}