using System.Globalization;
using System.Windows.Data;

namespace BookstoreAdmin.Converters
{
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue.ToString("F2", culture);
            }
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (decimal.TryParse(value?.ToString(), NumberStyles.Any, culture, out var result))
            {
                return result;
            }
            return 0m;
        }
    }
}
