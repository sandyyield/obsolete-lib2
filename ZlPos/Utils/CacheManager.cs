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
        public static void InsertScale(string key, object configObject)
        {

            //HttpRuntime.Cache.Insert(key, configObject);
            ContextCache.setScale(configObject as string);
        }

        public static object GetScale(string key)
        {
            //return HttpRuntime.Cache.Get(key);
            return ContextCache.getScale();
        }

        public static void InsertGprint(object configObject)
        {
            ContextCache.setGprint(configObject as string);
        }

        public static object GetGprint()
        {
            return ContextCache.getGprint();
        }

        public static void InsertBarcodeStyle(string obj)
        {
            ContextCache.setBarcodeStyle(obj);
        }

        public static string GetBarcodeStyle()
        {
            return ContextCache.getBarcodeStyle();
        }

        public static string GetBJQprint()
        {
            return ContextCache.getBJQprint();
        }

        public static void InsertBJQprint(string s)
        {
            ContextCache.SetBJQprint(s);
        }

        public static string GetBJQTemplet()
        {
            return ContextCache.getBJQTemplet();
        }
        
        public static void InsertBJQTemplet(string s)
        {
            ContextCache.setBJQTemplet(s);
        }

        public static string GetSPBQTemplet()
        {
            return ContextCache.getSPBQTemplet();
        }

        public static void InsertSPBQTemplet(string s)
        {
            ContextCache.setSPBQTemplet(s);
        }

        public static string GetDDBQTemplet()
        {
            return ContextCache.getDDBQTemplet();
        }

        public static void InsertDDBQTemplet(string s)
        {
            ContextCache.setDDBQTemplet(s);
        }

        //add 2018年11月28日
        /// <summary>
        /// 条码秤保存
        /// </summary>
        /// <param name="obj"></param>
        public static void InsertBarcodeScale(string obj)
        {
            ContextCache.setBarcodeScale(obj);
        }

        public static string GetBarcodeScale()
        {
            return ContextCache.getBarcodeScale();
        }
    }
}
