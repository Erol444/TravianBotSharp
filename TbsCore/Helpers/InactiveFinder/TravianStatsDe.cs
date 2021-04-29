using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using RestSharp;

using TbsCore.Models;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Helpers.InactiveFinder
{
    public static class TravianStatsDe
    {
        /// <summary>
        /// world code for our world from travianstats.de
        /// </summary>
        private async static Task<string> GetServerCode(Account acc)
        {
            // get serverUrl without https://
            var url = (new UriBuilder(acc.AccInfo.ServerUrl)).Host;

            var Client = acc.Wb.RestClient;
            //request to travaianstats.de
            Client.BaseUrl = new Uri("https://travianstats.de");
            var request = new RestRequest();

            var response = await Client.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) return null;

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(response.Content);
            // use this form to search code of our server

            // find our server
            return doc.GetElementbyId("welt")
                .Descendants()
                .FirstOrDefault(x => x.InnerText.Contains(url))?
                .GetAttributeValue("value", "");
        }

        /// <summary>
        /// Get all farm from travianstats.de
        /// </summary>
        /// <param name="serverCode"></param>
        /// <returns></returns>
        public async static Task<List<InactiveFarm>> GetFarms(Account acc, Coordinates Coords, int Distance)
        {
            var ServerCode = await GetServerCode(acc);
            if (string.IsNullOrEmpty(ServerCode)) return null;

            var Client = acc.Wb.RestClient;
            Client.BaseUrl = new Uri("https://travianstats.de");
            var request = new RestRequest($"?m=inactive_finder&w={ServerCode}", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Cookie", $"tcn_world={ServerCode}");
            request.AddParameter("m", "inactivefinder");
            request.AddParameter("w", ServerCode);
            request.AddParameter("x", $"{Coords.x}");
            request.AddParameter("y", $"{Coords.y}");
            request.AddParameter("distance", $"{Distance}");

            var response = await Client.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) return null;

            if (response.Content.Contains("Nothing found")) return null;

            var doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(response.Content);

            // table
            var table = doc.DocumentNode.SelectNodes("//table[@id='myTable']//tbody") // they use myTable for naming their table ?_?
                        .Descendants("tr")
                        .Where(tr => tr.Elements("td").Count() > 1)
                        .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim().Replace("\t", "").Replace("\n", "")).ToList())
                        .ToList();

            var result = new List<InactiveFarm>();
            foreach (var row in table)
            {
                try
                {
                    result.Add(new InactiveFarm()
                    {
                        //status = row[0]
                        distance = int.Parse(row[1]),
                        coord = MapParser.GetCoordinates(row[2]),
                        namePlayer = row[3],
                        nameAlly = row[4],
                        nameVill = row[5],
                        population = int.Parse(row[6])
                        //functions = row[7]
                    });
                }
                catch (Exception) { }
            }

            return result;
        }
    }
}