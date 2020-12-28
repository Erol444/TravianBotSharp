using System;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.TroopsModels
{
    public class TroopCurrentlyImproving
    {
        public Classificator.TroopsEnum Troop { get; set; }
        public int Level { get; set; }
        public TimeSpan Time { get; set; }
    }
}
