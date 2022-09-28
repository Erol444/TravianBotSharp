using MainCore.Enums;
using MainCore.Models.Runtime;
using System.Collections.Generic;

namespace MainCore.TravianData
{
    public static class BuildingsData
    {
        public static (TribeEnums, List<PrerequisiteBuilding>) GetPrerequisiteBuildings(BuildingEnums building)
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
    }
}