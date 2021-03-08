using System.Threading.Tasks;

using TbsCore.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class CheckSitter : BotTask
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        public override async Task<TaskRes> Execute(Account acc)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            // sitter cannot access to auction
            // so we check auction button
            // since auction button is shown in all page of game
            // we just make sure call this after login task and it will be fine

            var auction = acc.Wb.Html.DocumentNode.SelectSingleNode("//a[contains(@class,'auction')]");
            if (auction != null && auction.Attributes["class"].Value.Contains("disable"))
            {
                acc.AccInfo.Sitter = true;
                return TaskRes.Executed;
            }
            acc.AccInfo.Sitter = true;

            return TaskRes.Executed;
        }
    }
}