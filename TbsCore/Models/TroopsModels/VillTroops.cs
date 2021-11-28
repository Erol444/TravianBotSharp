using System.Collections.Generic;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;

namespace TbsCore.Models.TroopsModels
{
    public class VillTroops
    {
        //TODO add my troops in other villages, with village coords  etc
        //TODO: troops count, market stuff

        public void Init()
        {
            CurrentlyTraining = new CurrentlyTraining();
            CurrentlyTraining.Init();
            Researched = new HashSet<Classificator.TroopsEnum>();
            Levels = new List<TroopLevel>();
            ToResearch = new HashSet<Classificator.TroopsEnum>();
            ToImprove = new HashSet<Classificator.TroopsEnum>();
        }

        public int Settlers { get; set; } // For settling new village. TODO: make creation of new villages more error-proof

        public Classificator.TroopsEnum? TroopToTrain { get; set; } //for high speed servers where you only train 1 troop type

        /// <summary>
        /// Currently training troops in village
        /// </summary>
        public CurrentlyTraining CurrentlyTraining { get; set; }

        /// <summary>
        /// Troop smithy level. Based on this you can also get researched troops
        /// </summary>
        public List<TroopLevel> Levels { get; set; }

        /// <summary>
        /// Already researched troops. First troop type (eg. legionair/clubswinger/phalax...) should
        /// be added when adding new village.
        /// </summary>
        public HashSet<Classificator.TroopsEnum> Researched { get; set; }

        /// <summary>
        /// Troops that need to be researched in academy
        /// </summary>
        public HashSet<Classificator.TroopsEnum> ToResearch { get; set; }

        /// <summary>
        /// Troops that need to be improved in smithy. Troops will be improved to lvl 20 by default.
        /// TODO: make this changeable
        /// </summary>
        public HashSet<Classificator.TroopsEnum> ToImprove { get; set; }

    }
}