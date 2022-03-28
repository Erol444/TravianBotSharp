using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;

namespace TbsCore.Tasks.LowLevel
{
    /// <summary>
    /// This task changes access (and restarts selenium driver) for the account and sets the next access change, if there are multiple access'.
    /// </summary>
    public class ChangeAccess : BotTask
    {
        private readonly Random rand = new Random();

        public override async Task<TaskRes> Execute(Account acc)
        {
            // may add setting for these
            // how "lag" for duals login to your account (in minutes)
            var min = 10 * 60; // 10 mins
            var max = 30 * 60; // 30 mins
            int nextTime = rand.Next(min, max);
            var completeChange = DateTime.Now.AddSeconds(nextTime);

            acc.Logger.Information($"New proxy will be change at {completeChange}");
            acc.Wb.Close();

            string previousLog = "";
            do
            {
                await Task.Delay(1000);
                var delay = completeChange - DateTime.Now;
                int minutes = (int)delay.TotalMinutes;
                if (minutes <= 0) break;
                var log = $"Chrome will reopen in {minutes} mins";
                if (log != previousLog)
                {
                    acc.Logger.Information(log);
                    previousLog = log;
                }
            }
            while (true);
            // Use the different access
            await acc.Wb.Init(acc);

            var nextProxyChange = TimeHelper.GetNextProxyChange(acc);
            if (nextProxyChange != TimeSpan.MaxValue)
            {
                Vill = null;
                NextExecute = DateTime.Now + nextProxyChange;
            }

            return TaskRes.Executed;
        }
    }
}