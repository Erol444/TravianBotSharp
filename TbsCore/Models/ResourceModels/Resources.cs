namespace TbsCore.Models.ResourceModels
{
    public class Resources
    {
        public long Wood { get; set; }
        public long Clay { get; set; }
        public long Iron { get; set; }
        public long Crop { get; set; }

        public long[] ToArray()
        {
            return new[] {Wood, Clay, Iron, Crop};
        }

        public long Sum()
        {
            return Wood + Clay + Iron + Crop;
        }

        public override string ToString()
        {
            return $"Wood: {Wood}, Clay: {Clay}, Iron: {Iron}, Crop: {Crop}";
        }
    }
}