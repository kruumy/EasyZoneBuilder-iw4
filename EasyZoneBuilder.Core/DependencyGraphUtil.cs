using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyZoneBuilder.Core
{
    public static class DependencyGraphUtil
    {
        public static readonly FileInfo File = new FileInfo(Path.Combine(Environment.CurrentDirectory, "dependency_graph.json"));

        public static async Task GenerateDependencyGraphJson()
        {
            string[] zones = Core.Settings.IW4.Zones;
            Console.WriteLine($"Reading {zones.Count()} zones...");
            Dictionary<string, Dictionary<string, AssetType>> zone_asset_assetType = await ZoneBuilder.ListAssets(zones);
            Dictionary<string, List<string>> asset_zones = new Dictionary<string, List<string>>();
            foreach ( KeyValuePair<string, Dictionary<string, AssetType>> item in zone_asset_assetType )
            {
                foreach ( KeyValuePair<string, AssetType> item1 in item.Value )
                {
                    if ( !asset_zones.TryGetValue($"{item1.Value}:{item1.Key}", out _) )
                    {
                        asset_zones[ $"{item1.Value}:{item1.Key}" ] = new List<string>();
                    }
                    asset_zones[ $"{item1.Value}:{item1.Key}" ].Add(item.Key); // TODO: might need to remove duplicates
                }
            }
            System.IO.File.WriteAllText(File.FullName, Core.TinyJson.JSONWriter.ToJson(asset_zones));
            Console.WriteLine("Done!");
        }

        public static Dictionary<string, AssetType> GetAssets( string zone )
        {
            Dictionary<string, List<string>> dependency_graph = Get();
            Dictionary<string, AssetType> ret = new Dictionary<string, AssetType>();

            foreach ( KeyValuePair<string, List<string>> dependency_graph_item in dependency_graph )
            {
                string[] assetLine = dependency_graph_item.Key.Split(':');
                if ( dependency_graph_item.Value.Any(z => z == zone) )
                {
                    ret[ assetLine[ 1 ] ] = AssetTypeUtil.Parse(assetLine[ 0 ]);
                }
            }
            return ret;
        }

        public static async Task<Dictionary<string, AssetType>> GetAssetsAsync( string zone )
        {
            return await Task.Run(() => GetAssets(zone));
        }

        public static IEnumerable<string> GetRequiredZones( ModCSV csv )
        {
            // Took a lot of inspiration from https://github.com/XLabsProject/iw4-zone-asset-finder/blob/main/iw4-zone-asset-finder/Commands/BuildRequirements.cs
            Dictionary<string, List<string>> dependency_graph = Get();
            List<KeyValuePair<string, List<string>>> assets_zones = new List<KeyValuePair<string, List<string>>>();
            foreach ( KeyValuePair<string, AssetType> asset in csv )
            {
                string dependency_graph_assetQuery = $"{asset.Value}:{asset.Key}";
                if ( dependency_graph.TryGetValue(dependency_graph_assetQuery, out List<string> queryResult) )
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

        public static async Task<IEnumerable<string>> GetRequiredZonesAsync( ModCSV csv )
        {
            return await Task.Run(() => GetRequiredZones(csv));
        }

        public static Dictionary<string, List<string>> Get()
        {
            return Core.TinyJson.JSONParser.FromJson<Dictionary<string, List<string>>>(System.IO.File.ReadAllText(File.FullName));
        }

        public static async Task<Dictionary<string, List<string>>> GetAsync()
        {
            return await Task.Run(() => Get());
        }

        public async static Task<IEnumerable<string>> GetZones()
        {
            HashSet<string> result = new HashSet<string>();
            Dictionary<string, List<string>> graph = await GetAsync();
            foreach ( List<string> item in graph.Values )
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
