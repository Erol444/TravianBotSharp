using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.Update
{
    public class UpdateDorf2 : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            acc.Tasks.Remove(this.GetType(), Vill, thisTask: this);

            if (!acc.Wb.CurrentUrl.Contains("/dorf2.php")) // Don't re-navigate
            {
                await NavigationHelper.ToDorf2(acc);
            }

            return TaskRes.Executed;
        }
    }
}