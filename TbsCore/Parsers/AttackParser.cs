using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Helpers;
using TbsCore.Models.AttackModels;
using TbsCore.Models.ResourceModels;

namespace TbsCore.Parsers
{
    public static class AttackParser
    {
        //map_details
        public static List<VillageReport> ParseVillageReports(HtmlAgilityPack.HtmlDocument html)
        {
            var ret = new List<VillageReport>();

            var reports = html.GetElementbyId("troop_info").Descendants("tr");
            foreach (var report in reports)
            {

                string reportType = report.Descendants("img")
                    .First(x => x.HasClass("iReport"))
                    .GetClasses()
                    .First(x => x != "iReport");
                var a = report.Descendants("a").First();

                ret.Add(new VillageReport
                {
                    Type = AttackHelper.GetReportType(reportType),
                    Id = a.GetAttributeValue("href", ""),
                    ReportTime = a.InnerText
                });
            }
            return ret;
        }

        internal static AttackReport ParseAttack(HtmlAgilityPack.HtmlDocument html)
        {
            var report = new AttackReport();
            var content = html.GetElementbyId("content");

            // TODO parse: type of attack, attacker (nickname, id), Attacker Village (id, coords, name), Deffender (nickname, id), Deffending village (id, coords, name)
            // Attacker troops, killed, tribe, additional information (ram&cata report)

            // Deffender List<troops, tribe, killed>
            var attacker = content.Descendants("div").First(x => x.HasClass("attacker"));
            var deffender = content.Descendants("div").First(x => x.HasClass("defender"));

            report.Deffender = ParseHeader(deffender);
            report.Attacker = ParseHeader(attacker);

            var infos = content.Descendants("table").Where(x => x.HasClass("additionalInformation")).ToList();
            foreach (var info in infos)
            {
                var res = info.Descendants("div").FirstOrDefault(x => x.HasClass("res"));
                if (res != null && res.ChildNodes.Count == 4) report.Resources = ParseBountyRes(res);

                var cranny = info.Descendants("i").FirstOrDefault(x => x.HasClass("g23Icon"));
                if (cranny != null) report.CrannySize = (int)Parser.RemoveNonNumeric(cranny.ParentNode.InnerText);
            }
            return report;
        }

        private static CombatParticipant ParseHeader(HtmlNode header)
        {
            var headline = header.Descendants("div").First(x => x.HasClass("troopHeadline")).ChildNodes.First(x => x.Name == "div");
            var ahref = headline.ChildNodes.Where(x => x.Name == "a").ToArray();
            var ret = new CombatParticipant();

            ret.Username = ahref[0].InnerText;
            ret.UserId = (int)Parser.RemoveNonNumeric(ahref[0].GetAttributeValue("href", ""));
            ret.VillageName = ahref[1].InnerText;
            ret.VillageId = MapParser.GetKarteHref(headline) ?? 0;
            return ret;
        }

        private static Resources ParseBountyRes(HtmlAgilityPack.HtmlNode node)
        {
            var resNodes = node.ChildNodes;
            return new Resources
            {
                Wood = Parser.RemoveNonNumeric(resNodes[0].InnerText),
                Clay = Parser.RemoveNonNumeric(resNodes[1].InnerText),
                Iron = Parser.RemoveNonNumeric(resNodes[2].InnerText),
                Crop = Parser.RemoveNonNumeric(resNodes[3].InnerText)
            };
        }
    }
}
