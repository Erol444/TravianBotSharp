using MainCore.Enums;
using MainCore.Models.Runtime;
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

        public static List<PrerequisiteBuilding> GetPrerequisiteBuilding(this TroopEnums troop)
        {
            var ret = new List<PrerequisiteBuilding>();
            switch (troop)
            {
                //romans
                case TroopEnums.Praetorian:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 1 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.Imperian:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.EquitesLegati:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 1 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    return ret;

                case TroopEnums.EquitesImperatoris:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    return ret;

                case TroopEnums.EquitesCaesaris:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 10 });
                    return ret;

                case TroopEnums.RomanRam:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 1 });
                    return ret;

                case TroopEnums.RomanCatapult:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    return ret;

                case TroopEnums.RomanChief:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 20 });
                    return ret;
                //Teutons
                case TroopEnums.Spearman:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 1 });
                    return ret;

                case TroopEnums.Axeman:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 3 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.Scout:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 1 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    return ret;

                case TroopEnums.Paladin:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 3 });
                    return ret;

                case TroopEnums.TeutonicKnight:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 10 });
                    return ret;

                case TroopEnums.TeutonRam:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 1 });
                    return ret;

                case TroopEnums.TeutonCatapult:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 10 });
                    return ret;

                case TroopEnums.TeutonChief:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 20 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.RallyPoint, Level = 5 });
                    return ret;
                //Gauls
                case TroopEnums.Swordsman:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 1 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.Pathfinder:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 1 });
                    return ret;

                case TroopEnums.TheutatesThunder:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 3 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    return ret;

                case TroopEnums.Druidrider:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 5 });
                    return ret;

                case TroopEnums.Haeduan:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 10 });
                    return ret;

                case TroopEnums.GaulRam:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 1 });
                    return ret;

                case TroopEnums.GaulCatapult:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 10 });
                    return ret;

                case TroopEnums.GaulChief:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 20 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    return ret;
                //Egyptians
                case TroopEnums.AshWarden:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Barracks, Level = 1 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.KhopeshWarrior:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.SopduExplorer:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 1 });
                    return ret;

                case TroopEnums.AnhurGuard:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 5 });
                    return ret;

                case TroopEnums.ReshephChariot:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 10 });
                    return ret;

                case TroopEnums.EgyptianRam:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 5 });
                    return ret;

                case TroopEnums.EgyptianCatapult:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 10 });
                    return ret;

                case TroopEnums.EgyptianChief:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 20 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    return ret;
                //Huns
                case TroopEnums.Bowman:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 3 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.Spotter:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 1 });
                    return ret;

                case TroopEnums.SteppeRider:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 3 });
                    return ret;

                case TroopEnums.Marksman:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 5 });
                    return ret;

                case TroopEnums.Marauder:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 10 });
                    return ret;

                case TroopEnums.HunRam:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 1 });
                    return ret;

                case TroopEnums.HunCatapult:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 10 });
                    return ret;

                case TroopEnums.HunChief:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 20 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    return ret;

                default: return ret;
            }
        }
    }
}