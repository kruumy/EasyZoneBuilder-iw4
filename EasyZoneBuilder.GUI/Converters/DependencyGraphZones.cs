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
                HashSet<string> result = new HashSet<string>();
                foreach ( List<string> item in graph.Values )
                {
                    foreach ( string item1 in item )
                    {
                        result.Add(item1);
                    }
                }
                return result;
            }
            return null;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
