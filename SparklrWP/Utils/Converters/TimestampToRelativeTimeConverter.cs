using SparklrWP.Utils.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SparklrWP.Utils.Converters
{
    public class TimestampToRelativeTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int timestamp;
            if (Int32.TryParse(value.ToString(), out timestamp))
            {
                return timestamp.FormatTime();
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
