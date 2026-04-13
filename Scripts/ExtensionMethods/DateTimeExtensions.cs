using System;

namespace manhnd_sdk.Scripts.ExtensionMethods
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns a new <see cref="DateTime"/> object with the specified year, month, and day. <br></br>
        /// If any of the parameters are null, the corresponding value from the original <see cref="DateTime"/> is used.
        /// </summary>
        /// <param name="year">The new year value. If null, the original year is used.</param>
        /// <param name="month">The new month value. If null, the original month is used.</param>
        /// <param name="day">The new day value. If null, the original day is used.</param>
        public static DateTime WithDate(this DateTime dt, int? year = null, int? month = null, int? day = null) {
            int newYear = year ?? dt.Year;
            int newMonth = (month == null || month > 12 || month < 1) ? dt.Month : month.Value;
            int newDay = day ?? dt.Day;

            // Ensure the new date is valid by clamping day if necessary
            int daysInMonth = DateTime.DaysInMonth(newYear, newMonth);
            newDay = Math.Min(newDay, daysInMonth);

            return new DateTime(newYear, newMonth, newDay, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
        }
    }
}