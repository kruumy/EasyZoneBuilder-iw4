using System;
using System.Linq;

namespace EasyZoneBuilder.Core
{
    public enum AssetType // just need to add to this to add support for more asset types.
    {
        xanim,
        xmodel,
        fx
    }

    public static class AssetTypeUtil
    {
        public static AssetType Parse( string str )
        {
            str = str.ToLower().Trim();
            string[] array = typeof(AssetType).GetEnumNames();
            for ( int i = 0; i < array.Length; i++ )
            {
                if ( array[ i ] == str )
                {
                    return (AssetType)i;
                }
            }
            throw new ArgumentException(str, nameof(str));
        }

        public static bool IsSupportedAssetType(string str)
        {
            return typeof(AssetType).GetEnumNames().Any(e => e == str);
        }
    }
}
