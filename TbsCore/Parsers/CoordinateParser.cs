using TravBotSharp.Files.Models;

namespace TravBotSharp.Files.Parsers
{
    public static class CoordinateParser
    {
        public static Coordinates GetCoordinates(string str)
        {
            var coords = str.Replace("(", "").Replace(")", "").Trim().Split('|');
            return new Coordinates() { x = (int)Parser.ParseNum(coords[0]), y = (int)Parser.ParseNum(coords[1]) };
        }
    }
}
