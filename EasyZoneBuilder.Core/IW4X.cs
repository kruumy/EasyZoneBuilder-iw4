using System.IO;
using System.Linq;

namespace EasyZoneBuilder.Core
{
    public class IW4X
    {
        public readonly DirectoryInfo Directory;
        public readonly Mod[] Mods;
        public IW4X( DirectoryInfo Directory )
        {
            this.Directory = Directory;
            DirectoryInfo[] moddirs = Directory.GetDirectories().First(d => d.Name.ToLower() == "mods").GetDirectories();
            Mods = new Mod[ moddirs.Length ];
            for ( int i = 0; i < moddirs.Length; i++ )
            {
                Mods[ i ] = new Mod(moddirs[ i ]);
            }
        }
    }
}
