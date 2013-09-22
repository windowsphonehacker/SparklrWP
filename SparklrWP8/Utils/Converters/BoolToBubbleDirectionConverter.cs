using Coding4Fun.Toolkit.Controls;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SparklrWP.Utils.Converters
{
    public class BoolToBubbleDirectionConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;

            if (v)
            {
                return ChatBubbleDirection.LowerRight;
            }
            else
            {
                return ChatBubbleDirection.UpperLeft;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
