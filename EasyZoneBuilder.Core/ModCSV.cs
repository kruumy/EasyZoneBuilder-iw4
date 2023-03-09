using EasyZoneBuilder.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static EasyZoneBuilder.Core.ModCSV;

namespace EasyZoneBuilder.Core
{
    public class ModCSV : Dictionary<string, EntryInfomation>, IFileInfo, ISync
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
            if ( !File.Exists )
            {
                Push();
            }
            this.Clear();
            foreach ( string line in System.IO.File.ReadAllLines(this.File.FullName) )
            {
                string[] splitLine = line.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                this[ splitLine[ 1 ].Trim() ] = new EntryInfomation()
                {
                    AssetType = AssetTypeUtil.Parse(splitLine[ 0 ].Trim()),
                    Zone = splitLine[ 2 ].Trim()
                };
            }
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
