namespace TbsCore.Models.ResourceModels
{
    public class Resources
    {
        public long Wood { get; set; }
        public long Clay { get; set; }
        public long Iron { get; set; }
        public long Crop { get; set; }

        public long[] ToArray() => new long[] { Wood, Clay, Iron, Crop };
    };
}
