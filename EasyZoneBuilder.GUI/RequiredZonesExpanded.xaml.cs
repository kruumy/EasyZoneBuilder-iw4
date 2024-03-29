﻿using EasyZoneBuilder.Core;
using System.Collections.Generic;
using System.Windows;

namespace EasyZoneBuilder.GUI
{
    /// <summary>
    /// Interaction logic for RequiredZonesExpanded.xaml
    /// </summary>
    public partial class RequiredZonesExpanded : Window
    {
        public RequiredZonesExpanded()
        {
            InitializeComponent();
        }

        private async void FindRequiredZonesBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( Mod.Instance.selectedMod.SelectedItem is Core.Mod sMod )
            {
                FindRequiredZonesBtn.IsEnabled = false;
                object oldContent = FindRequiredZonesBtn.Content;
                FindRequiredZonesBtn.Content = "Finding...";
                await DependencyGraph.DefaultInstance.Pull();
                RefreshAssetGrid();
                FindRequiredZonesBtn.Content = oldContent;
                FindRequiredZonesBtn.IsEnabled = true;
            }
        }

        private void RefreshAssetGrid()
        {
            if ( Mod.Instance.selectedMod.SelectedItem is Core.Mod sMod )
            {
                Dictionary<string, Dictionary<string, AssetType>> reqZones = DependencyGraph.DefaultInstance.GetRequiredZones(sMod.CSV);
                Dictionary<KeyValuePair<string, AssetType>, string> items = ConvertToNewDictionary(reqZones);
                foreach ( KeyValuePair<string, AssetType> item in sMod.CSV )
                {
                    if ( !items.ContainsKey(item) )
                    {
                        items.Add(item, string.Empty);
                    }
                }
                AssetGrid.ItemsSource = items;
            }
            AssetGrid.Items.Refresh();
        }

        private Dictionary<KeyValuePair<string, AssetType>, string> ConvertToNewDictionary( Dictionary<string, Dictionary<string, AssetType>> originalDict )
        {
            Dictionary<KeyValuePair<string, AssetType>, string> newDict = new Dictionary<KeyValuePair<string, AssetType>, string>();

            foreach ( KeyValuePair<string, Dictionary<string, AssetType>> kvp in originalDict )
            {
                foreach ( KeyValuePair<string, AssetType> asset in kvp.Value )
                {
                    newDict.Add(asset, kvp.Key);
                }
            }
            return newDict;
        }


        private void DeleteContextMenuItem_Click( object sender, RoutedEventArgs e )
        {
            if ( Mod.Instance.selectedMod.SelectedItem is Core.Mod sMod && AssetGrid.SelectedItems.Count > 0 )
            {
                foreach ( object item in AssetGrid.SelectedItems )
                {
                    sMod.CSV.Remove(((KeyValuePair<KeyValuePair<string, AssetType>, string>)item).Key.Key);
                }
                sMod.CSV.Push();
                RefreshAssetGrid();
            }
        }
    }
}
