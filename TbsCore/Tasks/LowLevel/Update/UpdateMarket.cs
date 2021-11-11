using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.LowLevel
{
    public class UpdateMarket : Update
    {
        public UpdateMarket(Village vill, DateTime executeAt, TaskPriority priority = TaskPriority.Medium) : base(vill, executeAt, priority)
        {
            BuildingType = BuildingEnum.Marketplace;
        }

        public override async Task<TaskRes> Execute(Account acc)
        {
            return await base.Execute(acc);
        }
    }
}