using System;
using UnityEngine;

namespace Framework
{
    public static class LuaManager
    {
        private static bool mInited = false;
        public static bool Init()
        {
            if(!mInited)
            {
                GameObject go = new GameObject("luaRoot");
                GameObject.DontDestroyOnLoad(go);
                go.AddComponent<LuaClient>();
            }
            return true;
        }
    }
}
