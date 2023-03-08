using System.IO;
using System.Threading.Tasks;

namespace EasyZoneBuilder.Core
{
    public class FastFile
    {
        public readonly FileInfo File;
        public readonly ModCSV CSV;
        public FastFile( FileInfo File )
        {
            this.File = File;
            this.CSV = new ModCSV(new FileInfo(Path.Combine(File.Directory.FullName, Path.GetFileNameWithoutExtension(File.FullName) + ".csv")));
        }

        public async Task Push()
        {
            CSV.Push();
            await ZoneBuilder.BuildZone(CSV, File);
        }
        public async Task Pull()
        {
            foreach ( string anim in await ZoneBuilder.ListAssets(AssetType.xanim, File) )
            {
                CSV[ anim ] = AssetType.xanim;
            }
            foreach ( string model in await ZoneBuilder.ListAssets(AssetType.xmodel, File) )
            {
                CSV[ model ] = AssetType.xmodel;
            }
        }
    }
}
