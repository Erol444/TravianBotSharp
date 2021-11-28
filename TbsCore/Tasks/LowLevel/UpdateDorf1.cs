using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;

namespace TbsCore.Tasks.LowLevel
{
    public class UpdateDorf1 : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            acc.Tasks.Remove(this.GetType(), Vill, thisTask: this);

            if (!acc.Wb.CurrentUrl.Contains("/dorf1.php")) // Don't re-navigate
            {
                await NavigationHelper.MainNavigate(acc, NavigationHelper.MainNavigationButton.Resources);
            }

            // 60% to check update dorf2
            Random ran = new Random();
            if (ran.Next(1, 100) > 40)
            {
                acc.Tasks.Add(new UpdateDorf2()
                {
                    ExecuteAt = DateTime.Now.AddMinutes(1),
                    Vill = Vill
                }, true, Vill);
            }

            return TaskRes.Executed;
        }
    }
}