using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.Update
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
                    switch (acc.AccInfo.ServerVersion)
                    {
                        case Classificator.ServerVersionEnum.T4_5:
                            await acc.Wb.Navigate(acc.AccInfo.ServerUrl + url);
                            break;

                        case Classificator.ServerVersionEnum.TTwars:
                            await acc.Wb.Navigate(acc.AccInfo.ServerUrl + "/" + url);
                            break;
                    }
                    await Task.Delay(AccountHelper.Delay(acc) * 5);
                }
                else return TaskRes.Executed;
            }
        }
    }
}