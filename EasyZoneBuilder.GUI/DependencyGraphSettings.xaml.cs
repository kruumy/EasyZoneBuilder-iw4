using EasyZoneBuilder.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace EasyZoneBuilder.GUI
{
    /// <summary>
    /// Interaction logic for DependencyGraphSettings.xaml
    /// </summary>
    public partial class DependencyGraphSettings : Window
    {
        public DependencyGraphSettings()
        {
            InitializeComponent();
        }

        private ConsoleWriter ConsoleWriter;

        private void ConsoleOutputBox_Loaded( object sender, RoutedEventArgs e )
        {
            ConsoleWriter = new ConsoleWriter(ConsoleOutputBox);
        }

        private void ConsoleOutputBox_Unloaded( object sender, RoutedEventArgs e )
        {
            ConsoleWriter.Dispose();
            ConsoleWriter = null;
        }

        private async void RegenerateDependencyGraphBtn_Click( object sender, RoutedEventArgs e )
        {
            if ( GetSelectedAssetTypes().Count() <= 0 )
            {
                Console.WriteLine("Please select atleast one asset type!");
                return;
            }
            else if ( GetSelectedZones().Count() <= 0 )
            {
                Console.WriteLine("Please select atleast one zone!");
                return;
            }
            else if ( MessageBoxResult.OK == MessageBox.Show("Are you sure? This will take a few minutes.", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning) )
            {
                AllowWindowClose = false;
                RegenerateDependencyGraphBtn.IsEnabled = false;
                object oldContent = RegenerateDependencyGraphBtn.Content;
                RegenerateDependencyGraphBtn.Content = "Generating...";
                await DependencyGraph.DefaultInstance.GenerateDependencyGraphJson(GetSelectedZones(), GetSelectedAssetTypes());
                RegenerateDependencyGraphBtn.Content = oldContent;
                RegenerateDependencyGraphBtn.IsEnabled = true;
                AllowWindowClose = true;
                MessageBox.Show($"Successfully written to '{DependencyGraph.DefaultInstance.File.Name}'!\nPlease restart EasyZoneBuilder.");
                DependencyGraphInfoBox_Loaded(sender, e);

            }
        }

        private async void DependencyGraphInfoBox_Loaded( object sender, RoutedEventArgs e )
        {
            if ( DependencyGraph.DefaultInstance.File.Exists )
            {
                await DependencyGraph.DefaultInstance.Pull();
                DependencyGraphInfoBox.Items.Clear();
                DependencyGraphInfoBox.Items.Add($"Asset Count = {DependencyGraph.DefaultInstance.Count}");
                DependencyGraphInfoBox.Items.Add($"Zones Count = {DependencyGraph.DefaultInstance.GetZones().Count()}");
            }
        }

        private void SelectAssetTypes_Loaded( object sender, RoutedEventArgs e )
        {
            foreach ( string item in typeof(AssetType).GetEnumNames() )
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = new TextBlock { Text = item }
                };
                SelectAssetTypes.Children.Add(checkBox);

            }
        }

        private IEnumerable<string> GetSelectedZones()
        {
            foreach ( object item in SelectZones.Children )
            {
                if ( item is CheckBox checkbox && (bool)checkbox.IsChecked )
                {
                    yield return (checkbox.Content as TextBlock).Text;
                }
            }
        }

        private IEnumerable<AssetType> GetSelectedAssetTypes()
        {
            foreach ( object item in SelectAssetTypes.Children )
            {
                if ( item is CheckBox checkbox && (bool)checkbox.IsChecked )
                {
                    yield return (AssetType)Enum.Parse(typeof(AssetType), (checkbox.Content as TextBlock).Text.ToString());
                }
            }
        }

        private static void SetAllCheckBoxes( UIElementCollection collection, bool ischecked )
        {
            foreach ( object item in collection )
            {
                if ( item is CheckBox checkbox )
                {
                    checkbox.IsChecked = ischecked;
                }
            }
        }
        private void SelectAllButton_Click( object sender, RoutedEventArgs e )
        {
            SetAllCheckBoxes(SelectAssetTypes.Children, true);
        }

        private void SelectNoneButton_Click( object sender, RoutedEventArgs e )
        {
            SetAllCheckBoxes(SelectAssetTypes.Children, false);
        }

        private void SelectZones_Loaded( object sender, RoutedEventArgs e )
        {
            foreach ( string item in Core.Settings.DefaultInstance.IW4.GetZones() )
            {
                SelectZones.Children.Add(new CheckBox
                {
                    Content = new TextBlock { Text = item }
                });
            }
        }

        private void SelectNoneZonesButton_Click( object sender, RoutedEventArgs e )
        {
            SetAllCheckBoxes(SelectZones.Children, false);
        }

        private void SelectAllZonesButton_Click( object sender, RoutedEventArgs e )
        {
            SetAllCheckBoxes(SelectZones.Children, true);
        }

        private void SelectRecommendedZonesButton_Click( object sender, RoutedEventArgs e )
        {
            foreach ( object item in SelectZones.Children )
            {
                if ( item is CheckBox checkbox && checkbox.Content is TextBlock text )
                {
                    if (
                        !text.Text.Contains("mp_") &&
                        !text.Text.Contains("_mp") &&
                        !text.Text.Contains("iw4x")
                        )
                    {
                        checkbox.IsChecked = true;
                    }
                    else
                    {
                        checkbox.IsChecked = false;
                    }
                }
            }
        }


        private bool AllowWindowClose = true;
        private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            e.Cancel = !AllowWindowClose;
        }
    }

    internal class ConsoleWriter : TextWriter, IDisposable
    {
        private readonly TextBox _output;

        public ConsoleWriter( TextBox output )
        {
            _output = output;
            Console.SetOut(this);
        }

        public override void Write( char value )
        {
            _output.Dispatcher.Invoke(() => _output.AppendText(value.ToString()));
        }

        public override Encoding Encoding => Encoding.Unicode;

        protected override void Dispose( bool disposing )
        {
            Console.SetOut(Console.Out);
            base.Dispose(disposing);
        }
    }
}
