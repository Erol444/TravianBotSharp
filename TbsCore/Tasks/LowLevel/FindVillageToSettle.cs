using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.MapModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class FindVillageToSettle : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var htmlDoc = acc.Wb.Html;
            var wb = acc.Wb.Driver;

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/karte.php");

            var mainVill = AccountHelper.GetMainVillage(acc);

            var ajaxToken = HttpHelper.GetAjaxToken(wb);
            var values = new Dictionary<string, string>
            {
                {"cmd", "mapPositionData"},
                {"data[x]", mainVill.Coordinates.x.ToString()},
                {"data[y]", mainVill.Coordinates.y.ToString()},
                {"data[zoomLevel]", "3"},
                {"ajaxToken", ajaxToken},
            };
            var content = new FormUrlEncodedContent(values);

            var resString = await HttpHelper.SendPostReq(acc, content, "/ajax.php?cmd=mapPositionData");

            var root = JsonConvert.DeserializeObject<MapPositionData.Root>(resString);

            if (!root.response.error)
            {
                var closesCoords = new Coordinates();
                var closest = 1000.0;
                foreach (var tile in root.response.data.tiles)
                {
                    if (tile.c == null || !tile.c.StartsWith("{k.vt}")) continue;

                    // Check if village type meets criteria
                    if (acc.NewVillages.Types.Count != 0)
                    {
                        var num = (int)Parser.RemoveNonNumeric(tile.c.Split('f')[1]);
                        var type = (Classificator.VillTypeEnum)(++num);
                        if (!acc.NewVillages.Types.Any(x => x == type)) continue;
                    }

                    Coordinates coords = new Coordinates()
                    {
                        x = Int32.Parse(tile.x),
                        y = Int32.Parse(tile.y),
                    };
                    var distance = MapHelper.CalculateDistance(acc, mainVill.Coordinates, coords);
                    if (closest > distance)
                    {
                        closest = distance;
                        closesCoords = coords;
                    }
                }

                //acc.Settings.new
                acc.NewVillages.Locations.Add(new Models.VillageModels.NewVillage()
                {
                    coordinates = closesCoords,
                    Name = NewVillageHelper.GenerateName(acc),
                });
            }
            return TaskRes.Executed;
        }
    }
}
