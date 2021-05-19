using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class UpdateDorf2 : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            TaskExecutor.RemoveTaskTypes(acc, this.GetType(), Vill, this);

            if (!acc.Wb.CurrentUrl.Contains("/dorf2.php")) // Don't re-navigate
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php");
            }
            
            return TaskRes.Executed;
        }
    }
}
