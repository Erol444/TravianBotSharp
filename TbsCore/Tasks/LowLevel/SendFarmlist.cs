using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.TroopsModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendFarmlist : BotTask
    {
        public FarmList FL { get; set; }
        private HtmlNode GetFlNode(HtmlDocument htmlDoc) => htmlDoc.GetElementbyId("raidList" + this.FL.Id);
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?tt=99&id=39");

            //TODO: if there is no rally point, switch to different village!]
            var flNode = GetFlNode(acc.Wb.Html);

            if (flNode == null)
            {
                var mainVill = AccountHelper.GetMainVillage(acc);
                if (mainVill == this.Vill) return TaskRes.Executed; // No gold account?
                await VillageHelper.SwitchVillage(acc, mainVill.Id);
            }
            if (acc.Farming.TrainTroopsAfterFL)
            {
                TaskExecutor.AddTask(acc, new TrainTroops()
                {
                    ExecuteAt = DateTime.Now.AddSeconds(2),
                    Troop = acc.Villages[0].Troops.TroopToTrain ?? Classificator.TroopsEnum.Hero,
                    Vill = this.Vill,
                    HighSpeedServer = true
                });
            }

            await DriverHelper.ExecuteScript(acc, $"Travian.Game.RaidList.toggleList({this.FL.Id});");

            // Update flNode!
            flNode = GetFlNode(acc.Wb.Html);

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

            var sendFlScript = "var wrapper = document.getElementsByClassName('buttonWrapper')[0];";
            sendFlScript += "wrapper.getElementsByClassName('startButton')[0].click();";
            wb.ExecuteScript(sendFlScript);

            return TaskRes.Executed;
        }
    }
}
