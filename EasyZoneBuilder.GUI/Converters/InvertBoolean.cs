using System;
using System.Globalization;
using System.Windows.Data;

namespace EasyZoneBuilder.GUI.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertBoolean : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            bool @bool = (bool)value;
            return !@bool;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
