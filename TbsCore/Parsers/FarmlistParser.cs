using System.Collections.Generic;
using System.Linq;
using TravBotSharp.Files.Models.TroopsModels;

namespace TravBotSharp.Files.Parsers
{
    public static class FarmlistParser
    {
        public static List<FarmList> ParseFL(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            var list = new List<FarmList>();
            var raidList = htmlDoc.GetElementbyId("raidList");
            //No raid lists? Maybe no rally point, change village
            if (raidList == null) return null;
            var fls = raidList.ChildNodes.Where(x => x.Id.StartsWith("list"));
            foreach (var fl in fls)
            {
                FarmList farmList = new FarmList();
                farmList.Enabled = true;
                farmList.Name = fl.Descendants("div").First(x => x.HasClass("listTitleText")).InnerText.Trim();
                farmList.Id = (int)Parser.RemoveNonNumeric(fl.Id);
                farmList.NumOfFarms = (int)Parser.RemoveNonNumeric(fl.Descendants("span").First(x => x.HasClass("raidListSlotCount")).InnerText.Split('/')[0]);
                list.Add(farmList);
            }
            return list;
        }
    }
}
