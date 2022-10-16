using MainCore.Enums;
using System.Collections.Generic;

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

        public static List<TroopEnums> GetTroops(this TribeEnums tribe)
        {
            return (tribe) switch
            {
                TribeEnums.Romans => new()
                {
                    TroopEnums.Legionnaire,
                    TroopEnums.Praetorian,
                    TroopEnums.Imperian,
                    TroopEnums.EquitesLegati,
                    TroopEnums.EquitesImperatoris,
                    TroopEnums.EquitesCaesaris,
                    TroopEnums.RomanRam,
                    TroopEnums.RomanCatapult,
                    TroopEnums.RomanChief,
                    TroopEnums.RomanSettler,
                },
                TribeEnums.Teutons => new()
                {
                    TroopEnums.Clubswinger,
                    TroopEnums.Spearman,
                    TroopEnums.Axeman,
                    TroopEnums.Scout,
                    TroopEnums.Paladin,
                    TroopEnums.TeutonicKnight,
                    TroopEnums.TeutonRam,
                    TroopEnums.TeutonCatapult,
                    TroopEnums.TeutonChief,
                    TroopEnums.TeutonSettler,
                },
                TribeEnums.Gauls => new()
                {
                   TroopEnums.Phalanx,
                    TroopEnums.Swordsman,
                    TroopEnums.Pathfinder,
                    TroopEnums.TheutatesThunder,
                    TroopEnums.Druidrider,
                    TroopEnums.Haeduan,
                    TroopEnums.GaulRam,
                    TroopEnums.GaulCatapult,
                    TroopEnums.GaulChief,
                    TroopEnums.GaulSettler,
                },
                TribeEnums.Nature => new()
                {
                    TroopEnums.Rat,
                    TroopEnums.Spider,
                    TroopEnums.Snake,
                    TroopEnums.Bat,
                    TroopEnums.WildBoar,
                    TroopEnums.Wolf,
                    TroopEnums.Bear,
                    TroopEnums.Crocodile,
                    TroopEnums.Tiger,
                    TroopEnums.Elephant,
                },
                TribeEnums.Natars => new()
                {
                    TroopEnums.Pikeman,
                    TroopEnums.ThornedWarrior,
                    TroopEnums.Guardsman,
                    TroopEnums.BirdsOfPrey,
                    TroopEnums.Axerider,
                    TroopEnums.NatarianKnight,
                    TroopEnums.Warelephant,
                    TroopEnums.Ballista,
                    TroopEnums.NatarianEmperor,
                    TroopEnums.Settler,
                },
                TribeEnums.Egyptians => new()
                {
                    TroopEnums.SlaveMilitia,
                    TroopEnums.AshWarden,
                    TroopEnums.KhopeshWarrior,
                    TroopEnums.SopduExplorer,
                    TroopEnums.AnhurGuard,
                    TroopEnums.ReshephChariot,
                    TroopEnums.EgyptianRam,
                    TroopEnums.EgyptianCatapult,
                    TroopEnums.EgyptianChief,
                    TroopEnums.EgyptianSettler,
                },
                TribeEnums.Huns => new()
                {
                    TroopEnums.Mercenary,
                    TroopEnums.Bowman,
                    TroopEnums.Spotter,
                    TroopEnums.SteppeRider,
                    TroopEnums.Marksman,
                    TroopEnums.Marauder,
                    TroopEnums.HunRam,
                    TroopEnums.HunCatapult,
                    TroopEnums.HunChief,
                    TroopEnums.HunSettler,
                },
                _ => null,
            };
        }
    }
}