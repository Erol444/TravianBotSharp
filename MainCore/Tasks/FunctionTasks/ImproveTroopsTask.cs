using FluentResults;
using MainCore.Tasks.Base;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public sealed class ImproveTroopsTask : VillageBotTask
    {
        public ImproveTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
        }

        public override Result Execute()
        {
            //var commands = new List<Func<Result>>()
            //{
            //    Update,
            //    ChooseTroop,
            //    CheckImproving,
            //    CheckResource,
            //    Improve,
            //    NextExecute,
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

        //protected Result Update()
        //{
        //    var taskUpdate = new UpdateTroopLevel(VillageId, AccountId, CancellationToken);
        //    var result = taskUpdate.Execute();
        //    return result;
        //}

        //protected Result ChooseTroop()
        //{
        //    _troop = GetTroop();
        //    if (Troop != TroopEnums.None) return Result.Ok();
        //    return Result.Fail(new Skip("There isn't any troop to upgrade."));
        //}

        //protected Result CheckImproving()
        //{
        //    if (IsTroopImproving())
        //    {
        //        NextExecute();
        //        return Result.Fail(new Skip("There is troop in progress"));
        //    }
        //    return Result.Ok();
        //}

        //protected Result CheckResource()
        //{
        //    if (IsEnoughResource()) return Result.Ok();
        //    return Result.Fail(new Skip("There isn't enough resource"));
        //}

        //protected Result Improve()
        //{
        //    var html = _chromeBrowser.GetHtml();
        //    var researches = html.DocumentNode.Descendants("div").Where(x => x.HasClass("research")).ToList();
        //    foreach (var research in researches)
        //    {
        //        if (GetTroop(research) != (int)Troop) continue;
        //        var upgradeButton = research.Descendants("button").FirstOrDefault(x => x.HasClass("green"));
        //        if (upgradeButton is null) return Result.Fail(new Retry("Cannot found upgrade button"));
        //        var chrome = _chromeBrowser.GetChrome();
        //        var upgradeElements = chrome.FindElements(By.XPath(upgradeButton.XPath));
        //        if (upgradeElements.Count == 0) return Result.Fail(new Retry("Cannot found upgrade button"));

        //        var result = _generalHelper.Click(AccountId, upgradeElements[0]);
        //        if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
        //        return Result.Ok();
        //    }
        //    return Result.Ok();
        //}

        //protected Result NextExecute()
        //{
        //    var html = _chromeBrowser.GetHtml();
        //    var table = html.DocumentNode.Descendants("table").FirstOrDefault(x => x.HasClass("under_progress"));
        //    var timer = table.Descendants("span").FirstOrDefault(x => x.HasClass("timer"));
        //    var time = timer.GetAttributeValue("value", 0);
        //    if (time < 0) ExecuteAt = DateTime.Now;
        //    else ExecuteAt = DateTime.Now.AddSeconds(time);
        //    return Result.Ok();
        //}

        //protected TroopEnums GetTroop()
        //{
        //    using var context = _contextFactory.CreateDbContext();
        //    var settings = context.VillagesSettings.Find(VillageId);
        //    var boolean = settings.GetTroopUpgrade();
        //    var troops = context.VillagesTroops.Where(x => x.VillageId == VillageId).ToArray();

        //    var buildings = context.VillagesBuildings.Where(x => x.VillageId == VillageId);
        //    var smithy = buildings.FirstOrDefault(x => x.Type == BuildingEnums.Smithy);

        //    for (int i = 0; i < boolean.Length; i++)
        //    {
        //        if (!boolean[i]) continue;
        //        var troop = troops[i];
        //        if (troop.Level == -1)
        //        {
        //            _logManager.Warning(AccountId, $"{(TroopEnums)troop.Id} is not researched");
        //            boolean[troop.Id % 10 - 1] = false;
        //            settings.SetTroopUpgrade(boolean);
        //            context.Update(settings);
        //            continue;
        //        }
        //        if (troop.Level == 20)
        //        {
        //            _logManager.Warning(AccountId, $"{(TroopEnums)troop.Id} is max level");
        //            boolean[(int)Troop % 10 + 1] = false;
        //            settings.SetTroopUpgrade(boolean);
        //            context.Update(settings);
        //            continue;
        //        }

        //        if (smithy.Level == troop.Level)
        //        {
        //            continue;
        //        }

        //        context.SaveChanges();
        //        return (TroopEnums)troop.Id;
        //    }
        //    context.SaveChanges();
        //    return TroopEnums.None;
        //}

        //protected abstract bool IsTroopImproving();

        //protected abstract bool IsEnoughResource();

        //protected static int GetTroop(HtmlNode node)
        //{
        //    var img = node.Descendants("img").FirstOrDefault(x => x.HasClass("unit"));
        //    if (img is null) return 0;
        //    var troopNum = img.GetClasses().FirstOrDefault(x => x != "unit");
        //    var value = troopNum.ToNumeric();
        //    return value;
        //}
    }
}