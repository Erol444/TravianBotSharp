using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Threading.Tasks;
using TbsCore.Extensions;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.TroopsModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class AddFarm : BotTask
    {
        public int FarmListId { get; set; }
        public Coordinates Coordinates { get; set; }
        public List<TroopsRaw> Troops { get; set; }
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?tt=99&id=39");

            wb.ExecuteScript($"Travian.Game.RaidList.addSlot({this.FarmListId},'','','rallyPoint');"); //show "Add raid" popup
            await Task.Delay(AccountHelper.Delay());
            
            //select coordinates
            await wb.FindElementById("xCoordInput").Write(Coordinates.x);
            await wb.FindElementById("yCoordInput").Write(Coordinates.y);

            await Task.Delay(AccountHelper.Delay());

            //add number of troops to the input boxes
            foreach (var troop in Troops)
            {
                int troopNum = (int)troop.Type % 10;
                await wb.FindElementByName("t" + troopNum).Write(troop.Number);
            }
            await Task.Delay(AccountHelper.Delay());

            //click "save"
            wb.ExecuteScript("Travian.Game.RaidList.saveSlot(getSelectedListId(), $('edit_form').toQueryString().parseQueryString(), true);");
            return TaskRes.Executed;
        }
    }
}
