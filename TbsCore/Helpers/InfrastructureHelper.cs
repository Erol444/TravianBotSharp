namespace TravBotSharp.Files.Helpers
{
    public static class InfrastructureHelper
    {
        public static Classificator.BuildingEnum GetTribesWall(Classificator.TribeEnum? tribe)
        {
            switch (tribe)
            {
                case Classificator.TribeEnum.Teutons:
                    return Classificator.BuildingEnum.EarthWall;
                case Classificator.TribeEnum.Romans:
                    return Classificator.BuildingEnum.CityWall;
                case Classificator.TribeEnum.Gauls:
                    return Classificator.BuildingEnum.Palisade;
                case Classificator.TribeEnum.Egyptians:
                    return Classificator.BuildingEnum.StoneWall;
                case Classificator.TribeEnum.Huns:
                    return Classificator.BuildingEnum.MakeshiftWall;
                default:
                    return Classificator.BuildingEnum.Site;
            }
        }
    }
}