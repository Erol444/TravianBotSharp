using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TbsCrossPlatform.Models.Enums;

namespace TbsCrossPlatform.Models.Database
{
    public class Building
    {
        [ForeignKey("Village")]
        public int VillageId { get; set; }

        /// <summary>
        /// Location of the building
        /// </summary>

        public int Location { get; set; }

        /// <summary>
        /// Type of the building
        /// </summary>
        public BuildingEnum Type { get; set; }

        public int Level { get; set; }

        /// <summary>
        /// Whether the building is currently being upgraded
        /// </summary>
        public bool UnderConstruction { get; set; }
    }
}