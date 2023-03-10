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
            this.Clear();
            foreach ( string line in System.IO.File.ReadAllLines(this.File.FullName) )
            {
                string[] splitLine = line.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                if ( splitLine.Length > 1 )
                {
                    this[ splitLine[ 1 ].Trim() ] = AssetTypeUtil.Parse(splitLine[ 0 ].Trim());
                }
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
            File.Refresh();
        }

        public TempFileCopy TempCopy( FileInfo destination )
        {
            return new TempFileCopy(this.File, destination);
        }

        public void AddRange( IEnumerable<KeyValuePair<string, AssetType>> pairs )
        {
            foreach ( KeyValuePair<string, AssetType> pair in pairs )
            {
                this[ pair.Key ] = pair.Value;
            }
        }
    }
}
