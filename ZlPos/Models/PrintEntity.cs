using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class PrintEntity
    {
        public string content { get; set; } //正文文体
        public int heightLevel { get; set; }//纵向放大倍数
        public int widthLevel { get; set; }//横向放大倍数

        //add 2018年9月4日 '0'or null =false '1'=true
        public string isQRCode { get; set; } //是否为二维码

        //add 2018年10月15日
        public string needAutoNewLine { get; set; } //是否需要换行

        //add 2018年12月10日 
        public string isLogo { get; set; }

        public string layout { get; set; } //0 左对齐  1 居中 2右对齐

        public string isModelDrivePrint { get; set; } //1为本地模板驱动打印
    }
}
