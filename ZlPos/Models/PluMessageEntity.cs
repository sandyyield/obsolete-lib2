using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class PluMessageEntity
    {
        [SugarColumn(IsNullable = false,IsPrimaryKey = true)]
        public string uid { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopCode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commoditycode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string plu { get; set; }
        [SugarColumn(IsNullable = true)]
        public string indate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string tare { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string barcode { get; set; }//条码
        [SugarColumn(IsIgnore = true)]
        public string price { get; set; }//单价
        [SugarColumn(IsIgnore = true)]
        public string commodityName { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string type { get; set; }//称重商品或计件商品,0-称重 1-计件
        [SugarColumn(IsIgnore = true)]
        public string shopName { get; set; }//条码秤店名
    }
}
