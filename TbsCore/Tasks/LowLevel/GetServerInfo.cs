using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.LowLevel
{
    public class GetServerInfo : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await Task.Yield();
            // Get Map size
            var size = DriverHelper.GetJsObj<long>(acc, "window.TravianDefaults.Map.Size.top");
            acc.AccInfo.MapSize = (int)size;
            acc.Logger.Information($"Server map size is {acc.AccInfo.MapSize}x{acc.AccInfo.MapSize}");

            // Get server speed
            var speed = DriverHelper.GetJsObj<long>(acc, "Travian.Game.speed");
            acc.AccInfo.ServerSpeed = (int)speed;
            acc.Logger.Information($"Server speed is {acc.AccInfo.ServerSpeed}");

            // Get server version
            acc.AccInfo.ServerVersion = (acc.Wb.Html.GetElementbyId("sidebarBoxDailyquests") == null ? Classificator.ServerVersionEnum.T4_5 : Classificator.ServerVersionEnum.TTwars);
            acc.Logger.Information($"Server version is {acc.AccInfo.ServerVersion}");

            return TaskRes.Executed;
        }
    }
}