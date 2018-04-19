using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class CommodityEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsNullable = true)]
        public String id { get; set; }
        [SugarColumn(IsNullable = true)]
        public String commodityname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String commoditypic { get; set; }

        [SugarColumn(IsNullable = true)]
        public String spec { get; set; }
        [SugarColumn(IsNullable = true)]
        public String buyprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public String saleprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public String wholesaleprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public String memberprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public String categorycode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String categoryname { get; set; }

        [SugarColumn(IsNullable = true)]
        public String canscore { get; set; }
        [SugarColumn(IsNullable = true)]
        public String score { get; set; }
        [SugarColumn(IsNullable = true)]
        public String shortname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String mnemonic { get; set; }
        [SugarColumn(IsNullable = true)]
        public String brandcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String brandname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String unitcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String unitname { get; set; }

        [SugarColumn(IsNullable = true)]
        public String expiretime { get; set; }
        [SugarColumn(IsNullable = true)]
        public String suppliercode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String suppliername { get; set; }
        [SugarColumn(IsNullable = true)]
        public String shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String buyspec { get; set; }
        [SugarColumn(IsNullable = true)]
        public String commoditystatus { get; set; }
        [SugarColumn(IsNullable = true)]
        public String commoditytype { get; set; }
        [SugarColumn(IsNullable = true)]
        public String pricing { get; set; }
        [SugarColumn(IsNullable = true)]
        public String commission { get; set; }
        [SugarColumn(IsNullable = true)]
        public String commissionratio { get; set; }
        [SugarColumn(IsNullable = true)]
        public String commissionmoney { get; set; }
        [SugarColumn(IsNullable = true)]
        public String del { get; set; }
        [SugarColumn(IsNullable = true)]
        public String creattime { get; set; }

        // TODO: 2017/10/17
        [SugarColumn(IsNullable = true)]
        public String branchcode { get; set; }
        // add: 2018/2/26
        [SugarColumn(IsNullable = true)]
        public String commoditycode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String businessmodel { get; set; }
        [SugarColumn(IsNullable = true)]
        public String businessmodelname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String commissionrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public String intaxrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public String outtaxrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public String canstock { get; set; }
        [SugarColumn(IsNullable = true)]
        public String dispatchprice { get; set; }



        //add 2018年3月14日
        [SugarColumn(IsNullable = true)]
        public String commodityclassify { get; set; }
    }
}
