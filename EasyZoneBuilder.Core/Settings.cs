using EasyZoneBuilder.Core.TinyJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace EasyZoneBuilder.Core
{
    public static class Settings
    {
        public static bool IsTargetExecutablePathValid => Settings.File.Exists && !string.IsNullOrEmpty(Settings.TargetExecutablePath) && System.IO.File.Exists(Settings.TargetExecutablePath);
        private static string _TargetExecutablePath = string.Empty;
        public static string TargetExecutablePath
        {
            get
            {
                return _TargetExecutablePath;
            }
            set
            {
                _TargetExecutablePath = value;
                if ( !string.IsNullOrEmpty(value) && System.IO.File.Exists(value) )
                {
                    FileInfo fileInfo = new FileInfo(value);
                    if ( fileInfo != null && fileInfo.Exists )
                    {
                        ZoneBuilder.Initialize(fileInfo);
                        IW4 = new IW4(fileInfo.Directory);
                    }
                }
                Push();
            }
        }

        [IgnoreDataMember]
        public static IW4 IW4 { get; private set; }
        [IgnoreDataMember]
        public static FileInfo File => new FileInfo(Path.Combine(Environment.CurrentDirectory, "EasyZoneBuilder.appsettings.json"));
        static Settings()
        {
            if ( File.Exists )
            {
                Pull();
            }
        }

        public static void Pull()
        {
            string rawFileContents = System.IO.File.ReadAllText(File.FullName);
            Dictionary<string, string> json = TinyJson.JSONParser.FromJson<Dictionary<string, string>>(rawFileContents);
            TargetExecutablePath = (string)(json[ nameof(TargetExecutablePath) ]);
        }
        public static void Push()
        {
            Dictionary<string, string> json = new Dictionary<string, string>();
            json[ nameof(TargetExecutablePath) ] = TargetExecutablePath;
            System.IO.File.WriteAllText(File.FullName, json.ToJson());
        }
    }
}
