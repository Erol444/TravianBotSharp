using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Helpers;

namespace TbsCore.Tasks.LowLevel
{
    /// <summary>
    /// Used on TTWars to constantly send resources to your main account
    /// </summary>
    public class TransitToMainAcc : BotTask
    {
        public Coordinates coords;
        public int delay;

        public override async Task<TaskRes> Execute(Account acc)
        {
            acc.Tasks.Add(new TransitToMainAcc
            {
                coords = this.coords,
                delay = this.delay,
                ExecuteAt = DateTime.Now.AddSeconds(delay),
                Vill = this.Vill
            }, true);

            await Task.Delay(AccountHelper.Delay());

            //Resources res = new Resources() { Wood = 50000000, Clay = 50000000, Iron = 50000000, Crop = 50000000 };
            acc.Tasks.Add(new SendResources() { ExecuteAt = DateTime.Now, Coordinates = coords, Vill = this.Vill });

            return TaskRes.Executed;
        }
    }
}