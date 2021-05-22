using System.Threading.Tasks;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.LowLevel
{
    /// <summary>
    /// Extend beginners protection
    /// </summary>
    public class ExtendProtection : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/options/game?extendBeginnersProtection");
            return TaskRes.Executed;
        }
    }
}