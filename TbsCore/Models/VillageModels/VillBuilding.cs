﻿using System.Collections.Generic;
using TbsCore.Models.AccModels;
using TbsCore.Models.BuildingModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.TravianData;

namespace TbsCore.Models.VillageModels
{
    public class VillBuilding
    {
        public Building[] Buildings { get; set; }

        /// <summary>
        ///     Whether or not we want to use insta build (2 gold) in this village
        /// </summary>
        public bool InstaBuild { get; set; }

        /// <summary>
        ///     If currently building is above specified hours, insta build
        /// </summary>
        public int InstaBuildHours { get; set; }

        public List<BuildingCurrently> CurrentlyBuilding { get; set; }
        public List<DemolishTask> DemolishTasks { get; set; }
        public List<BuildingTask> Tasks { get; set; }
        public bool AutoBuildResourceBonusBuildings { get; set; }

        public void Init(Account acc)
        {
            Buildings = InitializeArray(40, acc); //there are 40 slots for buildings (except for WW, TODO!)
            Tasks = new List<BuildingTask>();
            CurrentlyBuilding = new List<BuildingCurrently>();
            DemolishTasks = new List<DemolishTask>();
        }


        private Building[] InitializeArray(int length, Account acc)
        {
            var array = new Building[length];
            for (var i = 0; i < length; ++i)
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