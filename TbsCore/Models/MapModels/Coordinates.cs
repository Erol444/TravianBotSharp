using System;

namespace TbsCore.Models.MapModels
{
    public class Coordinates : IEquatable<Coordinates>
    {
        public Coordinates() { }
        public Coordinates(int x, int y) 
        {
            this.x = x;
            this.y = y;
        }

        public int x { get; set; }
        public int y { get; set; }

        public bool Equals(Coordinates other)
        {
            if (other == null) return false;
            return other.x == x && other.y == y;
        }

        public override string ToString() => $"({x}/{y})";
    }
}