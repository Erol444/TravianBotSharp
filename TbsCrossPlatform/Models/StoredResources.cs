using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TbsCrossPlatform.Models
{
    public class StoredResources : Resources
    {
        [Key]
        [ForeignKey("Village")]
        public int VillageId { get; set; }

        public int FreeCrop { get; set; }
        public int Warehouse { get; set; }
        public int Ganary { get; set; }
    }
}