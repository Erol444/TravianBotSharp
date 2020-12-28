using System;
using System.Collections.Generic;
using System.Text;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.BuildingModels
{
    public class Prerequisite
    {
        public Classificator.BuildingEnum Building { get; set; }
        public int Level { get; set; }
    }
}
