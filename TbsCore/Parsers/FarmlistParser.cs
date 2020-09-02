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
            var vills = raidList.ChildNodes.Where(x => x.HasClass("villageWrapper"));
            foreach(var vill in vills)
            {
                string villName = vill.Descendants("a").First(x => x.HasClass("villageName")).InnerText;
                var fls = vill.Descendants("div").Where(x => x.HasClass("raidList"));

                foreach(var fl in fls)
                {
                    FarmList farmList = new FarmList();
                    farmList.Enabled = true;
                    string FlName = fl.Descendants("div").First(x => x.HasClass("listName")).InnerText.Trim();
                    farmList.Name = (villName + " - " + FlName).Replace("\t","").Replace("\n", "").Replace("\r", "");
                    farmList.Id = (int)Parser.RemoveNonNumeric(fl.GetAttributeValue("data-listid", ""));
                    farmList.NumOfFarms = (int)Parser.RemoveNonNumeric(fl.Descendants("span").First(x => x.HasClass("slotsCount")).InnerText);
                    list.Add(farmList);
                }
            }

            return list;
        }
    }
}
