using EasyZoneBuilder.Core.Interfaces;
using System.IO;

namespace EasyZoneBuilder.Core
{
    public class IW4 : IDirectoryInfo
    {
        public DirectoryInfo Directory { get; }
        public Mod[] Mods { get; }
        public string[] Zones
        {
            get
            {
                string[] fullPath = System.IO.Directory.GetFiles(Path.Combine(Directory.FullName, "zone/english"), "*.ff", SearchOption.TopDirectoryOnly);
                string[] names = new string[ fullPath.Length ];
                for ( int i = 0; i < fullPath.Length; i++ )
                {
                    names[ i ] = Path.GetFileNameWithoutExtension(fullPath[ i ]);
                }
                return names;
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
