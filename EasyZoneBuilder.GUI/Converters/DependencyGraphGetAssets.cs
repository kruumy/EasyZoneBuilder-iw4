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
                DependencyGraph Graph = values[ 0 ] as DependencyGraph;
                string Map = values[ 1 ] as string;
                IEnumerable<KeyValuePair<string, AssetType>> assets = Graph?.GetAssets(Map);

                if ( values.Length >= 3 )
                {
                    AssetType assetType = (AssetType)values[ 2 ];
                    assets = assets.Where(item => item.Value == assetType);
                }

                if ( values.Length >= 4 )
                {
                    string[] searchQueries = (values[ 3 ] as string).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach ( string searchQuery in searchQueries )
                    {
                        assets = assets.Where(item => item.Key.Contains(searchQuery));
                    }
                }




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
