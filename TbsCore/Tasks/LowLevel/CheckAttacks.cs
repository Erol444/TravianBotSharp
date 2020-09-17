using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class CheckAttacks : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?gid=16&tt=1&filter=1&subfilters=1");

            //(var attackWithHero, var count)= TroopsMovementParser.FullAttackWithHero(htmlDoc);

            //if (attackWithHero)
            //{
            //    IoHelper.AlertUser(vill.Name + " is under attack!");
            //}

            //if(count > 0)
            //{
            //    this.NextExecute = DateTime.Now.AddMinutes(1);
            //}


            return TaskRes.Executed;
        }
    }
}
