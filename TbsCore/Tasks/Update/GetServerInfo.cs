using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.Update
{
    public class GetServerInfo : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            {
                var result = await Update(acc);
                if (!result) return TaskRes.Executed;
            }

            var size = DriverHelper.GetJsObj<long>(acc, "Travian.Variables.Map.Size.top");
            acc.AccInfo.MapSize = (int)size;
            acc.Logger.Information($"Server map size is {acc.AccInfo.MapSize * 2}x{acc.AccInfo.MapSize * 2}");

            // Get server speed
            var speed = DriverHelper.GetJsObj<long>(acc, "Travian.Variables.Speed");
            acc.AccInfo.ServerSpeed = (int)speed;
            acc.Logger.Information($"Server speed is {acc.AccInfo.ServerSpeed}x");

            return TaskRes.Executed;
        }
    }
}