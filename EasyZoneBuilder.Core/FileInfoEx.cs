using System;
using System.Collections.Generic;
using System.IO;

namespace EasyZoneBuilder.Core
{
    public class FileInfoEx : FileSystemInfoEx
    {
        public FileInfoEx( string FullName ) : base(FullName)
        {
        }

        public FileInfoEx( DirectoryInfoEx dir, string filename ) : base(Path.Combine(dir.FullName, filename))
        {

        }

        public override bool Exists => File.Exists(FullName);
        public string NameWithoutExtention => Path.GetFileNameWithoutExtension(FullName);

        public DirectoryInfoEx Directory => new DirectoryInfoEx(System.IO.Directory.GetParent(FullName).FullName);

        public override void Touch()
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

        public void WriteAllText(string content)
        {
            File.WriteAllText(FullName, content);
        }
        public string ReadAllText()
        {
            return File.ReadAllText(FullName);
        }

        public override void Delete()
        {
            File.Delete(FullName);
        }

        public void Move( FileInfoEx destination )
        {
            File.Move(this.FullName,destination.FullName);
        }

        internal IEnumerable<string> ReadAllLines()
        {
            return File.ReadAllLines(FullName);
        }
    }
}
