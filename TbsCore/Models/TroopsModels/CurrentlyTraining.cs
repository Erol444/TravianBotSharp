using System.Collections.Generic;

namespace TravBotSharp.Files.Models.TroopsModels
{
    public class CurrentlyTraining
    {
        public void Init()
        {
            Barracks = new List<TroopsCurrentlyTraining>();
            Stable = new List<TroopsCurrentlyTraining>();
            GB = new List<TroopsCurrentlyTraining>();
            GS = new List<TroopsCurrentlyTraining>();
            Workshop = new List<TroopsCurrentlyTraining>();
        }
        public List<TroopsCurrentlyTraining> Barracks { get; set; }
        public List<TroopsCurrentlyTraining> Stable { get; set; }
        public List<TroopsCurrentlyTraining> GB { get; set; }
        public List<TroopsCurrentlyTraining> GS { get; set; }
        public List<TroopsCurrentlyTraining> Workshop { get; set; }
    }
}
