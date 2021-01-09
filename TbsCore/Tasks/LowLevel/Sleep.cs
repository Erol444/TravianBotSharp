using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class Sleep : ReopenDriver
    {
        public bool AutoSleep { get; set; }
        public int MinSleepSec { get; set; }
        public int MaxSleepSec { get; set; }
        public override async Task<TaskRes> Execute(Account acc)
        {
            if (AutoSleep)
            {
                MinSleepSec = acc.Settings.Time.MinSleep * 60;
                MaxSleepSec = acc.Settings.Time.MaxSleep * 60;
            }
            var rand = new Random();
            int sleepSec = rand.Next(MinSleepSec, MaxSleepSec);
            var sleepEnd = DateTime.Now.AddSeconds(sleepSec);

            acc.Wb.Log($"Sleep will end at {sleepEnd}");

            base.LowestPrio = TaskPriority.High;
            base.ReopenAt = sleepEnd;

            await base.Execute(acc);


            if (AutoSleep)
            {
                this.NextExecute = DateTime.Now + TimeHelper.GetWorkTime(acc);
            }

            return TaskRes.Executed;
        }
    }
}
