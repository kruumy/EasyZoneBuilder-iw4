using EasyZoneBuilder.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EasyZoneBuilder.GUI
{
    /// <summary>
    /// Interaction logic for NewZone.xaml
    /// </summary>
    public partial class NewZone : UserControl
    {
        public NewZone()
        {
            InitializeComponent();
            cvs.Filter += Cvs_Filter;
        }

        private void Cvs_Filter( object sender, FilterEventArgs e )
        {
            e.Accepted = false;
            if ( e.Item is KeyValuePair<string, AssetType> asset )
            {
                if ( !string.IsNullOrEmpty(SelectAssetTypeComboBox.Text) )
                {
                    e.Accepted = SelectAssetTypeComboBox.Text == "None" || asset.Value == AssetTypeUtil.Parse(SelectAssetTypeComboBox.Text);
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
                AssetGrid.ItemsSource = Array.Empty<string>();
                cvs.Source = await DependencyGraphUtil.GetAssetsAsync(ss);
                AssetGrid.ItemsSource = cvs.View;
                ReadZoneBtn.Content = oldBtnContext;
                ReadZoneBtn.IsEnabled = true;
            }
        }

        private async void SelectZoneComboBox_Loaded( object sender, RoutedEventArgs e )
        {
            if ( DependencyGraphUtil.File.Exists )
            {
                SelectZoneComboBox.ItemsSource = await DependencyGraphUtil.GetZones();
                SelectZoneComboBox.SelectedIndex = 0;
            }
        }

        private void SelectAssetTypeComboBox_Loaded( object sender, RoutedEventArgs e )
        {
            SelectAssetTypeComboBox.Items.Clear();
            SelectAssetTypeComboBox.Items.Add("None");
            SelectAssetTypeComboBox.SelectedIndex = 0;
            foreach ( string item in typeof(AssetType).GetEnumNames() )
            {
                SelectAssetTypeComboBox.Items.Add(item);
            }
        }

        private async void SearchBtn_Click( object sender, RoutedEventArgs e )
        {
            SearchTextBox.IsEnabled = false;
            SearchBtn.IsEnabled = false;
            object oldContent = SearchBtn.Content;
            SearchBtn.Content = "Filtering...";
            await Dispatcher.InvokeAsync(() => cvs.View.Refresh(), System.Windows.Threading.DispatcherPriority.Background);
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
