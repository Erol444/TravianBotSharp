using FluentResults;
using MainCore.Errors;
using MainCore.Models.Runtime;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.TTWars
{
    public class NPCTask : Base.NPCTask
    {
        public NPCTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
        }

        public NPCTask(int villageId, int accountId, Resources ratio, CancellationToken cancellationToken = default) : this(villageId, accountId, ratio, cancellationToken)

        {
        }

        public override Result Execute()
        {
            return base.Execute();
        }

        protected override Result CheckGold()
        {
            using var context = _contextFactory.CreateDbContext();
            var info = context.AccountsInfo.Find(AccountId);

            var goldNeed = 5;
            var result = info.Gold > goldNeed;
            return result ? Result.Ok() : Result.Fail(new Skip("Not enough gold"));
        }

        protected override Result EnterNumber()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);
            var ratio = new int[4];
            if (_ratio is null)
            {
                ratio[0] = setting.AutoNPCWood;
                ratio[1] = setting.AutoNPCClay;
                ratio[2] = setting.AutoNPCIron;
                ratio[3] = setting.AutoNPCCrop;
            }
            else
            {
                ratio[0] = _ratio.Wood;
                ratio[1] = _ratio.Clay;
                ratio[2] = _ratio.Iron;
                ratio[3] = _ratio.Crop;
            }
            var ratioSum = ratio.Sum();

            if (ratioSum == 0)
            {
                Array.ForEach(ratio, x => x = 1);
                ratioSum = 4;
            }

            var html = _chromeBrowser.GetHtml();
            var nodeSum = _systemPageParser.GetNpcSumNode(html);
            var sumCurrent = nodeSum.InnerText.ToNumeric();
            var current = new long[4];
            for (var i = 0; i < 4; i++)
            {
                current[i] = (long)(sumCurrent * ratio[i]) / ratioSum;
            }
            var sum = current.Sum();
            var diff = sumCurrent - sum;
            current[3] += diff;

            var chrome = _chromeBrowser.GetChrome();
            var script = "";

            for (int i = 0; i < 4; i++)
            {
                script = $"document.getElementById('m2[{i}]').value = {current[i]};";
                chrome.ExecuteScript(script);
            }

            return Result.Ok();
        }
    }
}