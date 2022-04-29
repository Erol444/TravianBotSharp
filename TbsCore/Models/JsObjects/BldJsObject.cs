using Newtonsoft.Json;
using TbsCore.Helpers;

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