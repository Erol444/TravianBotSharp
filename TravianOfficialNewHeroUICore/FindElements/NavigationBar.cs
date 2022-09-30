using HtmlAgilityPack;
using System.Linq;

namespace TravianOfficialNewHeroUICore.FindElements
{
    public static class NavigationBar
    {
        public static HtmlNode GetResourceButton(HtmlDocument doc)
        {
            var navigationBar = doc.GetElementbyId("navigation");
            if (navigationBar is null) return null;
            var buttonNode = navigationBar.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("accesskey", 0) == 1);
            return buttonNode;
        }

        public static HtmlNode GetBuildingButton(HtmlDocument doc)
        {
            var navigationBar = doc.GetElementbyId("navigation");
            if (navigationBar is null) return null;
            var buttonNode = navigationBar.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("accesskey", 0) == 2);
            return buttonNode;
        }

        public static HtmlNode GetMapButton(HtmlDocument doc)
        {
            var navigationBar = doc.GetElementbyId("navigation");
            if (navigationBar is null) return null;
            var buttonNode = navigationBar.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("accesskey", 0) == 3);
            return buttonNode;
        }

        public static HtmlNode GetStatisticsButton(HtmlDocument doc)
        {
            var navigationBar = doc.GetElementbyId("navigation");
            if (navigationBar is null) return null;
            var buttonNode = navigationBar.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("accesskey", 0) == 4);
            return buttonNode;
        }

        public static HtmlNode GetReportsButton(HtmlDocument doc)
        {
            var navigationBar = doc.GetElementbyId("navigation");
            if (navigationBar is null) return null;
            var buttonNode = navigationBar.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("accesskey", 0) == 5);
            return buttonNode;
        }

        public static HtmlNode GetMessageButton(HtmlDocument doc)
        {
            var navigationBar = doc.GetElementbyId("navigation");
            if (navigationBar is null) return null;
            var buttonNode = navigationBar.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("accesskey", 0) == 6);
            return buttonNode;
        }

        public static HtmlNode GetDailyButton(HtmlDocument doc)
        {
            var navigationBar = doc.GetElementbyId("navigation");
            if (navigationBar is null) return null;
            var buttonNode = navigationBar.Descendants("a").FirstOrDefault(x => x.GetAttributeValue("accesskey", 0) == 7);
            return buttonNode;
        }
    }
}