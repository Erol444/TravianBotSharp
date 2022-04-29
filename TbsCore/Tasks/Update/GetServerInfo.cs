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
            // Get Map size
            var size = DriverHelper.GetJsObj<long>(acc, "window.TravianDefaults.Map.Size.top");
            acc.AccInfo.MapSize = (int)size;
            acc.Logger.Information($"Server map size is {acc.AccInfo.MapSize}x{acc.AccInfo.MapSize}");

            // Get server speed
            var speed = DriverHelper.GetJsObj<long>(acc, "Travian.Game.speed");
            acc.AccInfo.ServerSpeed = (int)speed;
            acc.Logger.Information($"Server speed is {acc.AccInfo.ServerSpeed}x");

            return TaskRes.Executed;
        }
    }
}