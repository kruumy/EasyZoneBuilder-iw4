using EasyZoneBuilder.Core;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static System.Net.WebRequestMethods;

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
        }

        private void selectedZone_Loaded( object sender, RoutedEventArgs e )
        {
            selectedZone.ItemsSource = Settings.IW4X.Zones;
        }
        private CollectionViewSource cvs = new CollectionViewSource();
        private async void readFastFileBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( selectedZone.SelectedItem is string ss  && selectedAssetType.SelectedItem is string sat)
            {
                readFastFileBtn.IsEnabled = false;
                cvs.Source = await ZoneBuilder.ListAssets(AssetTypeUtil.Parse(sat), ss);
                AssetList.ItemsSource = cvs.View;
                cvs.Filter += Cvs_Filter;
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
                if (e.Item is string s)
                {
                    e.Accepted = s.Contains(SearchBox.Text);
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
        }

        private void AddToCSVMenuItem_Click( object sender, RoutedEventArgs e )
        {
            if ( Mod.Instance.selectedMod.SelectedItem is Core.Mod sMod &&
                AssetList.SelectedItem is string selectedAsset &&
                this.selectedAssetType.SelectedItem is string selectedAssetType &&
                selectedZone.SelectedItem is string _selectedZone)
            {
                sMod.CSV[ selectedAsset ] = new ModCSV.EntryInfomation()
                {
                    Zone = _selectedZone,
                    AssetType = AssetTypeUtil.Parse(selectedAssetType)
                };
                sMod.CSV.Push();
            }
        }

        private void SearchBtn_Click( object sender, RoutedEventArgs e )
        {
            cvs?.View?.Refresh();
        }

        private void SearchBox_TextChanged( object sender, TextChangedEventArgs e )
        {
            cvs?.View?.Refresh();
        }
    }
}
