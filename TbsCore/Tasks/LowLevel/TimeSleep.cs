using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;

namespace TbsCore.Tasks.LowLevel
{
    public class TimeSleep : ReopenDriver
    {
        private readonly Random rand = new Random();
        private DateTime sleepEnd;

        public override async Task<TaskRes> Execute(Account acc)
        {
            sleepEnd = GetSleepEnd(acc);
            acc.Logger.Information($"Chrome will open at {sleepEnd}");
            await base.Execute(acc);
            NextExecute = DateTime.Now + TimeHelper.GetWorkTime(acc);

            return TaskRes.Executed;
        }

        public DateTime GetSleepEnd(Account acc)
        {
            var min = acc.Settings.Time.MinSleep * 60;
            var max = acc.Settings.Time.MaxSleep * 60;
            int sleepSec = rand.Next(min, max);
            return DateTime.Now.AddSeconds(sleepSec);
        }

        public override int GetMinutes(Account acc)
        {
            var delay = sleepEnd - DateTime.Now;
            return (int)delay.TotalMinutes;
        }
    }
}