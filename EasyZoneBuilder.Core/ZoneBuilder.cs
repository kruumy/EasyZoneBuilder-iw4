using EasyZoneBuilder.Core.TinyJson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EasyZoneBuilder.Core
{
    public static class ZoneBuilder
    {
        public static FileInfo Iw4x { get; private set; }

        public static class Default
        {
            public static string[] XAnims => TinyJson.JSONParser.FromJson<string[]>(File.ReadAllText("default_xanims.json"));
            public static string[] XModels => TinyJson.JSONParser.FromJson<string[]>(File.ReadAllText("default_xmodels.json"));
            public static string[] Get( AssetType assetType )
            {
                if ( assetType == AssetType.xanim )
                {
                    return XAnims;
                }
                else if ( assetType == AssetType.xmodel )
                {
                    return XModels;
                }
                else
                {
                    return null;
                }
            }

            public static async Task Regenerate()
            {
                File.WriteAllText("default_xanims.json", (await ZoneBuilder.ListAssets(AssetType.xanim)).ToJson());
                File.WriteAllText("default_xmodels.json", (await ZoneBuilder.ListAssets(AssetType.xmodel)).ToJson());
            }
        }
        public static void Init( FileInfo iw4x )
        {
            Iw4x = iw4x;
        }
        public static async Task<string> Execute( params string[] commands )
        {
            StringBuilder args = new StringBuilder();
            args.Append("-zonebuilder -stdout");
            foreach ( var com in commands )
            {
                args.Append(" +");
                args.Append(com);
            }
            Process p = new Process();
            p.StartInfo.FileName = Iw4x.FullName;
            p.StartInfo.WorkingDirectory = Iw4x.Directory.FullName;
            p.StartInfo.Arguments = args.ToString();
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            string raw = await p.StandardOutput.ReadToEndAsync();
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
            foreach ( var item in zones )
            {
                command.Add("loadzone " + item);
            }
            command.Add("listassets " + assetType.ToString());
            string[] lines = await ExecuteLines(command.ToArray());
            // TODO cut out the first "loading" line
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
            var destination = new FileInfo(Path.Combine(Iw4x.Directory.FullName, @"zone\english", file.Name));
            using ( TempFileCopy temp = new TempFileCopy(file, destination) )
            {
                string command = Path.GetFileNameWithoutExtension(file.FullName);
                return await ListAssets(assetType, command);
            }
        }

        public static async Task BuildZone( ModCSV csv, FileInfo destination )
        {
            List<string> commands = new List<string>(csv.RequiredZones.Count + 1);
            foreach ( string zone in csv.RequiredZones )
            {
                commands.Add("loadzone " + zone);
            }
            FileInfo csvdest = new FileInfo(Path.Combine(Iw4x.Directory.FullName, @"zone_source", csv.File.Name));
            csv.Push();
            using (var tempcopy = csv.TempCopy(csvdest) )
            {
                commands.Add("buildzone mod");
                await ExecuteLines(commands.ToArray());
            }
            System.IO.File.Move(Path.Combine(Iw4x.Directory.FullName, "zone",Path.GetFileNameWithoutExtension(csv.File.FullName) + ".ff"),destination.FullName);
        }
    }
}
