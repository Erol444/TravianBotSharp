using System;
using System.Collections.Generic;
using System.Text;

namespace TbsCore.Models.CombatModels
{
    /// <summary>
    /// 
    /// </summary>
    public class CombatPoints
    {
        public long i { get; set; }
        public long c { get; set; }

        public static CombatPoints zero()
        {
            return new CombatPoints(0, 0);
        }
        public static CombatPoints off(long value, bool isInfantry)
        {
            return isInfantry
                ? new CombatPoints(value, 0)
                : new CombatPoints(0, value);
        }
        public static CombatPoints both(int value)
        {
            return new CombatPoints(value, value);
        }

        // operation methods
        public static CombatPoints add(CombatPoints a, CombatPoints b)
        {
            return a.Add(b);
        }
        public static CombatPoints sum(CombatPoints[] ps)
        {
            var total = CombatPoints.zero();
            foreach(var cp in ps)
            {
                total.i += cp.i;
                total.c += cp.c;
            }
            return total;
        }

        // instance methods
        public CombatPoints()
        {
            this.i = 0;
            this.c = 0;
        }
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


        public CombatPoints Add(CombatPoints that) {
            this.i += that.i;
            this.c += that.c;
            return new CombatPoints(this.i + that.i, this.c + that.c);
        }
        public CombatPoints Mul(double m) {
            return new CombatPoints(this.i * m, this.c * m);
        }
        public CombatPoints Mask(CombatPoints mask) {
            return new CombatPoints(0 < mask.i ? this.i : 0, 0 < mask.c ? this.c : 0);
        }
    }
}
