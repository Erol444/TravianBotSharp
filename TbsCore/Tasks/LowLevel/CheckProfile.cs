using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.AttackModels;
using TbsCore.Parsers;

namespace TbsCore.Tasks.LowLevel
{
    public class CheckProfile : BotTask
    {
        public int UserId { get; set; }
        public TravianUser Profile { get; } = new TravianUser() {
            Villages = new List<TravianVillage>()
        };

        /// <summary>
        /// Only used by higher level tasks
        /// </summary>
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/spieler.php?uid={UserId}");

            var vills = acc.Wb.Html.GetElementbyId("villages")
                .ChildNodes
                .First(x => x.Name == "tbody")
                .Descendants("tr");

            foreach (var vill in vills)
            {
                var villProfile = new TravianVillage();
                var nameNode = vill.ChildNodes.First(x => x.HasClass("name"));

                villProfile.Capital = nameNode.Descendants("span").Any(x => x.HasClass("mainVillage"));
                villProfile.Id = MapParser.GetKarteHref(nameNode) ?? 0;
                villProfile.Name = nameNode.InnerText;
                villProfile.Population = (int)Parser.RemoveNonNumeric(vill.Descendants("td").First(x => x.HasClass("inhabitants")).InnerHtml);
                villProfile.Coordinates = MapParser.GetCoordinates(vill);

                Profile.Villages.Add(villProfile);
            }

            return TaskRes.Executed;
        }
    }
}