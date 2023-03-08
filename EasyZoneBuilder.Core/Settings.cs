using EasyZoneBuilder.Core.TinyJson;
using System.IO;

namespace EasyZoneBuilder.Core
{
    public static class Settings
    {
        public struct Data
        {
            public string Iw4xPath;
        }
        public readonly static IW4X IW4X;
        public static Data Value;
        public readonly static string FILENAME = "settings.json";
        static Settings()
        {
            if ( File.Exists(FILENAME) )
            {
                Value = TinyJson.JSONParser.FromJson<Data>(File.ReadAllText(FILENAME));
                ZoneBuilder.Init(new FileInfo(Value.Iw4xPath));
                IW4X = new IW4X(Directory.GetParent(Value.Iw4xPath));
            }
            else
            {
                Value = new Data();
                File.WriteAllText(FILENAME, Value.ToJson());
            }
        }
    }
}
