using System;
using System.Linq;
using System.Runtime.InteropServices;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.TravianData
{
    public static class TroopCost
    {
        public static readonly int[,] Cost = new int[,] {
            {  0, 0, 0, 0 }, // None
            { 120, 100, 150, 30 }, // Legionnaire
            { 100, 130, 160, 70 },
            { 150, 160, 210, 80 },
            { 140, 160, 20, 40 },
            { 550, 440, 320, 100 },
            { 550, 640, 800, 180 },
            { 900, 360, 500, 70 },
            { 950, 1350, 600, 90 },
            { 30750, 27200, 45000, 37500 },
            { 4600, 4200, 5800, 4400 },
            { 95, 75, 40, 40 },
            { 145, 70, 85, 40 },
            { 130, 120, 170, 70 },
            { 160, 100, 50, 50 },
            { 370, 270, 290, 75 },
            { 450, 515, 480, 80 },
            { 1000, 300, 350, 70 },
            { 900, 1200, 600, 60 },
            { 35500, 26600, 25000, 27200 },
            { 5800, 4400, 4600, 5200 },
            { 100, 130, 55, 30 },
            { 140, 150, 185, 60 },
            { 170, 150, 20, 40 },
            { 350, 450, 230, 60 },
            { 360, 330, 280, 120 },
            { 500, 620, 675, 170 },
            { 950, 555, 330, 75 },
            { 960, 1450, 630, 90 },
            { 30750, 45400, 31000, 37500 },
            { 4400, 5600, 4200, 3900 },
            { 45, 60, 30, 15 },
            { 115, 100, 145, 60 },
            { 170, 180, 220, 80 },
            { 170, 150, 20, 40 },
            { 360, 330, 280, 120 },
            { 450, 560, 610, 180 },
            { 995, 575, 340, 80 },
            { 980, 1510, 660, 100 },
            { 34000, 50000, 34000, 42000 },
            { 4560, 5890, 4370, 4180 },
            { 130, 80, 40, 40 },
            { 140, 110, 60, 60 },
            { 170, 150, 20, 40 },
            { 290, 370, 190, 45 },
            { 320, 350, 330, 50 },
            { 450, 560, 610, 140 },
            { 1060, 330, 360, 70 },
            { 950, 1280, 620, 60 },
            { 37200, 27600, 25200, 27600 },
            { 6100, 4600, 4800, 5400 }
        };
        public static readonly int[] TrainTime = new int[] {/*Troop=None*/ 0, 1600, 1760, 1920, 1360, 2640, 3520, 4600, 9000, 90700, 26900, 720, 1120, 1200, 1120, 2400, 2960, 4200, 9000, 70500, 31000, 1040, 1440, 1360, 2480, 2560, 3120, 5000, 9000, 90700, 22700, 530, 1320, 1440, 1360, 2560, 3240, 4800, 9000, 90700, 24800, 810, 1120, 1360, 2400, 2480, 2990, 4400, 9000, 90700, 28950 };
        public static int[] GetResourceCost(Classificator.TroopsEnum troop, bool great)
        {
            var troopId = (int)troop;
            if (troopId > 40) troopId -= 20; //since we don't have values for nature/natars
            var res = Cost.GetRow(troopId);
            if (great) //if we are training in GB/GS, cost of each troop is 3 times as much.
            {
                for (int i = 0; i < res.Length; i++)
                {
                    res[i] *= 3;
                }
            }
            return res;
        }
        //You would probably have to take into account ally training bonus, artifacts, helmets, horse fountain...
        public static TimeSpan GetTrainingTime(Account acc, Village vill, Classificator.TroopsEnum troop, bool great)
        {
            var buildingType = TroopsHelper.GetTroopBuilding(troop, great);
            var building = vill.Build.Buildings.FirstOrDefault(x => x.Type == buildingType);
            var troopId = (int)troop;
            if (troopId > 40) troopId -= 20; //since we don't have values for nature/natars
            var baseTime = TrainTime[troopId];
            var sec = Math.Pow(0.9, building.Level - 1) * baseTime;
            var millis = sec % 1;
            var baseTimespan = new TimeSpan(0, 0, 0, (int)sec, (int)millis * 1000);
            return new TimeSpan(baseTimespan.Ticks / acc.AccInfo.ServerSpeed);
        }

        public static T[] GetRow<T>(this T[,] array, int row)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("Not supported for managed types.");

            if (array == null)
                throw new ArgumentNullException("array");

            int cols = array.GetUpperBound(1) + 1;
            T[] result = new T[cols];

            int size;

            if (typeof(T) == typeof(bool))
                size = 1;
            else if (typeof(T) == typeof(char))
                size = 2;
            else
                size = Marshal.SizeOf<T>();

            Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);

            return result;
        }
    }

}
