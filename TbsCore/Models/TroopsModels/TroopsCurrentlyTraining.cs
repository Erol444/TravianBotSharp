using System;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.TroopsModels
{
    public class TroopsCurrentlyTraining
    {
        public Classificator.TroopsEnum Troop { get; set; }
        public int TrainNumber { get; set; }
        public DateTime FinishTraining { get; set; }
    }
}