using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyZoneBuilder.Core
{
    public static class DependencyGraphUtil
    {
        public static async Task GenerateDependencyGraphJson()
        {
            Dictionary<string, Dictionary<string, string[]>> json = new Dictionary<string, Dictionary<string, string[]>>();
            foreach ( string zone in Core.Settings.IW4.Zones )
            {
                Console.WriteLine(zone);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                json[ zone ] = new Dictionary<string, string[]>();
                foreach ( string assetType in typeof(AssetType).GetEnumNames() )
                {
                    Console.Write(assetType);
                    json[ zone ][ assetType ] = (await ZoneBuilder.ListAssets(AssetTypeUtil.Parse(assetType), zone)).ToArray();
                    Console.Write(":" + json[ zone ][ assetType ].Length + "\n");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            Dictionary<string, List<string>> newJson = new Dictionary<string, List<string>>();
            foreach ( KeyValuePair<string, Dictionary<string, string[]>> item in json )
            {
                string zone = item.Key;
                foreach ( KeyValuePair<string, string[]> item1 in item.Value )
                {
                    string assetType = item1.Key;
                    string[] assets = item1.Value;
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
            Dictionary<string, List<string>> dependency_graph = Core.TinyJson.JSONParser.FromJson<Dictionary<string, List<string>>>(File.ReadAllText("dependency_graph.json"));
            List<KeyValuePair<string, List<string>>> assets_zones = new List<KeyValuePair<string, List<string>>>();
            foreach ( KeyValuePair<string, AssetType> asset in csv )
            {
                string dependency_graph_assetQuery = $"{asset.Value}:{asset.Key}";
                List<string> queryResult = dependency_graph[ dependency_graph_assetQuery ];
                assets_zones.Add(new KeyValuePair<string, List<string>>(asset.Key, queryResult));
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
