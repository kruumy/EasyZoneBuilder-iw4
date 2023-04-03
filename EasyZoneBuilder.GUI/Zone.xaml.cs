using EasyZoneBuilder.Core;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EasyZoneBuilder.GUI
{
    /// <summary>
    /// Interaction logic for NewZone.xaml
    /// </summary>
    public partial class Zone : UserControl
    {
        public Zone()
        {
            InitializeComponent();
        }

        public DependencyGraph Graph => Core.DependencyGraph.DefaultInstance;
        private void AddToCSVMenuItem_Click( object sender, RoutedEventArgs e )
        {
            if ( Mod.Instance.selectedMod.SelectedItem is Core.Mod sMod && AssetGrid.SelectedItems.Count > 0 )
            {
                foreach ( object item in AssetGrid.SelectedItems )
                {
                    if ( item is KeyValuePair<string, AssetType> kv )
                    {
                        sMod.CSV[ kv.Key ] = kv.Value;
                        sMod.CSV.Push();
                    }
                }
            }
        }
    }
}
