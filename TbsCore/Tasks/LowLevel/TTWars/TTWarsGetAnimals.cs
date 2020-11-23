using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class TTWarsGetAnimals : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php");

            Random rnd = new Random();
            int sec = rnd.Next(725, 740);
            TaskExecutor.AddTask(acc, new TTWarsGetAnimals() { ExecuteAt = DateTime.Now.AddSeconds(sec), Vill = AccountHelper.GetMainVillage(acc) });

            //Open payment wizard on tab Plus features (where you can buy stuff with gold)
            var script = "window.fireEvent('startPaymentWizard', {data:{activeTab: 'paymentFeatures'}});";
            await DriverHelper.ExecuteScript(acc, script);

            script = "$$('.paymentWizardMenu').addClass('hide');$$('.buyGoldInfoStep').removeClass('active');$$('.buyGoldInfoStep#3').addClass('active');$$('.paymentWizardMenu#buyResources').removeClass('hide');";
            await DriverHelper.ExecuteScript(acc, script);

            var buy = acc.Wb.Html.DocumentNode.SelectNodes("//*[text()[contains(., '3000')]]")[0];
            while (buy.Name != "button") buy = buy.ParentNode;
            
            wb.FindElementById(buy.Id).Click();

            //Clicking on buy button DOES NOT trigger a page reloag. We have to do it manually.
            return TaskRes.Executed;
        }
    }
}
