using System;
using System.Linq;
using TbsCore.Models.AccModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.VillageModels;

namespace TravBotSharp.Files.Helpers
{
    public static class TimeHelper
    {
        /// <summary>
        /// Get DateTime when there will be enough resources, based on production
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="required">Resources required</param>
        /// <returns>DateTime</returns>
        public static DateTime EnoughResToUpgrade(Village vill, Resources required)
        {
            long[] production = vill.Res.Production.ToArray();
            long[] resRequired = required.ToArray();

            DateTime ret = DateTime.Now.AddMinutes(3);
            for (int i = 0; i < 4; i++)
            {
                DateTime toWaitForThisRes = DateTime.MinValue;
                if (resRequired[i] > 0)
                {
                    // In case of negative crop, we will never have enough crop
                    if (production[i] <= 0) return DateTime.MaxValue;

                    float hoursToWait = (float)resRequired[i] / (float)production[i];
                    float secToWait = hoursToWait * 3600;
                    toWaitForThisRes = DateTime.Now.AddSeconds(secToWait);
                }

                if (ret < toWaitForThisRes) ret = toWaitForThisRes;
            }
            return ret;
        }

        /// <summary>
        /// Multiplies a timespan by some value
        /// </summary>
        /// <param name="timeSpan">Original TimeSpan</param>
        /// <param name="multiplyBy"></param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan MultiplyTimespan(TimeSpan timeSpan, int multiplyBy)
        {
            return (TimeSpan.FromTicks(timeSpan.Ticks * multiplyBy));
        }

        /// <summary>
        /// Calculate when the next sleep will occur
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>TimeSpan of the working time. After this, account should sleep</returns>
        public static TimeSpan GetWorkTime(Account acc)
        {
            var rand = new Random();
            TimeSpan workTime = new TimeSpan(0,
                rand.Next(acc.Settings.Time.MinWork, acc.Settings.Time.MaxWork),
                0);
            return workTime;
        }

        /// <summary>
        /// Calculate when next proxy change should occur
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>TimeSpan when next proxy change should occur</returns>
        public static TimeSpan GetNextProxyChange(Account acc)
        {
            var proxyCount = acc.Access.AllAccess.Count;
            if (proxyCount == 1) return TimeSpan.MaxValue;

            var min = (int)((24 * 60) / proxyCount);

            // +- 30min
            var rand = new Random();

            return new TimeSpan(0, min + rand.Next(-30, 30), 0);
        }

        /// <summary>
        /// Gets the TimeSpan when the next normal or high priority task should be executed
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan NextPrioTask(Account acc, Tasks.BotTask.TaskPriority prio)
        {
            Tasks.BotTask firstTask = null;

            switch (prio)
            {
                case Tasks.BotTask.TaskPriority.High:
                    firstTask = acc.Tasks.FirstOrDefault(x =>
                        x.Priority == Tasks.BotTask.TaskPriority.High
                    );
                    break;
                case Tasks.BotTask.TaskPriority.Medium:
                    firstTask = acc.Tasks.FirstOrDefault(x =>
                        x.Priority == Tasks.BotTask.TaskPriority.High ||
                        x.Priority == Tasks.BotTask.TaskPriority.Medium
                    );
                    break;
                case Tasks.BotTask.TaskPriority.Low:
                    firstTask = acc.Tasks.FirstOrDefault();
                    break;
            }

            if (firstTask == null) return TimeSpan.MaxValue;

            return (firstTask.ExecuteAt - DateTime.Now);
        }
    }
}