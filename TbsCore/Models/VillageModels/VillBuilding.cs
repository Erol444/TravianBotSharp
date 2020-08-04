using System.Collections.Generic;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.Building;

namespace TravBotSharp.Files.Models.VillageModels
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
            array[39].Type = InfrastructureHelper.GetTribesWall(acc.AccInfo.Tribe);
            array[38].Type = Helpers.Classificator.BuildingEnum.RallyPoint;
            return array;
        }
    }
}
