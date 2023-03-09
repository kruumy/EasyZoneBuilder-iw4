using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyZoneBuilder.Core
{
    public class Mod
    {
        public DirectoryInfo Directory { get; }
        public readonly FileInfo FastFile;
        public readonly ModCSV CSV;
        public readonly Precache Precache;
        public Mod( DirectoryInfo Directory )
        {
            this.Directory = Directory;
            this.FastFile = new FileInfo(Path.Combine(Directory.FullName,"mod.ff"));
            this.CSV = new ModCSV(new FileInfo(Path.Combine(Directory.FullName, Path.GetFileNameWithoutExtension(FastFile.FullName) + ".csv")));
            this.Precache = new Precache(new FileInfo(Path.Combine(Directory.FullName, "_precache.gsc")));
        }

        public async Task BuildZone()
        {
            CSV.Push();
            await ZoneBuilder.BuildZone(CSV, FastFile);
        }

        public void SmartCopyCSVToPrecache()
        {
            List<string> toRemoveFromPreacache = new List<string>();
            foreach ( KeyValuePair<string, AssetType> entry in Precache)
            {
                if (!ZoneBuilder.Default.Get(entry.Value).Any(d => d == entry.Key))
                {
                    toRemoveFromPreacache.Add(entry.Key);
                }
            }
            foreach ( string key in toRemoveFromPreacache.Distinct() )
            {
                Precache.Remove(key);
            }
            foreach ( KeyValuePair<string, ModCSV.EntryInfomation> entry in CSV)
            {
                Precache[ entry.Key ] = entry.Value.AssetType;
            }
        }
    }
}
