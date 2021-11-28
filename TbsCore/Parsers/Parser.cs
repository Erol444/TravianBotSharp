using System;
using System.Text;
using System.Text.RegularExpressions;

namespace TbsCore.Parsers
{
    public static class Parser
    {
        public static long ParseNum(string str)
        {
            return Convert.ToInt64(Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(str.Replace("−", "-"))).Replace("?", ""));
        }

        public static long RemoveNonNumeric(string str)
        {
            if (string.IsNullOrEmpty(str)) return 1;
            string onlyNumeric = Regex.Replace(str, "[^0-9-]", "");
            if (string.IsNullOrEmpty(onlyNumeric)) return long.MaxValue; //In TTwars, Unlimited gold
            return long.Parse(onlyNumeric);
        }

        public static float RemoveNonNumericFloat(string str)
        {
            string onlyNumeric = Regex.Replace(str, "[^.0-9-]", "");
            if (string.IsNullOrEmpty(onlyNumeric)) return float.MaxValue; //In TTwars, Unlimited gold
            return float.Parse(onlyNumeric);
        }

        public static string RemoveNumeric(string str)
        {
            return Regex.Replace(str, "[0-9]", "");
        }
    }
}