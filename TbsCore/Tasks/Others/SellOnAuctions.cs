using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Parsers;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.Others
{
    public class SellOnAuctions : BotTask
    {
        private readonly Random rand = new Random();

        public override async Task<TaskRes> Execute(Account acc)
        {
            await Task.Yield();
            acc.Logger.Warning("This feature is temporary disabled");
            return TaskRes.Executed;
            //if (!acc.Wb.CurrentUrl.Contains("auction?action=sell"))
            //{
            //    string xPathSellTab = null;

            //    switch (acc.AccInfo.ServerVersion)
            //    {
            //        case ServerVersionEnum.TTwars:
            //            xPathSellTab = "//*[@id='content']/div[4]/div[2]/div[3]/a";
            //            break;

            //        case ServerVersionEnum.T4_5:
            //            xPathSellTab = "//*[@id='heroAuction']/div[2]/div[2]/div[3]/a";
            //            break;
            //    }
            //    // enter right tab

            //    do
            //    {
            //        await NavigationHelper.ToAuction(acc);

            //        var node = acc.Wb.Html.DocumentNode.SelectSingleNode(xPathSellTab);
            //        if (node == null) continue;
            //        var element = acc.Wb.Driver.FindElement(By.XPath(node.XPath));
            //        if (element == null) continue;
            //        element.Click();

            //        try
            //        {
            //            await DriverHelper.WaitPageChange(acc, "auction?action=sell");
            //        }
            //        catch
            //        {
            //            continue;
            //        }
            //        break;
            //    }
            //    while (true);
            //}

            //acc.Wb.UpdateHtml();
            //var nodeAllItem = acc.Wb.Html.GetElementbyId("itemsToSale");
            //if (nodeAllItem == null) return TaskRes.Executed;

            //var nodeItems = nodeAllItem.Descendants("div").Where(x => !x.HasClass("disabled") && x.HasClass("item"));
            //if (nodeItems.Count() == 0) return TaskRes.Executed;

            //foreach (var nodeItem in nodeItems)
            //{
            //    (var heroItemEnum, int amount) = HeroParser.ParseItemNode(nodeItem);
            //    if (heroItemEnum == null) continue;

            //    var category = HeroHelper.GetHeroItemCategory(heroItemEnum ?? HeroItemEnum.Others_None_0);
            //    switch (category)
            //    {
            //        case HeroItemCategory.Others:
            //            continue;
            //        case HeroItemCategory.Resource:
            //            continue;

            //        case HeroItemCategory.Stackable:
            //            if (amount < 5) continue;
            //            break;

            //        case HeroItemCategory.Horse:
            //            continue;
            //    }

            //    var nodeParentItem = nodeItem.ParentNode;
            //    var nodeItemXPath = acc.Wb.Html.DocumentNode.SelectSingleNode($"//*[@id='{nodeParentItem.Id}']/div");
            //    if (nodeItem.Id != nodeItemXPath.Id) continue;
            //    var element = acc.Wb.Driver.FindElement(By.XPath(nodeItemXPath.XPath));
            //    if (element == null) continue;
            //    element.Click();
            //    await Task.Delay(rand.Next(1500, 2500));

            //    int counter = 3;
            //    do
            //    {
            //        counter--;
            //        acc.Wb.UpdateHtml();
            //        var nodeDialog = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("dialogVisible"));
            //        if (nodeDialog == null) continue;

            //        var button = acc.Wb.Html.DocumentNode.SelectSingleNode("//*[@id='mainLayout']/body/div[1]/div/div/div/div/form/div[6]/button");
            //        if (button == null) continue;
            //        try
            //        {
            //            var elementButton = acc.Wb.Driver.FindElement(By.XPath(button.XPath));
            //            if (elementButton == null) continue;
            //            elementButton.Click();
            //        }
            //        catch { }
            //        break;
            //    }
            //    while (counter > 0);
            //}
        }
    }
}