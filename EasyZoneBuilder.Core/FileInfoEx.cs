using System.IO;

namespace EasyZoneBuilder.Core
{
    public class FileInfoEx : FileSystemInfoEx
    {
        public FileInfoEx( string FullName ) : base(FullName)
        {
        }

        public override bool Exists => File.Exists(FullName);
        public string NameWithoutExtention => Path.GetFileNameWithoutExtension(FullName);

        public DirectoryInfoEx Directory => new DirectoryInfoEx(System.IO.Directory.GetParent(FullName).FullName);

        public override void Create()
        {
            using ( FileStream file = File.Create(FullName) )
            {
                file.Close();
            }
        }

        public void AssertExist()
        {
            if ( !this.Exists )
            {
                throw new FileNotFoundException(nameof(FullName), FullName);
            }
        }
    }
}
