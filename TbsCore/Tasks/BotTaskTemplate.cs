using System.Threading.Tasks;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.LowLevel
{
    public class BotTaskTemplate : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php");
            return TaskRes.Executed;
        }
    }
}