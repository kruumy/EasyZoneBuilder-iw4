using System;
using System.IO;

namespace EasyZoneBuilder.Core
{
    public class TempFileCopy : IDisposable
    {
        public readonly FileInfo File;
        public readonly FileInfo Destination;
        public TempFileCopy( FileInfo File, FileInfo Destination )
        {
            this.File = File;
            this.Destination = Destination;
            System.IO.File.Copy(File.FullName, Destination.FullName,true);
        }

        public void Dispose()
        {
            System.IO.File.Delete(Destination.FullName);
        }
    }
}
