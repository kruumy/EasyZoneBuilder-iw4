using System.Linq;

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
        leaderboarddef,
        physpreset,
        phys_collmap,
        pixelshader,
        vertexshader,
        vertexdecl,
        techset,
        sndcurve,
        loaded_sound,
        col_map_sp,
        col_map_mp,
        com_map,
        game_map_sp,
        game_map_mp,
        fx_map,
        gfx_map,
        ui_map,
        font,
        menufile,
        menu,
        snddriverglobals,
        fx,
        impactfx,
        aitype,
        mptype,
        character,
        xmodelalias,
        rawfile,
        structureddatadef,
        tracer,
        vehicle,
        addon_map_ents
    }

    public static class AssetTypeUtil
    {
        public static bool IsSupportedAssetType( string str )
        {
            return typeof(AssetType).GetEnumNames().Any(e => e == str);
        }
    }
}
