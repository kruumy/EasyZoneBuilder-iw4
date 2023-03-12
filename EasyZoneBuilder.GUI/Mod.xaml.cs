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
            selectedMod.ItemsSource = Core.Settings.IW4.Mods;
        }

        public void ReadModCsvBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod )
            {
                sMod.CSV.File.Refresh();
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
                detectedZonesBox.Text = string.Empty;
                CsvGrid.ItemsSource = sMod.CSV;
            }
            CsvGrid.Items.Refresh();
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

        private void DeleteContextMenuItem_Click( object sender, RoutedEventArgs e )
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

        private void writePrecacheBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod )
            {
                writePrecacheBtn.IsEnabled = false;
                sMod.CSV.Push();
                sMod.SyncCSVToPrecache();
                sMod.Precache.Push();
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

        private async void FindRequiredZonesBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedMod.SelectedItem is Core.Mod sMod )
            {
                FindRequiredZonesBtn.IsEnabled = false;
                object oldContent = FindRequiredZonesBtn.Content;
                FindRequiredZonesBtn.Content = "Finding...";
                detectedZonesBox.Text = string.Empty;
                await DependencyGraph.DefaultInstance.Pull();
                foreach ( string zone in DependencyGraph.DefaultInstance.GetRequiredZones(sMod.CSV) )
                {
                    detectedZonesBox.Text += zone + ", ";
                }
                if ( detectedZonesBox.Text.Length > 2 )
                {
                    detectedZonesBox.Text = detectedZonesBox.Text.Remove(detectedZonesBox.Text.Length - 2, 2);
                }
                FindRequiredZonesBtn.Content = oldContent;
                FindRequiredZonesBtn.IsEnabled = true;
            }
        }

        private void DependencyGraphSettingsBtn_Click( object sender, RoutedEventArgs e )
        {
            DependencyGraphSettings window = new DependencyGraphSettings();
            window.ShowDialog();
        }
    }
}
