using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VeloPortal.Domain.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetLocalTimeFromBaseOnTimeZone()
        {
            //List<string> list = GetAllTimeZones();
            TimeZoneInfo localZone = TimeZoneInfo.Local;

            string zoneid = localZone.Id;
            string zoneName = localZone.DisplayName;
            string zoneStandardName = localZone.StandardName;
            string zoneDispalyName = localZone.DaylightName;

            DateTime utcTime = DateTime.UtcNow;
            //DateTime utcTime = DateTime.Now;
            //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(zoneid);
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return localTime;
        }

        public class CustomDateTimeConverter : JsonConverter<DateTime>
        {
            private static readonly string[] SupportedFormats = new[]
        {
            "dd-MMM-yyyy",       // e.g., 03-Sep-2025
            "dd-MMM-yyyy hh:mm tt",        // 03-Sep-2025 05:30 PM
            "yyyy-MM-ddTHH:mm:ssZ",  // ISO 8601 UTC
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss.fff",
            "yyyy-MM-dd"
        };

            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = reader.GetString();
                if (value == null)
                    throw new JsonException("Date value is null.");

                if (DateTime.TryParseExact(value, SupportedFormats, CultureInfo.InvariantCulture,
                                           DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var date))
                {
                    return date;
                }

                // fallback: try default DateTime parsing
                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out date))
                    return date;

                throw new JsonException($"Invalid date format. Expected formats: {string.Join(", ", SupportedFormats)} or standard ISO.");
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                // Always write in ISO 8601 format
                writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture));
            }
        }
    }
}
