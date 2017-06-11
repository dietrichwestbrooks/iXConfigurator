using System;
using System.Globalization;
using System.Windows.Data;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class NullOrEmptyToBoolValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
