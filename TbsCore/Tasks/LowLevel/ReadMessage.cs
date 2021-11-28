using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;

namespace TbsCore.Tasks.LowLevel
{
    public class ReadMessage : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            while (true)
            {
                await NavigationHelper.MainNavigate(acc, NavigationHelper.MainNavigationButton.Messages);
                var msg = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("messageStatusUnread"));
                if (msg != null)
                {
                    var url = msg.ParentNode.GetAttributeValue("href", "").Replace("amp;", "");
                    await acc.Wb.Navigate(acc.AccInfo.ServerUrl + "/" + url);
                    await Task.Delay(AccountHelper.Delay(acc) * 5);
                }
                else return TaskRes.Executed;
            }
        }
    }
}