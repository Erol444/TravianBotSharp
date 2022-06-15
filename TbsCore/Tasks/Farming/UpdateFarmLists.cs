using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.TroopsModels;
using TbsCore.Parsers;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.Farming
{
    public class UpdateFarmLists : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            Vill = acc.Villages.First();
            {
                acc.Logger.Information($"Checking current village ...");
                var result = await NavigationHelper.SwitchVillage(acc, Vill);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }
            await NavigationHelper.ToRallyPoint(acc, this.Vill, NavigationHelper.RallyPointTab.Farmlist);

            var foundFLs = FarmlistParser.ParseFL(acc.Wb.Html);
            if (foundFLs == null)
            {
                acc.Logger.Warning("No FL, do you have rally point in this village?");
                this.Vill = AccountHelper.GetMainVillage(acc);
                this.NextExecute = DateTime.Now.AddSeconds(10);
                return TaskRes.Executed;
            }
            foreach (var oldFl in acc.Farming.FL)
            {
                var foundFL = foundFLs.FirstOrDefault(x => x.Id == oldFl.Id);
                if (foundFL == null) //FL was removed!
                {
                    acc.Farming.FL.Remove(oldFl);
                    continue;
                }
                //update the Name of FL (maybe it was changed)
                oldFl.Name = foundFL.Name;
                oldFl.NumOfFarms = foundFL.NumOfFarms;
                foundFLs.Remove(foundFL);
            }
            //If we added a new FL (and was previously not in acc.Farming.FL, it should still be in foundFLs list. So add them
            acc.Farming.FL.AddRange(foundFLs);

            // Read all farms in all farmlists
            foreach (var farmlist in acc.Farming.FL)
            {
                var flNode = GetFlNode(acc.Wb.Html, farmlist.Id);
                if (flNode.Descendants("div").Any(x => x.HasClass("expandCollapse") && x.HasClass("collapsed")))
                {
                    await DriverHelper.ExecuteScript(acc, $"Travian.Game.RaidList.toggleList({farmlist.Id});");
                    await Task.Delay(AccountHelper.Delay(acc) * 2);
                    // Update flNode!
                    flNode = GetFlNode(acc.Wb.Html, farmlist.Id);
                }

                farmlist.Farms = new List<GoldClubFarm>();
                foreach (var farm in flNode.Descendants("tr").Where(x => x.HasClass("slotRow")))
                {
                    var coords = MapParser.GetPositionDetails(farm);
                    if (coords != null) farmlist.Farms.Add(new GoldClubFarm(coords));
                }
            }
            return TaskRes.Executed;
        }

        private HtmlNode GetFlNode(HtmlDocument htmlDoc, int id)
        {
            return htmlDoc.GetElementbyId("raidList" + id);
        }
    }
}