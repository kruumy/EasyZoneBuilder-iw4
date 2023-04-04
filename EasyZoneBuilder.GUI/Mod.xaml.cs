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

        public Settings Settings => Core.Settings.DefaultInstance;

        private async void writeFastFileBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod )
            {
                writeFastFileBtn.IsEnabled = false;
                object oldContent = writeFastFileBtn.Content;
                writeFastFileBtn.Content = "Writing...";
                sMod.CSV.Push();
                await sMod.BuildZone();
                writeFastFileBtn.Content = oldContent;
                writeFastFileBtn.IsEnabled = true;
                MessageBox.Show($"Wrote to mod.ff successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void DeleteContextMenuItem_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod && CsvGrid.SelectedItems.Count > 0 )
            {
                while ( CsvGrid.SelectedItems.Count > 0 )
                {
                    sMod.CSV.Remove(((KeyValuePair<string, AssetType>)CsvGrid.SelectedItems[ 0 ]).Key);
                }
                sMod.CSV.Push();
            }
        }

        private async void writePrecacheBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod )
            {
                writePrecacheBtn.IsEnabled = false;
                object oldContent = writePrecacheBtn.Content;
                writePrecacheBtn.Content = "Writing...";
                sMod.CSV.Push();
                await sMod.SyncCSVToPrecache();
                sMod.Precache.Push();
                writePrecacheBtn.Content = oldContent;
                writePrecacheBtn.IsEnabled = true;
                MessageBox.Show("Wrote to _precache.gsc successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void DependencyGraphSettingsBtn_Click( object sender, RoutedEventArgs e )
        {
            DependencyGraphSettings window = new DependencyGraphSettings();
            window.ShowDialog();
        }

        private void RequiredZonesExpanded_Click( object sender, RoutedEventArgs e )
        {
            RequiredZonesExpanded window = new RequiredZonesExpanded();
            window.ShowDialog();
        }
    }
}
