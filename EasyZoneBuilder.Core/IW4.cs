using EasyZoneBuilder.Core.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace EasyZoneBuilder.Core
{
    public class IW4 : IDirectoryInfo
    {
        public DirectoryInfoEx Directory { get; }
        public ObservableCollection<Mod> Mods { get; } = new ObservableCollection<Mod>();

        public static readonly string[] DEFAULT_MP_ZONES =
        {
            "code_pre_gfx_mp",
            "localized_code_pre_gfx_mp",
            "localized_code_post_gfx_mp",
            "common_mp",
            "localized_common_mp",
            "ui_mp",
            "localized_ui_mp"
        };

        public static readonly string[] BLACKLISTED_ZONES =
        {
            "mp_ambush_sh",
            "mp_bloc",
            "mp_cargoship",
            "mp_fav_tropical",
            "mp_killhouse",
            "mp_nuked",
            "mp_nuked_shaders",
            "mp_storm_spring",
            "team_opfor",
            "team_opforce_airborne",
            "team_opforce_composite",
            "team_rangers",
            "team_spetsnaz",
            "team_tf141",
            "team_us_army",
            "iw4x_code_post_gfx_mp"
        };
        // TODO: check why these zones dont work in zonebuilder,
        // my guess is that it needs some other zone loaded before it

        public IEnumerable<string> GetZones()
        {
            IEnumerable<string> fullPath = System.IO.Directory.EnumerateFiles(Path.Combine(Directory.FullName, "zone"), "*.ff", SearchOption.AllDirectories);
            IEnumerable<string> remove = System.IO.Directory.EnumerateFiles(Path.Combine(Directory.FullName, "zone"), "*.ff", SearchOption.TopDirectoryOnly);
            fullPath = fullPath.Difference(remove);
            foreach ( string path in fullPath )
            {
                string name = Path.GetFileNameWithoutExtension(path);
                if ( !BLACKLISTED_ZONES.Any(z => name == z) )
                {
                    yield return name;
                }
            }

        }

        public IW4( DirectoryInfoEx Directory )
        {
            this.Directory = Directory;
            DirectoryInfoEx modFolder = Directory.GetDirectory("mods");
            if ( modFolder.Exists )
            {
                foreach ( DirectoryInfoEx dir in modFolder.GetDirectories() )
                {
                    Mods.Add(new Mod(dir));
                }
            }

        }
    }
}
