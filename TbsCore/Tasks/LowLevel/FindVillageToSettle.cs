using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TbsCore.Models.MapModels;
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
            var wb = acc.Wb.Driver;

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/karte.php");

            Coordinates closesCoords = null;

            var mainVill = AccountHelper.GetMainVillage(acc);
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    var ajaxToken = HttpHelper.GetAjaxToken(acc);
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

                    var root = JsonConvert.DeserializeObject<MapPositionDataT4_4.Root>(resString);
                    closesCoords = HandleT4_4Data(acc, root);
                    break;

                case Classificator.ServerVersionEnum.T4_5:
                    var mapInfo = new SendMapInfoT4_5();
                    mapInfo.zoomLevel = 1;
                    //mapInfo.data.Add()
                    /*
                    T4.5 Endpoint:
                    /api/v1/ajax/mapInfo
                    Data: {"data":[{"position":{"x0":180,"y0":-320,"x1":189,"y1":-311}},{"position":{"x0":180,"y0":-330,"x1":189,"y1":-321}},{"position":{"x0":180,"y0":-340,"x1":189,"y1":-331}}],"zoomLevel":1}
                    Response: useless.

                    /api/v1/ajax/mapPositionData
                    Data: {"data":{"x":188,"y":-374,"zoomLevel":1,"ignorePositions":[]}}
                    Response: MapPositionDataT4_5.Root
                    */
                    break;
            }




            if (closesCoords == null) return TaskRes.Retry;

            acc.NewVillages.Locations.Add(new Models.VillageModels.NewVillage()
            {
                coordinates = closesCoords,
                Name = NewVillageHelper.GenerateName(acc),
            });

            return TaskRes.Executed;
        }

        private SendMapInfoT4_5 GenerateMapInfo(Coordinates coords)
        {
            var ret = new SendMapInfoT4_5();
            ret.zoomLevel = 3;
            ret.data = new List<Datum>();

            //ret.

            return ret;
        }
        private Coordinates HandleT4_4Data(Account acc, MapPositionDataT4_4.Root root)
        {
            var mainVill = AccountHelper.GetMainVillage(acc);

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
                return closesCoords;
            }
            return null;
        }
    }
}
