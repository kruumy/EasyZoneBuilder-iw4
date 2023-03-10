using EasyZoneBuilder.Core.TinyJson;
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
        public static FileInfo TargetExecutable { get; private set; }

        public static string LastError { get; private set; } = string.Empty;

        public static class Default
        {
            public static string[] XAnims => TinyJson.JSONParser.FromJson<string[]>(File.ReadAllText("default_xanims.json"));
            public static string[] XModels => TinyJson.JSONParser.FromJson<string[]>(File.ReadAllText("default_xmodels.json"));
            public static string[] Get( AssetType assetType )
            {
                switch ( assetType )
                {
                    case AssetType.xanim:
                        return XAnims;
                    case AssetType.xmodel:
                        return XModels;
                    default:
                        return null;
                }
            }

            public static async Task Regenerate()
            {
                File.WriteAllText("default_xanims.json", (await ZoneBuilder.ListAssets(AssetType.xanim)).ToJson());
                File.WriteAllText("default_xmodels.json", (await ZoneBuilder.ListAssets(AssetType.xmodel)).ToJson());
            }
        }
        public static void Initialize( FileInfo iw4x )
        {
            TargetExecutable = iw4x;
        }
        public static async Task<string> Execute( params string[] commands )
        {
            StringBuilder args = new StringBuilder();
            args.Append("-zonebuilder -stdout");
            foreach ( string com in commands )
            {
                args.Append(" +");
                args.Append(com);
            }
            Process p = new Process();
            p.StartInfo.FileName = TargetExecutable.FullName;
            p.StartInfo.WorkingDirectory = TargetExecutable.Directory.FullName;
            p.StartInfo.Arguments = args.ToString();
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            string raw = await p.StandardOutput.ReadToEndAsync();
            LastError = await p.StandardError.ReadToEndAsync();
            p.Dispose();
            return raw.Substring(raw.LastIndexOf('"') + 3).Replace("\r", string.Empty); ;
        }

        public static async Task<string[]> ExecuteLines( params string[] commands )
        {
            return (await Execute(commands)).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static async Task<IEnumerable<string>> ListAssets( AssetType assetType, params string[] zones )
        {
            List<string> command = new List<string>(zones.Length + 1);
            foreach ( string item in zones )
            {
                command.Add("loadzone " + item);
            }
            command.Add("listassets " + assetType.ToString());
            string[] lines = await ExecuteLines(command.ToArray());
            if ( lines.Length > 0 && lines[ 0 ].Contains("Loading") )
            {
                lines = lines.Skip(1).ToArray();
            }
            if ( zones.Length > 0 )
            {
                return lines.Difference(Default.Get(assetType));
            }
            else
            {
                return lines;
            }
        }

        public static async Task<IEnumerable<string>> ListAssets( AssetType assetType, FileInfo file )
        {
            FileInfo destination = new FileInfo(Path.Combine(TargetExecutable.Directory.FullName, @"zone\english", file.Name));
            using ( TempFileCopy temp = new TempFileCopy(file, destination) )
            {
                string command = Path.GetFileNameWithoutExtension(file.FullName);
                return await ListAssets(assetType, command);
            }
        }

        public static async Task BuildZone( ModCSV csv, FileInfo destination )
        {
            List<string> commands = new List<string>();
            foreach ( string zone in DependencyGraphUtil.GetRequiredZones(csv) )
            {
                Debug.WriteLine(zone);
                commands.Add("loadzone " + zone);
            }
            commands = commands.Distinct().ToList();
            FileInfo csvdest = new FileInfo(Path.Combine(TargetExecutable.Directory.FullName, @"zone_source", csv.File.Name));
            if ( !csvdest.Directory.Exists )
            {
                csvdest.Directory.Create();
            }
            csv.Push();
            using ( TempFileCopy tempcopy = csv.TempCopy(csvdest) )
            {
                commands.Add("buildzone mod");
                await ExecuteLines(commands.ToArray());
            }
            string buildZoneOutput = Path.Combine(TargetExecutable.Directory.FullName, "zone", Path.GetFileNameWithoutExtension(csv.File.FullName) + ".ff");
            if ( File.Exists(buildZoneOutput) )
            {
                System.IO.File.Delete(destination.FullName);
                System.IO.File.Move(buildZoneOutput, destination.FullName);
            }
            else
            {
                throw new Exception(LastError);
            }
        }
    }
}
