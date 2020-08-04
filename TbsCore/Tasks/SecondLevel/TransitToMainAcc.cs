using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    /// <summary>
    /// Used on TTWars to constantly send resources to your main account
    /// </summary>
    public class TransitToMainAcc : BotTask
    {
        public Coordinates coords;
        public int delay;

        public override async Task<TaskRes> Execute(HtmlDocument htmlDoc, ChromeDriver wb, Files.Models.AccModels.Account acc)
        {
            TaskExecutor.AddTaskIfNotExists(acc, new TransitToMainAcc { coords = this.coords, delay = this.delay, ExecuteAt = DateTime.Now.AddSeconds(delay), vill = this.vill });

            //Resources res = new Resources() { Wood = 50000000, Clay = 50000000, Iron = 50000000, Crop = 50000000 };
            TaskExecutor.AddTask(acc, new SendResources() { ExecuteAt = DateTime.Now, Coordinates = coords, vill = this.vill });

            return TaskRes.Executed;
        }
    }
}
