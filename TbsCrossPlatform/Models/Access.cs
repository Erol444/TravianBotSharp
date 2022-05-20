using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TbsCrossPlatform.Models
{
    [Index(nameof(UserAgent), Name = "Index_Useragent")]
    public class Access
    {
        /// <summary>
        /// Account's id
        /// </summary>
        [ForeignKey("Account")]
        public string AccountId { get; set; }

        /// <summary>
        /// Access's id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Access's password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Access's useragent
        /// </summary>
        ///
        public string UserAgent { get; set; }

        /// <summary>
        /// Is sitter ?
        /// </summary>
        public bool IsSitter { get; set; }

        /// <summary>
        /// Last time use this access
        /// </summary>
        public DateTime LastLogin { get; set; }
    }
}