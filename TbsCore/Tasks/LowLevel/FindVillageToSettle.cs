using Newtonsoft.Json;
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
                    //var mapInfo = GenerateMapInfo(mainVill.Coordinates);
                    //var contentMap = new StringContent(JsonConvert.SerializeObject(mapInfo));
                    //var mapInfoRes = await HttpHelper.SendPostReq(acc, contentMap, "/api/v1/ajax/mapInfo");

                    var mapPosition = new SendMapPositionT4_5.Root()
                    {
                        data = new SendMapPositionT4_5.Data()
                        {
                            x = mainVill.Coordinates.x,
                            y = mainVill.Coordinates.y,
                            zoomLevel = 3,
                            ignorePositions = new List<object>()
                        }
                    };
                    var contentPosition = new StringContent(JsonConvert.SerializeObject(mapPosition));
                    var mapPositionRes = await HttpHelper.SendPostReq(acc, contentPosition, "/api/v1/ajax/mapPositionData");
                    var mapPositionData = JsonConvert.DeserializeObject<MapPositionDataT4_5>(mapPositionRes);

                    closesCoords = HandleT4_5Data(acc, mapPositionData);
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

        private Coordinates HandleT4_5Data(Account acc, MapPositionDataT4_5 mapPositionData)
        {
            return null;
        }

        private SendMapInfoT4_5.Root GenerateMapInfo(Coordinates coords)
        {
            int startX = coords.x - (coords.x % 100);
            int startY = coords.y - (coords.y % 100);
            var ret = new SendMapInfoT4_5.Root();
            ret.zoomLevel = 3;
            ret.data = new List<SendMapInfoT4_5.Datum>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    ret.data.Add(new SendMapInfoT4_5.Datum()
                    {
                        position = new SendMapInfoT4_5.Position()
                        {
                            x0 = x * 100 + startX,
                            y0 = y * 100 + startY,
                            x1 = (x * 100 + startX) + 99,
                            y1 = (y * 100 + startY) + 99
                        }
                    });
                }
            }

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
