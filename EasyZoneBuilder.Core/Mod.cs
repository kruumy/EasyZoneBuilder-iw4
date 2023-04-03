using EasyZoneBuilder.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyZoneBuilder.Core
{
    public class Mod : IDirectoryInfo
    {
        public DirectoryInfoEx Directory { get; }
        public FileInfoEx FastFile { get; }
        public ModCSV CSV { get; }
        public Precache Precache { get; }
        public Mod( DirectoryInfoEx Directory )
        {
            this.Directory = Directory;
            this.FastFile = new FileInfoEx(Path.Combine(Directory.FullName, "mod.ff"));
            this.CSV = new ModCSV(new FileInfoEx(Path.Combine(Directory.FullName, Path.GetFileNameWithoutExtension(FastFile.FullName) + ".csv")));
            this.Precache = new Precache(new FileInfoEx(Path.Combine(Directory.FullName, "_precache.gsc")));
        }

        public async Task BuildZone()
        {
            CSV.Push();
            await ZoneBuilder.BuildZone(CSV, FastFile);
        }

        public async Task ReadZone()
        {
            FastFile.AssertExist();
            CSV.Clear();
            CSV.AddRange(await ZoneBuilder.ListAssets(FastFile));
            CSV.Push();
        }

        public async Task SyncCSVToPrecache()
        {
            Dictionary<string, AssetType> mptoKeep = new Dictionary<string, AssetType>();
            Dictionary<string, Dictionary<string, AssetType>> depOutput = await DependencyGraph.DefaultInstance.GetAssetsAsync(IW4.DEFAULT_MP_ZONES);
            Dictionary<AssetType, List<string>> defaultMP = depOutput.Values.Flip().Concat();
            foreach ( KeyValuePair<string, AssetType> entry in Precache )
            {
                if ( defaultMP[ entry.Value ].Any(a => a == entry.Key) )
                {
                    mptoKeep.Add(entry.Key, entry.Value);
                }
            }
            Precache.Clear();
            Precache.AddRange(mptoKeep);
            Precache.SetIndexerRange(CSV);
        }
    }
}
