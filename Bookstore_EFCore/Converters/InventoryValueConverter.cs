using System.Globalization;
using System.Windows.Data;

namespace BookstoreAdmin.Converters
{
    internal class InventoryValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int bookQuantity && values[1] is decimal bookPrice)
            {
                return bookQuantity * bookPrice;
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
