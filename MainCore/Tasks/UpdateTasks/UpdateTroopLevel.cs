using FluentResults;
using MainCore.Tasks.Base;
using System.Threading;

namespace MainCore.Tasks.UpdateTasks
{
    public class UpdateTroopLevel : VillageBotTask
    {
        public UpdateTroopLevel(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
        }

        public override Result Execute()
        {
            //var commands = new List<Func<Result>>()
            //{
            //    Update,
            //    CheckSmithy,
            //    Enter,
            //    GetInfo,
            //};

            //foreach (var command in commands)
            //{
            //    _logManager.Information(AccountId, $"[{GetName()}] Execute {command.Method.Name}");
            //    var result = command.Invoke();
            //    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            //    if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            //}
            return Result.Ok();
        }

        //private Result Update()
        //{
        //    var taskUpdate = new UpdateVillage(VillageId, AccountId, CancellationToken);
        //    var result = taskUpdate.Execute();
        //    return result;
        //}

        //private Result CheckSmithy()
        //{
        //    if (!IsVaild())
        //    {
        //        return Result.Fail(new Skip("Missing smithy"));
        //    }
        //    return Result.Ok();
        //}

        //private bool IsVaild()
        //{
        //    using var context = _contextFactory.CreateDbContext();
        //    var villageBuilding = context.VillagesBuildings.Where(x => x.VillageId == VillageId);
        //    var smithy = villageBuilding.FirstOrDefault(x => x.Type == BuildingEnums.Smithy);
        //    if (smithy is null) return false;
        //    if (smithy.Level <= 0) return false;
        //    return true;
        //}

        //private Result Enter()
        //{
        //    using var context = _contextFactory.CreateDbContext();
        //    var villageBuilding = context.VillagesBuildings.Where(x => x.VillageId == VillageId);
        //    var smithy = villageBuilding.FirstOrDefault(x => x.Type == BuildingEnums.Smithy);
        //    var result = _generalHelper.GoToBuilding(AccountId, smithy.Id);
        //    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
        //    return Result.Ok();
        //}

        //private Result GetInfo()
        //{
        //    var html = _chromeBrowser.GetHtml();
        //    var researches = html.DocumentNode.Descendants("div").Where(x => x.HasClass("research"));
        //    using var context = _contextFactory.CreateDbContext();
        //    var troops = context.VillagesTroops.Where(x => x.VillageId == VillageId);
        //    foreach (var research in researches)
        //    {
        //        var lvl = research.Descendants("span").FirstOrDefault(x => x.HasClass("level")).InnerText;
        //        var isProgressing = false;
        //        if (lvl.Contains('+'))
        //        {
        //            lvl = lvl.Split('+')[0];
        //            isProgressing = true;
        //        }
        //        var troopId = GetTroop(research);
        //        var troop = troops.FirstOrDefault(x => x.Id == troopId);
        //        troop.Level = lvl.ToNumeric() + (isProgressing ? 1 : 0);
        //        context.Update(troop);
        //    }
        //    context.SaveChanges();
        //    _eventManager.OnTroopLevelUpdate(VillageId);
        //    return Result.Ok();
        //}

        //private static int GetTroop(HtmlNode node)
        //{
        //    var img = node.Descendants("img").First(x => x.HasClass("unit"));
        //    var troopNum = img.GetClasses().FirstOrDefault(x => x != "unit");
        //    var value = troopNum.ToNumeric();
        //    return value;
        //}
    }
}