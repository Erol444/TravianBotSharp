using System.Collections.Generic;
using TbsCore.Helpers;
using TbsCore.Models.VillageModels;

namespace TbsCore.Models.AccModels
{
    public class NewVillageSettings
    {
        public void Init()
        {
            Locations = new List<NewVillage>();
            Types = new List<Classificator.VillTypeEnum>();
            AutoSettleNewVillages = true;
            NameTemplate = "0#NUM#";
            //DefaultSettings = new VillSettings();
            //DefaultSettings.Init();
        }

        /// <summary>
        /// Auto settle new villages. This includes training settler when residence is 10.
        /// </summary>
        public bool AutoSettleNewVillages { get; set; }

        /// <summary>
        /// Building tasks to be imported when a new village is found.
        /// </summary>
        public string BuildingTasksLocationNewVillage { get; set; }

        /// <summary>
        /// User specified locations for next settlement. If empty and AutoFindVillages is true, bot will find next village to settle.
        /// </summary>
        public List<NewVillage> Locations { get; set; }

        /// <summary>
        /// Types of villages to auto find for next settlement. If empty, any type is ok.
        /// </summary>
        public List<Classificator.VillTypeEnum> Types { get; set; }

        /// <summary>
        /// Auto find villages around your main village to settle. Only if we run out of user specified Locations to settle.
        /// </summary>
        public bool AutoFindVillages { get; set; }

        /// <summary>
        /// The default village settings for all new villages. Configurable inside the overview tab.
        /// </summary>
        //public VillSettings DefaultSettings { get; set; }
        /// <summary>
        /// Name template for new villages
        /// </summary>
        public string NameTemplate { get; set; }
    }
}