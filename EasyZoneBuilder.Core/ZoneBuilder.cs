using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyZoneBuilder.Core
{
    public static class ZoneBuilder
    {
        public static FileInfoEx TargetExecutable { get; private set; }

        private static readonly int MAX_COMMANDS_PER_EXECUTATION = 30;

        public static void Initialize( FileInfoEx iw4x )
        {
            TargetExecutable = iw4x;
        }
        public static async Task<string> Execute( params string[] commands )
        {
            StringBuilder ret = new StringBuilder();
            IEnumerable<string[]> batchCommands = commands.SplitIntoChunks(MAX_COMMANDS_PER_EXECUTATION);
            int i = 0;
            foreach ( string[] bcommands in batchCommands )
            {
                i++;
                Console.WriteLine($"{i}/{batchCommands.Count()} with {bcommands.Count()} zones");
                StringBuilder args = new StringBuilder();
                args.Append("-nosteam -zonebuilder -stdout");
                foreach ( string com in bcommands )
                {
                    args.Append(" +");
                    args.Append(com);
                }
                args.Append(" +quit");
                using ( Process p = new Process() )
                {
                    p.StartInfo.FileName = TargetExecutable.FullName;
                    p.StartInfo.WorkingDirectory = TargetExecutable.Directory.FullName;
                    p.StartInfo.Arguments = args.ToString();
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    string raw = await p.StandardOutput.ReadToEndAsync();
                    string err = await p.StandardError.ReadToEndAsync();
                    if ( !string.IsNullOrEmpty(err.Trim()) )
                    {
                        throw new Exception($"{TargetExecutable.FullName} {args} : " + err);
                    }
                    ret.AppendLine(raw.Substring(raw.LastIndexOf('"') + 3).Replace("\r", string.Empty));
                }
            }
            return ret.ToString();
        }

        public static async Task<string[]> ExecuteLines( params string[] commands )
        {
            return (await Execute(commands)).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static async Task<Dictionary<string, AssetType>> ListAssets( string zone )
        {
            Dictionary<string, AssetType> ret = new Dictionary<string, AssetType>();
            string[] rawLines = await ExecuteLines($"verifyzone {zone}");
            for ( int i = 5; i < rawLines.Length; i++ )
            {
                ParseVerifyZoneLine(ref ret, rawLines[ i ]);
            }
            return ret;
        }

        private static void ParseVerifyZoneLine( ref Dictionary<string, AssetType> dictToAddTo, string line )
        {
            string[] rawSplit = line.Split(':');
            try
            {
                rawSplit[ 1 ] = rawSplit[ 1 ].Trim();
                if ( AssetTypeUtil.IsSupportedAssetType(rawSplit[ 1 ]) )
                {
                    if ( Enum.TryParse(rawSplit[ 1 ], out AssetType assetType) )
                    {
                        dictToAddTo[ rawSplit[ 2 ].Trim() ] = assetType;
                    }
                }
            }
            catch ( ArgumentException ) { }
            catch ( IndexOutOfRangeException ) { Debug.WriteLine(line); }
        }
        public static async Task<Dictionary<string, Dictionary<string, AssetType>>> ListAssets( IEnumerable<AssetType> assetTypes, IEnumerable<string> zones )
        {
            Dictionary<string, Dictionary<string, AssetType>> dict = await ListAssets(zones);
            foreach ( KeyValuePair<string, Dictionary<string, AssetType>> entry in dict.ToList() )
            {
                dict[ entry.Key ] = entry.Value.Where(item => assetTypes.Any(asset => item.Value == asset)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            return dict;
        }
        public static async Task<Dictionary<string, Dictionary<string, AssetType>>> ListAssets( IEnumerable<string> zones )
        {
            string[] commands = new string[ zones.Count() ];
            for ( int i = 0; i < commands.Length; i++ )
            {
                commands[ i ] = "verifyzone " + zones.ElementAt(i);
            }
            string[] rawLines = await ExecuteLines(commands); // TODO: find zones that cant be loaded and break the batch
            Dictionary<string, Dictionary<string, AssetType>> ret = new Dictionary<string, Dictionary<string, AssetType>>();

            string currentZone = string.Empty;
            for ( int i = 0; i < rawLines.Length; i++ )
            {
                string line = rawLines[ i ];
                if ( !string.IsNullOrEmpty(line.Trim()) )
                {
                    if ( line.StartsWith("Loading zone") )
                    {
                        currentZone = line.Substring(14, line.LastIndexOf('\'') - 14);
                        if ( !ret.TryGetValue(currentZone, out _) )
                        {
                            ret[ currentZone ] = new Dictionary<string, AssetType>();
                        }
                        i += 4;
                    }
                    else
                    {
                        Dictionary<string, AssetType> asset = ret[ currentZone ];
                        ParseVerifyZoneLine(ref asset, line);
                    }
                }
            }
            return ret;
        }

        public static async Task<IEnumerable<string>> ListAssets( IEnumerable<AssetType> assetTypes, string zone )
        {
            IEnumerable<KeyValuePair<string, AssetType>> dict = await ListAssets(zone);
            foreach ( AssetType assetType in assetTypes )
            {
                dict = dict.Where(item => item.Value == assetType);
            }
            return dict.Select(item => item.Key);
        }

        public static async Task<IEnumerable<string>> ListAssets( AssetType assetType, string zone )
        {
            Dictionary<string, AssetType> dict = await ListAssets(zone);
            return dict.Where(item => item.Value == assetType).Select(item => item.Key);
        }

        public static async Task<IEnumerable<string>> ListAssets( AssetType assetType, FileInfoEx file )
        {
            Dictionary<string, AssetType> dict = await ListAssets(file);
            return dict.Where(item => item.Value == assetType).Select(item => item.Key);
        }

        public static async Task<Dictionary<string, AssetType>> ListAssets( FileInfoEx file )
        {
            FileInfoEx destination = TargetExecutable.Directory.GetDirectory("zone").GetDirectory("english").GetFile(file.Name);
            using ( TempFileCopy temp = new TempFileCopy(file, destination) )
            {
                string command = Path.GetFileNameWithoutExtension(file.FullName);
                return await ListAssets(command);
            }
        }

        public static async Task BuildZone( ModCSV csv, FileInfoEx destination )
        {
            List<string> commands = new List<string>();
            await DependencyGraph.DefaultInstance.Pull();
            foreach ( string zone in DependencyGraph.DefaultInstance.GetRequiredZones(csv).Keys )
            {
                Debug.WriteLine(zone);
                commands.Add("loadzone " + zone);
            }
            commands = commands.Distinct().ToList();
            FileInfoEx csvdest = TargetExecutable.Directory.GetDirectory("zone_source").GetFile(csv.File.Name);
            if ( !csvdest.Directory.Exists )
            {
                csvdest.Directory.Touch();
            }
            csv.Push();
            using ( TempFileCopy tempcopy = csv.TempCopy(csvdest) )
            {
                commands.Add("buildzone mod");
                await ExecuteLines(commands.ToArray());
            }
            FileInfoEx buildZoneOutput = TargetExecutable.Directory.GetDirectory("zone").GetFile(csv.File.NameWithoutExtention + ".ff");
            buildZoneOutput.AssertExist();
            destination.Delete();
            buildZoneOutput.Move(destination);
        }
    }
}
