
using System;

namespace TravBotSharp.Files.Models
{
    public class Coordinates : IEquatable<Coordinates>
    {
        public int x { get; set; }
        public int y { get; set; }

        public bool Equals(Coordinates other)
        {
            if (other == null) return false;
            return (other.x == this.x && other.y == this.y);
        }
    }
}
