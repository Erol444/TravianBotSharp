using MainCore.Enums;
using MainCore.TravianData;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if TRAVIAN_OFFICIAL

using TravianOfficialCore.Parsers;

#elif TRAVIAN_OFFICIAL_HEROUI

using TravianOfficialNewHeroUICore.Parsers;

#elif TTWARS

using TTWarsCore.Parsers;

#endif

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
            if (!IsCorrectVillage())
            {
                SwitchVillage();
            }

            await base.Execute();
            Update();
        }

        private void Update()
        {
            var currentUrl = ChromeBrowser.GetCurrentUrl();
            var dorf = 0;
            if (currentUrl.Contains("dorf")) UpdateCurrentlyBuilding();
            if (currentUrl.Contains("dorf1"))
            {
                UpdateDorf1();
                dorf = 1;
            }
            else if (currentUrl.Contains("dorf2"))
            {
                UpdateDorf2();
                dorf = 2;
            }

            UpdateResource();

            using var context = ContextFactory.CreateDbContext();
            var updateTime = context.VillagesUpdateTime.Find(VillageId);
            if (updateTime is null)
            {
                switch (dorf)
                {
                    case 1:
                        updateTime = new()
                        {
                            VillageId = VillageId,
                            Dorf1 = DateTime.Now
                        };
                        break;

                    case 2:

                        updateTime = new()
                        {
                            VillageId = VillageId,
                            Dorf2 = DateTime.Now
                        };
                        break;

                    default:
                        break;
                }

                context.VillagesUpdateTime.Add(updateTime);
            }
            else
            {
                switch (dorf)
                {
                    case 1:

                        updateTime.Dorf1 = DateTime.Now;
                        break;

                    case 2:

                        updateTime.Dorf2 = DateTime.Now;
                        break;

                    default:
                        break;
                }
            }
            context.SaveChanges();
        }

        private bool IsCorrectVillage()
        {
            var html = ChromeBrowser.GetHtml();

            var listNode = VillagesTable.GetVillageNodes(html);
            foreach (var node in listNode)
            {
                var id = VillagesTable.GetId(node);
                if (id != VillageId) continue;
                return VillagesTable.IsActive(node);
            }
            return false;
        }

        private void SwitchVillage()
        {
            do
            {
                var html = ChromeBrowser.GetHtml();

                var listNode = VillagesTable.GetVillageNodes(html);
                foreach (var node in listNode)
                {
                    var id = VillagesTable.GetId(node);
                    if (id != VillageId) continue;

                    var chrome = ChromeBrowser.GetChrome();
                    var elements = chrome.FindElements(By.XPath(node.XPath));
                    elements[0].Click();
                }
            }
            while (!IsCorrectVillage());
        }

        private void UpdateResource()
        {
            var html = ChromeBrowser.GetHtml();
            using var context = ContextFactory.CreateDbContext();
            var resource = context.VillagesResources.Find(VillageId);

            if (resource is null)
            {
                context.VillagesResources.Add(new()
                {
                    VillageId = VillageId,
                    Wood = StockBar.GetWood(html),
                    Clay = StockBar.GetClay(html),
                    Iron = StockBar.GetIron(html),
                    Crop = StockBar.GetCrop(html),
                    Warehouse = StockBar.GetWarehouseCapacity(html),
                    Granary = StockBar.GetGranaryCapacity(html),
                    FreeCrop = StockBar.GetFreeCrop(html),
                });
            }
            else
            {
                resource.Wood = StockBar.GetWood(html);
                resource.Clay = StockBar.GetClay(html);
                resource.Iron = StockBar.GetIron(html);
                resource.Crop = StockBar.GetCrop(html);
                resource.Warehouse = StockBar.GetWarehouseCapacity(html);
                resource.Granary = StockBar.GetGranaryCapacity(html);
                resource.FreeCrop = StockBar.GetFreeCrop(html);
            }
            context.SaveChanges();
        }

        private void UpdateCurrentlyBuilding()
        {
            var html = ChromeBrowser.GetHtml();
            using var context = ContextFactory.CreateDbContext();
            var nodes = VillageCurrentlyBuilding.GetNodes(html);
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var building = context.VillagesCurrentlyBuildings.Find(VillageId, i);

                var strType = VillageCurrentlyBuilding.GetType(node);
                var level = VillageCurrentlyBuilding.GetLevel(node);
                var duration = VillageCurrentlyBuilding.GetDuration(node);

                var type = (BuildingEnums)Enum.Parse(typeof(BuildingEnums), strType);

                if (building is null)
                {
                    context.VillagesCurrentlyBuildings.Add(new()
                    {
                        Id = i,
                        VillageId = VillageId,
                        Type = type,
                        Level = level,
                        CompleteTime = DateTime.Now.Add(duration),
                    });
                }
                else
                {
                    building.Type = type;
                    building.Level = level;
                    building.CompleteTime = DateTime.Now.Add(duration);
                }
            }
            for (int i = nodes.Count; i < 4; i++) // we will save 3 slot for each village, Roman can build 3 building in one time
            {
                var building = context.VillagesCurrentlyBuildings.Find(VillageId, i);
                if (building is null) continue;
                if (building is null)
                {
                    context.VillagesCurrentlyBuildings.Add(new()
                    {
                        Id = i,
                        VillageId = VillageId,
                        Type = 0,
                        Level = -1,
                        CompleteTime = DateTime.MaxValue,
                    });
                }
                else
                {
                    building.Type = 0;
                    building.Level = -1;
                    building.CompleteTime = DateTime.MaxValue;
                }
            }
            context.SaveChanges();
        }

        private void UpdateDorf1()
        {
            var html = ChromeBrowser.GetHtml();
            var resFields = VillageFields.GetResourceNodes(html);
            using var context = ContextFactory.CreateDbContext();
            foreach (var fieldNode in resFields)
            {
                var id = VillageFields.GetId(fieldNode);
                var resource = context.VillagesBuildings.Find(VillageId, id);
                var level = VillageFields.GetLevel(fieldNode);
                var type = VillageFields.GetType(fieldNode);
                var isUnderConstruction = VillageFields.IsUnderConstruction(fieldNode);
                if (resource is null)
                {
                    context.VillagesBuildings.Add(new()
                    {
                        VillageId = VillageId,
                        Id = id,
                        Level = level,
                        Type = (BuildingEnums)type,
                        IsUnderConstruction = isUnderConstruction,
                    });
                }
                else
                {
                    resource.Level = level;
                    resource.Type = (BuildingEnums)type;
                    resource.IsUnderConstruction = isUnderConstruction;
                }
            }
            context.SaveChanges();

            var currentlyBuilding = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId).ToList();
            if (currentlyBuilding.Count > 0)
            {
                foreach (var building in currentlyBuilding)
                {
                    var build = context.VillagesBuildings.FirstOrDefault(x => x.IsUnderConstruction && x.Type == building.Type && x.Level - building.Level < 3);
                    if (build is null) continue;
                    building.Location = build.Id;
                }
                context.SaveChanges();
            }
        }

        private void UpdateDorf2()
        {
            var html = ChromeBrowser.GetHtml();
            var buildingNodes = VillageInfrastructure.GetBuildingNodes(html);
            using var context = ContextFactory.CreateDbContext();
            var tribe = context.AccountsInfo.Find(AccountId).Tribe;
            foreach (var buildingNode in buildingNodes)
            {
                var id = VillageInfrastructure.GetId(buildingNode);
                var building = context.VillagesBuildings.Find(VillageId, id);
                var level = VillageInfrastructure.GetLevel(buildingNode);
                var type = VillageInfrastructure.GetType(buildingNode);
                if (id == 39)
                {
                    var wall = BuildingsData.GetTribesWall(tribe);
                    type = (int)wall;
                }
                var isUnderConstruction = VillageInfrastructure.IsUnderConstruction(buildingNode);
                if (building is null)
                {
                    context.VillagesBuildings.Add(new()
                    {
                        VillageId = VillageId,
                        Id = id,
                        Level = level,
                        Type = (BuildingEnums)type,
                        IsUnderConstruction = isUnderConstruction,
                    });
                }
                else
                {
                    building.Level = level;
                    building.Type = (BuildingEnums)type;
                    building.IsUnderConstruction = isUnderConstruction;
                }
            }
            context.SaveChanges();
            var currentlyBuilding = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == VillageId).ToList();
            if (currentlyBuilding.Count > 0)
            {
                foreach (var building in currentlyBuilding)
                {
                    var build = context.VillagesBuildings.FirstOrDefault(x => x.IsUnderConstruction && x.Type == building.Type && x.Level - building.Level < 3);
                    if (build is null) continue;
                    building.Location = build.Id;
                }
                context.SaveChanges();
            }
        }
    }
}