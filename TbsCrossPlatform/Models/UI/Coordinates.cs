namespace TbsCrossPlatform.Models.UI
{
    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return $"{X}|{Y}";
        }
    }
}