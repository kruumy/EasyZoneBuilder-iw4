﻿using EasyZoneBuilder.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyZoneBuilder.Core
{
    public class ModCSV : ObservableDictionary<string, AssetType>, IFileInfo, ISync
    {

        public ModCSV( FileInfoEx File )
        {
            this.File = File;
            if ( File.Exists )
            {
                Pull();
            }
            this.CollectionChanged += ModCSV_CollectionChanged;
        }

        private void ModCSV_CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
        {
            RaisePropertyChanged(nameof(RequiredZones));
            RaisePropertyChanged(nameof(HasUnknownAssets));
        }

        public IEnumerable<string> RequiredZones => Core.DependencyGraph.DefaultInstance.GetRequiredZones(this).Keys;
        public bool HasUnknownAssets => Core.DependencyGraph.DefaultInstance.HasUnknownAssets(this);

        public FileInfoEx File { get; }

        public void Pull()
        {
            this.Clear();
            foreach ( string line in this.File.ReadAllLines() )
            {
                string[] splitLine = line.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                if ( splitLine.Length > 1 )
                {
                    if ( Enum.TryParse(splitLine[ 0 ].Trim(), out AssetType type) )
                    {
                        this[ splitLine[ 1 ].Trim() ] = type;
                    }
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
            File.WriteAllText(sb.ToString());
        }

        public TempFileCopy TempCopy( FileInfoEx destination )
        {
            return new TempFileCopy(this.File, destination);
        }
    }
}
