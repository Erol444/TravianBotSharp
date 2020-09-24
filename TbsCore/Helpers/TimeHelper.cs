using RestSharp;
using System;
using System.Linq;
using System.Net;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;

namespace TravBotSharp.Files.Helpers
{
    public static class TimeHelper
    {
        /// <summary>
        /// Get TimeSpan when there will be enough resources, based on production
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="required">Resources required</param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan EnoughResToUpgrade(Village vill, Resources required)
        {
            long[] production = ResourcesHelper.ResourcesToArray(vill.Res.Production);
            long[] resRequired = ResourcesHelper.ResourcesToArray(required);

            TimeSpan timeSpan = new TimeSpan(0);
            for (int i = 0; i < 4; i++)
            {
                TimeSpan toWaitForThisRes = new TimeSpan(0);
                if (resRequired[i] > 0)
                {
                    // In case of negative crop, we will never have enough crop
                    if (production[i] <= 0) return TimeSpan.MaxValue;

                    float hoursToWait = (float)resRequired[i] / (float)production[i];
                    float secToWait = hoursToWait * 3600;
                    toWaitForThisRes = new TimeSpan(0, 0, (int)secToWait);
                }

                if (toWaitForThisRes > timeSpan) timeSpan = toWaitForThisRes;
            }
            return timeSpan;
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
        public static TimeSpan NextNormalOrHighPrioTask(Account acc)
        {
            var firstTask = acc.Tasks.FirstOrDefault(x =>
                x.Priority == Tasks.BotTask.TaskPriority.Medium ||
                x.Priority == Tasks.BotTask.TaskPriority.High
            );
            if (firstTask == null) return TimeSpan.MaxValue;

            return (firstTask.ExecuteAt - DateTime.Now);
        }
    }
}