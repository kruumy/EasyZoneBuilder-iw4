using EasyZoneBuilder.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<Core.Mod> Mods => Core.Settings.IW4.Mods;

        public void ReadModCsvBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod )
            {
                if ( sMod.CSV.File.Exists )
                {
                    sMod.CSV.Pull();
                }
                if ( sMod.CSV.Count <= 0 && sMod.FastFile.Exists )
                {
                    if ( MessageBoxResult.Yes == MessageBox.Show("Empty mod.csv detected!\nWould you like to generate the mod.csv from the mod.ff?", "Notice", MessageBoxButton.YesNo, MessageBoxImage.Question) )
                    {
                        readFastFileContextMenu_Click(sender, e);
                    }
                }
            }
        }

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
                foreach ( object item in CsvGrid.SelectedItems )
                {
                    sMod.CSV.Remove(((KeyValuePair<string, AssetType>)item).Key);
                }
                sMod.CSV.Push();
                ReadModCsvBtn_Click(sender, e);
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

        private async void readFastFileContextMenu_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod && sMod.FastFile.Exists )
            {
                ReadModCsvBtn.IsEnabled = false;
                object oldContent = ReadModCsvBtn.Content;
                ReadModCsvBtn.Content = "Reading...";
                await sMod.ReadZone();
                ReadModCsvBtn_Click(sender, e);
                ReadModCsvBtn.Content = oldContent;
                ReadModCsvBtn.IsEnabled = true;
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
