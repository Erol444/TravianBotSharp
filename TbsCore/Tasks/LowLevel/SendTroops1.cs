using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using TbsCore.Extensions;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.SendTroopsModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendTroops1 : BotTask
    {
        public TroopsMovementModel TroopsMovement { get; set; }
        //TODO Add options for catapult/scout targets inside SendTroops2!

        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?tt=2&id=39");

            //add number of troops to the input boxes
            for (int i = 0; i < TroopsMovement.Troops.Length; i++)
            {
                if (TroopsMovement.Troops[i] == 0) continue;
                wb.ExecuteScript($"document.getElementsByName('t{i + 1 }')[0].value='{TroopsMovement.Troops[i]}'");
                await Task.Delay(222);
            }

            //select coordinates
            await wb.FindElementById("xCoordInput").Write(TroopsMovement.Coordinates.x);
            await wb.FindElementById("yCoordInput").Write(TroopsMovement.Coordinates.y);

            //Select type of troop sending
            string script = "var radio = document.getElementsByClassName(\"radio\");for(var i = 0; i < radio.length; i++){";
            script += $"if(radio[i].value == \"{(int)TroopsMovement.MovementType}\") radio[i].checked = \"checked\"}}";
            await DriverHelper.ExecuteScript(acc, script);

            //Click on "Send" button
            wb.FindElementById("xCoordInput").Click();

            return TaskRes.Executed;
        }
    }
}
