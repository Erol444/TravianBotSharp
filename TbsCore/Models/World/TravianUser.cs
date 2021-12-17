using System;
using System.Collections.Generic;

namespace TbsCore.Models.AttackModels
{
    public class TravianUser
    {
        // Username
        // Rank
        // Tribe
        // Ally
        // Hero!! For checking if anything changed, to recognize fake attacks
        
        public List<TravianVillage> Villages { get; set; }
    }

}