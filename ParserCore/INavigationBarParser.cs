using HtmlAgilityPack;

namespace ParserCore
{
    public interface INavigationBarParser
    {
        public HtmlNode GetResourceButton(HtmlDocument doc);

        public HtmlNode GetBuildingButton(HtmlDocument doc);

        public HtmlNode GetMapButton(HtmlDocument doc);

        public HtmlNode GetStatisticsButton(HtmlDocument doc);

        public HtmlNode GetReportsButton(HtmlDocument doc);

        public HtmlNode GetMessageButton(HtmlDocument doc);

        public HtmlNode GetDailyButton(HtmlDocument doc);
    }
}