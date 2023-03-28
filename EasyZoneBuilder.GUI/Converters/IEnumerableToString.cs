using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace EasyZoneBuilder.GUI.Converters
{
    [ValueConversion(typeof(IEnumerable), typeof(string))]
    public class IEnumberableToString : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( value is IEnumerable items )
            {
                StringBuilder sb = new StringBuilder();
                foreach ( object item in items )
                {
                    sb.Append(", " + item);
                }
                if ( sb.Length >= 2 )
                {
                    sb.Remove(0, 2);
                }
                return sb.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
