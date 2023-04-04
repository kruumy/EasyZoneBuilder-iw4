using System;
using System.Globalization;
using System.Windows.Data;

namespace EasyZoneBuilder.GUI.Converters
{
    [ValueConversion(typeof(int), typeof(bool))]
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            int @int = (int)value;
            return @int > 0;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
