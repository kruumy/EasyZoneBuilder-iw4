using EasyZoneBuilder.Core.TinyJson;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using static EasyZoneBuilder.Core.ModCSV;

namespace EasyZoneBuilder.Core
{
    public class ModCSV : Dictionary<string, EntryInfomation>
    {
        public FileInfo File { get; private set; }
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
            foreach ( KeyValuePair<string, EntryInfomation> item in this )
            {
                sb.AppendLine($"{item.Value.AssetType},{item.Key},{item.Value.Zone}");
            }
            System.IO.File.WriteAllText(File.FullName, sb.ToString());
        }
        public void Pull()
        {
            if( !File.Exists )
            {
                Push();
            }
            this.Clear();
            foreach ( string line in System.IO.File.ReadAllLines(this.File.FullName) )
            {
                string[] splitLine = line.Split(new char[] { ',' },System.StringSplitOptions.RemoveEmptyEntries);
                this[ splitLine[ 1 ] ] = new EntryInfomation() 
                { 
                    AssetType = AssetTypeUtil.Parse(splitLine[0]),
                    Zone = splitLine[2]
                };
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

        public struct EntryInfomation
        {
            public AssetType AssetType { get; set; }
            public string Zone { get; set; }
        }
    }
}
