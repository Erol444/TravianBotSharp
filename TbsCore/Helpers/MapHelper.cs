using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.TroopsModels;
using TbsCore.Parsers;

namespace TbsCore.Helpers
{
    public static class MapHelper
    {
        public static List<object> jsonToVillTypes(string json)
        {
            return null;
            //from cmd=mapPositionData json get villtypenum's and corresponding coordinates
        }

        /// <summary>
        /// Send raw HTTP request to the server and request the map tiles around the coords. This mimics browser on the map page.
        /// </summary>
        public static async Task<List<MapTile>> GetMapTiles(Account acc, Coordinates coords)
        {
            var bearerToken = DriverHelper.GetBearerToken(acc);

            var reqMapInfo = new RestRequest
            {
                Resource = "/api/v1/map/info",
                Method = Method.Post,
                RequestFormat = DataFormat.Json
            };
            reqMapInfo.AddHeader("authorization", $"Bearer {bearerToken}");
            reqMapInfo.AddHeader("content-type", "application/json; charset=UTF-8");
            reqMapInfo.AddJsonBody(GenerateMapInfo(coords));

            var mapInfoRes = await HttpHelper.SendPostReqAsync(acc, reqMapInfo);

            var mapPosition = new SendMapPositionT4_5.Root()
            {
                data = new SendMapPositionT4_5.Data()
                {
                    x = coords.x,
                    y = coords.y,
                    zoomLevel = 3,
                    ignorePositions = new List<object>()
                }
            };

            var reqMapPosition = new RestRequest
            {
                Resource = "/api/v1/map/position",
                Method = Method.Post,
                RequestFormat = DataFormat.Json
            };
            reqMapPosition.AddHeader("authorization", $"Bearer {bearerToken}");
            reqMapPosition.AddHeader("content-type", "application/json; charset=UTF-8");
            reqMapPosition.AddJsonBody(mapPosition);

            var mapPositionRes = await HttpHelper.SendPostReqAsync(acc, reqMapPosition);
            var mapPositionData = JsonConvert.DeserializeObject<MapPositionDataT4_5>(mapPositionRes);
            return mapPositionData.tiles.Select(x => x.GetMapTile()).ToList();
        }

        /// <summary>
        /// Sends HTTP request to the server and gets number of animals inside the oasis
        /// </summary>
        public static async Task<TroopsBase> GetOasisAnimals(Account acc, Coordinates oasis)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            string html = "";

            var bearerToken = DriverHelper.GetBearerToken(acc);

            var reqMapInfo = new RestRequest
            {
                Resource = "/api/v1/ajax/viewTileDetails",
                Method = Method.Post,
                RequestFormat = DataFormat.Json
            };
            reqMapInfo.AddHeader("authorization", $"Bearer {bearerToken}");
            reqMapInfo.AddHeader("content-type", "application/json; charset=UTF-8");
            reqMapInfo.AddJsonBody(oasis);

            var tileDetails = await HttpHelper.SendPostReqAsync(acc, reqMapInfo);

            var tile = JsonConvert.DeserializeObject<TileDetailsT4_5>(tileDetails);
            html = WebUtility.HtmlDecode(tile.html);

            htmlDoc.LoadHtml(html);
            return TroopsParser.GetOasisAnimals(htmlDoc);
        }

        #region Private helpers

        private static SendMapInfoT4_5.Root GenerateMapInfo(Coordinates coords)
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

        #endregion Private helpers
    }
}