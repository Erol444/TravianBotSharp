using System.Linq;

namespace MainCore.Helper
{
    public static class StringHelper
    {
        public static bool IsNumeric(this string value)
        {
            return value.All(char.IsDigit);
        }
    }
}