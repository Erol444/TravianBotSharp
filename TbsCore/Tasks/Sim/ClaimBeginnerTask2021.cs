using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Tasks.LowLevel;
using TbsCore.Tasks.Update;

namespace TbsCore.Tasks.Sim
{
    public class ClaimBeginnerTask2021 : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            // Claim village-wide rewards
            await DriverHelper.ClickById(acc, "questmasterButton");
            await ClaimRewards(acc);

            if (acc.Wb.Html
                .GetElementbyId("sidebarBoxQuestmaster")?
                .Descendants()?
                .Any(x => x.HasClass("newQuestSpeechBubble")) ?? false)
            {
                // Claim account-wide rewards
                await DriverHelper.ClickByClassName(acc, "tabItem", 1);
                await ClaimRewards(acc);
            }

            acc.Tasks.Add(new HeroUpdateInfo() { ExecuteAt = DateTime.Now });

            return TaskRes.Executed;
        }

        private async Task ClaimRewards(Account acc)
        {
            int count = 0;
            do
            {
                await DriverHelper.ClickByClassName(acc, "collect", log: false);
                await Task.Delay(AccountHelper.Delay(acc));
                acc.Wb.UpdateHtml();
                count++;
                if (count > 50) break; // infinite loop ( i dont think there is over 50 quest waiting bot )
            }
            while (acc.Wb.Html.DocumentNode.Descendants("button").Any(x => x.HasClass("collect")));
        }
    }
}