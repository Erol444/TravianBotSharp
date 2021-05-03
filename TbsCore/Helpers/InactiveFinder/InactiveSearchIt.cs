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
    public class InactiveSearchIt
    {
        public async static Task<bool> CheckServerSupport(Account acc, string ServerUrl)
        {
            var Client = acc.Wb.RestClient;
            //request to inactivesearch.it
            Client.BaseUrl = new Uri($"https://www.inactivesearch.it/inactives/{ServerUrl}");
            var request = new RestRequest();

            var response = await Client.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) return false;
            if (response.Content.Contains("is not active")) return false;

            return true;
        }

        /// <summary>
        /// Get all farm from travianstats.de
        /// </summary>
        /// <returns></returns>
        public async static Task<List<InactiveFarm>> GetFarms(Account acc, Coordinates Coords, int Distance)
        {
            var serverUrl = acc.AccInfo.ServerUrl;
            // get serverUrl without https://
            var url = (new UriBuilder(serverUrl)).Host;

            if (!(await CheckServerSupport(acc, url))) return null;

            var Client = acc.Wb.RestClient;

            var moreFarm = true;
            var index = 1;
            var result = new List<InactiveFarm>();

            while (moreFarm)
            {
                Client.BaseUrl = new Uri($"https://www.inactivesearch.it/inactives/{url}?c={Coords.x}|{Coords.y}&page={index}");
                var request = new RestRequest(Method.GET);
                var response = await Client.ExecuteAsync(request);

                if (response.StatusCode != HttpStatusCode.OK) return null;

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(response.Content);

                // table
                var table = doc.DocumentNode.SelectNodes("//table[@class='table table-condensed table-inactives table-shadow']//tbody") // they use myTable for naming their table ?_?
                            .Descendants("tr")
                            .Where(tr => tr.Elements("td").Count() > 1)
                            .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim().Replace("\t", "").Replace("\n", "")).ToList())
                            .ToList();

                if (table.Count < 1)
                {
                    moreFarm = false;
                    break;
                }

                foreach (var row in table)
                {
                    try
                    {
                        var intDistance = int.Parse(row[0].Split('.').First());
                        if (intDistance > Distance)
                        {
                            moreFarm = false;
                            break;
                        }
                        result.Add(new InactiveFarm()
                        {
                            distance = intDistance,
                            coord = MapParser.GetCoordinates(row[1]),
                            nameVill = row[2],
                            // row[3] hide village button
                            // row[4] attack button
                            population = int.Parse(row[5].Split('+').First().Split('-').First()),
                            // row[6], [7], [8], [9] population previous day
                            // row[10] icon tribe
                            namePlayer = row[11],
                            nameAlly = row[12],
                        });
                    }
                    catch (Exception) { }
                }

                index++;
            }

            return result;
        }
    }
}