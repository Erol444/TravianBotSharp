using System.Threading.Tasks;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.Browser
{
    /// <summary>
    /// Task will close and reopen driver then the next Normal/High priority task has to be executed
    /// </summary>
    public abstract class ReopenDriver : BotTask
    {
        public bool ChangeAccess = false;

        public override async Task<TaskRes> Execute(Account acc)
        {
            StopFlag = false;
            acc.Wb.Close();
            string previousLog = "";
            do
            {
                if (StopFlag) break;
                await Task.Delay(1000);
                var minutes = GetMinutes(acc);
                if (minutes <= 0) break;
                string log = $"Chrome will reopen in {minutes} mins";

                if (log != previousLog)
                {
                    acc.Logger.Information(log);
                    previousLog = log;
                }
            }
            while (true);
            // Use the same access
            var result = await acc.Wb.Init(acc, ChangeAccess);
            if (!result)
            {
                acc.TaskTimer.Stop();
            }

            return TaskRes.Executed;
        }

        public abstract int GetMinutes(Account acc);
    }
}