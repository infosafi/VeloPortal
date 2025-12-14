using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Domain.Extensions
{
    public class ErrorTrackingExtension
    {
        public static string? ErrorMsg { get; set; }
        public static string? ErrorSrc { get; set; }
        public static string? ErrorLocation { get; set; }


        public static void ClearErrors()
        {
            ErrorSrc = string.Empty;
            ErrorMsg = string.Empty;
            ErrorLocation = string.Empty;
        }
        public static void SetError(Exception ex)
        {
            ErrorSrc = ex.Source;
            ErrorMsg = ex.Message;
            ErrorLocation = ex.StackTrace;

        }
        private static string? GetValueOrDefault(Hashtable? table, string? key, string defaultValue)
        {
            if (table == null) return defaultValue;

            if (key == null) return defaultValue;

            return table.ContainsKey(key) && table[key] != null
                ? table[key].ToString()
                : defaultValue;
        }
        public static void SetError(Hashtable errObject)
        {

            ErrorSrc = GetValueOrDefault(errObject, "Src", "Unknown Source");
            ErrorMsg = GetValueOrDefault(errObject, "Msg", "No message available");
            ErrorLocation = GetValueOrDefault(errObject, "Location", "Unknown Location");


        }

    }
}
