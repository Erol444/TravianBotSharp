using System;
using System.Collections.Generic;
using System.Text;
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
        public static List<Prerequisite> GetBuildingPrerequisites(Classificator.TroopsEnum troop)
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

        public static Classificator.TroopsEnum TribeFirstTroop(Classificator.TribeEnum? tribe)
        {
            switch (tribe)
            {
                case TribeEnum.Romans: return Classificator.TroopsEnum.Legionnaire;
                case TribeEnum.Teutons: return Classificator.TroopsEnum.Clubswinger;
                case TribeEnum.Gauls: return Classificator.TroopsEnum.Phalanx;
                case TribeEnum.Egyptians: return Classificator.TroopsEnum.SlaveMilitia;
                case TribeEnum.Huns: return Classificator.TroopsEnum.Mercenary;
                default: return TroopsEnum.None;
            }
        }

        public static Classificator.TroopsEnum TribeSettler(Classificator.TribeEnum? tribe)
        {
            switch (tribe)
            {
                case TribeEnum.Romans: return Classificator.TroopsEnum.RomanSettler;
                case TribeEnum.Teutons: return Classificator.TroopsEnum.TeutonSettler;
                case TribeEnum.Gauls: return Classificator.TroopsEnum.GaulSettler;
                case TribeEnum.Egyptians: return Classificator.TroopsEnum.EgyptianSettler;
                case TribeEnum.Huns: return Classificator.TroopsEnum.HunSettler;
                default: return TroopsEnum.None;
            }
        }

        public static bool IsTroopDefensive(Classificator.TroopsEnum troop)
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

        public static bool IsTroopOffensive(Classificator.TroopsEnum troop)
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
                case TroopsEnum.Marksman:
                case TroopsEnum.Bowman:
                case TroopsEnum.SteppeRider:
                case TroopsEnum.Marauder:
                case TroopsEnum.SlaveMilitia:
                case TroopsEnum.AshWarden:
                case TroopsEnum.KhopeshWarrior:
                case TroopsEnum.ReshephChariot:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsTroopRam(Classificator.TroopsEnum troop)
        {
            switch (troop)
            {
                case TroopsEnum.EgyptianRam:
                case TroopsEnum.GaulRam:
                case TroopsEnum.HunRam:
                case TroopsEnum.RomanRam:
                case TroopsEnum.TeutonRam:
                    return true;
                default:
                    return false;
            }
        }
    }
}
