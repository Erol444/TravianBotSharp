using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class UpdateDorf1 : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            TaskExecutor.RemoveTaskTypes(acc, this.GetType(), Vill, this);

            if (!acc.Wb.CurrentUrl.Contains("/dorf1.php")) // Don't re-navigate
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php");
            }

            // 60% to check update dorf2
            Random ran = new Random();
            if (ran.Next(1, 100) > 40)
            {
                TaskExecutor.AddTaskIfNotExistInVillage(acc, Vill, new UpdateDorf2()
                {
                    ExecuteAt = DateTime.Now.AddMinutes(1),
                    Vill = Vill
                });
            }

            return TaskRes.Executed;
        }
    }
}