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
            Dictionary<string, List<string>> dependency_graph = Core.TinyJson.JSONParser.FromJson<Dictionary<string, List<string>>>(File.ReadAllText("dependency_graph.json"));

            List<List<string>> allZones = new List<List<string>>();
            foreach ( KeyValuePair<string, AssetType> entry in csv )
            {
                allZones.Add(dependency_graph[ $"{entry.Value}:{entry.Key}" ]);
            }
            IEnumerable<string> ret = IntersectAll(allZones);
            return ret;
        }

        private static IEnumerable<T> IntersectAll<T>( IEnumerable<IEnumerable<T>> lists )
        {
            List<T> ret = new List<T>();
            IEnumerable<T> prevList = null;
            foreach ( IEnumerable<T> list in lists )
            {
                if ( prevList != null )
                {
                    IEnumerable<T> intersects = list.Intersect(prevList);
                    if ( intersects.Count() > 0 )
                    {
                        bool added = false;
                        foreach ( T intersect in intersects )
                        {
                            if ( ret.Contains(intersect) )
                            {
                                added = true;
                                ret.Add(intersect);
                                break;
                            }
                        }
                        if ( !added )
                        {
                            ret.Add(intersects.First());
                        }
                    }
                    else
                    {
                        bool added = false;
                        foreach ( T prev in list )
                        {
                            if ( ret.Contains(prev) )
                            {
                                added = true;
                                ret.Add(prev);
                                break;
                            }
                        }
                        if ( !added )
                        {
                            ret.Add(list.First());
                        }
                    }
                }
                prevList = list;
            }
            return ret.Distinct();
        }
    }
}
