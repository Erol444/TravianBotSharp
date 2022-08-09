using MainCore.Helper;

namespace MainCore.Tasks.Update
{
    public class UpdateVillage : UpdateInfo
    {
        public UpdateVillage(int villageId, int accountId) : base(accountId)
        {
            _villageId = villageId;
        }

        public override string Name => $"Update village {VillageId}";

        private readonly int _villageId;
        public int VillageId => _villageId;

        public override void Execute()
        {
            Navigate();
            base.Execute();
            Update();
        }

        private void Navigate()
        {
            using var context = ContextFactory.CreateDbContext();
            NavigateHelper.SwitchVillage(context, ChromeBrowser, VillageId);
        }

        private void Update()
        {
            using var context = ContextFactory.CreateDbContext();
            var currentUrl = ChromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf"))
            {
                UpdateHelper.UpdateCurrentlyBuilding(context, ChromeBrowser, VillageId);
            }
            if (currentUrl.Contains("dorf1"))
            {
                UpdateHelper.UpdateDorf1(context, ChromeBrowser, VillageId);
            }
            else if (currentUrl.Contains("dorf2"))
            {
                UpdateHelper.UpdateDorf2(context, ChromeBrowser, AccountId, VillageId);
            }

            UpdateHelper.UpdateResource(context, ChromeBrowser, VillageId);
        }
    }
}