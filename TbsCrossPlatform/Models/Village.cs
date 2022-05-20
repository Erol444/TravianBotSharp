using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TbsCrossPlatform.Models
{
    public class Village
    {
        /// <summary>
        /// Account's id
        /// </summary>
        [ForeignKey("Account")]
        public int AccountId { get; set; }

        /// <summary>
        /// Village's Id
        /// Same with travian village's id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Village's name
        /// </summary>
        public int Name { get; set; }

        /// <summary>
        /// Village's X coordinate
        /// </summary>
        public int CoordX { get; set; }

        /// <summary>
        /// Village's Y coordinate
        /// </summary>
        public int CoordY { get; set; }

        /// <summary>
        /// Is village active ?
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Is village underattack ?
        /// </summary>
        public bool IsUnderAttack { get; set; }
    }
}