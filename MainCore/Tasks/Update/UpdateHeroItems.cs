using MainCore.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainCore.Tasks.Update
{
    public class UpdateHeroItems : BotTask
    {
        public UpdateHeroItems(int accountId) : base(accountId)
        {
        }

        public override string Name => "Update hero's items";

        public override void Execute()
        {
            NavigateHelper.ToHeroInventory(ChromeBrowser);
            using var context = ContextFactory.CreateDbContext();
            UpdateHelper.UpdateHeroInventory(context, ChromeBrowser, AccountId);
        }
    }
}