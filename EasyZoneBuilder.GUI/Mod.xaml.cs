using EasyZoneBuilder.Core;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EasyZoneBuilder.GUI
{
    /// <summary>
    /// Interaction logic for Mod.xaml
    /// </summary>
    public partial class Mod : UserControl
    {
        public static Mod Instance;
        public Mod()
        {
            Instance = this;
            InitializeComponent();
        }

        private void selectedMod_Loaded( object sender, RoutedEventArgs e )
        {
            selectedMod.ItemsSource = Core.Settings.IW4X.Mods;
        }

        private void CsvGrid_Loaded( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod )
            {
                sMod.CSV.Pull();
                CsvGrid.ItemsSource = sMod.CSV;
            }
            
        }

        public void ReadModCsvBtn_Click( object sender, RoutedEventArgs e )
        {
            CsvGrid_Loaded(sender, e);
            CsvGrid.Items.Refresh();
        }

        private async void writeFastFileBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod )
            {
                writeFastFileBtn.IsEnabled = false;
                sMod.CSV.Push();
                await sMod.BuildZone();
                writeFastFileBtn.IsEnabled = true;
            }
        }

        private void DeleteContextMenuItem_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod && CsvGrid.SelectedItem is KeyValuePair<string, ModCSV.EntryInfomation> kv )
            {
                sMod.CSV.Remove(kv.Key);
                sMod.CSV.Push();
                ReadModCsvBtn_Click(sender, e);
            }
        }
    }
}
