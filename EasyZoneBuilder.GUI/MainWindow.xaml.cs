using EasyZoneBuilder.Core;
using System;
using System.Windows;

namespace EasyZoneBuilder.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if ( !Settings.DefaultInstance.IsTargetExecutablePathValid )
            {
                RunFirstTimeSetup();
            }
            if ( !DependencyGraph.DefaultInstance.File.Exists )
            {
                RunNoDependencyGraph();
            }

        }

        private void RunFirstTimeSetup()
        {
            AppSettings appSettings = new AppSettings();
            appSettings.ShowDialog();
            if ( !Settings.DefaultInstance.IsTargetExecutablePathValid )
            {
                Environment.Exit(0);
            }
        }

        private void RunNoDependencyGraph()
        {
            MessageBox.Show("No dependency graph detected!\nPress OK to generate one or download the default one from Github.", "Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            new DependencyGraphSettings().ShowDialog();
            Environment.Exit(0);
        }
    }
}
