using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Models.JsObjects
{
    public class Bld
    {
        [JsonProperty("stufe")]
        public int Level { get; set; }

        [JsonProperty("gid")]
        public Classificator.BuildingEnum Building { get; set; }

        [JsonProperty("aid")]
        public int Location { get; set; }
    }
}
