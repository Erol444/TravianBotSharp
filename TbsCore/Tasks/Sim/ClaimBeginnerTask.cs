using OpenQA.Selenium;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.SideBarModels;

namespace TbsCore.Tasks.Sim
{
    public class ClaimBeginnerTask : BotTask
    {
        public Quest QuestToClaim { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            {
                var result = await Update(acc);
                if (!result) return TaskRes.Executed;
            }

            var mentorTaskNode = acc.Wb.Html.GetElementbyId("mentorTaskList");
            if (mentorTaskNode == null)
            {
                acc.Logger.Warning("Cannot find mentor.");
                return TaskRes.Executed;
            }
            var questNode = mentorTaskNode.Descendants().FirstOrDefault(x => x.GetAttributeValue("data-questid", "").Equals($"{QuestToClaim.Id}"));
            if (questNode == null)
            {
                acc.Logger.Warning("Cannot find quest.");
                return TaskRes.Executed;
            }
            var questElement = acc.Wb.Driver.FindElement(By.XPath(questNode.XPath));
            questElement.Click();

            {
                var result = await Update(acc);
                if (!result) return TaskRes.Executed;
            }

            var buttonNode = acc.Wb.Html.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("questButtonNext"));
            if (buttonNode == null)
            {
                acc.Logger.Warning("Cannot find Next button");
                return TaskRes.Executed;
            }
            var buttonElement = acc.Wb.Driver.FindElement(By.XPath(buttonNode.XPath));
            buttonElement.Click();

            {
                var result = await Update(acc);
                if (!result) return TaskRes.Executed;
            }

            return TaskRes.Executed;
        }
    }
}