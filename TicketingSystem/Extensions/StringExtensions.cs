using System.Globalization;

namespace TicketingSystem.Extensions
{
    public static class StringExtensions
    {
        public static string ToISO8601(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        }

        public static DateTime FromISO8601(this string dateTime)
        {
            return DateTime.ParseExact(dateTime, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        }
    }
}
