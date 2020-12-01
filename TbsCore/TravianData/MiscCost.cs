using System;
using System.Collections.Generic;
using System.Text;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TbsCore.TravianData
{
    public static class MiscCost
    {
        private static readonly long[,] celebrationResources = new long[,] {
            { 6400, 6650, 5940, 1340 }, // Small Celebration
            { 29700, 33250, 32000, 6700 } // Big celebration
        };

        public static long[] CelebrationCost(bool big) => big ? 
            celebrationResources.GetRow(1) :
            celebrationResources.GetRow(0);

        public static bool EnoughResForCelebration(Village vill, bool big)
        {
            var cost = CelebrationCost(big);
            var res = vill.Res.Stored.Resources.ToArray();
            
            return ResourcesHelper.EnoughRes(res, cost);
        }
    }
}
