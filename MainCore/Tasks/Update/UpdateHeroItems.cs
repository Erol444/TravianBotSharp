using MainCore.Helper;

namespace MainCore.Tasks.Update
{
    public class UpdateHeroItems : AccountBotTask
    {
        public UpdateHeroItems(int accountId) : base(accountId, "Update hero's items")
        {
        }

        public override void Execute()
        {
            NavigateHelper.ToHeroInventory(_chromeBrowser);
            using var context = _contextFactory.CreateDbContext();
            UpdateHelper.UpdateHeroInventory(context, _chromeBrowser, AccountId);
        }
    }
}