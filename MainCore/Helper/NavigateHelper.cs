using MainCore.Services;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if TRAVIAN_OFFICIAL

using TravianOfficialCore.Parsers;

#elif TRAVIAN_OFFICIAL_HEROUI

using TravianOfficialNewHeroUICore.Parsers;

#elif TTWARS

using TTWarsCore.Parsers;

#endif

namespace MainCore.Helper
{
    public static class NavigateHelper
    {
        public static void SwitchVillage(AppDbContext context, IChromeBrowser chromeBrowser, int villageId)
        {
            while (!CheckHelper.IsCorrectVillage(context, chromeBrowser, villageId))
            {
                var html = chromeBrowser.GetHtml();

                var listNode = VillagesTable.GetVillageNodes(html);
                foreach (var node in listNode)
                {
                    var id = VillagesTable.GetId(node);
                    if (id != villageId) continue;

                    var chrome = chromeBrowser.GetChrome();
                    var elements = chrome.FindElements(By.XPath(node.XPath));
                    elements[0].Click();
                }
            }
        }
    }
}