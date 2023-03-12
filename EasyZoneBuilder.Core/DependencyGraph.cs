using EasyZoneBuilder.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EasyZoneBuilder.Core
{
    public class DependencyGraph : Dictionary<string, List<string>>, IASync, IFileInfo
    {
        [IgnoreDataMember]
        public readonly static DependencyGraph DefaultInstance = new DependencyGraph(new FileInfo(Path.Combine(Environment.CurrentDirectory, "dependency_graph.json")));
        public DependencyGraph( FileInfo File )
        {
            this.File = File;
        }
        public DependencyGraph( Dictionary<string, List<string>> dict, FileInfo File ) : base(dict)
        {
            this.File = File;
        }

        [IgnoreDataMember]
        public FileInfo File { get; }

        public async Task Pull()
        {
            string rawText = System.IO.File.ReadAllText(this.File.FullName);
            Dictionary<string, List<string>> Dict = null;
            await Task.Run(() => { Dict = TinyJson.JSONParser.FromJson<Dictionary<string, List<string>>>(rawText); });
            this.Clear();
            foreach ( KeyValuePair<string, List<string>> entry in Dict )
            {
                this.Add(entry.Key, entry.Value);
            }
        }

        public async Task Push()
        {
            string Dict = null;
            await Task.Run(() => { Dict = TinyJson.JSONWriter.ToJson(this); });
            System.IO.File.WriteAllText(File.FullName, Dict);
        }

        public Dictionary<string, AssetType> GetAssets( string zone )
        {
            Dictionary<string, AssetType> ret = new Dictionary<string, AssetType>();

            foreach ( KeyValuePair<string, List<string>> dependency_graph_item in this )
            {
                string[] assetLine = dependency_graph_item.Key.Split(':');
                if ( dependency_graph_item.Value.Any(z => z == zone) )
                {
                    ret[ assetLine[ 1 ] ] = AssetTypeUtil.Parse(assetLine[ 0 ]);
                }
            }
            return ret;
        }

        public async Task<Dictionary<string, AssetType>> GetAssetsAsync( string zone )
        {
            return await Task.Run(() => GetAssets(zone));
        }

        public async Task GenerateDependencyGraphJson( IEnumerable<string> zones )
        {
            Console.WriteLine($"Reading {zones.Count()} zones...");
            Dictionary<string, Dictionary<string, AssetType>> zone_asset_assetType = await ZoneBuilder.ListAssets(zones);
            this.Clear();
            foreach ( KeyValuePair<string, Dictionary<string, AssetType>> item in zone_asset_assetType )
            {
                foreach ( KeyValuePair<string, AssetType> item1 in item.Value )
                {
                    if ( !this.TryGetValue($"{item1.Value}:{item1.Key}", out _) )
                    {
                        this[ $"{item1.Value}:{item1.Key}" ] = new List<string>();
                    }
                    this[ $"{item1.Value}:{item1.Key}" ].Add(item.Key); // TODO: might need to remove duplicates
                }
            }
            await Push();
            Console.WriteLine("Done!");
        }

        public IEnumerable<string> GetRequiredZones( ModCSV csv )
        {
            // Took a lot of inspiration from https://github.com/XLabsProject/iw4-zone-asset-finder/blob/main/iw4-zone-asset-finder/Commands/BuildRequirements.cs
            List<KeyValuePair<string, List<string>>> assets_zones = new List<KeyValuePair<string, List<string>>>();
            foreach ( KeyValuePair<string, AssetType> asset in csv )
            {
                string dependency_graph_assetQuery = $"{asset.Value}:{asset.Key}";
                if ( this.TryGetValue(dependency_graph_assetQuery, out List<string> queryResult) )
                {
                    assets_zones.Add(new KeyValuePair<string, List<string>>(asset.Key, queryResult));
                }
                else
                {
                    return Array.Empty<string>();
                }
            }
            Dictionary<string, int> finalZoneScore = new Dictionary<string, int>();
            while ( assets_zones.Count > 0 )
            {
                Dictionary<string, int> zoneScore = new Dictionary<string, int>();
                foreach ( KeyValuePair<string, List<string>> asset_zones in assets_zones )
                {
                    foreach ( string zone in asset_zones.Value )
                    {
                        if ( !zoneScore.ContainsKey(zone) )
                        {
                            zoneScore[ zone ] = 0;
                        }
                        zoneScore[ zone ]++;
                    }
                }
                string nextZone = zoneScore.OrderByDescending(o => o.Value).First().Key;
                assets_zones.RemoveAll(o => o.Value.Contains(nextZone));
                finalZoneScore[ nextZone ] = zoneScore[ nextZone ];
            }
            return finalZoneScore.Keys;
        }

        public IEnumerable<string> GetZones()
        {
            HashSet<string> result = new HashSet<string>();
            foreach ( List<string> item in this.Values )
            {
                foreach ( string item1 in item )
                {
                    result.Add(item1);
                }
            }
            return result;
        }
    }
}