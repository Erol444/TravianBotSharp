using System;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.TravianData
{
    public static class BuildingsCost
    {
        private static readonly float[,] BuildingCost = new float[,] {
            {0,0,0,0,0F}, //Site
            {  40, 100,  50,  60, 1.67F}, //Woodcutter
            {  80,  40,  80,  50, 1.67F}, //Claypit
            { 100,  80,  30,  60, 1.67F},
            {  70,  90,  70,  20, 1.67F},
            { 520, 380, 290,  90, 1.80F},
            { 440, 480, 320,  50, 1.80F},
            { 200, 450, 510, 120, 1.80F},
            { 500, 440, 380,1240, 1.80F},
            {1200,1480, 870,1600, 1.80F},
            { 130, 160,  90,  40, 1.28F},
            {  80, 100,  70,  20, 1.28F},
            { 130, 210, 410, 130, 1.28F},
            {180, 250, 500, 160, 1.28F},
            {1750,2250,1530, 240, 1.28F},
            {  70,  40,  60,  20, 1.28F},
            { 110, 160,  90,  70, 1.28F},
            {  80,  70, 120,  70, 1.28F},
            { 180, 130, 150,  80, 1.28F},
            { 210, 140, 260, 120, 1.28F},
            { 260, 140, 220, 100, 1.28F},
            { 460, 510, 600, 320, 1.28F},
            { 220, 160,  90,  40, 1.28F},
            {  40,  50,  30,  10, 1.28F},
            {1250,1110,1260, 600, 1.28F},
            { 580, 460, 350, 180, 1.28F},
            { 550, 800, 750, 250, 1.28F},
            {2880,2740,2580, 990, 1.26F},
            {1400,1330,1200, 400, 1.28F},
            { 630, 420, 780, 360, 1.28F},
            { 780, 420, 660, 300, 1.28F},
            {  70,  90, 170,  70,1.28F},
            { 120, 200,   0,  80,1.28F},
            { 160, 100,  80,  60,1.28F},
            { 155, 130, 125,  70, 1.28F},
            {1460, 930,1250,1740, 1.40F},
            { 100, 100, 100, 100, 1.28F},
            { 700, 670, 700, 240, 1.33F},
            {650,800, 450, 200, 1.28F},
            { 400, 500, 350, 100, 1.28F},
            {66700,69050,72200,13200,1.0275F},
            { 780, 420, 660, 540, 1.28F},
            { 110, 160,  70,  60, 1.28F},
            {  50,  80,  40,  30, 1.28F},
            {1600,1250,1050, 200, 1.22F},
            { 910, 945, 910, 340, 1.31F}
        };

        /// <summary>
        /// Get the building cost for specific level
        /// </summary>
        /// <param name="building"></param>
        /// <param name="level"></param>
        public static long[] GetBuildingCost(Classificator.BuildingEnum building, int level)
        {
            long[] ret = new long[4];
            var baseCost = BuildingCost.GetRow((int)building);

            for (int i = 0; i < 4; i++)
            {
                var cost = baseCost[i] * Math.Pow(baseCost[4], level - 1);
                ret[i] = (long)Math.Round(cost / 5.0) * 5;
            }
            return ret;
        }
    }
}
