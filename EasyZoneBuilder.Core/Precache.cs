﻿using EasyZoneBuilder.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyZoneBuilder.Core
{
    public class Precache : ObservableDictionary<string, AssetType>, IFileInfo, ISync
    {
        public FileInfoEx File { get; }
        private static readonly string ANIM_FUNCNAME = "PrecacheMPAnim";
        private static readonly string MODEL_FUNCNAME = "PrecacheModel";
        public Precache( FileInfoEx File )
        {
            this.File = File;
            if ( this.File.Exists )
            {
                Pull();
            }

        }
        public void Push()
        {
            const string PRECACHE_PROLOUGE = "#include maps\\mp\\_utility;\n#include common_scripts\\utility;\n#using_animtree(\"multiplayer\");\nprecache()\n{\n";
            const string PRECACHE_EPOLOUGE = "}";
            StringBuilder sb = new StringBuilder(PRECACHE_PROLOUGE.Length + PRECACHE_EPOLOUGE.Length);
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
            File.WriteAllText(sb.ToString());
        }
        public void Pull()
        {
            string rawPrecache = File.ReadAllText();
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
