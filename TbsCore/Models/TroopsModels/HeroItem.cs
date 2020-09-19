using System;
using System.Collections.Generic;
using System.Text;
using TravBotSharp.Files.Helpers;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TbsCore.Models.TroopsModels
{
    public class HeroItem
    {
        public HeroItemEnum Item { get; set; }
        public int Count { get; set; }
    }
}