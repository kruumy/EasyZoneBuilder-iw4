using System.Linq;

namespace EasyZoneBuilder.Core
{
    public enum AssetType // just need to add to this to add support for more asset types.
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
        public static bool IsSupportedAssetType( string str )
        {
            return typeof(AssetType).GetEnumNames().Any(e => e == str);
        }
    }
}
