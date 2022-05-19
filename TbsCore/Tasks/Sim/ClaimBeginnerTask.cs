using OpenQA.Selenium;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.SideBarModels;

namespace TbsCore.Tasks.Sim
{
    public class ClaimBeginnerTask : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            do
            {
                var quest = acc.Quests.Quests.FirstOrDefault(x => x.finished);
                if (quest == null) return TaskRes.Executed;

                {
                    var result = await Update(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) return TaskRes.Executed;
                }

                var mentorTaskNode = acc.Wb.Html.GetElementbyId("mentorTaskList");
                if (mentorTaskNode == null)
                {
                    Retry(acc, "Cannot find mentor.");
                    if (StopFlag) return TaskRes.Executed;
                    continue;
                }
                var questNode = mentorTaskNode.Descendants().FirstOrDefault(x => x.GetAttributeValue("data-questid", "").Equals($"{quest.Id}"));
                if (questNode == null)
                {
                    Retry(acc, "Cannot find quest.");
                    if (StopFlag) return TaskRes.Executed;
                    continue;
                }
                var questElement = acc.Wb.Driver.FindElement(By.XPath(questNode.XPath));
                questElement.Click();

                {
                    await Task.Delay(1000);
                    var result = await Update(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) return TaskRes.Executed;
                }

                var buttonNode = acc.Wb.Html.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("questButtonNext"));
                if (buttonNode == null)
                {
                    Retry(acc, "Cannot find Get reward button");
                    if (StopFlag) return TaskRes.Executed;
                    continue;
                }
                var buttonElement = acc.Wb.Driver.FindElement(By.XPath(buttonNode.XPath));
                buttonElement.Click();

                {
                    var result = await Update(acc);
                    if (StopFlag) return TaskRes.Executed;
                    if (!result) return TaskRes.Executed;
                }
            }
            while (true);
        }
    }
}