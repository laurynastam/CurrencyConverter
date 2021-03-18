using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Readers.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime? GetDateTimeFromString(this string dateAsString) {

            if (dateAsString.Contains('/'))
            {
                DateTime dt = DateTime.ParseExact(dateAsString, "mm/", null, DateTimeStyles.None);
                return dt;
            }
            else if (dateAsString.Contains('-'))
            {
                DateTime dt = DateTime.ParseExact(dateAsString, "yyyy-mm-dd", null, DateTimeStyles.None);
                return dt;
            }
            else if (!String.IsNullOrEmpty(dateAsString))
            {
                try
                {
                    DateTime dt = DateTime.ParseExact(dateAsString, "dd MMMM yyyy", null, DateTimeStyles.None);
                    return dt;
                }
                catch (Exception ex) {
                    return null;
                }
            }
            else {
                return null;
            }

            return null;
        }
    }
}
