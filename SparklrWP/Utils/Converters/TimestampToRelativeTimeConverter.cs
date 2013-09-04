using System;
using System.Globalization;
using System.Windows.Data;

namespace SparklrWP.Utils.Converters
{
    public class TimestampToRelativeTimeConverter : IValueConverter
    {
        public static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int timestamp;
            if (Int32.TryParse(value.ToString(), out timestamp))
            {

                DateTime time = epoch.AddSeconds(timestamp);

                TimeSpan delta = DateTime.UtcNow.Subtract(time);

                if (delta.Days > 1)
                {
                    return String.Format("{0} days ago", delta.Days);
                }
                else if (delta.Days == 1)
                {
                    return "one day ago";
                }
                else if (delta.Hours > 1)
                {
                    return String.Format("{0} hours ago", delta.Hours);
                }
                else if (delta.Minutes > 1)
                {
                    return String.Format("{0} minutes ago", delta.Minutes);
                }
                else if (delta.Minutes == 1)
                {
                    return "one minute ago";
                }
                else if (delta.Seconds > 10)
                {
                    return String.Format("{0} seconds ago", delta.Seconds);
                }
                else
                {
                    return "now";
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
