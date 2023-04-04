using EasyZoneBuilder.Core.Interfaces;
using EasyZoneBuilder.Core.TinyJson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace EasyZoneBuilder.Core
{
    public class Settings : INotifyPropertyChanged, ISync
    {
        public static Settings DefaultInstance = new Settings(new FileInfoEx(Path.Combine(Environment.CurrentDirectory, "EasyZoneBuilder.appsettings.json")));
        private IW4 _IW4;
        private string _TargetExecutablePath = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public FileInfoEx File { get; }
        public bool IsTargetExecutablePathValid => File.Exists && !string.IsNullOrEmpty(this.TargetExecutablePath) && System.IO.File.Exists(this.TargetExecutablePath);

        public IW4 IW4
        {
            get => _IW4;
            private set
            {
                _IW4 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IW4)));
            }
        }

        public string TargetExecutablePath
        {
            get => _TargetExecutablePath;
            set
            {
                _TargetExecutablePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TargetExecutablePath)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsTargetExecutablePathValid)));
                if ( !string.IsNullOrEmpty(value) && System.IO.File.Exists(value) )
                {
                    FileInfoEx fileInfo = new FileInfoEx(value);
                    if ( fileInfo != null && fileInfo.Exists )
                    {
                        ZoneBuilder.Initialize(fileInfo);
                        IW4 = new IW4(fileInfo.Directory);
                    }
                }
                Push();
            }
        }

        private Settings( FileInfoEx File )
        {
            this.File = File;
            if ( File.Exists )
            {
                Pull();
            }
        }

        public void Pull()
        {
            string rawFileContents = File.ReadAllText();
            Dictionary<string, string> json = TinyJson.JSONParser.FromJson<Dictionary<string, string>>(rawFileContents);
            TargetExecutablePath = (string)(json[ nameof(TargetExecutablePath) ]);
        }

        public void Push()
        {
            Dictionary<string, string> json = new Dictionary<string, string>();
            json[ nameof(TargetExecutablePath) ] = TargetExecutablePath;
            File.WriteAllText(json.ToJson());
        }
    }
}
