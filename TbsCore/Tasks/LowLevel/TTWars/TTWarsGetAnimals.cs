using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class TTWarsGetAnimals : BotTask
    {
        public override async Task<TaskRes> Execute(HtmlDocument htmlDoc, ChromeDriver wb, Files.Models.AccModels.Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php");

            Random rnd = new Random();
            int sec = rnd.Next(725, 740);
            TaskExecutor.AddTask(acc, new TTWarsGetAnimals() { ExecuteAt = DateTime.Now.AddSeconds(sec), vill = AccountHelper.GetMainVillage(acc) });

            //Open payment wizard on tab Plus features (where you can buy stuff with gold)
            wb.ExecuteScript("window.fireEvent('startPaymentWizard', {data:{activeTab: 'paymentFeatures'}});");

            await Task.Delay(AccountHelper.Delay());

            wb.ExecuteScript("$$('.paymentWizardMenu').addClass('hide');$$('.buyGoldInfoStep').removeClass('active');$$('.buyGoldInfoStep#3').addClass('active');$$('.paymentWizardMenu#buyResources').removeClass('hide');"); //Excgabge resources button

            await Task.Delay(AccountHelper.Delay() * 2);

            htmlDoc.LoadHtml(wb.PageSource);

            var buy = htmlDoc.DocumentNode.SelectNodes("//*[text()[contains(., '3000')]]")[0];
            while (buy.Name != "button") buy = buy.ParentNode;
            var buyId = buy.GetAttributeValue("id", "");
            wb.ExecuteScript($"document.getElementById('{buyId}').click()");

            //Clicking on buy button DOES NOT trigger a page reloag. We have to do it manually.
            return TaskRes.Executed;
        }
    }
}
