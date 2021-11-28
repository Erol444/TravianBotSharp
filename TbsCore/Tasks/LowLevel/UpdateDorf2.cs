using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;

namespace TbsCore.Tasks.LowLevel
{
    public class UpdateDorf2 : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            acc.Tasks.Remove(this.GetType(), Vill, thisTask: this);

            if (!acc.Wb.CurrentUrl.Contains("/dorf2.php")) // Don't re-navigate
            {
                //await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php");
                await NavigationHelper.ToDorf2(acc);
            }

            return TaskRes.Executed;
        }
    }
}