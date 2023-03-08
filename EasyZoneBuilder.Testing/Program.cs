using EasyZoneBuilder.Core;
using System;
using System.IO;
using System.Linq;

namespace EasyZoneBuilder.Testing
{
    internal class Program
    {
        static void Main( string[] args )
        {
            ZoneBuilder.Init(new System.IO.FileInfo(@"F:\SteamLibrary\steamapps\common\Call of Duty Modern Warfare 2\iw4x.exe"));
            IW4X iw = new IW4X(new DirectoryInfo(@"F:\SteamLibrary\steamapps\common\Call of Duty Modern Warfare 2"));


            //ZoneBuilder.ListAssets(AssetType.xmodel,"oilrig").Result.ToList().ForEach(x => Console.WriteLine(x));
            /*
            iw.Mods[ 0 ].FastFile.Pull();
            Console.ReadLine();
            foreach (var item in iw.Mods[ 0 ].FastFile.XModels)
            {
                Console.WriteLine(item);
            }
            */
        }
    }
}
