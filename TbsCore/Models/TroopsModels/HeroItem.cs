using System;
using System.Collections.Generic;
using System.Text;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.TroopsModels
{
    public class HeroItem
    {
        public Classificator.HeroItemEnum Item { get; set; }
        public int Count { get; set; }
    }
}