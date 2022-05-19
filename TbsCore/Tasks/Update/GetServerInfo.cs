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
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.TTwars:
                    {
                        // Get Map size
                        var size = DriverHelper.GetJsObj<long>(acc, "window.TravianDefaults.Map.Size.top");
                        acc.AccInfo.MapSize = (int)size;
                        acc.Logger.Information($"Server map size is {acc.AccInfo.MapSize * 2}x{acc.AccInfo.MapSize * 2}");

                        // Get server speed
                        var speed = DriverHelper.GetJsObj<long>(acc, "Travian.Game.speed");
                        acc.AccInfo.ServerSpeed = (int)speed;
                        acc.Logger.Information($"Server speed is {acc.AccInfo.ServerSpeed}x");
                        break;
                    }
                case Classificator.ServerVersionEnum.T4_5:
                    {
                        var size = DriverHelper.GetJsObj<long>(acc, "Travian.Variables.Map.Size.top");
                        acc.AccInfo.MapSize = (int)size;
                        acc.Logger.Information($"Server map size is {acc.AccInfo.MapSize * 2}x{acc.AccInfo.MapSize * 2}");

                        // Get server speed
                        var speed = DriverHelper.GetJsObj<long>(acc, "Travian.Variables.Speed");
                        acc.AccInfo.ServerSpeed = (int)speed;
                        acc.Logger.Information($"Server speed is {acc.AccInfo.ServerSpeed}x");
                        break;
                    }
                default:
                    break;
            }

            return TaskRes.Executed;
        }
    }
}