using System;
using System.Collections.Generic;
using System.Text;
using TbsCore.Models.AccModels;
using TbsCore.Models.BuildingModels;
using TravBotSharp.Files.Helpers;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TbsCore.TravianData
{
    public class TroopsData
    {
        /// <summary>
        /// For getting building requirements for troop research
        /// This is for academy research only, for training check for training building!
        /// </summary>
        /// <param name="troop">Troop to get building prerequisites for</param>
        /// <returns>List of prerequisites</returns>
        public static List<Prerequisite> GetBuildingPrerequisites(TroopsEnum troop)
        {
            var ret = new List<Prerequisite>();
            switch (troop)
            {
                //romans
                case TroopsEnum.Praetorian:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 1 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.Imperian:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.EquitesLegati:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 1 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    return ret;
                case TroopsEnum.EquitesImperatoris:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    return ret;
                case TroopsEnum.EquitesCaesaris:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 10 });
                    return ret;
                case TroopsEnum.RomanRam:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 1 });
                    return ret;
                case TroopsEnum.RomanCatapult:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    return ret;
                case TroopsEnum.RomanChief:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 20 });
                    return ret;
                //Teutons
                case TroopsEnum.Spearman:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 1 });
                    return ret;
                case TroopsEnum.Axeman:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 3 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.Scout:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 1 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 5 });
                    return ret;
                case TroopsEnum.Paladin:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 3 });
                    return ret;
                case TroopsEnum.TeutonicKnight:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 10 });
                    return ret;
                case TroopsEnum.TeutonRam:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 1 });
                    return ret;
                case TroopsEnum.TeutonCatapult:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 10 });
                    return ret;
                case TroopsEnum.TeutonChief:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 20 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 5 });
                    return ret;
                //Gauls
                case TroopsEnum.Swordsman:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 1 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.Pathfinder:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 1 });
                    return ret;
                case TroopsEnum.TheutatesThunder:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 3 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    return ret;
                case TroopsEnum.Druidrider:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 5 });
                    return ret;
                case TroopsEnum.Haeduan:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 10 });
                    return ret;
                case TroopsEnum.GaulRam:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 1 });
                    return ret;
                case TroopsEnum.GaulCatapult:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 10 });
                    return ret;
                case TroopsEnum.GaulChief:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 20 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 10 });
                    return ret;
                //Egyptians
                case TroopsEnum.AshWarden:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Barracks, Level = 1 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.KhopeshWarrior:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.SopduExplorer:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 1 });
                    return ret;
                case TroopsEnum.AnhurGuard:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 5 });
                    return ret;
                case TroopsEnum.ReshephChariot:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 10 });
                    return ret;
                case TroopsEnum.EgyptianRam:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 5 });
                    return ret;
                case TroopsEnum.EgyptianCatapult:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 10 });
                    return ret;
                case TroopsEnum.EgyptianChief:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 20 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 10 });
                    return ret;
                //Huns
                case TroopsEnum.Bowman:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 3 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.Spotter:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 1 });
                    return ret;
                case TroopsEnum.SteppeRider:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 3 });
                    return ret;
                case TroopsEnum.Marksman:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 5 });
                    return ret;
                case TroopsEnum.Marauder:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 10 });
                    return ret;
                case TroopsEnum.HunRam:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 1 });
                    return ret;
                case TroopsEnum.HunCatapult:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 10 });
                    return ret;
                case TroopsEnum.HunChief:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 20 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 10 });
                    return ret;
                default: return ret;
            }
        }

        /// <summary>
        /// Is the troop infantry
        /// </summary>
        public static bool IsInfantry(TroopsEnum troop)
        {
            var building = GetTroopBuilding(troop, false);
            return building != BuildingEnum.Stable;
        }

        public static BuildingEnum GetTroopBuilding(TroopsEnum troop, bool great)
        {
            switch (troop)
            {
                case TroopsEnum.Legionnaire:
                case TroopsEnum.Praetorian:
                case TroopsEnum.Imperian:
                case TroopsEnum.Clubswinger:
                case TroopsEnum.Spearman:
                case TroopsEnum.Axeman:
                case TroopsEnum.Scout:
                case TroopsEnum.Phalanx:
                case TroopsEnum.Swordsman:
                case TroopsEnum.SlaveMilitia:
                case TroopsEnum.AshWarden:
                case TroopsEnum.KhopeshWarrior:
                case TroopsEnum.Mercenary:
                case TroopsEnum.Bowman:
                    if (great) return BuildingEnum.GreatBarracks;
                    return BuildingEnum.Barracks;

                case TroopsEnum.EquitesLegati:
                case TroopsEnum.EquitesImperatoris:
                case TroopsEnum.EquitesCaesaris:
                case TroopsEnum.Paladin:
                case TroopsEnum.TeutonicKnight:
                case TroopsEnum.Pathfinder:
                case TroopsEnum.TheutatesThunder:
                case TroopsEnum.Druidrider:
                case TroopsEnum.Haeduan:
                case TroopsEnum.SopduExplorer:
                case TroopsEnum.AnhurGuard:
                case TroopsEnum.ReshephChariot:
                case TroopsEnum.Spotter:
                case TroopsEnum.SteppeRider:
                case TroopsEnum.Marksman:
                case TroopsEnum.Marauder:
                    if (great) return BuildingEnum.GreatStable;
                    return BuildingEnum.Stable;

                case TroopsEnum.RomanRam:
                case TroopsEnum.RomanCatapult:
                case TroopsEnum.TeutonCatapult:
                case TroopsEnum.TeutonRam:
                case TroopsEnum.GaulRam:
                case TroopsEnum.GaulCatapult:
                case TroopsEnum.EgyptianCatapult:
                case TroopsEnum.EgyptianRam:
                case TroopsEnum.HunCatapult:
                case TroopsEnum.HunRam:
                    return BuildingEnum.Workshop;

                default:
                    return BuildingEnum.Site; //idk, should have error handling
            }
        }

        public static TroopsEnum TribeFirstTroop(TribeEnum? tribe)
        {
            switch (tribe)
            {
                case TribeEnum.Romans: return TroopsEnum.Legionnaire;
                case TribeEnum.Teutons: return TroopsEnum.Clubswinger;
                case TribeEnum.Gauls: return TroopsEnum.Phalanx;
                case TribeEnum.Egyptians: return TroopsEnum.SlaveMilitia;
                case TribeEnum.Huns: return TroopsEnum.Mercenary;
                default: return TroopsEnum.None;
            }
        }

        public static TroopsEnum TribeSettler(TribeEnum? tribe)
        {
            switch (tribe)
            {
                case TribeEnum.Romans: return TroopsEnum.RomanSettler;
                case TribeEnum.Teutons: return TroopsEnum.TeutonSettler;
                case TribeEnum.Gauls: return TroopsEnum.GaulSettler;
                case TribeEnum.Egyptians: return TroopsEnum.EgyptianSettler;
                case TribeEnum.Huns: return TroopsEnum.HunSettler;
                default: return TroopsEnum.None;
            }
        }

        public static bool IsTroopDefensive(Account acc, int i) =>
            IsTroopDefensive(TroopsHelper.TroopFromInt(acc, i));

        public static bool IsTroopDefensive(TroopsEnum troop)
        {
            switch (troop)
            {
                case TroopsEnum.Legionnaire:
                case TroopsEnum.Praetorian:
                case TroopsEnum.Spearman:
                case TroopsEnum.Paladin:
                case TroopsEnum.Phalanx:
                case TroopsEnum.Druidrider:
                case TroopsEnum.SlaveMilitia:
                case TroopsEnum.AshWarden:
                case TroopsEnum.AnhurGuard:
                case TroopsEnum.ReshephChariot:
                case TroopsEnum.Mercenary:
                case TroopsEnum.Marksman:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsTroopOffensive(Account acc, int i) =>
            IsTroopOffensive(TroopsHelper.TroopFromInt(acc, i));
        public static bool IsTroopOffensive(TroopsEnum troop)
        {
            switch (troop)
            {
                case TroopsEnum.Legionnaire:
                case TroopsEnum.Imperian:
                case TroopsEnum.EquitesCaesaris:
                case TroopsEnum.EquitesImperatoris:
                case TroopsEnum.Swordsman:
                case TroopsEnum.TheutatesThunder:
                case TroopsEnum.Haeduan:
                case TroopsEnum.Clubswinger:
                case TroopsEnum.Axeman:
                case TroopsEnum.TeutonicKnight:
                case TroopsEnum.Mercenary:
                case TroopsEnum.Bowman:
                case TroopsEnum.SteppeRider:
                case TroopsEnum.Marauder:
                case TroopsEnum.KhopeshWarrior:
                case TroopsEnum.ReshephChariot:
                    return true;
                default:
                    return false;
            }
        }  

        public static bool IsTroopRam(TroopsEnum troop) => IsTroopRam((int)troop);
        public static bool IsTroopRam(int troopInt) => (troopInt % 10) == 7;


        public static (int, int) GetTroopDeff(TroopsEnum troop) =>
            GetTroopDeff((int)troop);
        public static (int, int) GetTroopDeff(int troop) =>
            (TroopValues[troop, 1], TroopValues[troop, 2]);


        public static double GetTroopOff(TroopsEnum troop, int level = 1) =>
            GetTroopOff((int)troop, level);
        public static double GetTroopOff(int troop, int level = 1)
        {
            var upkeep = TroopValues[troop, 4];
            var baseOff = TroopValues[troop, 0];
            return ImprovedStat(baseOff, level, upkeep);
        }

        private static double ImprovedStat(int baseVal, int level, int upkeepBase)
        {
            if (level < 2) return baseVal;
            // https://github.com/kirilloid/travian/blob/master/src/model/t4/combat/army.ts
            // BASE_VALUE + (BASE_VALUE + 300 · UPKEEP / 7) · (1.007^LEVEL – 1) + UPKEEP · 0.0021
            double upkeep = (double)upkeepBase / 1.007F;
            return baseVal + (baseVal + 300.0F * (double)upkeep / 7.0F) * (Math.Pow(1.007F, level) - 1) + upkeep * 0.0021F;
        }


        /// <summary>
        /// Troops data: [offense, defense against infantry, defense against cavalry, speed, upkeep, training time, capacity, research time]
        /// </summary>
        public static int[,] TroopValues = new int[,]
        {
            {  0, 0, 0, 0, 0, 0, 0, 0 }, // None
            // Romans
            {40, 35, 50, 6 , 1,  2000, 50,  7800 },
            {30, 65, 35, 5 , 1,  2200, 20,  8400 },
            {70, 40, 25, 7 , 1,  2400, 50,  9000 },
            { 0,  20, 10, 16, 2,  1700, 0,   6900 },
            { 120,65, 50, 14, 3,  3300, 100, 11700 },
            { 180,80, 105,10, 4,  4400, 70,  15000 },
            { 60, 30, 75, 4 , 3,  4600, 0,   15600 },
            { 75, 60, 10, 3 , 6,  9000, 0,   28800 },
            { 50, 40, 30, 4 , 5, 90700, 0,   24475 },
            { 0,  80, 80, 5 , 1, 26900, 3000,0 },
            // Teutons
            { 40, 20, 5,  7 , 1,   900, 60,  4500 },
            { 10, 35, 60, 7 , 1,  1400, 40,  6000 },
            { 60, 30, 30, 6 , 1,  1500, 50,  6300 },
            { 0,  10, 5,  9 , 1,  1400, 0,   6000 },
            { 55, 100,40, 10, 2,  3000, 110, 10800 },
            { 150,50, 75, 9 , 3,  3700, 80,  12900 },
            { 65, 30, 80, 4 , 3,  4200, 0,   14400 },
            { 50, 60, 10, 3 , 6,  9000, 0,   28800 },
            { 40, 60, 40, 4 , 4, 70500, 0,   19425 },
            { 10, 80, 80, 5 , 1, 31000, 3000,0 },
            // Gauls
            { 15, 40, 50, 7 , 1,  1300, 35,  5700 },
            { 65, 35, 20, 6 , 1,  1800, 45,  7200 },
            { 0,  20, 10, 17, 2,  1700, 0,   6900 },
            { 90, 25, 40, 19, 2,  3100, 75,  11100 },
            { 45, 115,55, 16, 2,  3200, 35,  11400 },
            { 140,60, 165,13, 3,  3900, 65,  13500 },
            { 50, 30, 105,4 , 3,  5000, 0,   16800 },
            { 70, 45, 10, 3 , 6,  9000, 0,   28800 },
            { 40, 50, 50, 5 , 4, 90700, 0,   24475 },
            { 0,  80, 80, 5 , 1, 22700, 3000,0 },
            // Nature
            { 10, 25, 20, 20, 1, 0, 0, 1800 },
            { 20, 35, 40, 20, 1, 0, 0, 1800 },
            { 60, 40, 60, 20, 1, 0, 0, 1800 },
            { 80, 66, 50, 20, 1, 0, 0, 1800 },
            { 50, 70, 33, 20, 1, 0, 0, 1800 },
            { 100,80, 70, 20, 1, 0, 0, 1800 },
            { 250,140,200,20, 1, 0, 0, 1800 },
            { 450,380,240,20, 1, 0, 0, 1800 },
            { 200,170,250,20, 1, 0, 0, 1800 },
            { 600,440,520,20, 1, 0, 0, 0 },
            // Natars
            { 20, 35, 50, 6, 1, 0, 0, 1800 },
            { 65, 30, 10, 7, 1, 0, 0, 1800 },
            { 100,90, 75, 6, 1, 0, 0, 1800 },
            { 0,  10, 10, 25, 1, 0, 0, 1800 },
            { 155,80, 50, 14, 2, 0, 0, 1800 },
            { 170,140,80, 12, 3, 0, 0, 1800 },
            { 250,120,150,5, 4, 0, 0, 1800 },
            { 60, 45, 10, 3, 5, 0, 0, 1800 },
            { 80, 50, 50, 5, 1, 0, 0, 1800 },
            { 30, 40, 40, 5, 1, 0, 0, 0 },
            // Egyptians
            { 10, 30, 20, 7 , 1,   530, 15,   3390},
            { 30, 55, 40, 6 , 1,  1320, 50,   5760},
            { 65, 50, 20, 7 , 1,  1440, 45,   6120},
            { 0,  20, 10, 16, 2,  1360, 0,    5880},
            { 50, 110,50, 15, 2,  2560, 50,   9480},
            { 110,120,150,10, 3,  3240, 70,   11520},
            { 55, 30, 95, 4 , 3,  4800, 0,    16200},
            { 65, 55, 10, 3 , 6,  9000, 0,    28800},
            { 40, 50, 50, 4 , 4, 90700, 0,    24475},
            { 0,  80, 80, 5 , 1, 24800, 3000, 0},
            // Huns
            { 35, 40, 30, 6 , 1,   810, 50,   4230},
            { 50, 30, 10, 6 , 1,  1120, 30,   5160},
            { 0,  20, 10, 19, 2,  1360, 0,    5880},
            { 120,30, 15, 16, 2,  2400, 115,  9000},
            { 115,80, 70, 16, 2,  2480, 105,  9240},
            { 180,60, 40, 14, 3,  2990, 80,   10770},
            { 65, 30, 90, 4 , 3,  4400, 0,    15000},
            { 45, 55, 10, 3 , 6,  9000, 0,    28800},
            { 50, 40, 30, 5 , 4, 90700, 0,    24475},
            { 0,  80, 80, 5 , 1, 28950, 3000, 0},
        };
    }
}
