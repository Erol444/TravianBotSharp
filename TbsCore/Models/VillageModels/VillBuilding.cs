using System.Collections.Generic;
using TbsCore.Models.AccModels;
using TbsCore.Models.BuildingModels;
using TbsCore.Helpers;
using TbsCore.TravianData;

namespace TbsCore.Models.VillageModels
{
    public class VillBuilding
    {
        public void Init(Account acc)
        {
            Buildings = InitializeArray(40, acc); //there are 40 slots for buildings (except for WW, TODO!)
            Tasks = new List<BuildingTask>();
            CurrentlyBuilding = new List<BuildingCurrently>();
            DemolishTasks = new List<DemolishTask>();
        }

        public Building[] Buildings { get; set; }

        /// <summary>
        /// Whether or not we want to use insta build (2 gold) in this village
        /// </summary>
        public bool InstaBuild { get; set; }

        /// <summary>
        /// If currently building is above specified minuets, insta build (with gold)
        /// </summary>
        public int InstaBuildMinutes { get; set; }

        public List<BuildingCurrently> CurrentlyBuilding { get; set; }
        public List<DemolishTask> DemolishTasks { get; set; }
        public List<BuildingTask> Tasks { get; set; }
        public bool AutoBuildResourceBonusBuildings { get; set; }

        private Building[] InitializeArray(int length, Account acc)
        {
            Building[] array = new Building[length];
            for (int i = 0; i < length; ++i)
            {
                var building = new Building();
                building.Init(i + 1, 0, 0, false);
                array[i] = building;
            }
            array[39].Type = BuildingsData.GetTribesWall(acc.AccInfo.Tribe);
            array[38].Type = Classificator.BuildingEnum.RallyPoint;
            return array;
        }
    }
}