using EasyZoneBuilder.Core;
using Microsoft.Win32;
using System;
using System.IO;
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
            if ( !Settings.File.Exists || string.IsNullOrEmpty(Settings.TargetExecutablePath) || !File.Exists(Settings.TargetExecutablePath) )
            {
                RunFirstTimeSetup();
            }
        }

        private void RunFirstTimeSetup()
        {
            MessageBox.Show("Please select your IW4 executable after pressing OK...", "First Time Setup");
            OpenFileDialog selectIw4x = new OpenFileDialog
            {
                Title = "Select your IW4 executable",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "exe",
                Filter = "Executable files (*.exe)|*.exe",
                FilterIndex = 2,
                RestoreDirectory = false,
                ReadOnlyChecked = true,
                ShowReadOnly = true,
                Multiselect = false,
            };
            selectIw4x.ShowDialog();
            string fileName = Path.GetFileNameWithoutExtension(selectIw4x.FileName);
            if ( fileName != "iw4x" && fileName != "iw4m"  /* TODO add zonebuilder exe name */)
            {
                MessageBox.Show("Not legal executable, exiting...", "First Time Setup");
                Environment.Exit(0);
            }
            Settings.TargetExecutablePath = selectIw4x.FileName;
        }
    }
}
