namespace TbsCore.Models.ResourceModels
{
    public class Resources
    {
        public Resources() { }
        public Resources(long[] res)
        {
            Wood = res[0];
            Clay = res[1];
            Iron = res[2];
            Crop = res[3];
        }

        public long Wood { get; set; }
        public long Clay { get; set; }
        public long Iron { get; set; }
        public long Crop { get; set; }

        public long[] ToArray() => new long[] { Wood, Clay, Iron, Crop };
        public long Sum() => (Wood + Clay + Iron + Crop);
        public override string ToString() => $"Wood: {Wood}, Clay: {Clay}, Iron: {Iron}, Crop: {Crop}";
    };
}
