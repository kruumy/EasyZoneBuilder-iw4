using System;

namespace EasyZoneBuilder.Core
{
    public enum AssetType
    {
        xanim,
        xmodel,
        xmodelsurfs,
        image,
        material,
        sound,
        map_ents,
        lightdef,
        localize,
        weapon,
        stringtable,
        leaderboarddef
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
    }
}
