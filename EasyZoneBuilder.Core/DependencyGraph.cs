using EasyZoneBuilder.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EasyZoneBuilder.Core
{
    public class DependencyGraph : ObservableDictionary<string, List<string>>, IASync, IFileInfo
    {
        [IgnoreDataMember]
        public static readonly DependencyGraph DefaultInstance = new DependencyGraph(new FileInfoEx(Path.Combine(Environment.CurrentDirectory, "dependency_graph.json")));

        [IgnoreDataMember]
        public FileInfoEx File { get; }

        public DependencyGraph( FileInfoEx File )
        {
            this.File = File;
            if ( this.File.Exists )
            {
                StartPull();
            }
        }

        public async void StartPull()
        {
            await Pull();
        }

        public async Task GenerateDependencyGraphJson( IEnumerable<string> zones )
        {
            IEnumerable<AssetType> assetTypes = (AssetType[])typeof(AssetType).GetEnumValues();
            await GenerateDependencyGraphJson(zones, assetTypes);
        }
        public async Task GenerateDependencyGraphJson( IEnumerable<string> zones, IEnumerable<AssetType> assetTypes )
        {
            Console.WriteLine($"Reading {zones.Count()} zones...");
            Dictionary<string, Dictionary<string, AssetType>> zone_asset_assetType = await ZoneBuilder.ListAssets(assetTypes, zones);
            this.Clear();
            foreach ( KeyValuePair<string, Dictionary<string, AssetType>> item in zone_asset_assetType )
            {
                foreach ( KeyValuePair<string, AssetType> item1 in item.Value )
                {
                    if ( !this.TryGetValue($"{item1.Value}:{item1.Key}", out _) )
                    {
                        this[ $"{item1.Value}:{item1.Key}" ] = new List<string>();
                    }
                    this[ $"{item1.Value}:{item1.Key}" ].Add(item.Key);
                }
            }
            await Push();
            Console.WriteLine("Done!");
        }

        public Dictionary<string, AssetType> GetAssets( string zone )
        {
            Dictionary<string, AssetType> ret = new Dictionary<string, AssetType>();

            foreach ( KeyValuePair<string, List<string>> dependency_graph_item in this )
            {
                string[] assetLine = dependency_graph_item.Key.Split(':');
                if ( dependency_graph_item.Value.Any(z => z == zone) && AssetTypeUtil.IsSupportedAssetType(assetLine[ 0 ]) )
                {
                    if ( Enum.TryParse(assetLine[ 0 ], out AssetType assetType) )
                    {
                        ret[ assetLine[ 1 ] ] = assetType;
                    }
                }
            }
            return ret;
        }

        public Dictionary<string, Dictionary<string, AssetType>> GetAssets( IEnumerable<string> zones )
        {
            Dictionary<string, Dictionary<string, AssetType>> ret = new Dictionary<string, Dictionary<string, AssetType>>();
            foreach ( string zone in zones )
            {
                ret[ zone ] = GetAssets(zone);
            }
            return ret;
        }

        public async Task<Dictionary<string, AssetType>> GetAssetsAsync( string zone )
        {
            return await Task.Run(() => GetAssets(zone));
        }

        public async Task<Dictionary<string, Dictionary<string, AssetType>>> GetAssetsAsync( IEnumerable<string> zones )
        {
            return await Task.Run(() => GetAssets(zones));
        }

        public IEnumerable<string> GetAvailableAssetTypeNames()
        {
            HashSet<string> result = new HashSet<string>();
            foreach ( string item in this.Keys )
            {
                result.Add(item.Split(':')[ 0 ]);
            }
            return result;
        }

        public IEnumerable<AssetType> GetAvailableAssetTypes()
        {
            HashSet<AssetType> result = new HashSet<AssetType>();
            foreach ( string item in this.Keys )
            {
                if ( Enum.TryParse<AssetType>(item.Split(':')[ 0 ], true, out AssetType assetType) )
                {
                    result.Add(assetType);
                }
            }

            return result.OrderBy(item => item);
        }

        public string GetQueryString( KeyValuePair<string, AssetType> asset )
        {
            return GetQueryString(asset.Key, asset.Value);
        }

        public string GetQueryString( string assetName, AssetType assetType )
        {
            return $"{assetType}:{assetName}";
        }

        public bool HasUnknownAssets( ModCSV csv )
        {
            Dictionary<string, AssetType> assets = GetRequiredZones(csv).Values.Concat();
            return csv.Count > assets.Count();
        }

        public Dictionary<string, Dictionary<string, AssetType>> GetRequiredZones( ModCSV csv )
        {
            // Took a lot of inspiration from https://github.com/XLabsProject/iw4-zone-asset-finder/blob/main/iw4-zone-asset-finder/Commands/BuildRequirements.cs
            List<KeyValuePair<KeyValuePair<string, AssetType>, List<string>>> assets_zones = new List<KeyValuePair<KeyValuePair<string, AssetType>, List<string>>>();
            foreach ( KeyValuePair<string, AssetType> asset in csv )
            {
                string dependency_graph_assetQuery = GetQueryString(asset);
                if ( this.TryGetValue(dependency_graph_assetQuery, out List<string> queryResult) )
                {
                    if ( queryResult.Count > 0 )
                    {
                        assets_zones.Add(new KeyValuePair<KeyValuePair<string, AssetType>, List<string>>(new KeyValuePair<string, AssetType>(asset.Key, asset.Value), queryResult));
                    }
                    else
                    {
                        Debug.WriteLine($"{nameof(DependencyGraph)}.{nameof(GetRequiredZones)} : Warning, Missing Asset {dependency_graph_assetQuery}. In dependency graph but has no zones.");
                    }
                }
                else
                {
                    Debug.WriteLine($"{nameof(DependencyGraph)}.{nameof(GetRequiredZones)} : Warning, Missing Asset {dependency_graph_assetQuery}. Not in dependency graph.");
                }
            }
            Dictionary<string, Dictionary<string, AssetType>> finalZoneScore = new Dictionary<string, Dictionary<string, AssetType>>();
            while ( assets_zones.Count > 0 )
            {
                Dictionary<string, Dictionary<string, AssetType>> zoneScore = new Dictionary<string, Dictionary<string, AssetType>>();
                foreach ( KeyValuePair<KeyValuePair<string, AssetType>, List<string>> asset_zones in assets_zones )
                {
                    foreach ( string zone in asset_zones.Value )
                    {
                        if ( !zoneScore.TryGetValue(zone, out _) )
                        {
                            zoneScore[ zone ] = new Dictionary<string, AssetType>();
                        }
                        zoneScore[ zone ].Add(asset_zones.Key.Key, asset_zones.Key.Value);
                    }
                }
                string nextZone = zoneScore.OrderByDescending(o => o.Value.Count).First().Key;
                assets_zones.RemoveAll(o => o.Value.Contains(nextZone));
                finalZoneScore[ nextZone ] = zoneScore[ nextZone ];
            }
            return finalZoneScore;
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

        public async Task Pull()
        {
            string rawText = this.File.ReadAllText();
            Dictionary<string, List<string>> Dict = null;
            await Task.Run(() => { Dict = TinyJson.JSONParser.FromJson<Dictionary<string, List<string>>>(rawText); });
            this.Clear();
            this.AddRange(Dict);
        }

        public async Task Push()
        {
            string Dict = null;
            await Task.Run(() => { Dict = TinyJson.JSONWriter.ToJson(this); });
            File.WriteAllText(Dict);
        }
    }
}
