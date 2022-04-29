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
            StopFlag = false;
            do
            {
                if (StopFlag) return TaskRes.Executed;

                {
                    var result = await Update(acc);
                    if (!result) return TaskRes.Executed;
                }

                {
                    var result = await NavigationHelper.MainNavigate(acc, NavigationHelper.MainNavigationButton.Messages);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) return TaskRes.Executed;
                }

                var msg = acc.Wb.Html.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass("messageStatusUnread"));
                if (msg == null)
                {
                    acc.Logger.Information("Cannot found any unread message");
                    return TaskRes.Executed;
                }

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

                if (StopFlag) return TaskRes.Executed;
                await AccountHelper.DelayWait(acc, 5);
            } while (true);
        }
    }
}