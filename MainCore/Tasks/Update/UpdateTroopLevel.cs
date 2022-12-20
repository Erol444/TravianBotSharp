using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System.Linq;

namespace MainCore.Tasks.Update
{
    public class UpdateTroopLevel : VillageBotTask
    {
        private readonly INavigateHelper _navigateHelper;

        public UpdateTroopLevel(int villageId, int accountId) : base(villageId, accountId)
        {
            _navigateHelper = Locator.Current.GetService<INavigateHelper>();
        }

        public override Result Execute()
        {
            {
                var taskUpdate = new UpdateVillage(VillageId, AccountId);
                var result = taskUpdate.Execute(); ;
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            if (!IsVaild())
            {
                _logManager.Warning(AccountId, "Missing smithy", this);
                return Result.Ok();
            }

            {
                var result = Enter();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            GetTroopLevel();
            return Result.Ok();
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

        private Result Enter()
        {
            using var context = _contextFactory.CreateDbContext();
            var villageBuilding = context.VillagesBuildings.Where(x => x.VillageId == VillageId);
            var smithy = villageBuilding.FirstOrDefault(x => x.Type == BuildingEnums.Smithy);
            var result = _navigateHelper.GoToBuilding(AccountId, smithy.Id);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private void GetTroopLevel()
        {
            var html = _chromeBrowser.GetHtml();
            var researches = html.DocumentNode.Descendants("div").Where(x => x.HasClass("research"));
            using var context = _contextFactory.CreateDbContext();
            var troops = context.VillagesTroops.Where(x => x.VillageId == VillageId);
            foreach (var research in researches)
            {
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