using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.TroopsModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendFarmlist : BotTask
    {
        public FarmList FL { get; set; }
        public override async Task<TaskRes> Execute(HtmlDocument htmlDoc, ChromeDriver wb, Files.Models.AccModels.Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?tt=99&id=39");

            //TODO: if there is no rally point, switch to different village!]
            var flNode = htmlDoc.GetElementbyId("list" + this.FL.Id);

            if (flNode == null)
            {
                TaskExecutor.AddTask(acc, new SwitchVillage() { vill = AccountHelper.GetMainVillage(acc), ExecuteAt = DateTime.MinValue.AddMinutes(10), Priority = TaskPriority.High });
                this.NextExecute = DateTime.Now.AddSeconds(5);
                return TaskRes.Executed;
            }
            if (acc.Farming.TrainTroopsAfterFL)
            {
                TaskExecutor.AddTask(acc, new TrainTroops()
                {
                    ExecuteAt = DateTime.Now.AddSeconds(2),
                    Troop = acc.Villages[0].Troops.TroopToTrain ?? Classificator.TroopsEnum.Hero,
                    vill = this.vill,
                    HighSpeedServer = true
                });
            }

            wb.ExecuteScript($"Travian.Game.RaidList.toggleList({this.FL.Id});"); //Toggle the FL (show it)

            await Task.Delay(AccountHelper.Delay() * 2);

            htmlDoc.LoadHtml(wb.PageSource);
            await Task.Delay(AccountHelper.Delay());

            // Update flNode!
            flNode = htmlDoc.GetElementbyId("list" + this.FL.Id);
            foreach (var farm in flNode.Descendants("tr").Where(x => x.HasClass("slotRow")))
            {

                //iReport2 = yellow swords, iReport3 = red swords, iReport1 = successful raid
                var img = farm.ChildNodes.FirstOrDefault(x => x.HasClass("lastRaid"))?.Descendants("img");

                //there has to be an image (we already have a report) and wrong raid style to not check this farmlist:
                if (img.Count() != 0 && ( //no image -> no recent attack
                    (img.FirstOrDefault(x => x.HasClass("iReport3")) != null && this.FL.RaidStyle != RaidStyle.RaidLost) //raid was lost and we don't have RaidLost raidstyle
                    || (img.FirstOrDefault(x => x.HasClass("iReport2")) != null && (this.FL.RaidStyle == RaidStyle.RaidSuccessful)) //some casualities, but we only attack on successful
                    ))
                {
                    continue;
                }

                var checkbox = farm.Descendants("input").FirstOrDefault(x => x.HasClass("markSlot"));
                var str = $"document.getElementById('{checkbox.Id}').checked=true";
                wb.ExecuteScript(str); //Check the checkbox
            }

            await Task.Delay(AccountHelper.Delay() * 2);

            wb.ExecuteScript($"document.getElementById('{flNode.Id}').childNodes[1].submit()");

            return TaskRes.Executed;
        }
    }
}
