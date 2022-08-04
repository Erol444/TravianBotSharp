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
    }
}