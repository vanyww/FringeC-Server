using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFTerminal.Converters
{
    internal sealed class ImageConverter : IMultiValueConverter
    {
        public Object Convert(Object[] values, Type targetType, Object parameter, CultureInfo culture)
        {
            return (values[0] as ResourceDictionary)[values[1]];
        }

        public Object[] ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
