using EasyZoneBuilder.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyZoneBuilder.Core
{
    public class Mod : IDirectoryInfo
    {
        public DirectoryInfo Directory { get; }
        public FileInfo FastFile { get; }
        public ModCSV CSV { get; }
        public Precache Precache { get; }
        public Mod( DirectoryInfo Directory )
        {
            this.Directory = Directory;
            this.FastFile = new FileInfo(Path.Combine(Directory.FullName, "mod.ff"));
            this.CSV = new ModCSV(new FileInfo(Path.Combine(Directory.FullName, Path.GetFileNameWithoutExtension(FastFile.FullName) + ".csv")));
            this.Precache = new Precache(new FileInfo(Path.Combine(Directory.FullName, "_precache.gsc")));
        }

        public async Task BuildZone()
        {
            CSV.Push();
            await ZoneBuilder.BuildZone(CSV, FastFile);
            FastFile.Refresh();
        }

        public async Task ReadZone()
        {
            FastFile.Refresh();
            if ( FastFile.Exists )
            {
                CSV.Clear();
                CSV.AddRange(await ZoneBuilder.ListAssets(FastFile));
                CSV.Push();
            }
            else
            {
                throw new FileNotFoundException(nameof(FastFile), FastFile.FullName);
            }

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
            foreach ( KeyValuePair<string, AssetType> item in mptoKeep )
            {
                Precache.Add(item.Key, item.Value);
            }
            foreach ( KeyValuePair<string, AssetType> entry in CSV )
            {
                Precache[ entry.Key ] = entry.Value;
            }
        }
    }
}
