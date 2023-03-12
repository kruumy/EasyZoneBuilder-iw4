using EasyZoneBuilder.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyZoneBuilder.Core
{
    public class IW4 : IDirectoryInfo
    {
        public DirectoryInfo Directory { get; }
        public Mod[] Mods { get; }

        public readonly string[] BlacklistedZones =
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

        public string[] Zones
        {
            get
            {
                string[] fullPath = System.IO.Directory.GetFiles(Path.Combine(Directory.FullName, "zone"), "*.ff", SearchOption.AllDirectories);
                string[] toexclude = System.IO.Directory.GetFiles(Path.Combine(Directory.FullName, "zone"), "*.ff", SearchOption.TopDirectoryOnly);
                List<string> result = new List<string>(fullPath.Length);
                for ( int i = 0; i < fullPath.Length; i++ )
                {
                    string name = Path.GetFileNameWithoutExtension(fullPath[ i ]);
                    if ( !BlacklistedZones.Any(z => name == z) && !toexclude.Any(z => name == z) )
                    {
                        result.Add(name);
                    }
                }
                return result.ToArray();
            }
        }

        public IW4( DirectoryInfo Directory )
        {
            this.Directory = Directory;
            DirectoryInfo modFolder = new DirectoryInfo(Path.Combine(Directory.FullName, "mods"));
            if ( modFolder.Exists )
            {
                DirectoryInfo[] moddirs = modFolder.GetDirectories();
                Mods = new Mod[ moddirs.Length ];
                for ( int i = 0; i < moddirs.Length; i++ )
                {
                    Mods[ i ] = new Mod(moddirs[ i ]);
                }
            }
        }
    }
}
