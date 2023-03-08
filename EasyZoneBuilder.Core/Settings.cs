using EasyZoneBuilder.Core.TinyJson;
using System;
using System.IO;

namespace EasyZoneBuilder.Core
{
    public static class Settings
    {
        public struct Data
        {
            public string Iw4xPath;
        }
        public static IW4X IW4X { get; private set; }
        public static Data Value = new Data();
        public readonly static FileInfo FILE = new FileInfo(Path.Combine(Environment.CurrentDirectory,"settings.json"));
        static Settings()
        {
            if ( FILE.Exists )
            {
                Pull();
            }
        }

        public static void Push()
        {
            File.WriteAllText(FILE.FullName, Value.ToJson());
        }

        public static void Pull()
        {
            Value = TinyJson.JSONParser.FromJson<Data>(File.ReadAllText(FILE.FullName));
            if ( Value.Iw4xPath != null)
            {
                ZoneBuilder.Init(new FileInfo(Value.Iw4xPath));
                IW4X = new IW4X(Directory.GetParent(Value.Iw4xPath));
            }
        }
    }
}
