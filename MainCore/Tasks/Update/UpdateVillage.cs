using MainCore.Enums;
using MainCore.Helper;
using MainCore.TravianData;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainCore.Tasks.Update
{
    public class UpdateVillage : UpdateInfo
    {
        public UpdateVillage(int villageId, int accountId) : base(accountId)
        {
            VillageId = villageId;
        }

        public override string Name => $"Update village {VillageId}";

        public int VillageId { get; protected set; }

        public override async Task Execute()
        {
            Navigate();
            await base.Execute();
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