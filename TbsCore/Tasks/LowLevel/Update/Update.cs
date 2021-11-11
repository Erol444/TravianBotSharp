using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.LowLevel
{
    public class Update : BotTask
    {
        /// <summary>
        /// What building does bot will enter
        /// </summary>
        protected BuildingEnum BuildingType;

        /// <summary>
        /// Was bot in building
        /// </summary>
        protected bool IsInBuilding = false;

        public Update(Village vill, DateTime executeAt, TaskPriority priority = TaskPriority.Medium)
        {
            Vill = vill;
            ExecuteAt = executeAt;
            Priority = priority;
        }

        public override async Task<TaskRes> Execute(Account acc)
        {
            if (await VillageHelper.EnterBuilding(acc, Vill, BuildingType))
            {
                IsInBuilding = true;
            }
            return TaskRes.Executed;
        }
    }
}