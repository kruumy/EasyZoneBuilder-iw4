using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EasyZoneBuilder.Core
{
    public class Precache : Dictionary<string, AssetType>
    {
        public FileInfo File;
        private static readonly string ANIM_FUNCNAME = "PrecacheMPAnim";
        private static readonly string MODEL_FUNCNAME = "PrecacheMPModel";
        public Precache( FileInfo File )
        {
            this.File = File;
            if ( this.File.Exists )
            {
                Pull();
                Push();
            }
        }
        public void Push()
        {
            const string PRECACHE_PROLOUGE = "#include maps\\mp\\_utility;\n#include common_scripts\\utility;\n#using_animtree(\"multiplayer\");\nprecache()\n{\n";
            const string PRECACHE_EPOLOUGE = "}";
            StringBuilder sb = new StringBuilder();
            sb.Append(PRECACHE_PROLOUGE);
            foreach ( KeyValuePair<string, AssetType> entry in this )
            {
                if ( entry.Value == AssetType.xanim )
                {
                    sb.AppendLine($"{ANIM_FUNCNAME}(\"{entry.Key}\");");
                }
                else if ( entry.Value == AssetType.xmodel )
                {
                    sb.AppendLine($"{MODEL_FUNCNAME}(\"{entry.Key}\");");
                }
            }
            sb.Append(PRECACHE_EPOLOUGE);
            System.IO.File.WriteAllText(File.FullName, sb.ToString());
        }
        public void Pull()
        {
            string rawPrecache = System.IO.File.ReadAllText(File.FullName);
            for ( int i = 0; i < rawPrecache.Length; i++ )
            {
                if ( rawPrecache[ i ] == '/' && rawPrecache[ i + 1 ] == '/' )
                {
                    // entered comment and skipping till end
                    while ( rawPrecache[ i ] != '\n' && rawPrecache[ i ] != '\r' )
                    {
                        i++;
                    }
                }
                else if ( new string(rawPrecache.Skip(i).Take(ANIM_FUNCNAME.Length).ToArray()) == ANIM_FUNCNAME )
                {
                    this[ ParseFromFuncNameIndex(i, ANIM_FUNCNAME.Length) ] = AssetType.xanim;
                }
                else if ( new string(rawPrecache.Skip(i).Take(MODEL_FUNCNAME.Length).ToArray()) == MODEL_FUNCNAME )
                {
                    this[ ParseFromFuncNameIndex(i, MODEL_FUNCNAME.Length) ] = AssetType.xmodel;
                }
            }

            string ParseFromFuncNameIndex( int indexinRawString, int funcNameLength )
            {
                int startindex = indexinRawString + funcNameLength + 2;
                string target = rawPrecache.Substring(startindex);
                return target.Substring(0, target.IndexOf('"'));
            }
        }
    }
}
