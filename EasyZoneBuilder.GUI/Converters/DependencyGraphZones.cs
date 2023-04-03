using EasyZoneBuilder.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace EasyZoneBuilder.GUI.Converters
{
    [ValueConversion(typeof(DependencyGraph), typeof(IEnumerable<string>))]
    public class DependencyGraphZones : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( value is DependencyGraph graph )
            {
                IEnumerable<string> zones = graph.GetZones();
                return zones;
            }
            return null;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
