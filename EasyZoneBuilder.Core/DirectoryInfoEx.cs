using System.Collections.Generic;
using System.IO;

namespace EasyZoneBuilder.Core
{
    public class DirectoryInfoEx : FileSystemInfoEx
    {
        public DirectoryInfoEx( string FullName ) : base(FullName)
        {
        }

        public override bool Exists => Directory.Exists(FullName);

        public override void Create()
        {
            Directory.CreateDirectory(FullName);
        }

        public IEnumerable<DirectoryInfoEx> GetDirectories()
        {
            foreach ( string dir in Directory.EnumerateDirectories(FullName) )
            {
                yield return new DirectoryInfoEx(dir);
            }
        }
    }
}
