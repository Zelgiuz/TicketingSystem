﻿namespace TicketingSystem.Extensions
{
    public static class StringExtensions
    {
        public static string ToIso8601(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd-HH:mm:ssZ");
        }

        public static DateTime FromISO8601(this string dateTime)
        {
            return DateTime.ParseExact(dateTime, "yyyy-MM-dd-HH:mm:ssZ", null);
        }
    }
}
