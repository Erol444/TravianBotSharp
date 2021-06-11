using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.LowLevel
{
    public class TTWarsBuyAdventure : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/hero.php?t=3");

            var button = acc.Wb.Html.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("buyAdventure"));
            if (button == null)
            {
                acc.Logger.Warning("No button 'Buy' button found, perhaps you are not on vip ttwars server?");
                return TaskRes.Executed;
            }
            acc.Wb.ExecuteScript($"document.getElementById('{button.Id}').click()"); //Excgabge resources button
            return TaskRes.Executed;
        }
    }
}