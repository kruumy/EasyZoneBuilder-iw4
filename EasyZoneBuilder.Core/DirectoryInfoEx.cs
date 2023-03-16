using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EasyZoneBuilder.Core
{
    public class DirectoryInfoEx : FileSystemInfoEx
    {
        public DirectoryInfoEx( string FullName ) : base(FullName)
        {
        }

        public override bool Exists => Directory.Exists(FullName);

        public override void Touch()
        {
            Directory.CreateDirectory(FullName);
        }

        public IEnumerable<DirectoryInfoEx> GetDirectories()
        {
            return Directory.EnumerateDirectories(FullName).Select(dir => new DirectoryInfoEx(dir));
        }

        public DirectoryInfoEx GetDirectory(string name)
        {
            return new DirectoryInfoEx(Path.Combine(this.FullName, name));
        }

        public FileInfoEx GetFile(string name)
        {
            return new FileInfoEx(this,name);
        }

        public override void Delete()
        {
            Directory.Delete(FullName);
        }
    }
}
