using TbsReact.Models;
using TbsReact.Models.Villages;

namespace TbsReact.Extension
{
    public static class VillageExtension
    {
        public static Village GetInfo(this TbsCore.Models.VillageModels.Village village)
        {
            return new Village
            {
                Id = village.Id,
                Name = village.Name,
                Coordinate = new Coordinate
                {
                    X = village.Coordinates.x,
                    Y = village.Coordinates.y,
                },
            };
        }
    }
}