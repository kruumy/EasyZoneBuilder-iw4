using EasyZoneBuilder.Core.TinyJson;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace EasyZoneBuilder.Core
{
    public class ModCSV : Dictionary<string, AssetType>
    {
        public FileInfo File { get; private set; }
        public List<string> RequiredZones = new List<string>();
        private readonly string REQUIREDZONES_PREFIX = "// RequiredZones = ";
        public ModCSV( FileInfo File )
        {
            this.File = File;
            if ( File.Exists )
            {
                Pull();
            }
        }
        public void Push()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(REQUIREDZONES_PREFIX + RequiredZones.ToJson());
            foreach ( KeyValuePair<string, AssetType> item in this )
            {
                sb.AppendLine($"{item.Value},{item.Key}");
            }
            System.IO.File.WriteAllText(File.FullName, sb.ToString());
        }
        public void Pull()
        {
            this.Clear();
            foreach ( string line in System.IO.File.ReadAllLines(this.File.FullName) )
            {
                if (line.StartsWith(REQUIREDZONES_PREFIX) )
                {
                    RequiredZones = line.Substring(REQUIREDZONES_PREFIX.Length).FromJson<List<string>>();
                    continue;
                }
                int comma = line.IndexOf(',');
                if ( comma != -1 )
                {
                    AssetType value = AssetTypeUtil.Parse(line.Substring(0, comma));
                    string key = line.Substring(comma + 1);
                    this[ key ] = value;
                }
            }
        }
        public void Move( FileInfo File )
        {
            if ( this.File.FullName != File.FullName )
            {
                System.IO.File.Move(this.File.FullName, File.FullName);
            }
            this.File = File;
        }

        public TempFileCopy TempCopy( FileInfo destination )
        {
            return new TempFileCopy(this.File, destination);
        }
    }
}
