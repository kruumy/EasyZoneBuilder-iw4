using System.IO;
using System.Linq;

namespace EasyZoneBuilder.Core
{
    public class Mod
    {
        public readonly DirectoryInfo Directory;
        public readonly FastFile FastFile;
        public Mod( DirectoryInfo Directory )
        {
            this.Directory = Directory;
            this.FastFile = new FastFile(new FileInfo(Path.Combine(Directory.FullName,"mod.ff")));
        }
    }
}
