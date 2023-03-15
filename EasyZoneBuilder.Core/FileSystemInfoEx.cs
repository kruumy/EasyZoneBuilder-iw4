using System.IO;

namespace EasyZoneBuilder.Core
{
    public abstract class FileSystemInfoEx
    {
        public FileSystemInfoEx( string FullName )
        {
            this.FullName = Path.GetFullPath(FullName);
        }

        public abstract bool Exists { get; }
        public string FullName { get; private set; }
        public string Name => Path.GetFileName(FullName);

        public abstract void Create();
    }
}
