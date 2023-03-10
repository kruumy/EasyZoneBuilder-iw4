using EasyZoneBuilder.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasyZoneBuilder.Core
{
    public class ModCSV : Dictionary<string, AssetType>, IFileInfo, ISync
    {
        public ModCSV( FileInfo File )
        {
            this.File = File;
            if ( File.Exists )
            {
                Pull();
            }
        }

        public FileInfo File { get; private set; }

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
                this[ splitLine[ 1 ].Trim() ] = AssetTypeUtil.Parse(splitLine[ 0 ].Trim());
            }
        }

        public void Push()
        {
            StringBuilder sb = new StringBuilder();
            foreach ( KeyValuePair<string, AssetType> item in this )
            {
                sb.AppendLine($"{item.Value},{item.Key}");
            }
            System.IO.File.WriteAllText(File.FullName, sb.ToString());
        }

        public TempFileCopy TempCopy( FileInfo destination )
        {
            return new TempFileCopy(this.File, destination);
        }
    }
}
