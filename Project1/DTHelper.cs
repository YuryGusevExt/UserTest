using System;

namespace Project1
{
    public static class DTHelper
    {
        public static DateTime GetLocalDate(this DateTime date)
        {
            try
            {
                return (date.Kind != DateTimeKind.Local) ? TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.Local) : date;

            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }
    }
}
