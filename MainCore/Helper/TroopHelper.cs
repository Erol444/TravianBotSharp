using MainCore.Enums;

namespace MainCore.Helper
{
    public static class TroopHelper
    {
        public static TribeEnums GetTribe(this TroopEnums troop)
        {
            return (int)troop switch
            {
                >= (int)TroopEnums.Legionnaire and <= (int)TroopEnums.RomanSettler => TribeEnums.Romans,
                >= (int)TroopEnums.Clubswinger and <= (int)TroopEnums.TeutonSettler => TribeEnums.Teutons,
                >= (int)TroopEnums.Phalanx and <= (int)TroopEnums.GaulSettler => TribeEnums.Gauls,
                >= (int)TroopEnums.Rat and <= (int)TroopEnums.Elephant => TribeEnums.Nature,
                >= (int)TroopEnums.Pikeman and <= (int)TroopEnums.Settler => TribeEnums.Natars,
                >= (int)TroopEnums.SlaveMilitia and <= (int)TroopEnums.EgyptianSettler => TribeEnums.Egyptians,
                >= (int)TroopEnums.Mercenary and <= (int)TroopEnums.HunSettler => TribeEnums.Huns,
                _ => TribeEnums.Any,
            };
        }
    }
}