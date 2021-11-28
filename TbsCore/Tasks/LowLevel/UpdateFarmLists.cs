using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;
using TbsCore.Parsers;
using static TbsCore.Helpers.Classificator;
using HtmlAgilityPack;
using TbsCore.Models.TroopsModels;
using System.Collections.Generic;

namespace TbsCore.Tasks.LowLevel
{
    public class UpdateFarmLists : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await NavigationHelper.ToRallyPoint(acc, this.Vill, NavigationHelper.RallyPointTab.Farmlist);

            var foundFLs = FarmlistParser.ParseFL(acc.Wb.Html, acc.AccInfo.ServerVersion);
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
                var flNode = GetFlNode(acc.Wb.Html, acc.AccInfo.ServerVersion, farmlist.Id);
                if (acc.AccInfo.ServerVersion == ServerVersionEnum.TTwars ||
                        flNode.Descendants("div").Any(x => x.HasClass("expandCollapse") && x.HasClass("collapsed")))
                {
                    await DriverHelper.ExecuteScript(acc, $"Travian.Game.RaidList.toggleList({farmlist.Id});");
                    await Task.Delay(AccountHelper.Delay(acc) * 2);
                    // Update flNode!
                    flNode = GetFlNode(acc.Wb.Html, acc.AccInfo.ServerVersion, farmlist.Id);
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

        private HtmlNode GetFlNode(HtmlDocument htmlDoc, ServerVersionEnum version, int id)
        {
            switch (version)
            {
                case ServerVersionEnum.TTwars: return htmlDoc.GetElementbyId("list" + id);
                case ServerVersionEnum.T4_5: return htmlDoc.GetElementbyId("raidList" + id);
                default: return null;
            }
        }
    }
}