using System;
using TbsCore.Models.AccModels;

namespace TbsCore.Models.MapModels
{
    public class Coordinates : IEquatable<Coordinates>
    {
        public Coordinates()
        {
        }

        public Coordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Coordinates(Account acc, int kid)
        {
            var size = acc.AccInfo.MapSize;
            kid--;
            this.y = size - (kid / (size * 2 + 1));
            this.x = (kid % (size * 2 + 1)) - size;
        }

        public int x { get; set; }
        public int y { get; set; }

        public bool Equals(Coordinates other)
        {
            if (other == null) return false;
            return other.x == x && other.y == y;
        }

        // Used in cmd=mapPositionData, gets the map JSON where
        public int GetKid(Account acc)
        {
            return 1 + ((acc.AccInfo.MapSize - this.y) * (acc.AccInfo.MapSize * 2 + 1)) + acc.AccInfo.MapSize + this.x;
        }

        /// <summary>
        /// Calculate distance between two coordinates. This function takes into the account the map size.
        /// </summary>
        public float CalculateDistance(Account acc, Coordinates coords)
        {
            var size = acc.AccInfo.MapSize;
            var xDiff = Math.Abs(this.x - coords.x);
            var yDiff = Math.Abs(this.y - coords.y);
            if (xDiff > size) xDiff = 2 * size - xDiff;
            if (yDiff > size) yDiff = 2 * size - yDiff;
            var distance = Math.Sqrt(xDiff * xDiff + yDiff * yDiff); //Pitagoras theorem
            return (float)distance;
        }

        public override string ToString() => $"({x}|{y})";
    }
}