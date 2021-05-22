using TbsCore.Helpers;

namespace TbsCore.TravianData
{
    public static class TroopSpeed
    {
        private static readonly int[] speed = new int[] { /*Troop=None*/ 0, 6, 5, 7, 16, 14, 10, 4, 3, 4, 5, 7, 7, 6, 9, 10, 9, 4, 3, 4, 5, 7, 6, 17, 19, 16, 13, 4, 3, 5, 5, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 6, 7, 6, 25, 14, 12, 5, 3, 5, 5, 7, 6, 7, 16, 15, 10, 4, 3, 4, 5, 6, 6, 19, 16, 16, 14, 4, 3, 5, 5 };
        private static readonly int[] upkeep = new int[] { /*Troop=None*/ 0, 1, 1, 1, 2, 3, 4, 3, 6, 5, 1, 1, 1, 1, 1, 2, 3, 3, 6, 4, 1, 1, 1, 2, 2, 2, 3, 3, 6, 4, 1, 1, 1, 1, 1, 2, 2, 3, 3, 3, 5, 1, 1, 1, 1, 2, 3, 4, 5, 1, 1, 1, 1, 1, 2, 2, 3, 3, 6, 4, 1, 1, 1, 2, 2, 2, 3, 3, 6, 4, 1, };

        public static int GetTroopSpeed(Classificator.TroopsEnum troop)
        {
            var troopId = (int)troop;
            return speed[troopId];
        }

        public static int GetTroopUpkeep(Classificator.TroopsEnum troop)
        {
            var troopId = (int)troop;
            return upkeep[troopId];
        }
    }
}