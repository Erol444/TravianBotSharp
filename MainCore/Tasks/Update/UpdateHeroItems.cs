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
            IsFail = true;
            using var context = _contextFactory.CreateDbContext();
            NavigateHelper.ToHeroInventory(_chromeBrowser, context, AccountId);
            NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);

            UpdateHelper.UpdateHeroInventory(context, _chromeBrowser, AccountId);
            IsFail = false;
        }
    }
}