using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public string appcode { get; set; }
        /// <summary>
        /// 中仑零售
        /// </summary>
        public string appname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string versionno { get; set; }
        /// <summary>
        /// 68windows升级
        /// </summary>
        public string versiondesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string packagename { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string packagesize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string packagekey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string packageurl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string executeafter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string executeafterdec { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string publishurl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isforce { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string createtime { get; set; }
    }

    public class UpdateEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public Data data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int errorCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string errorMsg { get; set; }
    }


}
