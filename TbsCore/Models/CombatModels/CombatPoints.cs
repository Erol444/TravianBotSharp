using System;

namespace TbsCore.Models.CombatModels
{
    /// <summary>
    /// 
    /// </summary>
    public class CombatPoints
    {
        public long i { get; set; }
        public long c { get; set; }

        public static CombatPoints Zero()
        {
            return new CombatPoints(0, 0);
        }
        public static CombatPoints Off(long value, bool isInfantry)
        {
            return isInfantry
                ? new CombatPoints(value, 0)
                : new CombatPoints(0, value);
        }
        public static CombatPoints Both(int value)
        {
            return new CombatPoints(value, value);
        }

        // operation methods
        public static CombatPoints Sum(CombatPoints[] arr)
        {
            var total = CombatPoints.Zero();
            foreach (var cp in arr)
            {
                total.i += cp.i;
                total.c += cp.c;
            }
            return total;
        }

        // instance methods
        public CombatPoints(long inf, long cav)
        {
            this.i = inf;
            this.c = cav;
        }
        public CombatPoints(double inf, double cav)
        {
            this.i = (long)Math.Round(inf, MidpointRounding.AwayFromZero);
            this.c = (long)Math.Round(cav, MidpointRounding.AwayFromZero);
        }

        public void Add(CombatPoints that)
        {
            this.i += that.i;
            this.c += that.c;
        }
        public CombatPoints Mul(double m)
        {
            return new CombatPoints(this.i * m, this.c * m);
        }
        //public CombatPoints Mask(CombatPoints mask) {
        //    return new CombatPoints(0 < mask.i ? this.i : 0, 0 < mask.c ? this.c : 0);
        //}
    }
}
