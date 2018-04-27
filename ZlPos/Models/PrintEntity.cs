using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class PrintEntity
    {
        public string content { get; set; }
        public int heightLevel { get; set; }//纵向放大倍数
        public int widthLevel { get; set; }//横向放大倍数
    }
}
