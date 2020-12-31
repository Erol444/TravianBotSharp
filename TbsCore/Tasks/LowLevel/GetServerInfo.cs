using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class GetServerInfo : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php");

            // Get Map size
            var size = DriverHelper.GetJsObj<long>(acc, "window.TravianDefaults.Map.Size.top");
            acc.AccInfo.MapSize = (int)size;

            // Get server speed
            var speed = DriverHelper.GetJsObj<long>(acc, "Travian.Game.speed");
            acc.AccInfo.ServerSpeed = (int)speed;

            return TaskRes.Executed;
        }
    }
}