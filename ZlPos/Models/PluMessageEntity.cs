using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class PluMessageEntity
    {
        public string plu { get; set; }
        public string barcode { get; set; }//条码
        public string price { get; set; }//单价
        public string commodityName { get; set; }
        public string type { get; set; }//称重商品或计件商品,0-称重 1-计件
        public string indate { get; set; }//保质期
        public string tare { get; set; }//皮重
        public string shopName { get; set; }//条码秤店名
    }
}
