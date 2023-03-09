using System;

namespace EasyZoneBuilder.Core
{
    public enum AssetType
    {
        xanim,
        xmodel
    }

    public static class AssetTypeUtil
    {
        public static AssetType Parse( string str )
        {
            str = str.ToLower().Trim();
            if ( str == AssetType.xanim.ToString() )
            {
                return AssetType.xanim;
            }
            else if ( str == AssetType.xmodel.ToString() )
            {
                return AssetType.xmodel;
            }
            else
            {
                throw new ArgumentException(str, nameof(str));
            }
        }
    }
}
