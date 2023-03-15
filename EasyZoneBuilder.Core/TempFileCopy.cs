using System;

namespace EasyZoneBuilder.Core
{
    public class TempFileCopy : IDisposable
    {
        public FileInfoEx File { get; }
        public FileInfoEx Destination { get; }

        public TempFileCopy( FileInfoEx File, FileInfoEx Destination )
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
