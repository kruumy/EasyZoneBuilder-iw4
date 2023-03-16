﻿using EasyZoneBuilder.Core.TinyJson;
using System;
using System.Collections.Generic;
using System.IO;

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

        public static IW4 IW4 { get; private set; }
        public static FileInfoEx File => new FileInfoEx(Path.Combine(Environment.CurrentDirectory, "EasyZoneBuilder.appsettings.json"));
        static Settings()
        {
            if ( File.Exists )
            {
                Pull();
            }
        }

        public static void Pull()
        {
            string rawFileContents = File.ReadAllText();
            Dictionary<string, string> json = TinyJson.JSONParser.FromJson<Dictionary<string, string>>(rawFileContents);
            TargetExecutablePath = (string)(json[ nameof(TargetExecutablePath) ]);
        }
        public static void Push()
        {
            Dictionary<string, string> json = new Dictionary<string, string>();
            json[ nameof(TargetExecutablePath) ] = TargetExecutablePath;
            File.WriteAllText(json.ToJson());
        }
    }
}
