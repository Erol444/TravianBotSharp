using System;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Models.TroopsModels
{
    public class TroopCurrentlyImproving
    {
        public Classificator.TroopsEnum Troop { get; set; }
        public int Level { get; set; }
        public TimeSpan Time { get; set; }
    }
}
