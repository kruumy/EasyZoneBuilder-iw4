using EasyZoneBuilder.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace EasyZoneBuilder.GUI.Converters
{
    public class DependencyGraphGetAssets : IMultiValueConverter
    {
        public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
        {
            if ( !values.Any(item => item is null) )
            {
                string Map = values.FirstOrDefault(item => item is string) as string;
                DependencyGraph Graph = values.FirstOrDefault(item => item is DependencyGraph) as DependencyGraph;
                AssetType assetType = (AssetType)values.FirstOrDefault(item => item.GetType() == typeof(AssetType));
                IEnumerable<KeyValuePair<string, AssetType>> assets = Graph?.GetAssets(Map);
                assets = assets.Where(item => item.Value == assetType);
                return assets;
            }
            return null;
        }

        public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
