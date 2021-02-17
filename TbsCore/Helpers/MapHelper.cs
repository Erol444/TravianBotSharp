using System;
using System.Collections.Generic;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;

namespace TravBotSharp.Files.Helpers
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
    }
}
