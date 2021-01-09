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
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/tasks");

            await Task.Delay(AccountHelper.Delay());
            do
            {
                await DriverHelper.ClickByClassName(acc, "collect");
            }
            while (acc.Wb.Html.DocumentNode.Descendants("button").Any(x => x.HasClass("collect")));

            TaskExecutor.AddTask(acc, new HeroUpdateInfo() { ExecuteAt = DateTime.Now });

            return TaskRes.Executed;
        }
    }
}
