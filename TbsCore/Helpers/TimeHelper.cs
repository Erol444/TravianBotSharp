using System;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;

namespace TravBotSharp.Files.Helpers
{
    public static class TimeHelper
    {
        /// <summary>
        /// Get TimeSpan when there will be enough resources
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="required">Resources required</param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan EnoughResToUpgrade(Village vill, Resources required)
        {
            long[] resStored = { vill.Res.Stored.Resources.Wood, vill.Res.Stored.Resources.Clay, vill.Res.Stored.Resources.Iron, vill.Res.Stored.Resources.Crop };
            long[] production = { vill.Res.Production.WoodPerHour, vill.Res.Production.ClayPerHour, vill.Res.Production.IronPerHour, vill.Res.Production.CropPerHour };
            long[] resRequired = { required.Wood, required.Clay, required.Iron, required.Crop };
            TimeSpan timeSpan = new TimeSpan(0);
            for (int i = 0; i < 4; i++)
            {
                TimeSpan toWaitForThisRes = new TimeSpan(0);
                var neededRes = resRequired[i] - resStored[i];
                if (neededRes > 0)
                {
                    float hoursToWait = (float)neededRes / (float)production[i];
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
        /// Generate random time when the next sleep will occur
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
    }
}