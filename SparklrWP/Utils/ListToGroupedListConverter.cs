using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using Microsoft.Phone.Controls;
using System.Collections;
namespace SparklrWP.Utils
{
    public class ListToGroupedListConverter : IValueConverter
    {
        public const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IEnumerable)) return new Group<object>[] { };
            var listOfItems = value as IEnumerable;
            var first = listOfItems.OfType<object>().FirstOrDefault();
            if (first == null) return new Group<object>[] { };
            var itemType = first.GetType();
            var keyProperty = itemType.GetProperty(parameter as string);
            var grouped = (from ing in listOfItems.OfType<object>()
                           let keyValue = keyProperty.GetValue(ing, null) as string
                           group ing by keyValue.ToLower()[0]
                               into gd
                               select gd).ToArray();
            var groups = (from letter in Alphabet let set = grouped.FirstOrDefault(grouping => grouping.Key == letter) select new Group<object>(letter.ToString(), set)).ToArray();
            return groups;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
