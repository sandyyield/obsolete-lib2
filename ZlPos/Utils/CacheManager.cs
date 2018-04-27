using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;
using ZlPos.Bizlogic;

namespace ZlPos.Utils
{
    class CacheManager
    {
        //static string filePath = Application.StartupPath + "\\localcache";
        public static void Insert(string key,object configObject)
        {

            //HttpRuntime.Cache.Insert(key, configObject);
            ContextCache.setScale(configObject as string);
        }

        public static object Get(string key)
        {
            //return HttpRuntime.Cache.Get(key);
            return ContextCache.getScale();
        }
    }
}
