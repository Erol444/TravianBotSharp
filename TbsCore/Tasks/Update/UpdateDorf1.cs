using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.Update
{
    public class UpdateDorf1 : BotTask
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

            if (!acc.Wb.CurrentUrl.Contains("/dorf1.php")) // Don't re-navigate
            {
                var result = await NavigationHelper.ToDorf1(acc);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }

            {
                var result = await Update(acc);
                if (!result) return TaskRes.Executed;
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