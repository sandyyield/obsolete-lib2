using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class CommodityPriceEntity : ModelContext
    {
        [SugarColumn(IsNullable = true, IsPrimaryKey = true)]
        public String id { get; set; }
        [SugarColumn(IsNullable = true)]
        public String shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String branchcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String branchname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String commoditycode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String commodityname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String saleprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public String memberprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public String dispatchprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public String wholesaleprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public String buyprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public String updateuser { get; set; }
        [SugarColumn(IsNullable = true)]
        public String crateuser { get; set; }
        [SugarColumn(IsNullable = true)]
        public String createtime { get; set; }
        [SugarColumn(IsNullable = true)]
        public String updatetime { get; set; }

        //add 2018/10/20
        [SugarColumn(IsNullable = true)]
        public string spucode { get; set; }//SPU编码，区分服装商品
        [SugarColumn(IsNullable = true)]
        public string commoditylevel { get; set; }//1-SKU商品 2-SPU商品


    }
}
