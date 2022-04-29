using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.Update
{
    public class UpdateDorf2 : BotTask
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
            if (!acc.Wb.CurrentUrl.Contains("/dorf2.php")) // Don't re-navigate
            {
                var result = await NavigationHelper.ToDorf2(acc);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }

            {
                var result = await Update(acc);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }

            return TaskRes.Executed;
        }
    }
}