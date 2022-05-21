using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TbsCrossPlatform.Models.Database
{
    public class ProduceResources : Resources
    {
        [Key]
        [ForeignKey("Village")]
        public int VillageId { get; set; }
    }
}