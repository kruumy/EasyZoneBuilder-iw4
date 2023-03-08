using EasyZoneBuilder.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyZoneBuilder.GUI
{
    /// <summary>
    /// Interaction logic for Mod.xaml
    /// </summary>
    public partial class Mod : UserControl
    {
        public Mod()
        {
            InitializeComponent();
        }

        private void selectedMod_Loaded( object sender, RoutedEventArgs e )
        {
            selectedMod.ItemsSource = Core.Settings.IW4X.Mods;
        }

        private void CsvGrid_Loaded( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod)
            {
                CsvGrid.ItemsSource = sMod.FastFile.CSV;
            }
        }

        private void ReadModCsvBtn_Click( object sender, RoutedEventArgs e )
        {
            CsvGrid_Loaded(sender, e);
            CsvGrid.Items.Refresh();
        }

        private async void writeFastFileBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod )
            {
                writeFastFileBtn.IsEnabled = false;
                sMod.FastFile.CSV.Push();
                await sMod.FastFile.Push();
                writeFastFileBtn.IsEnabled = true;
            }
        }

        private async void readFastFileBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod )
            {
                readFastFileBtn.IsEnabled = false;
                await sMod.FastFile.Pull();
                sMod.FastFile.CSV.Push();
                ReadModCsvBtn_Click(sender, e);
                readFastFileBtn.IsEnabled = true;
            }
        }

        private void DeleteContextMenuItem_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod && CsvGrid.SelectedItem is KeyValuePair<string,AssetType> kv)
            {
                sMod.FastFile.CSV.Remove(kv.Key);
                sMod.FastFile.CSV.Push();
                ReadModCsvBtn_Click(sender, e);
            }
        }
    }
}
