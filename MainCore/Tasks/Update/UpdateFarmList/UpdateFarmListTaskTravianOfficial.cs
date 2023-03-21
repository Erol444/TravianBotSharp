using FluentResults;
using HtmlAgilityPack;
using MainCore.Errors;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Update.UpdateFarmList
{
    public class UpdateFarmListTaskTravianOfficial : UpdateFarmListTask
    {
        public UpdateFarmListTaskTravianOfficial(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
        }

        protected override List<HtmlNode> GetFarmNodes(HtmlDocument doc)
        {
            var raidList = doc.GetElementbyId("raidList");
            if (raidList is null) return new();
            var fls = raidList.Descendants("div").Where(x => x.HasClass("raidList"));

            return fls.ToList();
        }

        protected override int GetId(HtmlNode node)
        {
            var id = node.GetAttributeValue("data-listid", "0");
            var value = new string(id.Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(value);
        }

        protected override string GetName(HtmlNode node)
        {
            var flName = node.Descendants("div").FirstOrDefault(x => x.HasClass("listName"));
            if (flName is null) return null;
            return flName.InnerText.Trim();
        }

        protected override int GetNumOfFarms(HtmlNode node)
        {
            var slotCount = node.Descendants("span").FirstOrDefault(x => x.HasClass("raidListSlotCount"));
            if (slotCount is null) return 0;
            var slot = slotCount.InnerText.Split('/');
            if (slot.Length < 1) return 0;
            var value = new string(slot[0].Where(c => char.IsDigit(c)).ToArray());
            return int.Parse(value);
        }

        protected override Result GotoFarmListTab()
        {
            var result = _navigateHelper.SwitchTab(AccountId, 4);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
        }

        protected override Result GotoRallypoint()
        {
            var chrome = _chromeBrowser.GetChrome();
            var html = _chromeBrowser.GetHtml();
            var node = html.DocumentNode.SelectSingleNode($"//*[@id='villageContent']/div[{39 - 18}]");
            if (node is null)
            {
                return Result.Fail(new Retry($"Cannot find rallypoint element"));
            }
            var pathBuilding = node.Descendants("path").FirstOrDefault();
            if (pathBuilding is null)
            {
                return Result.Fail(new Retry($"Cannot find rallypoint element"));
            }
            var href = pathBuilding.GetAttributeValue("onclick", "");
            var script = href.Replace("&amp;", "&");
            chrome.ExecuteScript(script);
            {
                var result = _navigateHelper.WaitPageChanged(AccountId, $"?id={39}");
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = _navigateHelper.WaitPageLoaded(AccountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
        }

        protected override bool IsFarmListPage()
        {
            var chromeBrowser = _chromeManager.Get(AccountId);
            var url = chromeBrowser.GetCurrentUrl();
            if (!url.Contains("id=39")) return false;
            var html = chromeBrowser.GetHtml();

            var navigationBar = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("contentNavi") && x.HasClass("subNavi"));
            var navNodes = navigationBar.Descendants("a").Where(x => x.HasClass("tabItem")).ToArray();
            if (navNodes.Length < 5) return false;
            return navNodes[4].HasClass("active");
        }

        protected override bool IsFarmListTab()
        {
        }
    }
}