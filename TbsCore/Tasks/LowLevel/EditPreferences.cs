using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class EditPreferences : BotTask
    {
        /// <summary>
        /// Disable contextual help
        /// </summary>
        public bool? ContextualHelp { get; set; }

        /// <summary>
        /// Troop movements per page in rally point
        /// </summary>
        public int? TroopsPerPage { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            await VersionHelper.Navigate(acc, "/options.php", "/options");

            if (ContextualHelp != null)
                await DriverHelper.CheckById(acc, "v13", ContextualHelp ?? true);

            if (TroopsPerPage != null)
                await DriverHelper.WriteById(acc, "troopMovementsPerPage", TroopsPerPage ?? 10);

            var acceptButton = acc.Wb.Html.DocumentNode
                .Descendants("div")
                .First(x => x.HasClass("submitButtonContainer"))
                .Descendants("button")
                .First();

            await DriverHelper.ClickById(acc, acceptButton.Id);

            return TaskRes.Executed;
        }
    }
}