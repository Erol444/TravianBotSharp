using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Helper;
using System.Linq;

namespace MainCore.Tasks.Update
{
    public class UpdateTroopLevel : VillageBotTask
    {
        public UpdateTroopLevel(int villageId, int accountId) : base(villageId, accountId, "Update troop's level")
        {
        }

        public override void Execute()
        {
            Navigate();
            if (IsStop()) return;
            Update();
            if (IsStop()) return;
            if (!IsVaild()) return;
            Enter();
            if (IsStop()) return;
            GetTroopLevel();
        }

        private void Navigate()
        {
            using var context = _contextFactory.CreateDbContext();
            NavigateHelper.SwitchVillage(context, _chromeBrowser, VillageId, AccountId);
        }

        private void Update()
        {
            var taskUpdate = new UpdateDorf2(VillageId, AccountId);
            taskUpdate.CopyFrom(this);
            taskUpdate.Execute();
        }

        private bool IsVaild()
        {
            using var context = _contextFactory.CreateDbContext();
            var villageBuilding = context.VillagesBuildings.Where(x => x.VillageId == VillageId);
            var smithy = villageBuilding.FirstOrDefault(x => x.Type == BuildingEnums.Smithy);
            if (smithy is null) return false;
            if (smithy.Level <= 0) return false;
            return true;
        }

        private void Enter()
        {
            using var context = _contextFactory.CreateDbContext();
            var villageBuilding = context.VillagesBuildings.Where(x => x.VillageId == VillageId);
            var smithy = villageBuilding.FirstOrDefault(x => x.Type == BuildingEnums.Smithy);
            NavigateHelper.GoToBuilding(_chromeBrowser, smithy.Id, context, AccountId);
        }

        private void GetTroopLevel()
        {
            var html = _chromeBrowser.GetHtml();
            var researches = html.DocumentNode.Descendants("div").Where(x => x.HasClass("research"));
            using var context = _contextFactory.CreateDbContext();
            var troops = context.VillagesTroops.Where(x => x.VillageId == VillageId);
            foreach (var research in researches)
            {
                if (Cts.IsCancellationRequested)
                {
                    context.SaveChanges();
                    return;
                }
                var lvl = research.Descendants("span").FirstOrDefault(x => x.HasClass("level")).InnerText;
                if (lvl.Contains('+')) //troop is currently being improved. Only get current level
                {
                    lvl = lvl.Split('+')[0];
                }
                var troopId = GetTroop(research);
                var troop = troops.FirstOrDefault(x => x.Id == troopId);
                troop.Level = lvl.ToNumeric();
                context.Update(troop);
            }
            context.SaveChanges();
        }

        private static int GetTroop(HtmlNode node)
        {
            var img = node.Descendants("img").First(x => x.HasClass("unit"));
            var troopNum = img.GetClasses().FirstOrDefault(x => x != "unit");
            var value = troopNum.ToNumeric();
            return value;
        }
    }
}