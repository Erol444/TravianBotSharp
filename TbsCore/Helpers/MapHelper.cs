using Discord.Net.Queue;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.VillageModels;
using TbsCore.TravianData;
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

        //Used in cmd=mapPositionData, gets the map JSON where

        public static int KidFromCoordinates(Coordinates coords, Account acc)
        {
            return 1 + ((acc.AccInfo.MapSize - coords.y) * (acc.AccInfo.MapSize * 2 + 1)) + acc.AccInfo.MapSize + coords.x;
        }

        public static Coordinates CoordinatesFromKid(int? kid, Account acc)
        {
            if (kid == null) return null;
            return CoordinatesFromKid(kid ?? 0, acc);
        }

        public static Coordinates CoordinatesFromKid(int kid, Account acc)
        {
            var size = acc.AccInfo.MapSize;
            kid--;
            var y = size - (kid / (size * 2 + 1));
            var x = (kid % (size * 2 + 1)) - size;
            return new Coordinates()
            {
                x = x,
                y = y
            };
        }

        /// <summary>
        /// Calculate distance between two coordinates. This function takes into the account the map size.
        /// </summary>
        public static float CalculateDistance(Account acc, Coordinates coord1, Coordinates coord2)
        {
            var size = acc.AccInfo.MapSize;
            var xDiff = Math.Abs(coord1.x - coord2.x);
            var yDiff = Math.Abs(coord1.y - coord2.y);
            if (xDiff > size) xDiff = 2 * size - xDiff;
            if (yDiff > size) yDiff = 2 * size - yDiff;
            var distance = Math.Sqrt(xDiff * xDiff + yDiff * yDiff); //Pitagoras theorem
            return (float)distance;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <param name="root"></param>
        /// <returns> 0 = same distance  1 = this farther than other -1 = other farther than this</returns>
        public static int Compare(Account acc, Coordinates root, Coordinates a, Coordinates b)
        {
            var distanceA = CalculateDistance(acc, root, a);
            var distanceB = CalculateDistance(acc, root, b);
            if (distanceA - distanceB < 0.1) return 0;
            if (distanceA < distanceB) return -1;
            return 1;
        }

        /// <summary>
        /// Send raw HTTP request to the server and request the map tiles around the coords. This mimics browser on the map page.
        /// </summary>
        public static List<MapTile> GetMapTiles(Account acc, Coordinates coords)
        {
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    var ajaxToken = DriverHelper.GetJsObj<string>(acc, "ajaxToken");

                    var req = new RestSharp.RestRequest
                    {
                        Resource = "/ajax.php?cmd=mapPositionData",
                        Method = Method.POST,
                    };

                    req.AddParameter("cmd", "mapPositionData");
                    req.AddParameter("data[x]", coords.x.ToString());
                    req.AddParameter("data[y]", coords.y.ToString());
                    req.AddParameter("data[zoomLevel]", "3");
                    req.AddParameter("ajaxToken", ajaxToken);

                    var resString = HttpHelper.SendPostReq(acc, req);

                    var root = JsonConvert.DeserializeObject<MapPositionDataT4_4.Root>(resString);
                    if (root.response.error) throw new Exception("Unable to get T4.4 map position data!\n" + root.response.error);
                    return root.response.data.tiles.Select(x => x.GetMapTile()).ToList();

                //case Classificator.ServerVersionEnum.T4_5:
                //{
                //    var bearerToken = DriverHelper.GetBearerToken(acc);

                //    var reqMapInfo = new RestSharp.RestRequest
                //    {
                //        Resource = "/api/v1/ajax/mapInfo",
                //        Method = Method.POST,
                //        RequestFormat = DataFormat.Json
                //    };
                //    reqMapInfo.AddHeader("authorization", $"Bearer {bearerToken}");
                //    reqMapInfo.AddHeader("content-type", "application/json; charset=UTF-8");
                //    reqMapInfo.AddJsonBody(GenerateMapInfo(coords));

                //    var mapInfoRes = HttpHelper.SendPostReq(acc, reqMapInfo);

                //    var mapPosition = new SendMapPositionT4_5.Root()
                //    {
                //        data = new SendMapPositionT4_5.Data()
                //        {
                //            x = coords.x,
                //            y = coords.y,
                //            zoomLevel = 3,
                //            ignorePositions = new List<object>()
                //        }
                //    };

                //    var reqMapPosition = new RestSharp.RestRequest
                //    {
                //        Resource = "/api/v1/ajax/mapPositionData",
                //        Method = Method.POST,
                //        RequestFormat = DataFormat.Json
                //    };
                //    reqMapPosition.AddHeader("authorization", $"Bearer {bearerToken}");
                //    reqMapPosition.AddHeader("content-type", "application/json; charset=UTF-8");
                //    reqMapPosition.AddJsonBody(mapPosition);

                //    var mapPositionRes = HttpHelper.SendPostReq(acc, reqMapPosition);
                //    var mapPositionData = JsonConvert.DeserializeObject<MapPositionDataT4_5>(mapPositionRes);
                //    return mapPositionData.tiles.Select(x => x.GetMapTile()).ToList();
                //}
                default:
                    {
                        var bearerToken = DriverHelper.GetBearerToken(acc);

                        var reqMapInfo = new RestSharp.RestRequest
                        {
                            Resource = "/api/v1/map/info",
                            Method = Method.POST,
                            RequestFormat = DataFormat.Json
                        };
                        reqMapInfo.AddHeader("authorization", $"Bearer {bearerToken}");
                        reqMapInfo.AddHeader("content-type", "application/json; charset=UTF-8");
                        reqMapInfo.AddJsonBody(GenerateMapInfo(coords));

                        var mapInfoRes = HttpHelper.SendPostReq(acc, reqMapInfo);

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

                        var reqMapPosition = new RestSharp.RestRequest
                        {
                            Resource = "/api/v1/map/position",
                            Method = Method.POST,
                            RequestFormat = DataFormat.Json
                        };
                        reqMapPosition.AddHeader("authorization", $"Bearer {bearerToken}");
                        reqMapPosition.AddHeader("content-type", "application/json; charset=UTF-8");
                        reqMapPosition.AddJsonBody(mapPosition);

                        var mapPositionRes = HttpHelper.SendPostReq(acc, reqMapPosition);
                        var mapPositionData = JsonConvert.DeserializeObject<MapPositionDataT4_5>(mapPositionRes);
                        return mapPositionData.tiles.Select(x => x.GetMapTile()).ToList();
                    }
            }
        }

        /// <summary>
        /// Sends HTTP request to the server and gets number of animals inside the oasis
        /// </summary>
        public static int[] GetOasisAnimals(Account acc, Coordinates oasis)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            string html = "";

            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    var ajaxToken = DriverHelper.GetJsObj<string>(acc, "ajaxToken");

                    var req = new RestSharp.RestRequest
                    {
                        Resource = "/ajax.php?cmd=viewTileDetails",
                        Method = Method.POST,
                    };

                    req.AddParameter("cmd", "viewTileDetails");
                    req.AddParameter("x", oasis.x.ToString());
                    req.AddParameter("y", oasis.y.ToString());
                    req.AddParameter("ajaxToken", ajaxToken);

                    var resString = HttpHelper.SendPostReq(acc, req);

                    var root = JsonConvert.DeserializeObject<TileDetailsT4_4>(resString);
                    if (root.response.error) throw new Exception("Unable to get T4.4 tile details!\n" + root.response.error);

                    html = WebUtility.HtmlDecode(root.response.data.html);
                    break;

                case Classificator.ServerVersionEnum.T4_5:
                    var bearerToken = DriverHelper.GetBearerToken(acc);

                    var reqMapInfo = new RestSharp.RestRequest
                    {
                        Resource = "/api/v1/ajax/viewTileDetails",
                        Method = Method.POST,
                        RequestFormat = DataFormat.Json
                    };
                    reqMapInfo.AddHeader("authorization", $"Bearer {bearerToken}");
                    reqMapInfo.AddHeader("content-type", "application/json; charset=UTF-8");
                    reqMapInfo.AddJsonBody(oasis);

                    var tileDetails = HttpHelper.SendPostReq(acc, reqMapInfo);

                    var tile = JsonConvert.DeserializeObject<TileDetailsT4_5>(tileDetails);
                    html = WebUtility.HtmlDecode(tile.html);
                    break;
            }

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