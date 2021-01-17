using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ClaimBeginnerTask2021 : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            // Claim village-wide rewards
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/tasks");
            await ClaimRewards(acc);

            if (acc.Wb.Html
                .GetElementbyId("sidebarBoxQuestmaster")?
                .Descendants()?
                .Any(x => x.HasClass("newQuestSpeechBubble")) ?? false)
            {
                // Claim account-wide rewards
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/tasks?t=2");
                await ClaimRewards(acc);
            }

                TaskExecutor.AddTask(acc, new HeroUpdateInfo() { ExecuteAt = DateTime.Now });

            return TaskRes.Executed;
        }

        private async Task ClaimRewards(Account acc)
        {
            await Task.Delay(AccountHelper.Delay());
            do
            {
                await DriverHelper.ClickByClassName(acc, "collect", false);
            }
            while (acc.Wb.Html.DocumentNode.Descendants("button").Any(x => x.HasClass("collect")));
        }
    }
}
