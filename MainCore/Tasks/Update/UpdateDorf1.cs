using MainCore.Enums;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if TRAVIAN_OFFICIAL

using TravianOfficialCore.FindElements;

#elif TRAVIAN_OFFICIAL_HEROUI

using TravianOfficialNewHeroUICore.FindElements;

#elif TTWARS

using TTWarsCore.FindElements;

#endif

namespace MainCore.Tasks.Update
{
    public class UpdateDorf1 : UpdateVillage
    {
        public UpdateDorf1(int villageId, int accountId) : base(villageId, accountId)
        {
        }

        public override async Task Execute()
        {
            var result = ToDorf1();
            if (!result) return;
            await base.Execute();
        }

        private bool ToDorf1()
        {
            var currentUrl = ChromeBrowser.GetCurrentUrl();
            if (currentUrl.Contains("dorf1")) return true;
            var doc = ChromeBrowser.GetHtml();
            var node = NavigationBar.GetResourceButton(doc);
            if (node is null)
            {
                LogManager.Warning(AccountId, "Cannot find Resources button");
                TaskManager.UpdateAccountStatus(AccountId, AccountStatus.Offline);
                return false;
            }

            var chrome = ChromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(node.XPath));
            if (elements.Count == 0)
            {
                LogManager.Warning(AccountId, "Cannot find Resources button");
                TaskManager.UpdateAccountStatus(AccountId, AccountStatus.Offline);
                return false;
            }

            elements[0].Click();
            var wait = ChromeBrowser.GetWait();
            wait.Until(driver => driver.Url.Contains("dorf"));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            return true;
        }
    }
}