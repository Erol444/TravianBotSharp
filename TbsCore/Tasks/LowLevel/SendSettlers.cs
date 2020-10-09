using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.VillageModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendSettlers : BotTask
    {
        private NewVillage newVillage;
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            // Check if the account has enough culture points
            if (acc.AccInfo.CulturePoints.MaxVillages <= acc.AccInfo.CulturePoints.VillageCount)
            {
                this.Vill.Expansion.ExpensionAvailable = true;
                return TaskRes.Executed;
            }

            this.Vill.Expansion.ExpensionAvailable = false;

            //https://low4.ttwars.com/build.php?id=39&tt=2&kid=7274&a=6
            //https://low4.ttwars.com/build.php?id=39&tt=2&kid=7272&a=6
            if (acc.NewVillages.Locations.Count == 0)
            {
                if (acc.NewVillages.AutoFindVillages) // Find new village to settle
                {
                    TaskExecutor.AddTaskIfNotExists(acc, new FindVillageToSettle() {
                        ExecuteAt = DateTime.MinValue.AddHours(10)
                    });
                    this.NextExecute = DateTime.MinValue.AddHours(11);
                }
                else
                {
                    acc.Tasks.Remove(this);
                }
                return TaskRes.Executed;
            }

            newVillage = acc.NewVillages.Locations.FirstOrDefault();
            //acc.NewVillage.NewVillages.Remove(coords); //remove it after settling and changing the vill name??
            string kid = MapHelper.KidFromCoordinates(newVillage.coordinates, acc).ToString();
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id=39&tt=2&a=6&kid={kid}");

            //TODO: check if enough resources!!
            newVillage.SettlersSent = true;
            var button = acc.Wb.Html.GetElementbyId("btn_ok");

            if (button != null)
            {
                wb.ExecuteScript($"document.getElementById('btn_ok').click()"); //Click send button
            }

            return TaskRes.Executed;
        }
    }
}
