using FluentResults;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.TravianOfficial
{
    public class ImproveTroopsTask : Base.ImproveTroopsTask
    {
        public ImproveTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
        }

        public override Result Execute()
        {
            return base.Execute();
        }

        protected override bool IsTroopImproving()
        {
            var html = _chromeBrowser.GetHtml();
            var table = html.DocumentNode.Descendants("table").FirstOrDefault(x => x.HasClass("under_progress"));
            if (table is null) return false;

            var rows = table.Descendants("tbody").FirstOrDefault().Descendants("tr");

            if (rows.Count() == 1)
            {
                using var context = _contextFactory.CreateDbContext();
                var accountInfo = context.AccountsInfo.Find(AccountId);
                var hasPlus = accountInfo.HasPlusAccount;
                if (hasPlus)
                {
                    return false;
                }
            }
            var timer = table.Descendants("span").FirstOrDefault(x => x.HasClass("timer"));
            var time = timer.GetAttributeValue("value", 0);
            if (time < 0) ExecuteAt = DateTime.Now;
            else ExecuteAt = DateTime.Now.AddSeconds(time);
            _logManager.Warning(AccountId, $"Smithy is upgrading another troops", this);
            return true;
        }

        protected override bool IsEnoughResource()
        {
            var html = _chromeBrowser.GetHtml();
            var researches = html.DocumentNode.Descendants("div").Where(x => x.HasClass("research"));
            foreach (var research in researches)
            {
                if (GetTroop(research) != (int)Troop) continue;
                var resourceDiv = research.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));

                var resNodes = resourceDiv.ChildNodes.Where(x => x.HasClass("resource") || x.HasClass("resources")).ToList();
                var resNeed = new int[4];
                for (var i = 0; i < 4; i++)
                {
                    var node = resNodes[i];
                    resNeed[i] = node.InnerText.ToNumeric();
                }
                using var context = _contextFactory.CreateDbContext();
                var resCurrent = context.VillagesResources.Find(VillageId);
                if (resNeed[0] > resCurrent.Wood || resNeed[1] > resCurrent.Clay || resNeed[2] > resCurrent.Iron || resNeed[3] > resCurrent.Crop)
                {
                    var resMissing = new long[] { resNeed[0] - resCurrent.Wood, resNeed[1] - resCurrent.Clay, resNeed[2] - resCurrent.Iron, resNeed[3] - resCurrent.Crop };

                    _logManager.Warning(AccountId, $"Don't have enough resource (missing W: {resMissing[0]}, C: {resMissing[1]}, I: {resMissing[2]}, C: {resMissing[3]}", this);
                    return false;
                }
                return true;
            }
            return true;
        }
    }
}