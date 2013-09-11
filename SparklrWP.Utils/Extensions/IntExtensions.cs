using System;

namespace SparklrWP.Utils.Extensions
{
    public static class IntExtensions
    {
        public static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Formats a timestamp as relative time
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string FormatTime(this int timestamp)
        {
            DateTime time = epoch.AddSeconds(timestamp);

            TimeSpan delta = DateTime.UtcNow.Subtract(time);

            if (delta.TotalDays >= 2)
            {
                return String.Format("{0:0} days ago", delta.TotalDays);
            }
            else if (delta.TotalDays > 1)
            {
                return "one day ago";
            }
            else if (delta.TotalHours >= 2)
            {
                return String.Format("{0:0} hours ago", delta.TotalHours);
            }
            else if (delta.TotalHours > 1)
            {
                return "one hour ago";
            }
            else if (delta.TotalMinutes >= 2)
            {
                return String.Format("{0:0} minutes ago", delta.TotalMinutes);
            }
            else if (delta.TotalMinutes > 1)
            {
                return "one minute ago";
            }
            else if (delta.TotalSeconds > 10)
            {
                return String.Format("{0:0} seconds ago", delta.TotalSeconds);
            }
            else
            {
                return "now";
            }
        }
    }
}
