using System;
using System.Diagnostics;

namespace JungholmInstrumentsDesktop.Services
{
    /// <summary>
    /// Service for handling Swedish timezone (Europe/Stockholm) conversions.
    /// Handles both CET (UTC+1) and CEST (UTC+2) automatically based on daylight saving time.
    /// </summary>
    public static class TimezoneService
    {
        private static readonly TimeZoneInfo SwedishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"); // Windows ID for Europe/Stockholm
        
        /// <summary>
        /// Converts a UTC DateTime to Swedish timezone (Europe/Stockholm).
        /// </summary>
        public static DateTime ToSwedishTime(DateTime utcDateTime)
        {
            try
            {
                // Ensure the DateTime is UTC (if it's Unspecified, assume UTC)
                DateTime utcTime = utcDateTime.Kind == DateTimeKind.Utc 
                    ? utcDateTime 
                    : DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

                var swedishTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, SwedishTimeZone);
                return swedishTime;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TimezoneService] Error converting to Swedish time: {ex.Message}");
                // Fallback: try alternative timezone ID (for Linux/Mac)
                try
                {
                    var alternativeTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");
                    DateTime utcTime = utcDateTime.Kind == DateTimeKind.Utc 
                        ? utcDateTime 
                        : DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
                    return TimeZoneInfo.ConvertTimeFromUtc(utcTime, alternativeTz);
                }
                catch
                {
                    // Last resort: return as-is
                    return utcDateTime;
                }
            }
        }

        /// <summary>
        /// Converts a Swedish timezone DateTime to UTC.
        /// </summary>
        public static DateTime ToUtc(DateTime swedishDateTime)
        {
            try
            {
                // Assume the input is in Swedish timezone
                var swedishTime = DateTime.SpecifyKind(swedishDateTime, DateTimeKind.Unspecified);
                return TimeZoneInfo.ConvertTimeToUtc(swedishTime, SwedishTimeZone);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TimezoneService] Error converting to UTC: {ex.Message}");
                // Fallback
                try
                {
                    var alternativeTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");
                    var swedishTime = DateTime.SpecifyKind(swedishDateTime, DateTimeKind.Unspecified);
                    return TimeZoneInfo.ConvertTimeToUtc(swedishTime, alternativeTz);
                }
                catch
                {
                    return swedishDateTime.ToUniversalTime();
                }
            }
        }

        /// <summary>
        /// Gets the current Swedish timezone abbreviation (CET or CEST).
        /// </summary>
        public static string GetSwedishTimezoneAbbreviation()
        {
            try
            {
                var now = DateTime.UtcNow;
                var swedishTime = ToSwedishTime(now);
                var offset = SwedishTimeZone.GetUtcOffset(swedishTime);
                
                // CET is UTC+1, CEST is UTC+2
                return offset.Hours == 2 ? "CEST" : "CET";
            }
            catch
            {
                return "CET/CEST";
            }
        }

        /// <summary>
        /// Gets the Swedish timezone display name.
        /// </summary>
        public static string GetSwedishTimezoneDisplayName()
        {
            return $"Europe/Stockholm ({GetSwedishTimezoneAbbreviation()})";
        }
    }
}


