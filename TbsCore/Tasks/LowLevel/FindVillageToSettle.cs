using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;
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
                    var ajaxToken = DriverHelper.GetJsObj<string>(acc, "ajaxToken");

                    var req = new RestRequest
                    {
                        Resource = "/ajax.php?cmd=mapPositionData",
                        Method = Method.POST,
                    };

                    req.AddParameter("cmd", "mapPositionData");
                    req.AddParameter("data[x]", mainVill.Coordinates.x.ToString());
                    req.AddParameter("data[y]", mainVill.Coordinates.y.ToString());
                    req.AddParameter("data[zoomLevel]", "3");
                    req.AddParameter("ajaxToken", ajaxToken);

                    var resString = HttpHelper.SendPostReq(acc, req);

                    var root = JsonConvert.DeserializeObject<MapPositionDataT4_4.Root>(resString);
                    if (root.response.error) throw new Exception("Unable to get T4.4 map position data!\n" + root.response.error);

                    var mapTiles = root.response.data.tiles.Select(x => x.GetMapTile()).ToList();
                    closesCoords = GetClosestCoordinates(acc, mapTiles);
                    break;

                case Classificator.ServerVersionEnum.T4_5:
                    var bearerToken = DriverHelper.GetBearerToken(acc);

                    var reqMapInfo = new RestRequest
                    {
                        Resource = "/api/v1/ajax/mapInfo",
                        Method = Method.POST,
                        RequestFormat = DataFormat.Json
                    };
                    reqMapInfo.AddHeader("authorization", $"Bearer {bearerToken}");
                    reqMapInfo.AddHeader("content-type", "application/json; charset=UTF-8");
                    reqMapInfo.AddJsonBody(GenerateMapInfo(mainVill.Coordinates));

                    var mapInfoRes = HttpHelper.SendPostReq(acc, reqMapInfo);

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

                    var reqMapPosition = new RestRequest
                    {
                        Resource = "/api/v1/ajax/mapPositionData",
                        Method = Method.POST,
                        RequestFormat = DataFormat.Json
                    };
                    reqMapPosition.AddHeader("authorization", $"Bearer {bearerToken}");
                    reqMapPosition.AddHeader("content-type", "application/json; charset=UTF-8");
                    reqMapPosition.AddJsonBody(mapPosition);
                    //reqMapPosition.AddParameter("application/json", , ParameterType.RequestBody);
                    var mapPositionRes = HttpHelper.SendPostReq(acc, reqMapPosition);
                    var mapPositionData = JsonConvert.DeserializeObject<MapPositionDataT4_5>(mapPositionRes);

                    var mapTilesT45 = mapPositionData.tiles.Select(x => x.GetMapTile()).ToList();
                    closesCoords = GetClosestCoordinates(acc, mapTilesT45);
                    break;
            }

            if (closesCoords == null) return TaskRes.Retry;

            acc.NewVillages.Locations.Add(new NewVillage()
            {
                Coordinates = closesCoords,
                Name = NewVillageHelper.GenerateName(acc),
            });

            return TaskRes.Executed;
        }

        private Coordinates GetClosestCoordinates(Account acc, List<MapTile> tiles)
        {
            var mainVill = AccountHelper.GetMainVillage(acc);
            var closesCoords = new Coordinates();
            var closest = 1000.0;
            foreach (var tile in tiles)
            {
                if (tile.Title == null || !tile.Title.StartsWith("{k.vt}")) continue;

                // Check if village type meets criteria
                if (acc.NewVillages.Types.Count != 0)
                {
                    var num = (int)Parser.RemoveNonNumeric(tile.Title.Split('f')[1]);
                    var type = (Classificator.VillTypeEnum)(num);
                    if (!acc.NewVillages.Types.Any(x => x == type)) continue;
                }

                var distance = MapHelper.CalculateDistance(acc, mainVill.Coordinates, tile.Coordinates);
                if (distance < closest)
                {
                    closest = distance;
                    closesCoords = tile.Coordinates;
                }
            }
            return closesCoords;
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
    }
}
