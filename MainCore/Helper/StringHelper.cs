using System;
using System.Linq;

namespace MainCore.Helper
{
    public static class StringHelper
    {
        public static bool IsNumeric(this string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.All(char.IsDigit);
        }

        public static int ToNumeric(this string value)
        {
            var valueStr = new string(value.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(valueStr)) return 1;
            return int.Parse(valueStr);
        }

        public static string EnumStrToString(this string value)
        {
            var len = value.Length;
            for (int i = 1; i < len; i++)
            {
                if (char.IsUpper(value[i]))
                {
                    value = value.Insert(i, " ");
                    i++;
                    len++;
                }
            }
            return value;
        }

        public static TimeSpan ToDuration(this string value)
        {
            //00:00:02 (+332 ms), TTWars, milliseconds matter
            int ms = 0;
            if (value.Contains("(+"))
            {
                var parts = value.Split('(');
                ms = parts[1].ToNumeric();
                value = parts[0];
            }
            // h:m:s
            var arr = value.Split(':');
            var h = arr[0].ToNumeric();
            var m = arr[1].ToNumeric();
            var s = arr[2].ToNumeric();
            return new TimeSpan(0, h, m, s, ms);
        }
    }
}