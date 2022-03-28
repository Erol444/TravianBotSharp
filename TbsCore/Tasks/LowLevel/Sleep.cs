using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;

namespace TbsCore.Tasks.LowLevel
{
    public class Sleep : BotTask
    {
        private readonly Random rand = new Random();

        public override async Task<TaskRes> Execute(Account acc)
        {
            var min = acc.Settings.Time.MinSleep * 60;
            var max = acc.Settings.Time.MaxSleep * 60;
            int sleepSec = rand.Next(min, max);
            var sleepEnd = DateTime.Now.AddSeconds(sleepSec);

            acc.Logger.Information($"Sleep will end at {sleepEnd}");
            acc.Wb.Close();

            string previousLog = "";
            do
            {
                await Task.Delay(1000);
                var delay = sleepEnd - DateTime.Now;
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
            // Use the same access
            await acc.Wb.Init(acc, false);
            NextExecute = DateTime.Now + TimeHelper.GetWorkTime(acc);

            return TaskRes.Executed;
        }
    }
}