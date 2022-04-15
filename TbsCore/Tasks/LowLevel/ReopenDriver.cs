using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;
using System.Linq;

namespace TbsCore.Tasks.LowLevel
{
    /// <summary>
    /// Task will close and reopen driver then the next Normal/High priority task has to be executed
    /// </summary>
    public abstract class ReopenDriver : BotTask
    {
        public bool ChangeAccess = false;

        public override async Task<TaskRes> Execute(Account acc)
        {
            acc.Wb.Close();
            string previousLog = "";
            do
            {
                await Task.Delay(1000);
                var minutes = GetMinutes(acc);
                if (minutes <= 0) break;
                string log;
                switch (acc.Status)
                {
                    case Status.Paused:
                    case Status.Pausing:
                        log = $"Chrome will reopen in {minutes} mins but account is paused, chrome won't be open until bot is resumed";
                        break;

                    case Status.Stopping:
                        acc.Logger.Information("Account logout. Ignore reopen chrome");
                        return TaskRes.Executed;

                    default:
                        log = $"Chrome will reopen in {minutes} mins";
                        break;
                }

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