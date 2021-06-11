using System;
using TbsCore.Helpers;

namespace TbsCore.Models.VillageModels
{
    public class Building
    {
        public Building Init(int buildingPlace, byte lvl, byte buildingType, bool uc)
        {
            Id = (byte)buildingPlace;
            Level = lvl;
            Type = (Classificator.BuildingEnum)Enum.ToObject(typeof(Classificator.BuildingEnum), buildingType);
            UnderConstruction = uc;
            return this;
        }

        /// <summary>
        /// Type of the building
        /// </summary>
        public Classificator.BuildingEnum Type { get; set; }

        public byte Level { get; set; }

        /// <summary>
        /// Location of the building
        /// </summary>
        public byte Id { get; set; }

        /// <summary>
        /// Whether the building is currently being upgraded
        /// </summary>
        public bool UnderConstruction { get; set; }
    }
}