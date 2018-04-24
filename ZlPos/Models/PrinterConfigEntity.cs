using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class PrinterConfigEntity
    {
        public int id { get; set; }

        public String printerType { get; set; }

        public String pageWidth { get; set; }
        public object PageWidth { get; internal set; }
        public String port { get; set; }

        public String intBaud { get; set; }

        public String deviceId { get; set; }

        public String usbSystem { get; set; }

        public String printerBrand { get; set; }
        public object PrinterBrand { get; internal set; }

        //add 2018/01/15 打印小票份数
        public String printernumber { get; set; }
    }
}
