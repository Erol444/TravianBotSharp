using System;

namespace TbsCore.Extensions
{
    public static class DateTimeExtension
    {
        public static bool IsExpired(this DateTime specificDate)
        {
            return specificDate < DateTime.Now;
        }
    }
}