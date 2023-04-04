using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace EasyZoneBuilder.GUI
{
    /// <summary>
    /// Interaction logic for AppSettings.xaml
    /// </summary>
    public partial class AppSettings : Window
    {
        public AppSettings()
        {
            InitializeComponent();
        }



        private void Button_Click( object sender, RoutedEventArgs e )
        {
            OpenFileDialog selectIw4x = new OpenFileDialog
            {
                Title = "Select your IW4 executable",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "exe",
                Filter = "Executable files (*.exe)|*.exe",
                RestoreDirectory = false,
                ReadOnlyChecked = true,
                ShowReadOnly = true,
                Multiselect = false,
            };
            selectIw4x.ShowDialog();
            string fileName = Path.GetFileNameWithoutExtension(selectIw4x.FileName);
            if ( fileName == "iw4x" || fileName == "iw4m"  /* TODO add zonebuilder exe name */)
            {
                Core.Settings.DefaultInstance.TargetExecutablePath = selectIw4x.FileName;
                this.Close();
                return;
            }
            else
            {
                MessageBox.Show("Please select a supported executable!", "Error");
                return;
            }
        }
    }
}
