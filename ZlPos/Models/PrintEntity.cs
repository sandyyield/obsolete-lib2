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

        //add 2018年9月4日 '0'or null =false '1'=true
        public string isQRCode { get; set; }

        //add 2018年10月15日
        public string needAutoNewLine { get; set; }
    }
}
