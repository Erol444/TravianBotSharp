using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Helper.Implementations;
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
            {
                using var context = _contextFactory.CreateDbContext();
                NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);
            }
            IsFail = true;
            Navigate();
            if (IsStop()) return;
            Update();
            if (IsStop()) return;
            if (!IsVaild()) return;
            Enter();
            if (IsStop()) return;
            GetTroopLevel();
            IsFail = false;
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
                var isProgressing = false;
                if (lvl.Contains('+'))
                {
                    lvl = lvl.Split('+')[0];
                    isProgressing = true;
                }
                var troopId = GetTroop(research);
                var troop = troops.FirstOrDefault(x => x.Id == troopId);
                troop.Level = lvl.ToNumeric() + (isProgressing ? 1 : 0);
                context.Update(troop);
            }
            context.SaveChanges();
            _eventManager.OnTroopLevelUpdate(VillageId);
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