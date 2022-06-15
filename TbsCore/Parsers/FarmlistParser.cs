using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.TroopsModels;

namespace TbsCore.Parsers
{
    public static class FarmlistParser
    {
        public static List<FarmList> ParseFL(HtmlDocument htmlDoc)
        {
            var raidList = htmlDoc.GetElementbyId("raidList");
            if (raidList == null) return null;
            List<FarmList> list = new List<FarmList>();
            var vills = raidList.ChildNodes.Where(x => x.HasClass("villageWrapper"));
            foreach (var vill in vills)
            {
                string villName = vill.Descendants("a").First(x => x.HasClass("villageName")).InnerText;
                var fls = vill.Descendants("div").Where(x => x.HasClass("raidList"));

                foreach (var fl in fls)
                {
                    FarmList farmList = new FarmList
                    {
                        Enabled = true
                    };
                    string FlName = fl.Descendants("div").First(x => x.HasClass("listName")).InnerText.Trim();
                    farmList.Name = (villName + " - " + FlName).Replace("\t", "").Replace("\n", "").Replace("\r", "");
                    farmList.Id = (int)Parser.RemoveNonNumeric(fl.GetAttributeValue("data-listid", ""));
                    farmList.NumOfFarms = (int)Parser.RemoveNonNumeric(fl.Descendants("span").First(x => x.HasClass("slotsCount")).InnerText);
                    list.Add(farmList);
                }
            }
            return list;
        }
    }
}