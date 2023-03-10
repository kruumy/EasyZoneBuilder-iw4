using EasyZoneBuilder.Core;
using System;
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

        private void ZonesListBox_Loaded( object sender, RoutedEventArgs e )
        {
            ZonesListBox.ItemsSource = Settings.IW4.Zones;
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
            if ( MessageBoxResult.OK == MessageBox.Show("Are you sure? This will take a few minutes.", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning) )
            {
                RegenerateDependencyGraphBtn.IsEnabled = false;
                object oldContent = RegenerateDependencyGraphBtn.Content;
                RegenerateDependencyGraphBtn.Content = "Generating...";
                await DependencyGraph.DefaultInstance.GenerateDependencyGraphJson(Settings.IW4.Zones);
                RegenerateDependencyGraphBtn.Content = oldContent;
                RegenerateDependencyGraphBtn.IsEnabled = true;
                MessageBox.Show($"Successfully written to '{DependencyGraph.DefaultInstance.File.Name}'!");
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
