using MainCore.Models.Database;

namespace MainCore.Models.Runtime
{
    public class Resources
    {
        public Resources(long wood, long clay, long iron, long crop)
        {
            Wood = wood;
            Clay = clay;
            Iron = iron;
            Crop = crop;
        }

        public Resources(long[] resource)
        {
            Wood = resource[0];
            Clay = resource[1];
            Iron = resource[2];
            Crop = resource[3];
        }

        public Resources()
        {
        }

        public long Wood { get; set; }
        public long Clay { get; set; }
        public long Iron { get; set; }
        public long Crop { get; set; }

        public bool IsNegative()
        {
            return Wood < 0 || Clay < 0 || Iron < 0 || Crop < 0;
        }

        public void ZeroNegative()
        {
            if (Wood < 0) Wood = 0;
            if (Clay < 0) Clay = 0;
            if (Iron < 0) Iron = 0;
            if (Crop < 0) Crop = 0;
        }

        public override string ToString()
        {
            return $"Wood: {Wood}, Clay: {Clay}, Iron: {Iron}, Crop: {Crop}";
        }

        /// <summary>
        /// complier complain, change this before you use
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)(Wood + Clay + Iron + Crop);
        }

        public static bool operator >(Resources a, VillageResources b)
        {
            return (a.Wood > b.Wood && a.Clay > b.Clay && a.Iron > b.Iron && a.Crop > b.Crop);
        }

        public static bool operator <(Resources a, VillageResources b)
        {
            return (a.Wood < b.Wood && a.Clay < b.Clay && a.Iron < b.Iron && a.Crop < b.Crop);
        }

        public static bool operator ==(Resources a, VillageResources b)
        {
            return (a.Wood == b.Wood && a.Clay == b.Clay && a.Iron == b.Iron && a.Crop == b.Crop);
        }

        public static bool operator !=(Resources a, VillageResources b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            var good = obj as VillageResources;
            if (obj == null)
                return false;

            return this == good;
        }

        public static Resources operator +(Resources a, VillageResources b)
        {
            return new Resources(a.Wood + b.Wood, a.Clay + b.Clay, a.Iron + b.Iron, a.Crop + b.Crop);
        }

        public static Resources operator -(Resources a, VillageResources b)
        {
            return new Resources(a.Wood - b.Wood, a.Clay - b.Clay, a.Iron - b.Iron, a.Crop - b.Crop);
        }

        public static Resources operator -(Resources a, Resources b)
        {
            return new Resources(a.Wood - b.Wood, a.Clay - b.Clay, a.Iron - b.Iron, a.Crop - b.Crop);
        }

        public static Resources operator -(VillageResources a, Resources b)
        {
            return new Resources(a.Wood - b.Wood, a.Clay - b.Clay, a.Iron - b.Iron, a.Crop - b.Crop);
        }
    }
}