using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Internal;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class TTWarsGetRes : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php");

            Random rnd = new Random();
            int sec = rnd.Next(370, 380);
            TaskExecutor.AddTask(acc, new TTWarsGetRes() { ExecuteAt = DateTime.Now.AddSeconds(sec), Vill = AccountHelper.GetMainVillage(acc) });
            TaskExecutor.AddTask(acc, new TrainExchangeRes() { ExecuteAt = DateTime.Now.AddSeconds(sec + 5), troop = acc.Villages[0].Troops.TroopToTrain ?? Classificator.TroopsEnum.Hero, Vill = Vill });
            TaskExecutor.AddTask(acc, new TrainTroops()
            {
                ExecuteAt = DateTime.Now.AddSeconds(sec + 9),
                Troop = acc.Villages[0].Troops.TroopToTrain ?? Classificator.TroopsEnum.Hero,
                Vill = Vill,
                HighSpeedServer = true
            });


            var script = "window.fireEvent('startPaymentWizard', {data:{activeTab: 'paymentFeatures'}});";
            await DriverHelper.ExecuteScript(acc, script);

            script = "$$('.paymentWizardMenu').addClass('hide');$$('.buyGoldInfoStep').removeClass('active');$$('.buyGoldInfoStep#2').addClass('active');$$('.paymentWizardMenu#buyResources').removeClass('hide');";
            await DriverHelper.ExecuteScript(acc, script);


            //gold prosButton buyResources6
            //gold prosButton buyAnimal5
            var buy = acc.Wb.Html.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("buyResources6"));
            if (buy == null)
            {
                acc.Wb.Log("Can't find the button with class buyResources6. Are you sure you are on vip/unl TTWars server?");
                return TaskRes.Executed;
            }

            wb.FindElementById(buy.Id).Click();

            return TaskRes.Executed;
        }
    }
}
