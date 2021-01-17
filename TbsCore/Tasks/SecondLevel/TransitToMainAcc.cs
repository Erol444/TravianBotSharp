using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
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
            var wb = acc.Wb.Driver;
            TaskExecutor.AddTaskIfNotExists(acc, new TransitToMainAcc
            {
                coords = this.coords,
                delay = this.delay,
                ExecuteAt = DateTime.Now.AddSeconds(delay),
                Vill = this.Vill
            });

            //Resources res = new Resources() { Wood = 50000000, Clay = 50000000, Iron = 50000000, Crop = 50000000 };
            TaskExecutor.AddTask(acc, new SendResources() { ExecuteAt = DateTime.Now, Coordinates = coords, Vill = this.Vill });

            return TaskRes.Executed;
        }
    }
}
