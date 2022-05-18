using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.Browser
{
    /// <summary>
    /// This task changes access (and restarts selenium driver) for the account and sets the next access change, if there are multiple access'.
    /// </summary>
    public class ChangeAccess : ReopenDriver
    {
        private readonly Random rand = new Random();
        private DateTime sleepEnd;

        public override async Task<TaskRes> Execute(Account acc)
        {
            // may add setting for these
            // how "lag" for duals login to your account (in minutes)
            var min = 10 * 60; // 10 mins
            var max = 30 * 60; // 30 mins
            int nextTime = rand.Next(min, max);
            sleepEnd = DateTime.Now.AddSeconds(nextTime);

            acc.Logger.Information($"New proxy will be change at {sleepEnd}");
            ChangeAccess = true;
            await base.Execute(acc);

            var nextProxyChange = TimeHelper.GetNextProxyChange(acc);
            if (nextProxyChange != TimeSpan.MaxValue)
            {
                Vill = null;
                NextExecute = DateTime.Now + nextProxyChange;
            }

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