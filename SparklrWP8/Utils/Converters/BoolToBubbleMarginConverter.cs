using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SparklrWP.Utils.Converters
{
    public class BoolToBubbleMarginConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;

            if (v)
            {
                return new Thickness(75, 0, 0, 0);
            }
            else
            {
                return new Thickness(0, 0, 75, 0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
