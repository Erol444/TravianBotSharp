using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.Sim
{
    /// <summary>
    /// Extend beginners protection
    /// </summary>
    public class ExtendProtection : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            StopFlag = false;
            {
                acc.Logger.Information($"Checking current village ...");
                var result = await NavigationHelper.SwitchVillage(acc, Vill);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/options/game?extendBeginnersProtection");
            return TaskRes.Executed;
        }
    }
}