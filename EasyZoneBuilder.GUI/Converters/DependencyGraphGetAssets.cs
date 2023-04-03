using EasyZoneBuilder.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace EasyZoneBuilder.GUI.Converters
{
    public class DependencyGraphGetAssets : IMultiValueConverter
    {
        public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
        {
            string Map = string.Empty;
            DependencyGraph Graph = null;
            foreach ( object item in values )
            {
                if ( item is string s )
                {
                    Map = s;
                }
                else if ( item is DependencyGraph d )
                {
                    Graph = d;
                }
            }
            Dictionary<string, AssetType> assets = Graph?.GetAssets(Map);

            return assets;
        }

        public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
