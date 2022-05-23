﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TbsCrossPlatform.Models.Enums;

namespace TbsCrossPlatform.Models.Database
{
    [Index(nameof(Username), nameof(Server), Name = "Index_account")]
    public class Account
    {
        /// <summary>
        /// Id account, not related to Travian account id
        /// Based on account username & server url
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Account's username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Account's server
        /// remember remove https:// and / before saving
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Server's type
        /// Set when server input set
        /// </summary>
        public ServerTypeEnum ServerType { get; set; }

        public TribeEnum Tribe { get; set; }
    }
}