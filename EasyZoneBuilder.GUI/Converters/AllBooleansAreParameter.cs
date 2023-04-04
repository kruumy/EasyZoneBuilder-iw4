using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace EasyZoneBuilder.GUI.Converters
{
    public class AllBooleansAreParameter : IMultiValueConverter
    {
        public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
        {
            bool[] bools = new bool[ values.Length ];
            for ( int i = 0; i < values.Length; i++ )
            {
                try
                {
                    bools[ i ] = (bool)values[ i ];
                }
                catch
                {
                    return false;
                }
            }
            bool target;
            switch ( parameter )
            {
                case bool b:
                    target = b;
                    break;
                case string sb:
                    target = System.Convert.ToBoolean(sb);
                    break;
                default:
                    target = true;
                    break;
            }
            return bools.All(b => b == target);
        }

        public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
