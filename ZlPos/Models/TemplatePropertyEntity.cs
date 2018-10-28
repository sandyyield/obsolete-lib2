using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class TemplatePropertyEntity
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string directionX { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string directionY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string disX { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string disY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int isBarCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isEdit { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string font { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string height { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string angle { get; set; }
    }
}
