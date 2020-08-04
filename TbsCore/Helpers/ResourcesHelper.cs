using System;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;

namespace TravBotSharp.Files.Helpers
{
    public static class ResourcesHelper
    {
        /// <summary>
        /// If there are enough resources, return TimeSpan(0)
        /// Otherwise calculate how long it will take to get enough resources and transit res from
        /// main village, if we have that enabled. Return the one that takes less time.
        /// DateTime for usage in nextExecution time
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">(target) Village</param>
        /// <param name="res">Resources required</param>
        /// <returns></returns>
        public static DateTime EnoughResourcesOrTransit(Account acc, Village vill, Resources res)
        {
            DateTime enoughRes = DateTime.Now.Add(TimeHelper.EnoughResToUpgrade(vill, res));

            //We have enough resources, return DateTime.Now
            if (enoughRes < DateTime.Now.AddMilliseconds(1))
            {
                return DateTime.Now;
            }
            //Not enough resources, send resources
            //TODO: this isn't working like supposed to. If we have small transport time, (below 2min, return 2min, so bot has time to send res
            DateTime resTransit = MarketHelper.TransitResources(acc, vill);
            return (enoughRes < resTransit ? enoughRes : resTransit);
        }
        /// <summary>
        /// Calculate if we have enouh resources
        /// </summary>
        /// <param name="storedRes"></param>
        /// <param name="targetRes"></param>
        /// <returns>True if enough</returns>
        public static bool EnoughRes(long[] storedRes, long[] targetRes)
        {
            for (int i = 0; i < 4; i++)
            {
                if (storedRes[i] < targetRes[i]) return false;
            }
            return true;
        }
        public static long[] SendAmount(long[] storedRes, long[] targetRes)
        {
            var ret = new long[4];
            for (int i = 0; i < 4; i++)
            {
                ret[i] = targetRes[i] - storedRes[i];
                if (ret[i] < 0) ret[i] = 0;
            }
            return ret;
        }
        public static long[] ResourcesToArray(Resources res)
        {
            return new long[] { res.Wood, res.Clay, res.Iron, res.Crop };
        }

        public static int MaxTroopsToTrain(long[] stored1, long[] stored2, int[] cost)
        {

            var max = int.MaxValue;
            for (int i = 0; i < 4; i++)
            {
                //total resource we have in both villages
                var total = stored1[i] + stored2[i];

                var maxForThisRes = (int)(total / cost[i]);
                if (max > maxForThisRes) max = maxForThisRes;
            }
            return max;
        }
    }
}
