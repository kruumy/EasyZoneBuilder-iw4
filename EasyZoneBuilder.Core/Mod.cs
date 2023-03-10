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
        }

        public async Task ReadZone()
        {
            CSV.Clear();
            CSV.AddRange(await ZoneBuilder.ListAssets(FastFile));
            CSV.Push();
        }

        public void SyncCSVToPrecache()
        {
            List<string> toRemoveFromPreacache = new List<string>();
            foreach ( KeyValuePair<string, AssetType> entry in Precache )
            {
                if ( entry.Key.Contains("pb_") || entry.Key.Contains("mp_") ) // TODO: make this better, possible use dependecygraph or cache default mp assets
                {
                    toRemoveFromPreacache.Add(entry.Key);
                }
            }
            foreach ( string key in toRemoveFromPreacache.Distinct() )
            {
                Precache.Remove(key);
            }
            foreach ( KeyValuePair<string, AssetType> entry in CSV )
            {
                Precache[ entry.Key ] = entry.Value;
            }
        }
    }
}
