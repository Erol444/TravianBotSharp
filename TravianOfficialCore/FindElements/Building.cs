using HtmlAgilityPack;
using System.Linq;

namespace TravianOfficialCore.FindElements
{
    public static class Building
    {
        public static HtmlNode GetResourceField(HtmlDocument doc, int index)
        {
            return doc.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass($"buildingSlot{index}"));
        }

        public static HtmlNode GetBuilding(HtmlDocument doc, int index)
        {
            var location = index - 18; // - 19 + 1
            return doc.DocumentNode.SelectSingleNode($"//*[@id='villageContent']/div[{location}]");
        }
    }
}