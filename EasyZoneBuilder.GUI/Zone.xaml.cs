using EasyZoneBuilder.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
            cvs.Filter += Cvs_Filter;
        }

        public DependencyGraph Graph => Core.DependencyGraph.DefaultInstance;

        private void Cvs_Filter( object sender, FilterEventArgs e )
        {
            e.Accepted = false;
            if ( e.Item is KeyValuePair<string, AssetType> asset )
            {
                if ( !string.IsNullOrEmpty(SelectAssetTypeComboBox.Text) )
                {
                    e.Accepted = SelectAssetTypeComboBox.Text == "None" || Enum.TryParse(SelectAssetTypeComboBox.Text, out AssetType assetType) && assetType == asset.Value;
                }
                if ( e.Accepted )
                {
                    if ( string.IsNullOrEmpty(SearchTextBox.Text.Trim()) )
                    {
                        e.Accepted = true;
                    }
                    else
                    {
                        e.Accepted = asset.Key.Contains(SearchTextBox.Text);
                    }
                }
            }
        }

        public readonly CollectionViewSource cvs = new CollectionViewSource();
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
                        Mod.Instance.ReadModCsvBtn_Click(sender, e);
                    }
                }
            }
        }

        private async void ReadZoneBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( SelectZoneComboBox.SelectedItem is string ss )
            {
                ReadZoneBtn.IsEnabled = false;
                object oldBtnContext = ReadZoneBtn.Content;
                ReadZoneBtn.Content = "Reading...";
                await RefreshAssetGrid();
                ReadZoneBtn.Content = oldBtnContext;
                ReadZoneBtn.IsEnabled = true;
            }
        }

        private async Task RefreshAssetGrid()
        {
            if ( SelectZoneComboBox.SelectedItem is string ss )
            {
                AssetGrid.ItemsSource = Array.Empty<string>();
                cvs.Source = await DependencyGraph.DefaultInstance.GetAssetsAsync(ss);
                AssetGrid.ItemsSource = cvs.View;
            }
            AssetGrid.Items.Refresh();
        }

        private async void SearchBtn_Click( object sender, RoutedEventArgs e )
        {
            SearchTextBox.IsEnabled = false;
            SearchBtn.IsEnabled = false;
            object oldContent = SearchBtn.Content;
            SearchBtn.Content = "Filtering...";
            await RefreshAssetGrid();
            //await cvs.Dispatcher.InvokeAsync(() => cvs.View.Refresh(), System.Windows.Threading.DispatcherPriority.Input);
            SearchBtn.Content = oldContent;
            SearchBtn.IsEnabled = true;
            SearchTextBox.IsEnabled = true;
        }

        private void SearchTextBox_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
        {
            if ( e.Key == System.Windows.Input.Key.Enter && SearchTextBox.IsEnabled )
            {
                SearchBtn_Click(sender, e);
            }
        }
    }
}
