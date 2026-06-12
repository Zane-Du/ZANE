using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace ZANE.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSearching && isSearching)
            {
                return new SolidColorBrush(Color.FromRgb(255, 69, 0)); // OrangeRed
            }
            return new SolidColorBrush(Color.FromRgb(128, 128, 128)); // Gray
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}