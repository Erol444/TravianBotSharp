namespace TbsCore.Models.MapModels
{
    public class MapTile
    {
        public Coordinates Coordinates { get; set; }
        public string Title { get; set; }
    }

    /// <summary>
    /// The response from T4.5 when sending request to /viewTileDetails
    /// </summary>
    public class TileDetailsT4_5
    {
        public string html { get; set; }
    }

    /// <summary>
    /// For T4.4
    /// </summary>
    public class TilesDataT4_4
    {
        public string html { get; set; }
    }

    public class TilesResponseT4_4
    {
        public bool error { get; set; }
        public object errorMsg { get; set; }
        public TilesDataT4_4 data { get; set; }
    }

    public class TileDetailsT4_4
    {
        public TilesResponseT4_4 response { get; set; }
    }
}
