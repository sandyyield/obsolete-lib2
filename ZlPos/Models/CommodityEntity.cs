using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class CommodityEntity
    {
        public String id { get; set; }
        public String commodityname { get; set; }
        public String commoditypic { get; set; }

        public String spec { get; set; }
        public String buyprice { get; set; }
        public String saleprice { get; set; }
        public String wholesaleprice { get; set; }
        public String memberprice { get; set; }
        public String categorycode { get; set; }
        public String categoryname { get; set; }

        public String canscore { get; set; }
        public String score { get; set; }
        public String shortname { get; set; }
        public String mnemonic { get; set; }
        public String brandcode { get; set; }
        public String brandname { get; set; }
        public String unitcode { get; set; }
        public String unitname { get; set; }

        public String expiretime { get; set; }
        public String suppliercode { get; set; }
        public String suppliername { get; set; }
        public String shopcode { get; set; }
        public String buyspec { get; set; }
        public String commoditystatus { get; set; }
        public String commoditytype { get; set; }
        public String pricing { get; set; }
        public String commission { get; set; }
        public String commissionratio { get; set; }
        public String commissionmoney { get; set; }
        public String del { get; set; }
        public String creattime { get; set; }

        // TODO: 2017/10/17
        public String branchcode { get; set; }
        // add: 2018/2/26
        public String commoditycode { get; set; }
        public String businessmodel { get; set; }
        public String businessmodelname { get; set; }
        public String commissionrate { get; set; }
        public String intaxrate { get; set; }
        public String outtaxrate { get; set; }
        public String canstock { get; set; }
        public String dispatchprice { get; set; }



        //add 2018年3月14日
        public String commodityclassify { get; set; }
    }
}
