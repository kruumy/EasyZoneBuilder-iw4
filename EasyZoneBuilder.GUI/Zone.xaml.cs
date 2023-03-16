using EasyZoneBuilder.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EasyZoneBuilder.GUI
{
    /// <summary>
    /// Interaction logic for Zone.xaml
    /// </summary>
    public partial class Zone : UserControl
    {
        public Zone()
        {
            InitializeComponent();
            cvs.Filter += Cvs_Filter;
        }

        private void selectedZone_Loaded( object sender, RoutedEventArgs e )
        {
            selectedZone.ItemsSource = Settings.IW4.GetZones();
        }
        public readonly CollectionViewSource cvs = new CollectionViewSource();
        private async void readFastFileBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedZone.SelectedItem is string ss && selectedAssetType.SelectedItem is string sat )
            {
                readFastFileBtn.IsEnabled = false;
                object oldBtnContext = readFastFileBtn.Content;
                readFastFileBtn.Content = "Reading...";
                AssetList.ItemsSource = Array.Empty<string>();
                AssetsFoundLabel.Content = "Assets Found: 0";
                IEnumerable<string> assets = await ZoneBuilder.ListAssets(AssetTypeUtil.Parse(sat), ss);
                cvs.Source = assets;
                AssetList.ItemsSource = cvs.View;
                AssetsFoundLabel.Content = "Assets Found: " + assets.Count();
                readFastFileBtn.Content = oldBtnContext;
                readFastFileBtn.IsEnabled = true;
            }
        }

        private void Cvs_Filter( object sender, FilterEventArgs e )
        {
            SearchBox.Text = SearchBox.Text.Trim();
            if ( string.IsNullOrEmpty(SearchBox.Text) )
            {
                e.Accepted = true;
            }
            else
            {
                if ( e.Item is string s )
                {
                    e.Accepted = s.Contains(SearchBox.Text); // TODO: search by word and not phrase
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }

        private void selectedAssetType_Loaded( object sender, RoutedEventArgs e )
        {
            selectedAssetType.ItemsSource = typeof(AssetType).GetEnumNames();
            selectedAssetType.SelectedValue = 0;
        }

        private void AddToCSVMenuItem_Click( object sender, RoutedEventArgs e )
        {
            if ( Mod.Instance.selectedMod.SelectedItem is Core.Mod sMod &&
                AssetList.SelectedItem is string selectedAsset &&
                this.selectedAssetType.SelectedItem is string selectedAssetType &&
                selectedZone.SelectedItem is string _selectedZone )
            {
                sMod.CSV[ selectedAsset ] = AssetTypeUtil.Parse(selectedAssetType);
                sMod.CSV.Push();
                Mod.Instance.ReadModCsvBtn_Click(sender, e);
            }
        }

        private void SearchBtn_Click( object sender, RoutedEventArgs e )
        {
            cvs?.View?.Refresh();
        }
    }
}
