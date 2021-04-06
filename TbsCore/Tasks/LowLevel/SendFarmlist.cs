using HtmlAgilityPack;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.TroopsModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.TroopsModels;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendFarmlist : BotTask
    {
        public FarmList FL { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id=39&tt=99");

            var flNode = GetFlNode(acc.Wb.Html, acc.AccInfo.ServerVersion);

            // If there is no rally point, switch to different village
            if (flNode == null)
            {
                var mainVill = AccountHelper.GetMainVillage(acc);
                if (mainVill == this.Vill) return TaskRes.Executed; // No gold account?
                await VillageHelper.SwitchVillage(acc, mainVill.Id);
                flNode = GetFlNode(acc.Wb.Html, acc.AccInfo.ServerVersion);
                if (flNode == null) return TaskRes.Retry;
            }

            if (acc.Farming.TrainTroopsAfterFL) // For TTWars servers
            {
                TaskExecutor.AddTask(acc, new TrainTroops()
                {
                    ExecuteAt = DateTime.Now.AddSeconds(2),
                    Troop = Vill.Troops.TroopToTrain ?? Classificator.TroopsEnum.Hero,
                    Vill = this.Vill,
                    HighSpeedServer = true
                });
            }

            // If FL is collapsed, expand it
            if (acc.AccInfo.ServerVersion == ServerVersionEnum.T4_4 ||
                flNode.Descendants("div").Any(x => x.HasClass("expandCollapse") && x.HasClass("collapsed")))
            {
                await DriverHelper.ExecuteScript(acc, $"Travian.Game.RaidList.toggleList({this.FL.Id});");
                // Update flNode!
                flNode = GetFlNode(acc.Wb.Html, acc.AccInfo.ServerVersion);
            }

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
                await DriverHelper.CheckById(acc, checkbox.Id, true, update: false);
            }

            await Task.Delay(AccountHelper.Delay() * 2);

            switch (acc.AccInfo.ServerVersion)
            {
                case ServerVersionEnum.T4_4:
                    var sendFlScript = $"document.getElementById('{flNode.Id}').childNodes[1].submit()";
                    wb.ExecuteScript(sendFlScript);
                    break;

                case ServerVersionEnum.T4_5:
                    var startRaid = flNode.Descendants("button").FirstOrDefault(x => x.HasClass("startButton"));
                    acc.Wb.Driver.FindElementById(startRaid.Id).Click();
                    break;
            }

            acc.Wb.Log($"FarmList '{this.FL.Name}' was sent");
            return TaskRes.Executed;
        }

        private HtmlNode GetFlNode(HtmlDocument htmlDoc, ServerVersionEnum version)
        {
            switch (version)
            {
                case ServerVersionEnum.T4_4: return htmlDoc.GetElementbyId("list" + this.FL.Id);
                case ServerVersionEnum.T4_5: return htmlDoc.GetElementbyId("raidList" + this.FL.Id);
                default: return null;
            }
        }
    }
}