using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    /// <summary>
    /// For calculating how what is the transit speed (factor) of this server. On normal (x1) servers,
    /// this factor should be 1.
    /// </summary>
    public class GetServerSpeed : BotTask
    {
        //Array starts with level 0 woodcutter
        private readonly int[] WoodcutterProduction = { 3, 7, 13, 21, 31, 46, 70, 98, 140, 203, 280, 392, 525, 693, 889, 1120, 1400, 1820, 2240, 2800, 3430 };

        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/production.php?t=1");

            var table = acc.Wb.Html.DocumentNode.Descendants("table").FirstOrDefault(x => x.HasClass("row_table_data"));
            if (table == null)
            {
                this.ErrorMessage = "Production table not found.";
                return TaskRes.Executed;
            }
            var firstRow = table.ChildNodes.First(x => x.Name == "tbody").ChildNodes.First(x => x.Name == "tr");

            var levelCell = firstRow.ChildNodes.First(x => x.Name == "td");
            var level = (int)Parser.RemoveNonNumeric(levelCell.InnerText);
            if (level > 20)
            {
                this.ErrorMessage = "Woodcutter level above 20. Impossible.";
                return TaskRes.Executed;
            }

            var productionCell = firstRow.ChildNodes.First(x => x.HasClass("numberCell"));
            var production = Parser.RemoveNonNumeric(productionCell.InnerText);

            acc.AccInfo.ServerSpeed = (int)production / WoodcutterProduction[level];
            return TaskRes.Executed;
        }
    }
}
