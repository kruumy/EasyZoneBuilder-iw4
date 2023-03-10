using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyZoneBuilder.Core
{
    public static class DependencyGraphUtil
    {
        // TODO remove GenerateDependencyGraphJson and add a RegenerateZones(params string[] zones)

        public static async Task GenerateDependencyGraphJson()
        {
            Dictionary<string, Dictionary<string, List<string>>> json = new Dictionary<string, Dictionary<string, List<string>>>();
            foreach ( string zone in Core.Settings.IW4.Zones )
            {
                Debug.WriteLine(zone);
                json[ zone ] = new Dictionary<string, List<string>>();

                Dictionary<string, AssetType> assets = await ZoneBuilder.ListAssets(zone);
                foreach ( KeyValuePair<string, AssetType> asset in assets )
                {
                    if ( json[ zone ].TryGetValue(asset.Value.ToString(), out List<string> val) )
                    {
                        json[ zone ][ asset.Value.ToString() ].Add(asset.Key);
                    }
                    else
                    {
                        json[ zone ][ asset.Value.ToString() ] = new List<string>();
                    }
                }
            }
            Dictionary<string, List<string>> newJson = new Dictionary<string, List<string>>();
            foreach ( KeyValuePair<string, Dictionary<string, List<string>>> item in json )
            {
                string zone = item.Key;
                foreach ( KeyValuePair<string, List<string>> item1 in item.Value )
                {
                    string assetType = item1.Key;
                    List<string> assets = item1.Value;
                    foreach ( string asset in assets )
                    {
                        if ( !newJson.TryGetValue($"{assetType}:{asset}", out _) )
                        {
                            newJson[ $"{assetType}:{asset}" ] = new List<string>();
                        }
                        newJson[ $"{assetType}:{asset}" ].Add(zone);
                        newJson[ $"{assetType}:{asset}" ] = newJson[ $"{assetType}:{asset}" ].Distinct().ToList();
                    }
                }
            }
            File.WriteAllText("dependency_graph.json", Core.TinyJson.JSONWriter.ToJson(newJson));
        }
        public static IEnumerable<string> GetRequiredZones( ModCSV csv )
        {
            // Took a lot of inspiration from https://github.com/XLabsProject/iw4-zone-asset-finder/blob/main/iw4-zone-asset-finder/Commands/BuildRequirements.cs
            string rawdepgraph = File.ReadAllText("dependency_graph.json");
            Dictionary<string, List<string>> dependency_graph = Core.TinyJson.JSONParser.FromJson<Dictionary<string, List<string>>>(rawdepgraph);
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
    }
}
