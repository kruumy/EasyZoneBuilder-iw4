using System;
using System.IO;

namespace EasyZoneBuilder.Core
{
    public class TempFileCopy : IDisposable
    {
        public FileInfo File { get; }
        public FileInfo Destination { get; }

        public TempFileCopy( FileInfo File, FileInfo Destination )
        {
            this.File = File;
            this.Destination = Destination;
            System.IO.File.Copy(File.FullName, Destination.FullName, true);
        }
        ~TempFileCopy()
        {
            try
            {
                System.IO.File.Delete(Destination.FullName);
            }
            catch { }
        }

        public void Dispose()
        {
            System.IO.File.Delete(Destination.FullName);
            GC.SuppressFinalize(this);
        }
    }
}
