using MainCore.Enums;
using MainCore.Models.Database;
using MainCore.Models.Runtime;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MainCore
{
    public static class Extensions
    {
        public static bool IsResourceField(this BuildingEnums building)
        {
            int buildingInt = (int)building;
            // If id between 1 and 4, it's resource field
            return buildingInt < 5 && buildingInt > 0;
        }

        public static bool IsNotAdsUpgrade(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.CommandCenter => true,
                BuildingEnums.Palace => true,
                BuildingEnums.Residence => true,
                _ => false,
            };
        }

        public static bool IsWall(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.EarthWall => true,
                BuildingEnums.CityWall => true,
                BuildingEnums.Palisade => true,
                BuildingEnums.StoneWall => true,
                BuildingEnums.MakeshiftWall => true,
                _ => false,
            };
        }

        public static BuildingEnums GetWall(this TribeEnums tribe)
        {
            return tribe switch
            {
                TribeEnums.Teutons => BuildingEnums.EarthWall,
                TribeEnums.Romans => BuildingEnums.CityWall,
                TribeEnums.Gauls => BuildingEnums.Palisade,
                TribeEnums.Egyptians => BuildingEnums.StoneWall,
                TribeEnums.Huns => BuildingEnums.MakeshiftWall,
                _ => BuildingEnums.Site,
            };
        }

        public static bool IsMultipleAllow(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Warehouse => true,
                BuildingEnums.Granary => true,
                BuildingEnums.GreatWarehouse => true,
                BuildingEnums.GreatGranary => true,
                BuildingEnums.Trapper => true,
                BuildingEnums.Cranny => true,
                _ => false,
            };
        }

        public static int GetMaxLevel(this BuildingEnums building)
        {
            if (VersionDetector.IsTravianOfficial())
            {
                return building switch
                {
                    BuildingEnums.Brewery => 20,
                    BuildingEnums.Bakery => 5,
                    BuildingEnums.Brickyard => 5,
                    BuildingEnums.IronFoundry => 5,
                    BuildingEnums.GrainMill => 5,
                    BuildingEnums.Sawmill => 5,

                    BuildingEnums.Cranny => 10,
                    _ => 20,
                };
            }
            if (VersionDetector.IsTTWars())
            {
                return building switch
                {
                    BuildingEnums.Brewery => 10,
                    BuildingEnums.Bakery => 5,
                    BuildingEnums.Brickyard => 5,
                    BuildingEnums.IronFoundry => 5,
                    BuildingEnums.GrainMill => 5,
                    BuildingEnums.Sawmill => 5,

                    BuildingEnums.Cranny => 10,
                    _ => 20,
                };
            }

            return 0;
        }

        public static Color GetColor(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Site => Color.White,
                BuildingEnums.Woodcutter => Color.ForestGreen,
                BuildingEnums.ClayPit => Color.Orange,
                BuildingEnums.IronMine => Color.Gray,
                BuildingEnums.Cropland => Color.Yellow,
                _ => Color.LawnGreen,
            };
        }

        public static BuildingEnums GetTribesWall(this TribeEnums tribe) => tribe switch
        {
            TribeEnums.Teutons => BuildingEnums.EarthWall,
            TribeEnums.Romans => BuildingEnums.CityWall,
            TribeEnums.Gauls => BuildingEnums.Palisade,
            TribeEnums.Egyptians => BuildingEnums.StoneWall,
            TribeEnums.Huns => BuildingEnums.MakeshiftWall,
            _ => BuildingEnums.Site,
        };

        public static bool HasMultipleTabs(this BuildingEnums building) => building switch
        {
            BuildingEnums.RallyPoint => true,
            BuildingEnums.CommandCenter => true,
            BuildingEnums.Residence => true,
            BuildingEnums.Palace => true,
            BuildingEnums.Marketplace => true,
            BuildingEnums.Treasury => true,
            _ => false,
        };

        public static int GetBuildingsCategory(this BuildingEnums building) => building switch
        {
            BuildingEnums.GrainMill => 2,
            BuildingEnums.Sawmill => 2,
            BuildingEnums.Brickyard => 2,
            BuildingEnums.IronFoundry => 2,
            BuildingEnums.Bakery => 2,
            BuildingEnums.Barracks => 1,
            BuildingEnums.HerosMansion => 1,
            BuildingEnums.Academy => 1,
            BuildingEnums.Smithy => 1,
            BuildingEnums.Stable => 1,
            BuildingEnums.GreatBarracks => 1,
            BuildingEnums.GreatStable => 1,
            BuildingEnums.Workshop => 1,
            BuildingEnums.TournamentSquare => 1,
            BuildingEnums.Trapper => 1,
            _ => 0,
        };

        public static (TribeEnums, List<PrerequisiteBuilding>) GetPrerequisiteBuildings(this BuildingEnums building)
        {
            TribeEnums tribe = TribeEnums.Any;
            var ret = new List<PrerequisiteBuilding>();
            switch (building)
            {
                case BuildingEnums.Sawmill:
                    ret.Add(new() { Building = BuildingEnums.Woodcutter, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.Brickyard:
                    ret.Add(new() { Building = BuildingEnums.ClayPit, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.IronFoundry:
                    ret.Add(new() { Building = BuildingEnums.IronMine, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.GrainMill:
                    ret.Add(new() { Building = BuildingEnums.Cropland, Level = 5 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.Bakery:
                    ret.Add(new() { Building = BuildingEnums.Cropland, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    ret.Add(new() { Building = BuildingEnums.GrainMill, Level = 5 });
                    break;

                case BuildingEnums.Warehouse:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 1 });
                    break;

                case BuildingEnums.Granary:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 1 });
                    break;

                case BuildingEnums.Blacksmith:
                    //DOESN'T EXIST ANYMORE
                    tribe = TribeEnums.Nature; //Just a dirty hack, since user can't be Nature, he can't build Blacksmith
                    break;

                case BuildingEnums.Smithy:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.Academy, Level = 1 });
                    break;

                case BuildingEnums.TournamentSquare:
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 15 });
                    break;

                case BuildingEnums.MainBuilding:
                    break;

                case BuildingEnums.RallyPoint:
                    break;

                case BuildingEnums.Marketplace:
                    ret.Add(new() { Building = BuildingEnums.Warehouse, Level = 1 });
                    ret.Add(new() { Building = BuildingEnums.Granary, Level = 1 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    break;

                case BuildingEnums.Embassy:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 1 });
                    break;

                case BuildingEnums.Barracks:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 1 });
                    break;

                case BuildingEnums.Stable:
                    ret.Add(new() { Building = BuildingEnums.Smithy, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.Academy, Level = 5 });
                    break;

                case BuildingEnums.Workshop:
                    ret.Add(new() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.Academy:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.Barracks, Level = 3 });
                    break;

                case BuildingEnums.Cranny:
                    break;

                case BuildingEnums.TownHall:
                    ret.Add(new() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 10 });
                    break;

                case BuildingEnums.Residence:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 }); //no palace!
                    break;

                case BuildingEnums.Palace:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 }); //no residence!
                    ret.Add(new() { Building = BuildingEnums.Embassy, Level = 1 });
                    break;

                case BuildingEnums.Treasury:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 10 });
                    break;

                case BuildingEnums.TradeOffice:
                    ret.Add(new() { Building = BuildingEnums.Stable, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.Marketplace, Level = 20 });
                    break;

                case BuildingEnums.GreatBarracks:
                    ret.Add(new() { Building = BuildingEnums.Barracks, Level = 20 }); //not capital!
                    break;

                case BuildingEnums.GreatStable:
                    ret.Add(new() { Building = BuildingEnums.Stable, Level = 20 }); //not capital
                    break;

                case BuildingEnums.CityWall:
                    tribe = TribeEnums.Romans;
                    break;

                case BuildingEnums.EarthWall:
                    tribe = TribeEnums.Teutons;
                    break;

                case BuildingEnums.Palisade:
                    tribe = TribeEnums.Gauls;
                    break;

                case BuildingEnums.StonemasonsLodge:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 }); //capital
                    break;

                case BuildingEnums.Brewery:
                    tribe = TribeEnums.Teutons;
                    ret.Add(new() { Building = BuildingEnums.Granary, Level = 20 });
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    break;

                case BuildingEnums.Trapper:
                    tribe = TribeEnums.Gauls;
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 1 });
                    break;

                case BuildingEnums.HerosMansion:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 1 });
                    break;

                case BuildingEnums.GreatWarehouse:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 10 }); //art/ww vill
                    break;

                case BuildingEnums.GreatGranary:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 10 }); //art/ww vill
                    break;

                case BuildingEnums.WW: //ww vill
                    tribe = TribeEnums.Nature; //Just a dirty hack, since user can't be Nature, bot can't construct WW.
                    break;

                case BuildingEnums.HorseDrinkingTrough:
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.Stable, Level = 20 });
                    tribe = TribeEnums.Romans;
                    break;

                case BuildingEnums.StoneWall:
                    tribe = TribeEnums.Egyptians;
                    break;

                case BuildingEnums.MakeshiftWall:
                    tribe = TribeEnums.Huns;
                    break;

                case BuildingEnums.CommandCenter: //no res/palace
                    tribe = TribeEnums.Huns;
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.Waterworks:
                    tribe = TribeEnums.Egyptians;
                    ret.Add(new() { Building = BuildingEnums.HerosMansion, Level = 10 });
                    break;

                default:
                    break;
            }
            return (tribe, ret);
        }

        public static bool IsUsableWhenHeroAway(this HeroItemEnums item)
        {
            return item switch
            {
                HeroItemEnums.Ointment or HeroItemEnums.Scroll or HeroItemEnums.Bucket or HeroItemEnums.Tablets or HeroItemEnums.Book or HeroItemEnums.Artwork or HeroItemEnums.SmallBandage or HeroItemEnums.BigBandage or HeroItemEnums.Cage or HeroItemEnums.Wood or HeroItemEnums.Clay or HeroItemEnums.Iron or HeroItemEnums.Crop => true,
                _ => false,
            };
        }

        public static bool IsNumeric(this string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.All(char.IsDigit);
        }

        public static int ToNumeric(this string value)
        {
            var valueStr = new string(value.Where(c => char.IsDigit(c) || c == '-').ToArray());
            if (string.IsNullOrEmpty(valueStr)) return 0;
            return int.Parse(valueStr);
        }

        public static string EnumStrToString(this string value)
        {
            var len = value.Length;
            for (int i = 1; i < len; i++)
            {
                if (char.IsUpper(value[i]))
                {
                    value = value.Insert(i, " ");
                    i++;
                    len++;
                }
            }
            return value;
        }

        public static TimeSpan ToDuration(this string value)
        {
            //00:00:02 (+332 ms), TTWars, milliseconds matter
            int ms = 0;
            if (value.Contains("(+"))
            {
                var parts = value.Split('(');
                ms = parts[1].ToNumeric();
                value = parts[0];
            }
            // h:m:s
            var arr = value.Split(':');
            var h = arr[0].ToNumeric();
            var m = arr[1].ToNumeric();
            var s = arr[2].ToNumeric();
            return new TimeSpan(0, h, m, s, ms);
        }

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
            return tribe switch
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

        public static DateTime GetTimeWhenEnough(this VillageProduction production, long[] resRequired)
        {
            var productionArr = new long[] { production.Wood, production.Clay, production.Iron, production.Crop };

            var now = DateTime.Now;
            var ret = now.AddMinutes(2);

            for (int i = 0; i < 4; i++)
            {
                DateTime toWaitForThisRes = DateTime.MinValue;
                if (resRequired[i] > 0)
                {
                    // In case of negative crop, we will never have enough crop
                    if (productionArr[i] <= 0) return DateTime.MaxValue;

                    float hoursToWait = resRequired[i] / (float)productionArr[i];
                    float secToWait = hoursToWait * 3600;
                    toWaitForThisRes = now.AddSeconds(secToWait);
                }

                if (ret < toWaitForThisRes) ret = toWaitForThisRes;
            }
            return ret;
        }
    }
}